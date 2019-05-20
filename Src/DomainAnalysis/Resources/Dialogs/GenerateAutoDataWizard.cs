using DomainAnalysis.DocumentRanking.ClumpingRankDoc;
using DomainAnalysis.DataPrepare;
using DomainAnalysis.ExtractContent;
using DomainAnalysis.LabelTopic;
using DomainAnalysis.LabelTopicTerms;
using DomainAnalysis.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DomainAnalysis.Resources.Dialogs
{
    public partial class GenerateAutoDataWizard : Form
    {
        private int mSourceFileNumber = 0;
        BackgroundWorker backgroundWorker = null;     
        
        public GenerateAutoDataWizard()
        {
            InitializeComponent();
            
            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.ItemSize = new Size(0, 1);
            tabControl.SizeMode = TabSizeMode.Fixed;
            btnBack.Visible = false;
        }

        #region table control
        private void UpdateWizardControl()
        {
            if (tabControl.SelectedIndex == 0)
            {
                btnBack.Visible = false;
            }
            else if (tabControl.SelectedIndex == 1)
            {
                btnBack.Visible = true;
            }

            if (tabControl.SelectedIndex == tabControl.TabCount - 2)
            {
                btnNext.Text = "Next >";
            }
            else if (tabControl.SelectedIndex == tabControl.TabCount - 1)
            {
                btnNext.Text = "Finish";
            }
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWizardControl();

            switch (tabControl.SelectedIndex)
            {
                case 0:
                    InitializeTab0();
                    break;
                case 1:
                    InitializeTab1();
                    break;
                case 2:
                    InitializeTab2();
                    break;
                case 3:
                    InitializeTab3();
                    break;
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            tabControl.SelectedIndex--;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == tabControl.TabCount - 1)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            tabControl.SelectedIndex++;
        }
        #endregion        

        #region Startup
        private void InitializeTab0()
        {
            cbIsDelDestFile.Checked = Configures.GetAutoWizardIsDeleteExistingFile();
        }

        private void cbIsDelDestFile_CheckedChanged(object sender, EventArgs e)
        {
            Configures.SaveAutoWizardIsDeleteExistingFile(cbIsDelDestFile.Checked);
        }
        #endregion

        #region Count File Number
        private void CountFileNumber(string dirName)
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(CountFileNumber_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CountFileNumber_Completed);
            backgroundWorker.RunWorkerAsync(dirName);
        }

        private void CountFileNumber_DoWork(object sender, DoWorkEventArgs e)
        {
            mSourceFileNumber = FileMg.CountFileNumber(e.Argument.ToString());
            e.Result = mSourceFileNumber;
        }

        private void CountFileNumber_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            switch (tabControl.SelectedIndex)
            {
                case 1:
                    llRawFileNumber.Text = string.Format("{0}", e.Result);
                    break;
                case 2:
                    llExtractFileNumber.Text = string.Format("{0}", e.Result);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Copy Source File
        private void InitializeTab1()
        { 
            tbRawFileFolder.Text = Configures.GetAutoWizardRawFilePath();

            if (string.IsNullOrWhiteSpace(tbRawFileFolder.Text))
            {
                btnCopy.Enabled = false;
            }
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select the raw files directory";
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            folderBrowserDialog.SelectedPath = Configures.GetAutoWizardRawFilePath();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                tbRawFileFolder.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void tbSourceFileFolder_TextChanged(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(tbRawFileFolder.Text.Trim()))
            {
                Configures.SaveAutoWizardRawFilePath(tbRawFileFolder.Text);

                btnCopy.Enabled = true;
                CountFileNumber(tbRawFileFolder.Text);
            }            
        }        

        private void btnCopy_Click(object sender, EventArgs e)
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(CopyFile_DoWork);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(CopyFile_ProgressChanged);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CopyFile_Completed);
            backgroundWorker.RunWorkerAsync();
        }

        private void CopyFile_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                FileMg.DirectoryDelete(FileMg.AutoTmtOutputFileDir, true);
                FileMg.DeleteTmtCacheFile(FileMg.AutoTmtDataFileDir);
            }
            catch
            { }

            if (cbIsDelDestFile.Checked)
            {
                FileMg.ClearAutoFolder();
                FileMg.InitDataFolder();
            }

            e.Result = FileMg.DirectoryCopy(tbRawFileFolder.Text, FileMg.AutoSourceFileDir, true, false, backgroundWorker);
        }

        private void CopyFile_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbCopyFile.Value = (int)(e.ProgressPercentage * 1.0 / mSourceFileNumber * 100);
            llCopyIndexOfFile.Text = string.Format("{0}", e.ProgressPercentage);
            llCopyNameOfFile.Text = string.Format("{0}", e.UserState);
        }

        private void CopyFile_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            //llCopyIndexOfFile.Text = "Copy file completed, you can continue to next steps now";
            btnNext.Enabled = true;
            llExtractFileNumber.Text = string.Format("{0}", e.Result);
        }
        #endregion

        #region Extract to text
        private void InitializeTab2()
        { 
            CountFileNumber(FileMg.AutoSourceFileDir);
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(ExtractFile_DoWork);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ExtractFile_ProgressChanged);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ExtractFile_Completed);
            backgroundWorker.RunWorkerAsync();
        }

        private void ExtractFile_DoWork(object sender, DoWorkEventArgs e)
        {
            ExtractMg.ExtractFile(FileMg.AutoSourceFileDir, FileMg.AutoExtractTextFileDir, FileMg.AutoCleanTextFileDir, FileMg.AutoSemiCleanTextFileDir,
                FileMg.AutoTmtDataFileDir + Constants.TmtInputFileName, backgroundWorker);
        }

        private void ExtractFile_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbExtractFile.Value = (int)(e.ProgressPercentage * 1.0 / mSourceFileNumber * 100);
            llExtractIndexOfFile.Text = string.Format("{0}", e.ProgressPercentage);
            llExtractNameOfFile.Text = string.Format("{0}", e.UserState);
        }

        private void ExtractFile_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            llExtractNameOfFile.Text = "Extract file completed, you can continue to next steps now";
        }
        #endregion

        #region TMT operation
        private void InitializeTab3()
        {
            tbTopicNumberArray.Text = Configures.GetAutoWizardTopicNumberArray();
            tbMaxIteration.Text = Configures.GetAutoWizardMaxIteration();
        }

        private void tbTopicNumberArray_Leave(object sender, EventArgs e)
        {
            Configures.SaveAutoWizardTopicNumberArray(tbTopicNumberArray.Text);
        }

        private void tbMaxIteration_Leave(object sender, EventArgs e)
        {
            Configures.SaveAutoWizardMaxIteration(tbMaxIteration.Text);
        }

        private void btnRunTmt_Click(object sender, EventArgs e)
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(RunTmt_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunTmt_Completed);
            llTmtStatus.Text = "Generating...";
            backgroundWorker.RunWorkerAsync();
        }

        private void RunTmt_DoWork(object sender, DoWorkEventArgs e)
        {            
            TmtToolMg.RunTmtTool(tbTopicNumberArray.Text.Trim(), tbMaxIteration.Text.Trim());
            int maxIteration = Int32.Parse(Configures.GetAutoWizardMaxIteration());
            string termDistZipFilePath = FileMg.AutoTmtOutputFileDir + string.Format(Constants.TmtOutputTopicTermDistZipFilePathTemp, maxIteration.ToString("D5"));
            UnzipToolMg.RunUnzipTool(termDistZipFilePath, FileMg.AutoRDataFileDir);
            e.Result = true;
        }

        private void RunTmt_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)(e.Result))
            {
                llTmtStatus.Text = "Finish generating topic models, please click next to continue";
            }
            else
            {
                llTmtStatus.Text = "Failed to generate topic models, please check the data";
            }
        }
        #endregion

        #region Label and Calculate TFIDF score
        private void btnLabelTopic_Click(object sender, EventArgs e)
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(LabelTopic_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LabelTopic_Completed);
            llLabelTopicStatus.Text = "Generating...";
            backgroundWorker.RunWorkerAsync();   
        }

        private void LabelTopic_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!Directory.Exists(FileMg.AutoTopicLabelFileDir))
            {
                Directory.CreateDirectory(FileMg.AutoTopicLabelFileDir);
            }

            int minmumTopicNumber = PrepareTopicFile.GetMinimumTopicNumber();
            int maxIteration = Int32.Parse(Configures.GetAutoWizardMaxIteration());
            if (minmumTopicNumber != -1)
            {
                string summaryFilePath = FileMg.AutoTmtOutputFileDir + string.Format(Constants.TmtOutputSummaryFilePathTemp, maxIteration.ToString("D5"));
                string topicTermFilePath = FileMg.AutoTopicLabelFileDir + Constants.TopicTermFileName;
                PrepareTopicFile.Execute(summaryFilePath, topicTermFilePath);
                JNSPToolMg.RunJNSPTool();
                string jnspOutputFileName = FileMg.AutoJNSPDataFileDir + Constants.JnspOptionCNTFileName + Constants.JnspOptionWindowNumber + ".cnt";
                KDDLabel kddLabel = new KDDLabel(jnspOutputFileName, topicTermFilePath, summaryFilePath, FileMg.AutoTopicLabelFileDir + Constants.TopicLabelFileName);
                kddLabel.GenerateTopicLabel();
                TopicSim.CalTopicSimilarity(topicTermFilePath, FileMg.AutoTopicLabelFileDir + Constants.TopicSimilarityFileName);

                RToolMg.RunRTool();
            }
        }

        private void LabelTopic_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            llLabelTopicStatus.Text = "Finished generating topic and similarity. Click next to continue.";
        }
        #endregion

        private void btnRankRelatedFiles_Click(object sender, EventArgs e)
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(RankRelatedFiles_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RankRelatedFiles_Completed);
            llRankingStatus.Text = "Ranking...";
            backgroundWorker.RunWorkerAsync(); 
            
        }

        private void RankRelatedFiles_DoWork(object sender, DoWorkEventArgs e)
        {
            //RankDocByClumping docRank = new RankDocByClumping(Constants.TopicLabelFileDir + Constants.TopicTermFileName, Constants.DefaultCleanTextFileDir, Constants.TopicLabelFileDir + Constants.TopicRelatedFileName);
            //docRank.DoClumpingRank();
            //RankDocByClumpingImprove docRank = new RankDocByClumpingImprove(FileMg.AutoTopicLabelFileDir + Constants.TopicTermFileName, FileMg.AutoCleanTextFileDir, FileMg.AutoTopicLabelFileDir + Constants.TopicRelatedFileName);
            //docRank.DoClumpingRank();

            RankDocByClumpingDocLen docRank = new RankDocByClumpingDocLen(FileMg.AutoTopicLabelFileDir + Constants.TopicTermFileName, FileMg.AutoCleanTextFileDir, FileMg.AutoTopicLabelFileDir + Constants.TopicRelatedFileName);
            docRank.DoClumpingRank();
        }

        private void RankRelatedFiles_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            llRankingStatus.Text = "Finished ranking topic related files. Click finish button to complete this wizard.";
        }
    }
}

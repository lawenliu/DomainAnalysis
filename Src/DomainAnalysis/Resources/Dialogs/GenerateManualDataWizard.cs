using DomainAnalysis.DataPrepare;
using DomainAnalysis.DocumentRanking.ClumpingRankDoc;
using DomainAnalysis.DocumentRanking.VSMRankDoc;
using DomainAnalysis.ExtractContent;
using DomainAnalysis.LabelTopic;
using DomainAnalysis.LabelTopicTerms;
using DomainAnalysis.PrepareFile;
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
    public partial class GenerateManualDataWizard : Form
    {
        private int mSourceFileNumber = 0;
        BackgroundWorker backgroundWorker = null;     
        
        public GenerateManualDataWizard()
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
            cbIsDelDestFile.Checked = Configures.GetManualWizardIsDeleteExistingFile();
        }

        private void cbIsDelDestFile_CheckedChanged(object sender, EventArgs e)
        {
            Configures.SaveManualWizardIsDeleteExistingFile(cbIsDelDestFile.Checked);
        }
        #endregion

        #region Count files
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
            tbRawFileFolder.Text = Configures.GetManualWizardRawFilePath();

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
            folderBrowserDialog.SelectedPath = Configures.GetManualWizardRawFilePath();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                tbRawFileFolder.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void tbSourceFileFolder_TextChanged(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(tbRawFileFolder.Text.Trim()))
            {
                Configures.SaveManualWizardRawFilePath(tbRawFileFolder.Text);

                btnCopy.Enabled = true;
                CountFileNumber(tbRawFileFolder.Text);
            }            
        }        

        private void btnCopy_Click(object sender, EventArgs e)
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(CopyFile_DoWork);
            backgroundWorker.ProgressChanged += null;
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(CopyFile_ProgressChanged);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CopyFile_Completed);
            backgroundWorker.RunWorkerAsync();
        }

        private void CopyFile_DoWork(object sender, DoWorkEventArgs e)
        {
            if (cbIsDelDestFile.Checked)
            {
                FileMg.ClearManualFolder();
                FileMg.InitDataFolder();
            }

            e.Result = FileMg.DirectoryCopy(tbRawFileFolder.Text, FileMg.ManualSourceFileDir, true, false, backgroundWorker);
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
            CountFileNumber(FileMg.ManualSourceFileDir);
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
            ExtractMg.ExtractFile(FileMg.ManualSourceFileDir, FileMg.ManualExtractTextFileDir,
                FileMg.ManualCleanTextFileDir, FileMg.ManualSemiCleanTextFileDir, null, backgroundWorker);
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

        #region Label and Calculate TFIDF score

        public void InitializeTab3()
        {
            tbModelFilePath.Text = Configures.GetManualWizardModelFilePath();
        }

        private void btnSelectModelFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location;
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                tbModelFilePath.Text = openFileDialog.FileName;
                Configures.SaveManualWizardModelFilePath(tbModelFilePath.Text);
            }
        }

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
            if (!Directory.Exists(FileMg.ManualTopicLabelFileDir))
            {
                Directory.CreateDirectory(FileMg.ManualTopicLabelFileDir);
            }

            string modelFilePath = tbModelFilePath.Text;
            if (string.IsNullOrEmpty(modelFilePath.Trim()))
            {
                MessageBox.Show("Please select manual model file first!");
                return;
            }

            PrepareManualModel prepareManualModel = new PrepareManualModel(tbModelFilePath.Text, FileMg.ManualTopicLabelFileDir + Constants.TopicManualTermFileName);
            prepareManualModel.ParseManualModel();
        }

        private void LabelTopic_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            llLabelTopicStatus.Text = "Finished generating topic and similarity. Click next to continue.";
        }
        #endregion

        #region Ranking
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
            //RankDocByClumping docRank = new RankDocByClumping(Constants.TopicLabelFileDir + Constants.TopicManualTermFileName, Constants.DefaultCleanTextFileDir, Constants.TopicLabelFileDir + Constants.TopicManualRelatedFileName);
            //docRank.DoClumpingRank();

            //the algorithm version in RE conference
            //RankDocByClumpingImprove docRank = new RankDocByClumpingImprove(FileMg.ManualTopicLabelFileDir + Constants.TopicManualTermFileName, FileMg.ManualCleanTextFileDir, FileMg.ManualTopicLabelFileDir + Constants.TopicManualRelatedFileName);
            //docRank.DoClumpingRank();

            //RankDocByClumpingDocLen docRank = new RankDocByClumpingDocLen(FileMg.ManualTopicLabelFileDir + Constants.TopicManualTermFileName, FileMg.ManualCleanTextFileDir, FileMg.ManualTopicLabelFileDir + Constants.TopicManualRelatedFileName);
            //docRank.DoClumpingRank();


            //newest clumping algorithm
            RankDocByClumpingLessClumps docRank = new RankDocByClumpingLessClumps(FileMg.ManualTopicLabelFileDir + Constants.TopicManualTermFileName, FileMg.ManualCleanTextFileDir, FileMg.ManualTopicLabelFileDir + Constants.TopicManualRelatedFileName);
            docRank.DoClumpingRank(null);


            //vsm algo.
            //TopicDocRank docRank = new TopicDocRank(FileMg.ManualTopicLabelFileDir + Constants.TopicManualTermFileName, FileMg.ManualCleanTextFileDir, FileMg.ManualTopicLabelFileDir + Constants.TopicManualRelatedFileName);
            //docRank.executeRank();
        }

        private void RankRelatedFiles_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            llRankingStatus.Text = "Finished ranking topic related files. Click finish button to complete this wizard.";
        }
        #endregion
    }
}

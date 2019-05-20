using DomainAnalysis.DataPrepare;
using DomainAnalysis.DocumentRanking.ClumpingRankDoc;
using DomainAnalysis.ExtractContent;
using DomainAnalysis.LabelTopic;
using DomainAnalysis.LabelTopicTerms;
using DomainAnalysis.Utils;
using System;
using System.ComponentModel;
using System.IO;

namespace DomainAnalysis.GenerateDataMg
{
    class GenerateAutoDataMg
    {
        public static bool StartGenerate(BackgroundWorker backgroundWorker, string rawFilePath)
        {
            OutputMg.OutputHeader1(backgroundWorker, "Global", "Start to generate automated data!");
            bool rv = ClearFolder(backgroundWorker) &&
                        CopyRawFiles(backgroundWorker, rawFilePath) &&
                            ExtractSourceFiles(backgroundWorker) &&
                                GenerateTopicWithTmt(backgroundWorker) &&
                                    LabelTopic(backgroundWorker) &&
                                        RankTopic(backgroundWorker);
            if (rv)
            {
                OutputMg.OutputHeader1(backgroundWorker, "Global", "Finished generating automated model!");
                return true;
            }
            else
            {
                OutputMg.OutputHeader1(backgroundWorker, "Global", "Failed to generate automated model!");
                return false;
            }
        }

        private static bool ClearFolder(BackgroundWorker backgroundWorker)
        {
            try
            {
                FileMg.DirectoryDelete(FileMg.AutoTmtOutputFileDir, true);
                FileMg.DeleteTmtCacheFile(FileMg.AutoTmtDataFileDir);
            }
            catch
            { }

            if (Configures.GetAutoIsDeleteExistingFile())
            {
                OutputMg.OutputHeader1(backgroundWorker, "Step 0", "Clear output folder");
                OutputMg.OutputContent(backgroundWorker, "Start to clear");
                try
                {
                    FileMg.ClearAutoFolder();
                    FileMg.InitDataFolder();
                }
                catch
                {
                    OutputMg.OutputHeader1(backgroundWorker, "Failed", "Clear folder failed. Please try to run this tool as Administrator.");
                    return false;
                }
                
                OutputMg.OutputContent(backgroundWorker, "Finished clearing");
            }
            
            return true;
        }

        private static bool CopyRawFiles(BackgroundWorker backgroundWorker, string dirName)
        {            
            OutputMg.OutputHeader1(backgroundWorker, "Step 1", "Copy Source File");
            try
            {

                OutputMg.OutputContent(backgroundWorker, "Start to count number under " + dirName);
                int sourceFileNumber = FileMg.CountFileNumber(dirName);
                OutputMg.OutputContent(backgroundWorker, "Finished counting. Total file number is: " + sourceFileNumber);
                OutputMg.OutputContent(backgroundWorker, "Starting copy files");
                int numCopy = FileMg.DirectoryCopy(dirName, FileMg.AutoSourceFileDir, true, false, backgroundWorker);
                OutputMg.OutputContent(backgroundWorker, "Finished copying files. Total file number is: " + numCopy);
                return true;
            }
            catch
            {
                OutputMg.OutputHeader1(backgroundWorker, "Failed", "Copy files failed. Please try to run this tool as Administrator.");
                return false;
            }
        }

        private static bool ExtractSourceFiles(BackgroundWorker backgroundWorker)
        {
            OutputMg.OutputHeader1(backgroundWorker, "Step 2", "Extract Source File");
            OutputMg.OutputContent(backgroundWorker, "Start to count number under " + FileMg.AutoSourceFileDir);
            int sourceFileNumber = FileMg.CountFileNumber(FileMg.AutoSourceFileDir);
            OutputMg.OutputContent(backgroundWorker, "Finished counting. Total file number is: " + sourceFileNumber);
            OutputMg.OutputContent(backgroundWorker, "Start to extract files");
            ExtractMg.ExtractFile(FileMg.AutoSourceFileDir, FileMg.AutoExtractTextFileDir, FileMg.AutoCleanTextFileDir, FileMg.AutoSemiCleanTextFileDir,
                FileMg.AutoTmtDataFileDir + Constants.TmtInputFileName, backgroundWorker);
            OutputMg.OutputContent(backgroundWorker, "Finished extracting files");
            return true;
        }

        private static bool GenerateTopicWithTmt(BackgroundWorker backgroundWorker)
        {
            OutputMg.OutputHeader1(backgroundWorker, "Step 3", "Generate Topic with TMT");
            /* 1. Run Tmt Tool */
            OutputMg.OutputContent(backgroundWorker, "Start to run TMT");
            if(!TmtToolMg.RunTmtTool(Configures.GetAutoWizardTopicNumberArray(), Configures.GetAutoWizardMaxIteration()))
            {
                OutputMg.OutputContent(backgroundWorker, "Failed to startup TMT. Make sure you have authority to run command.");
                return false;
            }

            /* 2. Check Tmt output */
            int maxIter = Int32.Parse(Configures.GetAutoWizardMaxIteration());
            string termDistZipFilePath = FileMg.AutoTmtOutputFileDir + string.Format(Constants.TmtOutputTopicTermDistZipFilePathTemp, maxIter.ToString("D5"));
            if (!File.Exists(termDistZipFilePath))
            {
                OutputMg.OutputContent(backgroundWorker, "Cannot find the result file of topic modeling.");
                return false;
            }

            OutputMg.OutputContent(backgroundWorker, "Finished generating topic and term distribution.");

            /* 3. Unzip Term distribution */
            OutputMg.OutputContent(backgroundWorker, "Start to unzip term distribution file");
            if (!UnzipToolMg.RunUnzipTool(termDistZipFilePath, FileMg.AutoRDataFileDir)
                || !File.Exists(FileMg.AutoRDataFileDir + Constants.RInputFileName))
            {
                OutputMg.OutputContent(backgroundWorker, "Failed to unzip term distribution file. Make sure you have setup 7-zip.");
                return false;
            }

            OutputMg.OutputContent(backgroundWorker, "Finished unzipping term distribution file.");

            return true;
        }

        private static bool LabelTopic(BackgroundWorker backgroundWorker)
        {
            OutputMg.OutputHeader1(backgroundWorker, "Step 4", "Label Topic");
            /* 0. Create file directory */
            if (!Directory.Exists(FileMg.AutoTopicLabelFileDir))
            {
                Directory.CreateDirectory(FileMg.AutoTopicLabelFileDir);
            }

            /* 1. Get minimum topic number */
            OutputMg.OutputContent(backgroundWorker, "Getting minimum topic number.");
            int minmumTopicNumber = PrepareTopicFile.GetMinimumTopicNumber();
            int maxIteration = Int32.Parse(Configures.GetAutoWizardMaxIteration());
            if (minmumTopicNumber == -1)
            {
                OutputMg.OutputContent(backgroundWorker, "Failed to get minimum topic number. Please check whether you run TMT successfully.");
                return false;
            }

            /* 2. Run TMT */
            OutputMg.OutputContent(backgroundWorker, "Minimum topic number is " + minmumTopicNumber);
            string summaryFilePath = FileMg.AutoTmtOutputFileDir + string.Format(Constants.TmtOutputSummaryFilePathTemp, maxIteration.ToString("D5"));
            string topicTermFilePath = FileMg.AutoTopicLabelFileDir + Constants.TopicTermFileName;
            OutputMg.OutputContent(backgroundWorker, "Start to generate topic terms file.");
            PrepareTopicFile.Execute(summaryFilePath, topicTermFilePath);
            //LabelDomainTopic.Execute(Constants.DefaultSourceFileDir, Constants.TopicLabelFileDir + Constants.TopicTermFileName, summaryFilePath, Constants.TopicLabelFileDir + Constants.TopicLabelFileName);
            if (!File.Exists(topicTermFilePath))
            {
                OutputMg.OutputContent(backgroundWorker, "Failed to generate topic terms file.");
                return false;
            }

            OutputMg.OutputContent(backgroundWorker, "Finished generating topic terms file.");

            /* 3. Run JNSP */
            OutputMg.OutputContent(backgroundWorker, "Start to run JNSP tool.");
            if (!Directory.Exists(FileMg.AutoJNSPDataFileDir))
            {
                Directory.CreateDirectory(FileMg.AutoJNSPDataFileDir);
            }

            JNSPToolMg.RunJNSPTool();
            string jnspOutputFileName = FileMg.AutoJNSPDataFileDir + Constants.JnspOptionCNTFileName + Constants.JnspOptionWindowNumber + ".cnt";
            if (!File.Exists(jnspOutputFileName))
            {
                OutputMg.OutputContent(backgroundWorker, "Failed to run JNSP tool.");
                return false;
            }

            OutputMg.OutputContent(backgroundWorker, "Finished running JNSP tool.");

            /* 4. Label topic */
            OutputMg.OutputContent(backgroundWorker, "Start to label topic.");
            KDDLabel kddLabel = new KDDLabel(jnspOutputFileName, topicTermFilePath, summaryFilePath, FileMg.AutoTopicLabelFileDir + Constants.TopicLabelFileName);
            kddLabel.GenerateTopicLabel();
            if(!File.Exists(FileMg.AutoTopicLabelFileDir + Constants.TopicLabelFileName))
            {
                OutputMg.OutputContent(backgroundWorker, "Failed to label topic.");
                return false;
            }

            OutputMg.OutputContent(backgroundWorker, "Finished labeling topic");

            /* 5. Generate Similarity */
            OutputMg.OutputContent(backgroundWorker, "Start to generate topic similarity.");
            TopicSim.CalTopicSimilarity(topicTermFilePath, FileMg.AutoTopicLabelFileDir + Constants.TopicSimilarityFileName);
            if (!File.Exists(FileMg.AutoTopicLabelFileDir + Constants.TopicLabelFileName))
            {
                OutputMg.OutputContent(backgroundWorker, "Failed to generate topic similarity.");
                return false;
            }

            OutputMg.OutputContent(backgroundWorker, "Finished generating topic similarity");
            /* 6. Running R Tool */
            OutputMg.OutputContent(backgroundWorker, "Start to generate BiTree with R tool.");
            RToolMg.RunRTool();
            if (!File.Exists(FileMg.AutoRDataFileDir + Constants.ROutputFileName))
            {
                OutputMg.OutputContent(backgroundWorker, "Failed to generate BiTree with R tool. Please make sure you have setup R tool.");
                return false;
            }

            OutputMg.OutputContent(backgroundWorker, "Finished generating BiTree with R tool");

            return true;
        }

        private static bool RankTopic(BackgroundWorker backgroundWorker)
        {
            OutputMg.OutputHeader1(backgroundWorker, "Step 5", "Rank Topic Relative Files");
            //RankDocByClumping docRand = new RankDocByClumping(Constants.TopicLabelFileDir + Constants.TopicTermFileName, Constants.DefaultCleanTextFileDir, Constants.TopicLabelFileDir + Constants.TopicRelatedFileName);
            //docRand.DoClumpingRank();
            OutputMg.OutputContent(backgroundWorker, "Start to rank topic related files.");
            //RankDocByClumpingImprove docRank = new RankDocByClumpingImprove(FileMg.AutoTopicLabelFileDir + Constants.TopicTermFileName, FileMg.AutoCleanTextFileDir, FileMg.AutoTopicLabelFileDir + Constants.TopicRelatedFileName);
            //docRank.DoClumpingRank();
            RankDocByClumpingLessClumps docRank = new RankDocByClumpingLessClumps(FileMg.AutoTopicLabelFileDir + Constants.TopicTermFileName, FileMg.AutoCleanTextFileDir, FileMg.AutoTopicLabelFileDir + Constants.TopicRelatedFileName);
            docRank.DoClumpingRank(backgroundWorker);
            if (!File.Exists(FileMg.AutoTopicLabelFileDir + Constants.TopicRelatedFileName))
            {
                OutputMg.OutputContent(backgroundWorker, "Failed to rank topic related files.");
                return false;
            }

            OutputMg.OutputContent(backgroundWorker, "Finished ranking topic related files.");

            return true;
        }
    }
}

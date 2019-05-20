using DomainAnalysis.DataPrepare;
using DomainAnalysis.DocumentRanking.ClumpingRankDoc;
using DomainAnalysis.DocumentRanking.VSMRankDoc;
using DomainAnalysis.ExtractContent;
using DomainAnalysis.PrepareFile;
using DomainAnalysis.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainAnalysis.SummaryGeneration;

namespace DomainAnalysis.GenerateDataMg
{
    public class GenerateManualDataMg
    {
        public static bool StartGenerate(BackgroundWorker backgroundWorker, string rawFilePath, string modelFilePath)
        {
            OutputMg.OutputHeader1(backgroundWorker, "Global", "Start to generate manual data!");
            bool rv = ClearFolder(backgroundWorker) && 
                        CopyRawFiles(backgroundWorker, rawFilePath) && 
                            ExtractSourceFiles(backgroundWorker) && 
                            LabelTopic(backgroundWorker, modelFilePath) &&
                                RankTopic(backgroundWorker) &&
                                    ExtractComponent(backgroundWorker);
            if (rv)
            {
                OutputMg.OutputHeader1(backgroundWorker, "Global", "Finished generating manual model!");
                return true;
            }
            else
            {
                OutputMg.OutputHeader1(backgroundWorker, "Global", "Failed to generate manual model!");
                return false;
            }
        }

        private static bool ClearFolder(BackgroundWorker backgroundWorker)
        {
            if (Configures.GetAutoIsDeleteExistingFile())
            {
                OutputMg.OutputHeader1(backgroundWorker, "Step 0", "Clear output folder");
                OutputMg.OutputContent(backgroundWorker, "Start to clear");
                try
                {
                    FileMg.ClearManualFolder();
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
                int numCopy = FileMg.DirectoryCopy(dirName, FileMg.ManualSourceFileDir, true, false, backgroundWorker);
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
            OutputMg.OutputContent(backgroundWorker, "Start to count number under " + FileMg.ManualSourceFileDir);
            int sourceFileNumber = FileMg.CountFileNumber(FileMg.ManualSourceFileDir);
            OutputMg.OutputContent(backgroundWorker, "Finished counting. Total file number is: " + sourceFileNumber);
            OutputMg.OutputContent(backgroundWorker, "Start to extract files");
            ExtractMg.ExtractFile(FileMg.ManualSourceFileDir, FileMg.ManualExtractTextFileDir, FileMg.ManualCleanTextFileDir, FileMg.ManualSemiCleanTextFileDir,
                null, backgroundWorker);
            OutputMg.OutputContent(backgroundWorker, "Finished extracting files");
            return true;
        }

        private static bool LabelTopic(BackgroundWorker backgroundWorker, string modelFilePath)
        {
            OutputMg.OutputHeader1(backgroundWorker, "Step 3", "Label the Generated Topics");

            if (!Directory.Exists(FileMg.ManualTopicLabelFileDir))
            {
                Directory.CreateDirectory(FileMg.ManualTopicLabelFileDir);
            }

            OutputMg.OutputContent(backgroundWorker, "Start to parse manual model file.");
            PrepareManualModel prepareManualModel = new PrepareManualModel(modelFilePath, FileMg.ManualTopicLabelFileDir + Constants.TopicManualTermFileName);
            prepareManualModel.ParseManualModel();
            if (!File.Exists(FileMg.ManualTopicLabelFileDir + Constants.TopicManualTermFileName))
            {
                OutputMg.OutputContent(backgroundWorker, "Failed to parse manual model file, please check the file format.");
                return false;
            }

            OutputMg.OutputContent(backgroundWorker, "Finished parsing manual model file.");

            return true;
        }

        private static bool RankTopic(BackgroundWorker backgroundWorker)
        {
            OutputMg.OutputHeader1(backgroundWorker, "Step 4", "Rank Topic Relative Files");
            //RankDocByClumping docRank = new RankDocByClumping(Constants.TopicLabelFileDir + Constants.TopicManualTermFileName, Constants.DefaultCleanTextFileDir, Constants.TopicLabelFileDir + Constants.TopicManualRelatedFileName);
            //docRank.DoClumpingRank();
            OutputMg.OutputContent(backgroundWorker, "Start to rank topic related files.");
            //RankDocByClumpingImprove docRank = new RankDocByClumpingImprove(FileMg.ManualTopicLabelFileDir + Constants.TopicManualTermFileName, FileMg.ManualCleanTextFileDir, FileMg.ManualTopicLabelFileDir + Constants.TopicManualRelatedFileName);
            //docRank.DoClumpingRank();

            RankDocByClumpingLessClumps docRank = new RankDocByClumpingLessClumps(FileMg.ManualTopicLabelFileDir + Constants.TopicManualTermFileName, FileMg.ManualCleanTextFileDir, FileMg.ManualTopicLabelFileDir + Constants.TopicManualRelatedFileName);
            docRank.DoClumpingRank(backgroundWorker);

            Console.WriteLine("output path:" + FileMg.ManualTopicLabelFileDir + Constants.TopicManualRelatedFileName);
            //TopicDocRank docRank = new TopicDocRank(FileMg.ManualTopicLabelFileDir + Constants.TopicManualTermFileName, FileMg.ManualCleanTextFileDir, FileMg.ManualTopicLabelFileDir + Constants.TopicManualRelatedFileName);
            //docRank.executeRank();

            //RankingDocByClumpingCaleb docRank = new RankingDocByClumpingCaleb(FileMg.ManualTopicLabelFileDir + Constants.TopicManualTermFileName, FileMg.ManualCleanTextFileDir, FileMg.ManualTopicLabelFileDir + Constants.TopicManualRelatedFileName);
            //docRank.DoClumpingRank();

            //DocumentRanking.VSMRankDoc.TopicDocRank docRank = new DocumentRanking.VSMRankDoc.TopicDocRank(FileMg.ManualTopicLabelFileDir + Constants.TopicManualTermFileName, FileMg.ManualCleanTextFileDir, FileMg.ManualTopicLabelFileDir + Constants.TopicManualRelatedFileName);
            //docRank.executeRank();

            if (!File.Exists(FileMg.ManualTopicLabelFileDir + Constants.TopicManualRelatedFileName))
            {
                OutputMg.OutputContent(backgroundWorker, "Failed to rank topic related files.");
                return false;
            }

            OutputMg.OutputContent(backgroundWorker, "Finished ranking topic related files.");

            return true;
        }

        private static bool ExtractComponent(BackgroundWorker backgroundWorker)
        {
            OutputMg.OutputHeader1(backgroundWorker, "Step 5", "Extract Component Files");
            OutputMg.OutputContent(backgroundWorker, "Start to extract component files.");
            IdentifyComParagraphs relatedParaExtractor = new IdentifyComParagraphs();
            relatedParaExtractor.IdentifyComponentPara(backgroundWorker,
                FileMg.ManualTopicLabelFileDir + Constants.TopicManualTermFileName,
                FileMg.ManualTopicLabelFileDir + Constants.TopicManualRelatedFileName,
                FileMg.ManualCleanTextFileDir, FileMg.ManualCleanComponentFileDir);
            OutputMg.OutputContent(backgroundWorker, "Finished extracting component files.");
            return true;
        }
    }
}

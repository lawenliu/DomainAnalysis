using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DomainAnalysis.Utils
{
    public class Constants
    {
        public static string OutputHeader1Temp = "{0} : {1}";
        public static string OutputContentTemp = "   : {0}";
        
        public static string DefaultDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string DefaultApplicationDir = @"\DomainAnalysis";
        public static string DefaultAutoDataDir = @"\Automated";
        public static string DefaultManualDataDir = @"\Manual";
        public static string DefaultExternalToolDir = @"\External";
        public static string SourceFileDir = @"\SourceFile\";
        public static string ExtractTextFileDir = @"\ExtractTextFile\";
        public static string CleanTextFileDir =  @"\CleanTextFile\";
        public static string SemiCleanTextFileDir = @"\SemiCleanTextFile\";
        public static string CleanComponentFileDir = @"\CleanComponentFile\";
        public static string HighlightFileDir = @"\HighlightFile\";
        public static string TopicLabelFileDir = @"\TopicLabel\";
        public static string TMTOutputFileDir = @"\tmtOutput\";
        public static string TMTDataFileDir = @"\TMTData\";
        public static string RDataFileDir = @"\RData\";
        public static string JNSPDataFileDir = @"\JNSPData\";
        public static string TmtToolDir = @"\TmtTool\";
        public static string RToolDir = @"\RTool\";
        public static string JnspToolDir = @"\JnspTool\";

        public static string LocalExternalToolDir = @".\External";

        public static string TmtToolBatFileName = @"runTmtTool.bat";
        public static string TmtToolFileName = @"tmtTool.jar";
        public static string TmtScalaTemplateFileName = @"tmtTopicModeling.template";
        public static string TmtScalaGeneratedFileName = @"tmtTopicModeling.scala";
        public static string TmtInputFileName = @"tmtInputFile.csv";
        public static string TmtOutputInfoFileName = @"tmtOutputInfo.txt";
        
        public static string TmtOutputFileDirName = @"tmtOutput";
        public static string TmtOutputTopicDistribution = @"document-topic-distributions.csv";
        public static string TmtOutputTopicTermDistZipFilePathTemp = @"{0}\topic-term-distributions.csv.gz";
        public static string TmtOutputTopicTermDistUnzipFilePathTem = @"{0}\topic-term-distributions.csv";
        public static string TmtOutputSummaryFilePathTemp = @"{0}\summary.txt";
        public static string TmtMinimumTopicKey = "MinimumTopic";
        public static string UnzipToolBatFileName = @"run7ZipTool.bat";

        public static string TmtTopicNumberArraryPlaceHolder = "{PlaceHolderTopicNumberArray}";
        public static string TmtMaxIterationPlaceHolder = "{MaxIterationPlaceHolder}";
        public static string TmtInputFilePathPlaceHolder = "{InputFilePathPlaceHolder}";
        public static string TmtInfoFilePathPlaceHolder = "{InfoFilePathPlaceHolder}";
        public static string TmtOutputFileDirPlaceHolder = "{OutputFileDirPlaceHolder}";
        public static string TmtMinimumTopicPlaceHolder = "{MinimumTopicPlaceHolder}";

        
        public static string TopicTermFileName = @"TopicTerms.txt";
        public static string TopicLabelFileName = @"TopicLabel.txt";
        public static string TopicSimilarityFileName = @"TopicSimilarity.csv";
        public static string TopicRelatedFileName = @"TopicRelatedFiles.txt";
        public static string TopicManualTermFileName = @"TopicManualTerms.txt";
        public static string TopicManualRelatedFileName = @"TopicManualRelatedFiles.txt";
        
        public static string RToolBatFileName = @"runRTool.bat";
        public static string RScriptTemplateFileName = @"hierarchicalClustering.template";
        public static string RScriptGeneratedFileName = "hierarchicalClustering.R";
        public static string RInputFileName = @"topic-term-distributions.csv";
        public static string ROutputFileName = @"rOutputLayer.txt";

        public static string RInputFileNamePlaceHolder = "{RInputFileNamePlaceHolder}";
        public static string ROutputFileNamePlaceHolder = "{ROuputFileNamePlaceHolder}";
        
        public static string JnspToolBatFileName = @"runJnspTool.bat";
        public static string JnspOptionTemplateFileName = @"option.template";
        public static string JnspOptionGeneratedFieName = @"option.txt";
        public static string JnspOptionFreqCombo = @"freqcombo.txt";
        public static string JnspOptionStopWord = @"stopwords.txt";
        public static string JnspOptionWindowNumber = "2";
        public static string JnspOptionCNTFileName = "cntFile"; // output filename is: ctnFileName + JnspWindowNumber + "cnt"

        public static string JnspWindowNumberPlaceHolder = "{WindowNumberPlaceHolder}";
        public static string JnspTxtCleanDataDirPlaceHolder = "{TxtCleanDataDirPlaceHolder}";
        public static string JnspCNTFileNamePlaceHolder = "{CNTFileNamePlaceHolder}";
        public static string JnspFreqComboPlaceHolder = "{FreqComboPlaceHolder}";
        public static string JnspStopWordPlaceHolder = "{StopWordPlaceHolder}";

        public static int RenderNodeNumberThreshold = 10;
        public static double RenderEdgeWeightThreshold = 4.0;

        public static string HelpPageFilePath = @".\Resources\Files\ReadMe.pdf";

        #region Configure Key List
        public static string KeyAutoRawMaterialFileDir = "AutoRawMaterialFileDir";
        public static string KeyAutoIsDeleteExistingFile = "AutoIsDeleteExistingFile";
        public static string KeyAutoWizardIsDeleteExistingFile = "AutoWizardIsDeleteExistingFile";
        public static string KeyAutoWizardTopicNumberArray = "AutoWizardTopicNumberArray";
        public static string KeyAutoWizardMaxIteration = "AutoWizardMaxIteration";
        public static string KeyAutoWizardRawFilePath = "AutoWizardRawFilePath";
        public static string KeyManualRawMaterialFileDir = "ManualRawMaterialFileDir";
        public static string KeyManualModelFilePath = "ManualModelFilePath";
        public static string KeyManualIsDeleteExistingFile = "ManualIsDeleteExistingFile";
        public static string KeyManualWizardIsDeleteExistingFile = "ManualWizardIsDeleteExistingFile";
        public static string KeyManualWizardModelFilePath = "ManualWizardModelFilePath";
        public static string KeyManualWizardRawFilePath = "ManualWizardRawFilePath";
        public static string keyManualExtraSearchTermsPath = "ExtraManualSearchTerms";
        #endregion
    }
}

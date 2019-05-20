using DomainAnalysis.DataPrepare;
using DomainAnalysis.Model;
using DomainAnalysis.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DomainAnalysis.SummaryGeneration;
using System.ComponentModel;

namespace DomainAnalysis.RenderPrepare
{
    class RenderManualDataMg
    {
        private TreeElement mTreeRoot = new TreeElement();
        private Dictionary<string, List<RelatedFileModel>> mSimilarityDictionary = new Dictionary<string, List<RelatedFileModel>>();
        private Dictionary<string, string> mCompSummDictionary = new Dictionary<string, string>();
        private string mTopicTermFilePath = FileMg.ManualTopicLabelFileDir + Constants.TopicManualTermFileName;
        private Dictionary<string, List<string>> mCompTerms = null;

        public RenderManualDataMg()
        { }

        public bool PrepareRender(BackgroundWorker backgroundWorker, string modelFilePath, string searchTermFilePath, string cleanComponentDir)
        {
            OutputMg.OutputHeader1(backgroundWorker, "Global", "Start preparing data...");
            OutputMg.OutputHeader1(backgroundWorker, "Step 1", "Try to load source files");
            TryToLoadSourceFile(modelFilePath);
            OutputMg.OutputHeader1(backgroundWorker, "Step 1", "Finished loading source files");
            OutputMg.OutputHeader1(backgroundWorker, "Step 2", "Try to load similarity files");
            TryToLoadSimilarityFile();
            OutputMg.OutputHeader1(backgroundWorker, "Step 2", "Finished loading similarity files");
            OutputMg.OutputHeader1(backgroundWorker, "Step 3", "Try to load component summary files");
            //TryToLoadCompSummary(backgroundWorker, searchTermFilePath, cleanComponentDir);
            OutputMg.OutputHeader1(backgroundWorker, "Step 3", "Finished loading component summary files");

            OutputMg.OutputHeader1(backgroundWorker, "Global", "Finished preparing data...");
            return true;
        }

        public TreeElement TreeRoot
        {
            get { return mTreeRoot; }
        }

        public Dictionary<string, List<RelatedFileModel>> SimilarityDictionary
        {
            get { return mSimilarityDictionary; }
        }

        public Dictionary<string, string> CompSummDictionary
        {
            get { return mCompSummDictionary; }
        }

        public string TopicTermFilePath
        {
            get { return mTopicTermFilePath; }
        }

        private void TryToLoadSimilarityFile()
        {
            string similarityFilePath = FileMg.ManualTopicLabelFileDir + Constants.TopicManualRelatedFileName;
            if (File.Exists(similarityFilePath))
            {
                StreamReader sr = new StreamReader(similarityFilePath);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    string[] attrs = line.Split('\t');
                    if (attrs.Length == 3)
                    {
                        string rawFileName = attrs[1].Trim();
                        string rawFilePath = FileMg.ManualSourceFileDir + rawFileName.Remove(rawFileName.Length - 4);

                        int startIndex = rawFileName.LastIndexOf('\\');
                        if (startIndex > -1)
                        {
                            rawFileName = rawFileName.Substring(startIndex + 1);                            
                        }

                        rawFileName = rawFileName.Remove(rawFileName.Length - 4);

                        RelatedFileModel model = new RelatedFileModel();
                        model.TopicName = attrs[0].ToLower();
                        model.Similarity = attrs[2];
                        model.RelatedFileName = rawFileName;
                        model.RelatedFilePath = rawFilePath;

                        if (mSimilarityDictionary.ContainsKey(model.TopicName))
                        {
                            List<RelatedFileModel> similarityList = mSimilarityDictionary[model.TopicName];
                            similarityList.Add(model);
                            mSimilarityDictionary[model.TopicName] = similarityList;
                        }
                        else
                        {
                            List<RelatedFileModel> similarityList = new List<RelatedFileModel>();
                            similarityList.Add(model);
                            mSimilarityDictionary.Add(model.TopicName, similarityList);
                        }
                    }
                }

                sr.Close();
            }
        }
        
        private void TryToLoadCompSummary(BackgroundWorker backgroundWorker, string searchTermFilePath, string cleanComponentDir)
        {
            OutputMg.OutputContent(backgroundWorker, "Start to combine terms");
            CombineTerms termCombiner = new CombineTerms();
            Dictionary<string, List<string>> compTerms = termCombiner.GetCombinedTerms(searchTermFilePath, null);
            OutputMg.OutputContent(backgroundWorker, "Finished combining terms");
            OutputMg.OutputContent(backgroundWorker, "Start to generate summary");
            foreach (string comp in compTerms.Keys)
            {
                OutputMg.OutputContent(backgroundWorker, "-- Start to generate summary for " + comp);
                List<string> compTermList = compTerms[comp];

                //get the sentences of components
                string paraFile = cleanComponentDir + comp + ".txt";
                GenerateComponentSummary sentenceSplitter = new GenerateComponentSummary();
                List<string> candidateSentences = sentenceSplitter.SplitSingleFileSentence(paraFile);

                MMRSummary summaryGenerator = new MMRSummary();
                string summary = summaryGenerator.GenerateSummary(compTermList, candidateSentences);
                mCompSummDictionary.Add(comp, summary);
                OutputMg.OutputContent(backgroundWorker, "-- Finished generating summary for " + comp);
            }
            OutputMg.OutputContent(backgroundWorker, "Finished generating summary");
        }

        public void GenerateCompSummary(BackgroundWorker backgroundWorker, string searchTermFilePath, string cleanComponentDir, string compName)
        {

            OutputMg.OutputContent(backgroundWorker, "Start to generate summary");
            OutputMg.OutputContent(backgroundWorker, "-- Start to generate summary for " + compName);
            if (mCompTerms == null)
            {
                OutputMg.OutputContent(backgroundWorker, "Start to combine terms");
                CombineTerms termCombiner = new CombineTerms();
                mCompTerms = termCombiner.GetCombinedTerms(searchTermFilePath, null);
                OutputMg.OutputContent(backgroundWorker, "Finished combining terms");
            }

            if (mCompTerms == null || !mCompTerms.ContainsKey(compName) || mCompSummDictionary.ContainsKey(compName))
            {
                return;
            }
            
            List<string> compTermList = mCompTerms[compName];
            //get the sentences of components
            string paraFile = cleanComponentDir + compName + ".txt";
            GenerateComponentSummary sentenceSplitter = new GenerateComponentSummary();
            List<string> candidateSentences = sentenceSplitter.SplitSingleFileSentence(paraFile);

            if (!mCompSummDictionary.ContainsKey(compName))
            {
                MMRSummary summaryGenerator = new MMRSummary();
                string summary = summaryGenerator.GenerateSummary(compTermList, candidateSentences);
                mCompSummDictionary.Add(compName, summary);
            }
            
            OutputMg.OutputContent(backgroundWorker, "-- Finished generating summary for " + compName);
            OutputMg.OutputContent(backgroundWorker, "Finished generating summary");
        }

        private void TryToLoadSourceFile(string modelFilePath)
        {
            try
            {
                Stream myStream = File.OpenRead(modelFilePath);
                using (myStream)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(myStream);
                    //select root node
                    XmlNode rootNode = doc.SelectSingleNode("/map");
                    XmlNodeList nodeList = rootNode.ChildNodes;
                    mTreeRoot.Text = rootNode.Name;
                    mTreeRoot.SubElements = parseComponentLevel(nodeList);
                }
            }
            catch
            {
            }
        }

        private List<TreeElement> parseComponentLevel(XmlNodeList nodeList)
        {
            List<TreeElement> elemList = new List<TreeElement>();
            foreach (XmlNode node in nodeList)
            {
                if (node.Name.Equals("node", StringComparison.OrdinalIgnoreCase))
                {
                    TreeElement treeElement = new TreeElement();
                    treeElement.Text = node.Attributes["TEXT"].Value.ToLower();
                    if (node.HasChildNodes)
                    {
                        treeElement.SubElements = parseComponentLevel(node.ChildNodes);
                    }

                    elemList.Add(treeElement);
                }
            }

            return elemList;
        }

    }
}

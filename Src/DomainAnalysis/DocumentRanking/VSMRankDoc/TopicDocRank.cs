using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DomainAnalysis.Utils;

namespace DomainAnalysis.DocumentRanking.VSMRankDoc
{
    /*
     * rand the docs based on tf-idf and SVM
     * input: the key term list of topics, each term with a specific propability; a path of the doc
     * output: doc name and their related similarity between topics in decreased order. Write into a file
     * 
     * Note: the topic term is in the format: topicName: term1, value; term2, value; term3, value
     * the output file is in the format:
     * topicName\tdocName1\trelevance1
     * 
     */
    class TopicDocRank
    {
        //private string topicTermsPath = @"C:\Users\xlian\Dropbox\MyPaper\Simmons\manualModel\manualTopicTerms.txt";
        //private string docsPath = @"C:\Users\xlian\MyPapers\Simmons\txtStoreClean";
      //  private string topicNamePath = @"C:\Users\xlian\MyPapers\Simmons\topicLabel\labels-kdd.txt";//mapping from ID to name
        private string topicTermsPath = "";
        private string docsPath = "";
        private string topicNamePath = "";
        private string tfidfStore;
        private string simStorePath = "";
        //private string simStorePath = @"C:\Users\xlian\MyPapers\Simmons\manualModel\similarity.txt";
        Dictionary<string, Dictionary<string, float>> normalizedTopicTerms = new Dictionary<string, Dictionary<string, float>>();

        public TopicDocRank(string topicTermsFilePath, string txtCleanFileDir, string outputSimilarityFilePath)
        {
            topicTermsPath = topicTermsFilePath;
            docsPath = txtCleanFileDir;
            simStorePath = outputSimilarityFilePath + "vsm.txt";
        }

        public void executeRank()
        {
            getAllTopicTerms(); //get topic and the related terms, and do the normalization

            int txtDirLength = docsPath.Length;

            foreach (KeyValuePair<string, Dictionary<string, float>> entry in normalizedTopicTerms)
            {
                
                string topicName = entry.Key; //just the topic ID
              

                Dictionary<string, float> termAndValues = entry.Value;

                List<string> terms = new List<string>(termAndValues.Keys);

                List<float> queryVector = new List<float>(termAndValues.Values);

               // topicName = topicName.Replace(" ", string.Empty);

                tfidfStore = docsPath + "-ifidf\\" + topicName + ".csv";
                //for each document, generate the ifidf according to the keyterms of topic
                TFIDF tfidf = new TFIDF(terms, this.docsPath, tfidfStore);
                tfidf.calTfidf();

                string[] tfidfLines = FileOperators.ReadFileLines(tfidfStore);

                int lineScale = tfidfLines.Length;

                VSM vsm = new VSM();

                string simContent = "";

                Dictionary<string, double> docAndRelevance = new Dictionary<string, double>();

                for (int i = 1; i < lineScale; i++)
                {
                    string curLine = tfidfLines[i];
                    int firstComma = curLine.IndexOf(';');
                    string fileName = curLine.Substring(0, firstComma); //test if the length is right
                 
                    string valueStr = curLine.Substring(firstComma + 1);
                    string[] valueTerms = valueStr.Split(';');
                    List<float> docVector = new List<float>();
                    foreach (string valueTerm in valueTerms)
                    {
                        float value = float.Parse(valueTerm);
                        docVector.Add(value);
                    }
                    double sim = vsm.calSimilarity(docVector, queryVector);
                    if (sim > 0 )
                    {
                        docAndRelevance.Add(fileName, sim); //get the similarity between doc and topic
                    }
                }

                //execute decrease sorting on the docAndRelevance
                Dictionary<string, double> sortedByRelevance = DictionaryDecreasedSort.DecreasedByValue(docAndRelevance);
                foreach (string key in sortedByRelevance.Keys)
                {
                    double similarity = sortedByRelevance[key];
                    string fileName = key.Substring(txtDirLength);
                    simContent += topicName + "\t" + fileName + "\t" + similarity + "\r\n";
                }
                FileOperators.FileAppend(simStorePath, simContent); //simStorePath should contain relativePath
            }
            Console.WriteLine("DONE!!");
        }


        private void getAllTopicTerms()
        {
            Dictionary<string, Dictionary<string, string>> topicTerms = new Dictionary<string, Dictionary<string, string>>();
            string[] topicLines = FileOperators.ReadFileLines(topicTermsPath);
            Dictionary<string, string> topicIDNameMap = mapTopicIDName();
            foreach (string line in topicLines)
            {
                if (line.Length == 0)
                {
                    continue;
                }

                int colonIndex = line.IndexOf(":");
                string topicID = line.Substring(0, colonIndex);
                string topicName;
                if (topicIDNameMap.ContainsKey(topicID))
                {
                    topicName = topicIDNameMap[topicID];
                }
                else
                {
                    topicName = topicID;
                }
                Dictionary<string, string> termValueMap = new Dictionary<string, string>();

                string termValuePart = line.Substring(colonIndex + 1);
                string[] terms = termValuePart.Split(';');
                foreach (string termValue in terms)
                {
                    string[] termAndValue = termValue.Split(',');
                    string term = termAndValue[0].Trim().ToLower();
                    string value = termAndValue[1];
                    if (!termValueMap.ContainsKey(term))
                    {
                        termValueMap.Add(term, value);
                    }
                }
                topicTerms.Add(topicName, termValueMap);

            }

            NormalizeTopicRelevance normalizer = new NormalizeTopicRelevance();
            normalizedTopicTerms = normalizer.DoNormalize(topicTerms);
        }

        private Dictionary<string, string> mapTopicIDName()
        {
            Dictionary<string, string> IDNameMap = new Dictionary<string, string>();
            if (this.topicNamePath.Length > 0)
            {
                string[] fileContent = FileOperators.ReadFileLines(this.topicNamePath);
                foreach (string line in fileContent)
                {
                    string[] terms = line.Split(':');
                    string topicID = terms[0];
                    string topicName = terms[1];
                    IDNameMap.Add(topicID, topicName);
                }
            }
            return IDNameMap;
        }
    }
}

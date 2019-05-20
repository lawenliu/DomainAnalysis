using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainAnalysis.Utils;
using System.IO;

namespace DomainAnalysis.LabelTopicTerms
{
    class KDDLabel
    {
        string n_gramFile = "";
        string labelStorePath = "";
        string summaryFile = "";
        string topicTermFile = "";

        string filteredNGram = "";
        public KDDLabel(string n_gramFile, string topicTermFile,string summaryFile, string labelStorePath)
        {
            this.n_gramFile = n_gramFile;
            this.topicTermFile = topicTermFile;
            this.summaryFile = summaryFile;
            filteredNGram = n_gramFile + "_filtered";
            this.labelStorePath = labelStorePath;
        }

        public Dictionary<string, string> GetTopicWeights()
        {
            Dictionary<string, string> dictTopicWeight = new Dictionary<string, string>();
            StreamReader sr = new StreamReader(summaryFile);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line.StartsWith("Topic"))
                {
                    string[] segs = line.Split('\t');
                    if (segs.Length == 3)
                    {
                        dictTopicWeight.Add(segs[0].Trim(), segs[2].Trim());
                    }
                }
            }

            sr.Close();

            return dictTopicWeight;
        }

        public void GenerateTopicLabel()
        {
            Dictionary<string, string> topicWeights = GetTopicWeights();
            Dictionary<string, Dictionary<string, float>> topicTerms = ExtractTopicTerms.ParseTopicTerms(topicTermFile);
            Dictionary<string, Dictionary<string, float>> reducedTopicTerms = new Dictionary<string, Dictionary<string, float>>();

            List<string> filteredGrams = FilterNGram();
            List<string> topicNameList = new List<string>();
        //    string[] ngramLines = FileOperator.ReadFileLines(n_gramFile);
            StreamWriter sw = new StreamWriter(this.labelStorePath);
            foreach (string topicId in topicTerms.Keys)
            {
                Dictionary<string, float> termProps = topicTerms[topicId];

                float maxImportance = 0f;
                string finalLabel = "";
                foreach (string ngram in filteredGrams)
                {
                    int braceIndex = ngram.IndexOf("<>");
                    if (braceIndex > -1)
                    {
                        string first = ngram.Substring(0, braceIndex).Trim();
                        string second = ngram.Substring(braceIndex + 2).Trim();

                        if (first.Equals(second))
                        {
                            continue;
                        }

                        float importance = GetImportance(first, termProps) + GetImportance(second, termProps);

                        string tmpLabel = first + " " + second;
                        if (importance > maxImportance && !topicNameList.Contains(tmpLabel))
                        {
                            maxImportance = importance;
                            finalLabel = tmpLabel;
                            topicNameList.Add(finalLabel);
                        }
                    }
                }

                

                if (!reducedTopicTerms.ContainsKey(finalLabel)) //what's this? show the selected 10 topics in UI
                {
                    Console.WriteLine(finalLabel + ":" + topicWeights[topicId]);
                    sw.WriteLine(finalLabel + ":" + topicWeights[topicId]);
                    reducedTopicTerms.Add(finalLabel, termProps);
                }                
            }

            sw.Close();

            ExtractTopicTerms.GenerateReduceTopicTerms(topicTermFile, reducedTopicTerms);
        }

        private float GetImportance(string term, Dictionary<string, float> termProps)
        {
            float value = 0f;

            foreach (string tmpTerm in termProps.Keys)
            {
                //if (tmpTerm.Contains(term))
                //{
                //    value = termProps[tmpTerm];
                //}
                if (tmpTerm.Equals(term))
                {
                    value = termProps[tmpTerm];
                }
            }

            return value;
        }

        /*update here in future. We didnot consider the frequency now!
         */
        private List<string> FilterNGram()
        {
            List<string> ngrams = new List<string>();
            string[] ngramLines = FileOperators.ReadFileLines(n_gramFile);

            foreach (string ngram in ngramLines)
            {

                int spaceIndex = ngram.IndexOf(" ");
                if (spaceIndex > -1)
                {
                    string textFreqStr = ngram.Substring(0, spaceIndex);
                    string[] separators = {"<>"};
                    string[] terms = textFreqStr.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    if (terms.Length == 3)
                    {
                        string first = terms[0];
                        string second = terms[1];
                        string freqStr = terms[2];
                        if (first.Length > 2 && second.Length > 2)
                        {
                            float freq = float.Parse(freqStr);
                            ngrams.Add(first + "<>" + second);
                        }
                        
                    }
                    else
                    {
                        Console.WriteLine("check!");
                        
                    }
                  
                }
            }
            return ngrams;
        }
    }
}

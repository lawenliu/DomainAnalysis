using ExtractContent.Preprocess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DomainAnalysis.ExtractContent.Preprocess;

namespace DomainAnalysis.SummaryGeneration.VSM
{
    class TF_IDF
    {
        /*
         * return document and the related weights on all terms.
         * only care about the  docID because the final results in TiQi are requirements ID.
         */
        public Dictionary<string, List<float>> CalculateTFIDF(List<string> candidateSentences, List<string> targetTerms)
        {
            Dictionary<string, List<int>> tfs = CalculateTF(candidateSentences, targetTerms);
            List<double> idfs = CalculateIDF(tfs);
            Dictionary<string, List<float>> tf_idfs = new Dictionary<string, List<float>>();
            foreach (string docID in tfs.Keys)
            {
                List<int> termFreqs = tfs[docID];
                List<float> results = new List<float>();
                bool allZero = true;
                for (int i = 0; i < termFreqs.Count(); i++)
                {
                    int freq = termFreqs[i];
                    double idf = idfs[i];
                    double tf_idf = freq * idf;
                    if (tf_idf > 0)
                    {
                        allZero = false;
                    }
                    results.Add(Convert.ToSingle(tf_idf));
                }
                if (!allZero)
                {
                    tf_idfs.Add(docID, results);
                }
            }
            return tf_idfs;
        }

        /*
         * return document and the frequency of target terms. The frequency is stored in the list.
         * 
         */
        private Dictionary<string, List<int>> CalculateTF(List<string> candidateSentences, List<string> targetTerms)
        {
            Dictionary<string, List<int>> docTFs = new Dictionary<string, List<int>>();

            foreach (string sentence in candidateSentences)
            {
                
                List<int> tf_line = TFInOneText(sentence, targetTerms);
                if (!docTFs.ContainsKey(sentence))
                {
                    docTFs.Add(sentence, tf_line);
                }
            }
            
            return docTFs;
        }

        private List<int> TFInOneText(string text, List<string> targetTerms)
        {
            List<int> tfs = new List<int>();
            Regex wordRegex = new Regex("\\W");
            string removePuncSentence = wordRegex.Replace(text, " ");
            string copyText = removePuncSentence;
            string[] textTerms = text.Split(' ');
            Dictionary<string, int> stemmedTerms = new Dictionary<string, int>();
            Dictionary<string, string> oriStemMap = new Dictionary<string, string>();
            Porter2 porter = new Porter2();
            foreach (string termInText in textTerms)
            {
                if (oriStemMap.ContainsKey(termInText))
                {
                    string stemmed = oriStemMap[termInText];
                    stemmedTerms[stemmed]++;
                }
                else
                {
                    string stemmedTerm = porter.stem(termInText);
                    if (stemmedTerms.ContainsKey(stemmedTerm))
                    {
                        stemmedTerms[stemmedTerm]++;
                        oriStemMap.Add(termInText, stemmedTerm);
                    }
                    else
                    {
                        oriStemMap.Add(termInText, stemmedTerm);
                        stemmedTerms.Add(stemmedTerm, 1);
                    }
                }
            }

            foreach (string targetTerm in targetTerms)
            {
                if (stemmedTerms.ContainsKey(targetTerm))
                {
                    tfs.Add(stemmedTerms[targetTerm]);
                }
                else
                {
                    tfs.Add(0);
                }
            }

            return tfs;
        }

        private List<double> CalculateIDF(Dictionary<string, List<int>> docTFs)
        {
            List<double> idfs = new List<double>();
            float docScale = docTFs.Count;
            List<List<int>> tfCollection = docTFs.Values.ToList();
            List<int> firstTF = tfCollection.ElementAt(0);
            //the ith key term
            for (int i = 0; i < firstTF.Count(); i++)
            {
                int existDocCount = 0;
                foreach (List<int> oneTF in tfCollection)
                {
                    int value = oneTF[i];
                    if (value.Equals(0))
                    {
                        continue;
                    }
                    else
                    {
                        existDocCount++;
                    }
                }
                double idf = Math.Log10(docScale / (1 + existDocCount));
                idfs.Add(idf);
            }
            return idfs;
        }
    }
}

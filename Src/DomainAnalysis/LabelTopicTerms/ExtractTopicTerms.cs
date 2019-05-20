using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DomainAnalysis.Utils;
using System.IO;

namespace DomainAnalysis.LabelTopicTerms
{
    class ExtractTopicTerms
    {
        public static Dictionary<string, Dictionary<string, float>> ParseTopicTerms(string topicTermsPath)
        {
            Dictionary<string, Dictionary<string, float>> topicTerms = new Dictionary<string, Dictionary<string, float>>();
            string[] topicLines = FileOperators.ReadFileLines(topicTermsPath);
            foreach (string line in topicLines)
            {
                if (line.Length == 0)
                {
                    continue;
                }

                int colonIndex = line.IndexOf(":");
                string topicID = line.Substring(0, colonIndex);
                Dictionary<string, float> termValueMap = new Dictionary<string, float>();

                string termValuePart = line.Substring(colonIndex + 1);
                string[] terms = termValuePart.Split(';');
                foreach (string termValue in terms)
                {
                    string[] termAndValue = termValue.Split(',');
                    string term = termAndValue[0].Trim().ToLower();
                    string value = termAndValue[1];
                    if (!termValueMap.ContainsKey(term))
                    {
                        termValueMap.Add(term, float.Parse(value));
                    }
                }
                topicTerms.Add(topicID, termValueMap);
            }
            return topicTerms;
        }

        public static void GenerateReduceTopicTerms(string topicTermsPath, Dictionary<string, Dictionary<string, float>> newTopicTerms)
        {
            StreamWriter sw = new StreamWriter(topicTermsPath);
            foreach (string topicName in newTopicTerms.Keys)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(topicName + ":");
                foreach (string term in newTopicTerms[topicName].Keys)
                {
                    sb.Append(term + "," + newTopicTerms[topicName][term]);
                    sb.Append(";");
                }

                sb.Remove(sb.Length - 1, 1);
                sw.WriteLine(sb.ToString());
            }

            sw.Close();
        }
    }
}

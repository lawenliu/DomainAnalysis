using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis.DocumentRanking.ClumpingRankDoc
{
    /* get the topic and the related terms
     */
    class ParseTopicTerms
    {
        
        public static Dictionary<string, List<string>> GetTopicTermList(string topicTermContent)
        {
            Dictionary<string, List<string>> topicTerms = new Dictionary<string, List<string>>();
            string[] separators = new string[] {"\r\n"};
            string[] lines = topicTermContent.Split(separators,StringSplitOptions.RemoveEmptyEntries);
            foreach (string aLine in lines)
            {
                if (aLine.Contains(":"))
                {
                    int commaIndex = aLine.IndexOf(":");
                    string topicName = aLine.Substring(0, commaIndex);
                    string termValueStr = aLine.Substring(commaIndex + 1);
                    string[] termValueList = termValueStr.Split(';');
                    List<string> termList = new List<string>();
                    foreach (string termValuePair in termValueList)
                    {
                        string[] termValue = termValuePair.Split(',');
                        string term = termValue[0].ToLower();
                        termList.Add(term);
                    }
                    topicTerms.Add(topicName,termList);
                }
            }
            return topicTerms;
        }

        public static Dictionary<string, Dictionary<string, float>> GetTopicTermValueList(string topicTermContent)
        {
            Dictionary<string, Dictionary<string, float>> topicTerms = new Dictionary<string, Dictionary<string, float>>();
            string[] separators = new string[] { "\r\n" };
            string[] lines = topicTermContent.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string aLine in lines)
            {
                if (aLine.Contains(":"))
                {
                    int commaIndex = aLine.IndexOf(":");
                    string topicName = aLine.Substring(0, commaIndex);
                    string termValueStr = aLine.Substring(commaIndex + 1);
                    string[] termValueList = termValueStr.Split(';');
                    Dictionary<string, float> termList = new Dictionary<string, float>();
                    foreach (string termValuePair in termValueList)
                    {
                        string[] termValue = termValuePair.Split(',');
                        string term = termValue[0].ToLower().Trim();
                        if (termList.ContainsKey(term))
                        {
                            continue;
                        }
                        else if (isGeneralize(term, termList)) // if 
                        {
                            continue;
                        }
                        
                        string weight = termValue[1];
                        float weightValue = float.Parse(weight);
                        termList.Add(term,weightValue);
                    }
                    topicTerms.Add(topicName, termList);
                }
            }
            return topicTerms;
        }

        private static bool isGeneralize(string aTerm, Dictionary<string, float> termList)
        {
            foreach (string tmpTerm in termList.Keys)
            {
                string exTmpTerm = " " + tmpTerm + " ";
                string exAterm = " " + aTerm + " ";
                if (exTmpTerm.Contains(exAterm) || exAterm.Contains(exTmpTerm))
                {
                   return true ;
                }
                
            }
            return false;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DomainAnalysis.Utils;

namespace DomainAnalysis.Highlight
{
    class ReadTargetTopicTerms
    {
        public static List<string> ParseTopicTerms(string topicTermPath, string targetTopicName)
        {
            List<string> topicTerms = new List<string>();
            string[] topicTermLines = FileOperators.ReadFileLines(topicTermPath);
            foreach (string line in topicTermLines)
            {
                if (line.StartsWith(targetTopicName))
                {
                    int colonIndex = line.IndexOf(':');
                    string termValues = line.Substring(colonIndex + 1);
                    if (termValues.Contains(";"))
                    {
                        string[] termValueList = termValues.Split(';');
                        foreach (string termValuePair in termValueList)
                        {
                            int commaIndex = termValuePair.IndexOf(',');
                            string term = termValuePair.Substring(0, commaIndex).ToLower().Trim();
                            topicTerms.Add(term);
                        }
                    }
                    else
                    {
                        int commaIndex = termValues.IndexOf(',');
                        string term = termValues.Substring(0, commaIndex).ToLower().Trim();
                        topicTerms.Add(term);
                    }
                }
            }
            return topicTerms;
        }
    }
}

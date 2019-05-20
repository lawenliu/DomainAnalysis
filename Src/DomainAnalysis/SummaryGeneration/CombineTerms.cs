using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DomainAnalysis.Utils;

namespace DomainAnalysis.SummaryGeneration
{
    public class CombineTerms
    {
        public Dictionary<string, List<string>> GetCombinedTerms(string termPath, string expanedTermPath)
        {
            Dictionary<string, List<string>> compTerms = new Dictionary<string, List<string>>();
            if (!string.IsNullOrEmpty(termPath))
            {
                string[] termLines = FileOperators.ReadFileLines(termPath);
                if (termLines == null)
                {
                    return null;
                }

                foreach (string line in termLines)
                {
                    int colonIndex = line.IndexOf(":");
                    if (colonIndex > 0)
                    {
                        string compName = line.Substring(0, colonIndex);
                        List<string> terms = new List<string>();
                        string termStr = line.Substring(colonIndex + 1);
                        string[] termPairs = termStr.Split(';');
                        foreach (string pair in termPairs)
                        {
                            int commaIndex = pair.IndexOf(',');
                            if (commaIndex > 0)
                            {
                                string term = pair.Substring(0, commaIndex).Trim();
                                terms.Add(term);
                            }
                        }
                        compTerms.Add(compName, terms);
                    }
                }
            }
            
            if (!string.IsNullOrEmpty(expanedTermPath))
            {
                string[] expandedLines = FileOperators.ReadFileLines(expanedTermPath);
                foreach (string expandedLine in expandedLines)
                {
                    int colonIndex = expandedLine.IndexOf(':');
                    if (colonIndex > 0)
                    {
                        string compName = expandedLine.Substring(0, colonIndex);
                        if (compTerms.ContainsKey(compName))
                        {
                            List<string> existedTerms = compTerms[compName];
                            string termStr = expandedLine.Substring(colonIndex + 1);
                            string[] terms = termStr.Split(',');
                            foreach (string term in terms)
                            {
                                if (!existedTerms.Contains(term.Trim()))
                                {
                                    existedTerms.Add(term.Trim());
                                }
                            }
                            compTerms[compName] = existedTerms;
                        }
                        else
                        {
                            Console.WriteLine("Please check the component:" + compName);
                        }
                    }
                }
            }
            
            return compTerms;
        }

        public Dictionary<string, Dictionary<string, float>> GetCombinedTermsWeights(string termPath, string expanedTermPath)
        {
            Dictionary<string, Dictionary<string, float>> compTerms = new Dictionary<string, Dictionary<string, float>>();

            string[] termLines = FileOperators.ReadFileLines(termPath);
            foreach (string line in termLines)
            {
                int colonIndex = line.IndexOf(":");
                if (colonIndex > 0)
                {
                    string compName = line.Substring(0, colonIndex);
                    Dictionary<string, float> terms = new Dictionary<string, float>();
                    string termStr = line.Substring(colonIndex + 1);
                    string[] termPairs = termStr.Split(';');
                    foreach (string pair in termPairs)
                    {
                        string[] termWeight = pair.Split(',');
                        if (termWeight.Count() == 2)
                        {
                            string term = termWeight[0].Trim();
                            string weightStr = termWeight[1];
                            float weight = float.Parse(weightStr);
                            if (!terms.ContainsKey(term))
                            {
                                terms.Add(term, weight);
                            }
                            
                        }
                    }
                    compTerms.Add(compName, terms);
                }
            }
            string[] expandedLines = FileOperators.ReadFileLines(expanedTermPath);
            foreach (string expandedLine in expandedLines)
            {
                int colonIndex = expandedLine.IndexOf(':');
                if (colonIndex > 0)
                {
                    string compName = expandedLine.Substring(0, colonIndex);
                    if (compTerms.ContainsKey(compName))
                    {
                        Dictionary<string, float> existedTerms = compTerms[compName];
                        string termStr = expandedLine.Substring(colonIndex + 1);
                        string[] terms = termStr.Split(',');
                        foreach (string term in terms)
                        {
                            if (!existedTerms.ContainsKey(term.Trim()))
                            {
                                existedTerms.Add(term.Trim(), 0.2f);
                            }
                        }
                        compTerms[compName] = existedTerms;
                    }
                    else
                    {
                        Console.WriteLine("Please check the component:" + compName);
                    }
                }
            }

            return compTerms;
        }
    }
}

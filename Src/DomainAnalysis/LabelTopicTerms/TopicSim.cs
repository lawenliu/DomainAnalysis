using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainAnalysis.Utils;
using System.IO;

namespace DomainAnalysis.LabelTopic
{
    class TopicSim
    {
        public static void CalTopicSimilarity(string topicTermsFileName, string topicSimFileName)
        {
            Dictionary<string, Dictionary<string, double>> topicTermPropMap = ParseTopicTerms(topicTermsFileName);
            int simLength = topicTermPropMap.Values.Count;
            double[,] simMatrix = new double[simLength, simLength];
            for (int indexFirst = 0; indexFirst < topicTermPropMap.Values.Count - 1; indexFirst++)
            {
                for(int indexSecond = indexFirst + 1; indexSecond < topicTermPropMap.Values.Count; indexSecond++)
                {
                    List<double> propList1 = new List<double>(topicTermPropMap.Values.ElementAt(indexFirst).Values);
                    List<double> propList2 = new List<double>(topicTermPropMap.Values.ElementAt(indexSecond).Values);
                    double simResult = JensenShannonDivergence(propList1, propList2);

                    simMatrix[indexFirst, indexSecond] = simResult;
                    simMatrix[indexSecond, indexFirst] = simResult;
                }
            }

            GenerateSimFile(simMatrix, topicSimFileName);
        }

        private static Dictionary<string, Dictionary<string, double>> ParseTopicTerms(string topicTermsFileName)
        {
            Dictionary<string, Dictionary<string, double>> topicTermPropMap = new Dictionary<string, Dictionary<string, double>>();
            string[] topicTermLines = FileOperators.ReadFileLines(topicTermsFileName);
            for (int index = 0; index < topicTermLines.Length; index++)
            {
                string line = topicTermLines[index];
                int colonIndex = line.IndexOf(':');
                string topicName = line.Substring(0, colonIndex);
                string termValues = line.Substring(colonIndex + 1);
                Dictionary<string, double> termProps = new Dictionary<string, double>();
                if (termValues.Contains(";"))
                {
                    string[] termValueList = termValues.Split(';');
                    foreach (string termValuePair in termValueList)
                    {
                        int commaIndex = termValuePair.IndexOf(',');
                        string term = termValuePair.Substring(0, commaIndex);
                        string propStr = termValuePair.Substring(commaIndex + 1);
                        termProps.Add(term, double.Parse(propStr));
                    }
                }

                if(!topicTermPropMap.ContainsKey(topicName))
                {
                    topicTermPropMap.Add(topicName, termProps);
                }                
            }

            return topicTermPropMap;
        }

        /**
         * Returns the Jensen-Shannon divergence.
         */
        private static double JensenShannonDivergence(List<double> p1, List<double> p2)
        {
            List<double> average = new List<double>();
            for (int i = 0; i < p1.Count; ++i)
            {
                average.Add((p1[i] + p2[i]) / 2);
            }

            return (KlDivergence(p1, average) + KlDivergence(p2, average)) / 2;
        }

        private static double log2 = Math.Log(2);

        /**
         * Returns the KL divergence, K(p1 || p2).
         * 
         * The log is w.r.t. base 2.
         * <p>
         * *Note*: If any value in <tt>p2</tt> is <tt>0.0</tt> then the
         * KL-divergence is <tt>infinite</tt>. Limin changes it to zero instead of
         * infinite.
         */
        private static double KlDivergence(List<double> p1, List<double> p2)
        {
            double klDiv = 0.0;
            for (int i = 0; i < p1.Count; ++i) {
                if (p1[i] == 0) {
                    continue;
                }
                if (p2[i] == 0.0) {
                    continue;
                } // Limin

                klDiv += p1[i] * Math.Log(p1[i] / p2[i]);
            }
            return klDiv / log2; // moved this division out of the loop -DM
        }

        private static void GenerateSimFile(double[,] simMatrix, string simMatrixFileName)
        {
            StreamWriter sw = new StreamWriter(simMatrixFileName);

            for (int indexFirst = 0; indexFirst < Math.Sqrt(simMatrix.Length); indexFirst++)
            {
                String lineSim = string.Empty;
                for (int indexSecond = 0; indexSecond < Math.Sqrt(simMatrix.Length); indexSecond++)
                {
                    lineSim += simMatrix[indexFirst, indexSecond] + ","; 
                }

                lineSim = lineSim.Remove(lineSim.Length - 1);
                sw.WriteLine(lineSim);
            }

            sw.Close();
        }
    }
}

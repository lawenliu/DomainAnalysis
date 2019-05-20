using DomainAnalysis.DataPrepare;
using DomainAnalysis.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis.LabelTopicTerms
{
    class PrepareTopicFile
    {
        /*from the results of tmt, find the topicNum with minimum perplexity
         */
        public static int GetMinimumTopicNumber()
        {
            try
            {
                string tmtOutputInfo = FileMg.AutoTmtOutputFileDir + Constants.TmtOutputInfoFileName;
                StreamReader sr = new StreamReader(tmtOutputInfo);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] segs = line.Split(':');
                    if (segs.Length == 2)
                    {
                        if (segs[0].Equals(Constants.TmtMinimumTopicKey))
                        {
                            return Int32.Parse(segs[1].Trim());
                        }
                    }
                }
            }
            catch
            { 
            }

            return -1;
        }

        /*parse the summary.txt file of the standford tmt results
         */
        public static void Execute(string sourceFilePath, string destFilePath)
        {
            if (File.Exists(destFilePath))
            {
                File.Delete(destFilePath);
            }

            string topicContent = FileOperators.ReadFileText(sourceFilePath);
            string[] stringSeparators = new string[] { "\r\n\n\r\n" };
            string[] topicParts = topicContent.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            foreach (string topicPart in topicParts)
            {
                string[] topicPartSeparators = new string[] { "\r\n" };
                string[] lines = topicPart.Split(topicPartSeparators, StringSplitOptions.RemoveEmptyEntries);
                string singleTopicContent = "";
                float topicSumWeight = 0f;
                foreach (string line in lines)
                {
                    if (line.StartsWith("Topic"))
                    {
                        int spaceIndex = line.IndexOf("\t\t");
                        string topicId = line.Substring(0, spaceIndex);
                        string topicWeight = line.Substring(spaceIndex + 2);
                        topicSumWeight = float.Parse(topicWeight);
                        singleTopicContent += topicId + ":";
                    }
                    else
                    {
                        string trimedLine = line.Trim();
                        string[] terms = trimedLine.Split('\t');
                        string term = terms[0];
                        string relevance = terms[1];
                        float relevanceValue = float.Parse(relevance);
                        relevanceValue = relevanceValue / topicSumWeight;
                        singleTopicContent += term + "," + relevanceValue + ";";
                    }
                }

                if (singleTopicContent != "")
                {
                    singleTopicContent = singleTopicContent.Remove(singleTopicContent.Length - 1);
                    FileOperators.FileAppend(destFilePath, singleTopicContent);
                }
            }
        }
    }
}

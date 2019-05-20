using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Office.Interop.Word;
using Microsoft.Office.Core;
using System.Reflection;
using DomainAnalysis.Utils;

namespace DomainAnalysis.LabelTopicTerms
{
    class LabelDomainTopic
    {
        public static void Execute(string sourceFileDir, string topicTermFilePath, string summaryFilePath, string destFilePath)
        {
            Dictionary<string, List<string>> topicTerms = ParseTopicTerms(topicTermFilePath);
            List<TopicParaFreq> topicParaFreqList = GenerateFrequency(sourceFileDir, topicTerms);
            GenerateLabelFile(topicParaFreqList, summaryFilePath, destFilePath);
        }

        private static List<TopicParaFreq> GenerateFrequency(string sourceDirName, Dictionary<string, List<string>> topicTerms)
        {
            List<TopicParaFreq> topicParaFreqList = new List<TopicParaFreq>();

            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Name.EndsWith(".doc") || file.Name.EndsWith(".docx"))
                {
                    topicParaFreqList = AddTopicFreq(file.FullName, topicParaFreqList, topicTerms);
                }
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (DirectoryInfo subdir in dirs)
            {
                topicParaFreqList = GenerateFrequency(subdir.FullName, topicTerms);
            }

            return topicParaFreqList;
            
        }

        private static List<TopicParaFreq> AddTopicFreq(string filePath, List<TopicParaFreq> topicParaFreqList, Dictionary<string, List<string>> topicTerms)
        {
            var app = new Application();
            app.Visible = false;
            
            try
            {
                var doc = app.Documents.Open(filePath);
                string headingName = "";
                List<string> paras = new List<string>();
                //  doc.Activate();
                foreach (Paragraph paragraph in doc.Paragraphs)
                {
                    Style style = paragraph.get_Style() as Microsoft.Office.Interop.Word.Style;
                    string styleName = style.NameLocal;
                    string text = paragraph.Range.Text;

                    if (styleName.StartsWith("Heading"))
                    {
                        //Console.WriteLine("heading:" + text.ToString());
                        if (headingName.Length > 0 && paras.Count > 0 && !headingName.Contains("Glossary") && !headingName.Contains("Test") && !headingName.Contains("History") && !headingName.Contains("Architecture") && !headingName.Contains("General") && !headingName.Equals("Applicable Documents") && !headingName.Contains("Acronyms"))
                        {
                            //find the numbers related with keywords, here we fetch the frequency of keywords
                            topicParaFreqList = UpdateDictionary(topicParaFreqList, topicTerms, headingName, paras);
                        }

                        headingName = text.Trim();
                        paras.Clear();

                    }
                    else if (headingName.Length > 0)
                    {
                        paras.Add(text);
                    }
                }

                ((_Document)doc).Close();
            }
            finally
            {                
                ((_Application)app).Quit();
            }

            return topicParaFreqList;
        }

        private static List<TopicParaFreq> UpdateDictionary(List<TopicParaFreq> topicParaFreqList,
            Dictionary<string, List<string>> topicTerms, string paraHeading, List<string> textlist)
        {
            Console.WriteLine("updating paraheading:" + paraHeading);

            foreach (string topicId in topicTerms.Keys)
            {
                List<string> terms = topicTerms[topicId];
                int freq = CalKeywordFreqInPara(terms, textlist);

                if (freq == 0)
                {
                    continue;
                }


                int existingFreq = 0;

                
                foreach (TopicParaFreq topicParaFreq in topicParaFreqList)
                {
                    string curTopicID = topicParaFreq.topicId;
                    if (curTopicID.Equals(topicId))
                    {
                        existingFreq = topicParaFreq.freq;
                        if (existingFreq < freq)
                        {
                            topicParaFreq.freq = freq;
                            topicParaFreq.paraHeading = paraHeading;
                        }
                        break;
                    }
                }

                if (existingFreq == 0)
                {
                    TopicParaFreq tmpStruct = new TopicParaFreq();
                    tmpStruct.topicId = topicId;
                    tmpStruct.paraHeading = paraHeading;
                    tmpStruct.freq = freq;
                    topicParaFreqList.Add(tmpStruct);
                }
            }

            return topicParaFreqList;
        }

        //here,we get the paragraph, although we have not used them
        private static int CalKeywordFreqInPara(List<string> term, List<string> textList)
        {
            int freq = 0;
            foreach (string para in textList)
            {
                foreach (string keyword in term)
                {
                    string temp = para.ToLower();
                    if (para.ToLower().Contains(keyword))
                    {
                        freq++;
                    }
                }
            }
            return freq;
        }

        private static Dictionary<string, List<string>> ParseTopicTerms(string topicTermFilePath)
        {
            Dictionary<string, List<string>> topicTerms = new Dictionary<string, List<string>>();
            string[] topicTermLines = FileOperators.ReadFileLines(topicTermFilePath);
            foreach (string line in topicTermLines)
            {
                int colonIndex = line.IndexOf(':');
                string topicID = line.Substring(0, colonIndex);
                string termValues = line.Substring(colonIndex + 1);
                List<string> terms = new List<string>();
                if (termValues.Contains(";"))
                {
                    string[] termValueList = termValues.Split(';');
                    foreach (string termValuePair in termValueList)
                    {
                        int commaIndex = termValuePair.IndexOf(',');
                        string term = termValuePair.Substring(0, commaIndex);
                        terms.Add(term);
                    }
                }

                topicTerms.Add(topicID, terms);                
            }

            return topicTerms;
        }

        private static void GenerateLabelFile(List<TopicParaFreq> topicParaFreqList, string summaryFilePath, string outputFilePath)
        {
            Dictionary<string, TopicParaFreq> dictTPF = new Dictionary<string, TopicParaFreq>();
            foreach (TopicParaFreq topicParaFreq in topicParaFreqList)
            {
                dictTPF.Add(topicParaFreq.topicId, topicParaFreq);            
            }

            Dictionary<string, double> dictTopicWeight = new Dictionary<string, double>();
            StreamReader sr = new StreamReader(summaryFilePath);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line.StartsWith("Topic"))
                {
                    string[] segs = line.Split('\t');
                    if (segs.Length == 3)
                    {
                        dictTopicWeight.Add(segs[0].Trim(), double.Parse(segs[2].Trim()));
                    }
                }
            }

            sr.Close();

            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }

            StreamWriter sw = new StreamWriter(outputFilePath);
            foreach (string topicId in dictTopicWeight.Keys)
            {
                string line = topicId + ":" + dictTPF[topicId].paraHeading + ":" + dictTopicWeight[topicId];
                sw.WriteLine(line);
            }            

            sw.Close();
        }
    }
}

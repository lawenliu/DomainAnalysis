using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using DomainAnalysis.Utils;
using DomainAnalysis.DocumentRanking.ClumpingRankDoc;

namespace DomainAnalysis.DocumentRanking.ClumpingRankDoc
{
    //In this class, we try another idea to do the ranking. But finally we didn't use it in our paper.
    //only maintain the term frequency in each window and don't consider the term diversity. That's to say, we re-calculate the density of each window
    class RankingDocByClumpingCaleb
    {
        private string topicTermsFilePath = @""; //the file includes the topics and the related terms
        private string txtCleanFileDir = @""; //the project files
        //  private string topicLabelFilePath = @"";
        private string rankResult = @""; //the result of ranking

        private int _windowSize = 200; //twice as the size of keywords. Maybe we need to alter this value later
        Dictionary<string, Dictionary<string, float>> docTopicDensityMap;//map between one docPath and all topics info including topicName and the density
        Dictionary<string, Dictionary<string, float>> fileNameTopicDensityMap;//map between one fileName and all topics info including topicName and the maxDensity
       //add the above two similar variables because there are redundant files in different directory
        
        int filePathLength;

       // Dictionary<string, List<string>> topicTerms;
        Dictionary<string, Dictionary<string, float>> topicTerms;
       Dictionary<string, int> topicTermsInWindow = null;//record the frequency of topic terms in a specific window
       List<string> topicTermList; // store all of the terms that are indicative of all topics

         public RankingDocByClumpingCaleb(string topicTermsFilePath, string txtCleanFileDir,  string outputSimilarityFilePath)
        {
            this.topicTermsFilePath = topicTermsFilePath;
            this.txtCleanFileDir = txtCleanFileDir;
            rankResult = outputSimilarityFilePath;
            filePathLength = this.txtCleanFileDir.Length;
        }

        //find the density of all topics in each document
        //files in two levels
         public void DoClumpingRank()
         {
             if (File.Exists(rankResult))
             {
                 File.Delete(rankResult);
             }

             string topicTermContent = FileOperators.ReadFileText(topicTermsFilePath).ToLower();
             topicTerms = ParseTopicTerms.GetTopicTermValueList(topicTermContent);
             GetAllTopicTerms(topicTerms);

             docTopicDensityMap = new Dictionary<string, Dictionary<string, float>>();
             fileNameTopicDensityMap = new Dictionary<string, Dictionary<string, float>>();

             string[] fileEntities = Directory.GetFiles(txtCleanFileDir);
             CalDocDensityMap(fileEntities);
             string[] dirs = Directory.GetDirectories(txtCleanFileDir);
             foreach (string dir in dirs)
             {
                 string[] subFileEntities = Directory.GetFiles(dir);
                 CalDocDensityMap(subFileEntities);
             }

             WriteRankingResult();
         }

        //parse the dictionary of topic and their terms, and get a list of all terms related with all topics
         private void GetAllTopicTerms(Dictionary<string, Dictionary<string, float>> topicTerms)
         {
             topicTermList = new List<string>();
             foreach (string topicName in topicTerms.Keys)
             {
                 Dictionary<string, float> termWeightList = new Dictionary<string, float>();
                 List<string> curTopicTerms = topicTerms[topicName].Keys.ToList();
                 foreach (string curTopicTerm in curTopicTerms)
                 {
                     if (topicTermList.Contains(curTopicTerm))
                     {
                         continue;
                     }
                     else
                     {
                         topicTermList.Add(curTopicTerm);
                     }
                 }
             }
         }


         private void WriteRankingResult()
         {
             Dictionary<string, Dictionary<string, float>> topicDocDensity = new Dictionary<string, Dictionary<string, float>>();
            
             foreach (string doc in docTopicDensityMap.Keys) //transfer the primaryKey from docId to topicId
             {
                 Dictionary<string, float> topicDensity = docTopicDensityMap[doc];
                 foreach (string topicName in topicDensity.Keys)
                 {
                     float density = topicDensity[topicName];
                     if (topicDocDensity.ContainsKey(topicName))
                     {
                         topicDocDensity[topicName].Add(doc,density);
                     }
                     else
                     {
                         Dictionary<string, float> docDensity = new Dictionary<string,float>();
                         docDensity.Add(doc,density);
                         topicDocDensity.Add(topicName, docDensity);
                     }
                 }
             }

             foreach (string topicName in topicDocDensity.Keys)
             {
                 Dictionary<string, float> relevantDocDensity = topicDocDensity[topicName];
                 Dictionary<string, float> sortedDocDensity = DictionaryDecreasedSort.DecreasedByValue(relevantDocDensity);
                 string docAndDensity = "";
                 foreach (string key in sortedDocDensity.Keys)
                 {
                     string fileName = key.Substring(filePathLength);
                     float freq = sortedDocDensity[key];
                     if (freq > 0)
                     {
                         docAndDensity += topicName + "\t" + fileName + "\t" + sortedDocDensity[key] + "\r\n";
                     }
                 }
                 FileOperators.FileAppend(rankResult,docAndDensity);
             }
         }


         private void CalDocDensityMap(string[] fileEntities)
         {
             foreach (string fileEntity in fileEntities)
             {
                 int lastSlashIndex = fileEntity.LastIndexOf('\\');
                 string fileName = fileEntity.Substring(lastSlashIndex + 1);
                 if (!fileNameTopicDensityMap.ContainsKey(fileName))
                 {
                     string fileContent = FileOperators.ReadFileText(fileEntity).Replace("\n", " ").ToLower();
                     char[] delimiters = new char[] { ' '};
                     string[] tmpList = fileContent.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                     List<string> fileTermList = new List<string>();
                     foreach (string tmpTerm in tmpList)
                     {
                         if (tmpTerm.Length > 1)
                         {
                             fileTermList.Add(tmpTerm);
                         }
                     }

                     Dictionary<string, float> topicDensity = CalWindowDensityInOneFile(fileTermList); //get the topic and the related density for one file
                     
                    fileNameTopicDensityMap.Add(fileName, topicDensity);
                    docTopicDensityMap.Add(fileEntity, topicDensity);
                 }
                 else
                 {
                     Dictionary<string, float> topicDensity = fileNameTopicDensityMap[fileName];
                     docTopicDensityMap.Add(fileEntity, topicDensity);
                 }
             }
         }

         //find the density of all topics in one file
         private Dictionary<string, float> CalWindowDensityInOneFile(List<string> fileTermList)
         {
             int docTermLength = fileTermList.Count;
             int startIndex = 0;
             int backGuard = startIndex + _windowSize - 1;
             int endIndex = backGuard >= docTermLength - 1 ? docTermLength - 1 : backGuard;

             topicTermsInWindow = new Dictionary<string, int>(); // used to record the topic terms in a window
             Dictionary<string, float> windowDensity = new Dictionary<string,float>(); //maintain the max density of each topic in the current document

             for (; endIndex <= docTermLength - 1; startIndex++, endIndex++)
             {
                 CalDensityInCurWindow(fileTermList, startIndex, endIndex, windowDensity); //calculate the density of current window, update the topicTermInWindow (term and its frequency)
             
             }

             return windowDensity;
         }

         private string GetWindowStr(int startIndex, int endIndex, List<string> fileTermList)
         {
             string windowStr = "";
             for (int i = startIndex; i <= endIndex; i++)
             {
                 windowStr += fileTermList[i] + " ";
             }
             return windowStr;
         }

        //calculate the density of the current window. 
        //1. update the frequency of toic term in a window. Take away the FirstTerm in the last window, and add the last term in this window
        //2. calculate the density in this window
         private void CalDensityInCurWindow(List<string> fileTermList, int startIndex, int endIndex, Dictionary<string, float> windowDensity)
         {

             UpdateTopicTermInWindow(startIndex, endIndex, fileTermList);

             CalculateDensity(windowDensity);
             
         }

         private void UpdateTopicTermInWindow(int startIndex, int endIndex, List<string> fileTermList)
         {
             string windowStr = GetWindowStr(startIndex, endIndex, fileTermList);

             if (startIndex == 0) //for the first window
             {
                 foreach (string topicTerm in topicTermList)
                 {
                     if (topicTermsInWindow.ContainsKey(topicTerm))
                     {
                         continue;
                     }
                     else  //if current term has been calculated
                     {
                         string filteredStr;
                         int freq = 0;

                         filteredStr = windowStr.Replace(" " + topicTerm + " ", " ");
                         int freq_o = (windowStr.Length - filteredStr.Length) / (topicTerm.Length + 1);
                         string secondFilter = filteredStr.Replace(" " + topicTerm + "s ", " ");
                         int freq_s = (filteredStr.Length - secondFilter.Length) / (topicTerm.Length + 2);
                         freq = freq_o + freq_s;
                         windowStr = secondFilter;
                         topicTermsInWindow.Add(topicTerm, freq);
                     }
                 }
             }
             else if (startIndex > 0) // for the remaining windows
             {
                 string startHilight = isStartPosHighlight(topicTermList, fileTermList, startIndex - 1);
                 if (startHilight.Length > 0) //topicTermList is the set of all terms related with all topics
                 {
                     topicTermsInWindow[startHilight] -= 1;
                 }
                 string lastHilight = isLastPosHighlight(topicTermList, fileTermList, endIndex);
                 if (lastHilight.Length > 0)
                 {
                     topicTermsInWindow[lastHilight] += 1;
                 }
             }
         }

         private void CalculateDensity(Dictionary<string, float> windowDensity)
         {
             foreach (string topicName in topicTerms.Keys)
             {
                 Dictionary<string, float> curTopicTerms = topicTerms[topicName];

                 float topic_density = 0;
                 int diversity = 0;

                 foreach (string curTopicTerm in curTopicTerms.Keys)
                 {
                     int curTermFreq = topicTermsInWindow[curTopicTerm];

                     if (curTermFreq > 0)
                     {
                         diversity++;
                         float termWeight = curTopicTerms[curTopicTerm];
                         topic_density += (float)Math.Sqrt(curTermFreq) * termWeight;
                     }
                 }
               //  topic_density = 0.67f * (float)Math.Pow(diversity,1.5) * topic_density;
                 topic_density = diversity * topic_density;
                 float maxDensity = 0;
                 if (windowDensity.ContainsKey(topicName))
                 {
                     maxDensity = windowDensity[topicName];
                 }
                
                 if (topic_density > maxDensity)
                 {
                     windowDensity[topicName] = topic_density;
                 }
             }
         }

         private string isLastPosHighlight(List<string> topicTermList, List<string> fileTermList, int endIndex)
         {
             string lastPos = fileTermList[endIndex];
             foreach (string topicTerm in topicTermList)
             {
                 int topicTermLength = topicTerm.Split(' ').Length;
                 if (topicTermLength == 1)
                 {
                     if (topicTerm.Equals(lastPos))
                     {
                         return topicTerm;
                     }
                 }
                 else if (topicTermLength > 1 && topicTerm.EndsWith(lastPos))
                 {
                     string windowEndStr = "";
                     for (int i = endIndex - topicTermLength + 1; i <= endIndex; i++)
                     {
                         windowEndStr += fileTermList[i] + " ";
                     }
                     if (windowEndStr.Trim().Equals(topicTerm) || windowEndStr.Trim().Equals(topicTerm + "s"))
                     {
                         return topicTerm;
                     }
                 }
             }
             return "";
         }

         private string isStartPosHighlight(List<string> topicTermList, List<string> fileTermList, int startIndex)
         {
             string startPos = fileTermList[startIndex];
             foreach (string topicTerm in topicTermList)
             {
                 int topicTermLength = topicTerm.Split(' ').Length;
                 if (topicTermLength == 1)
                 {
                     if (topicTerm.Equals(startPos))
                     {
                         return topicTerm;
                     }
                 }
                 else if (topicTermLength > 1 && topicTerm.StartsWith(startPos))
                 {
                     string windowStartStr = "";
                     for (int i = 0; i < topicTermLength; i++)
                     {
                         windowStartStr += fileTermList[startIndex + i] + " ";
                     }
                     if (windowStartStr.Trim().Equals(topicTerm) || windowStartStr.Trim().Equals(topicTerm + "s"))
                     {
                         return topicTerm;
                     }
                 }
             }
             return "";
         }
    }
}

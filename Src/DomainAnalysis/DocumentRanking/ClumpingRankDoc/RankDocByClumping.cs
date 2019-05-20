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
    class RankDocByClumping
    {
      
        private string topicTermsFilePath = @""; //the file includes the topics and the related terms
        private string txtCleanFileDir = @""; //the project files
        //  private string topicLabelFilePath = @"";
        private string rankResult = @""; //the result of ranking
        //private string tempWindowDensityPath = @"C:\Users\xlian\MyPapers\Simmons\docRank\clumpingTemp\vsm";

        private int _windowSize = 200; //twice as the size of keywords. Maybe we need to alter this value later
        Dictionary<string, Dictionary<string, int>> docTopicDensityMap;//map between one docPath and all topics info including topicName and the density
        Dictionary<string, Dictionary<string, int>> fileNameTopicDensityMap;//map between one fileName and all topics info including topicName and the maxDensity
       //add the above two similar variables because there are redundant files in different directory
        
        int filePathLength;

        Dictionary<string, List<string>> topicTerms;

         public RankDocByClumping(string topicTermsFilePath, string txtCleanFileDir,  string outputSimilarityFilePath)
        {
            this.topicTermsFilePath = topicTermsFilePath;
            this.txtCleanFileDir = txtCleanFileDir;
            rankResult = outputSimilarityFilePath;
            filePathLength = this.txtCleanFileDir.Length + 1;
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
             topicTerms = ParseTopicTerms.GetTopicTermList(topicTermContent);

             docTopicDensityMap = new Dictionary<string, Dictionary<string, int>>();
             fileNameTopicDensityMap = new Dictionary<string, Dictionary<string, int>>();

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

         private void WriteRankingResult()
         {
             Dictionary<string, Dictionary<string, int>> topicDocDensity = new Dictionary<string, Dictionary<string, int>>();
            
             foreach (string doc in docTopicDensityMap.Keys) //transfer the primaryKey from docId to topicId
             {
                 Dictionary<string, int> topicDensity = docTopicDensityMap[doc];
                 foreach (string topicName in topicDensity.Keys)
                 {
                     int density = topicDensity[topicName];
                     if (topicDocDensity.ContainsKey(topicName))
                     {
                         topicDocDensity[topicName].Add(doc,density);
                     }
                     else
                     {
                         Dictionary<string, int> docDensity = new Dictionary<string,int>();
                         docDensity.Add(doc,density);
                         topicDocDensity.Add(topicName, docDensity);
                     }
                 }
             }

             foreach (string topicName in topicDocDensity.Keys)
             {
                 Dictionary<string, int> relevantDocDensity = topicDocDensity[topicName];
                 Dictionary<string, int> sortedDocDensity = DictionaryDecreasedSort.DecreasedByValue(relevantDocDensity);
                 string docAndDensity = "";
                 foreach (string key in sortedDocDensity.Keys)
                 {
                     string fileName = key.Substring(filePathLength);
                     int freq = sortedDocDensity[key];
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
                     List<string> fileTermList = new List<string>(fileContent.Split(' '));
                     Dictionary<string, int> topicDensity = CalWindowDensityInOneFile(fileTermList); //get the topic and the related density for one file
                     
                    fileNameTopicDensityMap.Add(fileName, topicDensity);
                    docTopicDensityMap.Add(fileEntity, topicDensity);
                     
                 }
                 else
                 {
                     Dictionary<string, int> topicDensity = fileNameTopicDensityMap[fileName];
                     docTopicDensityMap.Add(fileEntity, topicDensity);
                 }
             }
         }


         //find the density of all topics in one file
         private Dictionary<string, int> CalWindowDensityInOneFile(List<string> fileTermList)
         {
             int docTermLength = fileTermList.Count;
             int startIndex = 0;
             int backGuard = startIndex + _windowSize - 1;
             int endIndex = backGuard >= docTermLength - 1 ? docTermLength - 1 : backGuard;

             Dictionary<string, int> windowDensity = CalDensityInOneWindow(fileTermList, startIndex, endIndex); //first window
             string firstWord = fileTermList[startIndex];
             Dictionary<string, int> density_toNext = GetDensityToNext(windowDensity, startIndex,fileTermList); //for next replace

             startIndex++;
             endIndex++;

             for (; endIndex <= docTermLength - 1; startIndex++, endIndex++)
             {
                 string lastPos = fileTermList[endIndex];
                 Dictionary<string, int> curDensity = GetCurWindowUpdate(density_toNext, endIndex, windowDensity,fileTermList); //critical update
                 // UpdateWindowDensity(curDensity, windowDensity);
                 string firstPos = fileTermList[startIndex];
                 density_toNext = GetDensityToNext(curDensity, startIndex,fileTermList);
             }

             return windowDensity;
         }

        //private  void CalAndUpdateDensityInOneWindow(List<string> fileTermList, int startIndex, int endIndex, Dictionary<string, int> windowDensity)
        // {
        //     Dictionary<string, int> topicDensity_aWindow = new Dictionary<string,int>();
        //     Dictionary<string, int> termDensity_aWindow = new Dictionary<string, int>(); //term and the related frequency in one specific window
        //     string windowStr = "";
        //     for(int i = startIndex; i <= endIndex; i++)
        //     {
        //         windowStr += fileTermList[i] + " ";
        //     }

        //    foreach (string topicName in topicTerms.Keys)
        //    {
                
        //        List<string> topicTermList = topicTerms[topicName];
        //        int topic_density = 0;
        //        foreach (string topicTerm in topicTermList)
        //        {
        //            if (topicTerm.StartsWith("["))
        //            {
        //                continue;
        //            }
        //            if (termDensity_aWindow.ContainsKey(topicTerm)) //if current term has been calculated
        //            {
        //                topic_density += termDensity_aWindow[topicTerm];
        //            }
        //            else //compute the freq of current term in the file
        //            {
        //                string filteredStr;
        //                int freq = 0;
        //                filteredStr = windowStr.Replace(" " + topicTerm + " ", " "); //edit
        //                freq = (windowStr.Length - filteredStr.Length) / (topicTerm.Length + 1);
        //                //if (topicTerm.StartsWith(" "))
        //                //{
        //                //    filteredStr = windowStr.Replace(topicTerm, " ");
        //                //    freq = (windowStr.Length - filteredStr.Length) / (topicTerm.Length - 1);
        //                //}
        //                //else
        //                //{
        //                //    filteredStr = windowStr.Replace(" " + topicTerm + " ", " "); //edit
        //                //    freq = (windowStr.Length - filteredStr.Length) / (topicTerm.Length + 1);
        //                //}
                        
        //                topic_density += freq;
        //                termDensity_aWindow.Add(topicTerm,freq);

        //                windowStr = filteredStr; //reduce the length the string to save the match time
        //            }
        //        }

        //        if (windowDensity.ContainsKey(topicName))
        //        {
        //            int density = windowDensity[topicName];
        //            if (topic_density > density)
        //            {
        //                windowDensity[topicName] = topic_density;
        //            }
        //        }
        //        else
        //        {
        //            windowDensity.Add(topicName,topic_density);

        //        }
        //        topicDensity_aWindow.Add(topicName,topic_density);
        //    }
                
        // }


         private Dictionary<string, int> CalDensityInOneWindow(List<string> fileTermList, int startIndex, int endIndex)
         {
             Dictionary<string, int> topicDensity_aWindow = new Dictionary<string,int>();
             Dictionary<string, int> termDensity_aWindow = new Dictionary<string, int>(); //term and the related frequency in one specific window
             string windowStr = " ";
             for(int i = startIndex; i <= endIndex; i++)
             {
                 windowStr += fileTermList[i] + " ";
             }
             //calculate the first circle

            foreach (string topicName in topicTerms.Keys)
            {
                
                List<string> topicTermList = topicTerms[topicName];
                int topic_density = 0;
                foreach (string topicTerm in topicTermList)
                {
                    if (topicTerm.StartsWith("["))
                    {
                        continue;
                    }
                    if (termDensity_aWindow.ContainsKey(topicTerm)) //if current term has been calculated
                    {
                        topic_density += termDensity_aWindow[topicTerm];
                    }
                    else //compute the freq of current term in the file
                    {
                        string filteredStr;
                        int freq = 0;

                        filteredStr = windowStr.Replace(" " + topicTerm + " ", " "); //edit
                        freq = (windowStr.Length - filteredStr.Length) / (topicTerm.Length + 1);

                        //if (topicTerm.StartsWith(" "))
                        //{
                        //    filteredStr = windowStr.Replace(topicTerm, " ");
                        //    freq = (windowStr.Length - filteredStr.Length) / (topicTerm.Length - 1);
                        //}
                        //else
                        //{
                        //    filteredStr = windowStr.Replace(" " + topicTerm + " ", " ");
                        //    freq = (windowStr.Length - filteredStr.Length) / (topicTerm.Length + 1);
                        //}
                        
                        topic_density += freq;
                        termDensity_aWindow.Add(topicTerm,freq);

                        windowStr = filteredStr; //reduce the length the string to save the match time
                    }
                }

                topicDensity_aWindow.Add(topicName,topic_density);
            }
                
             return topicDensity_aWindow;
         }

        Dictionary<string, int> GetDensityToNext(Dictionary<string, int> allDensity, int startIndex, List<string> fileTermList)
        {
            Dictionary<string, int> toNext = new Dictionary<string,int>();
            foreach(string topicName in topicTerms.Keys)
            {
                List<string> topicTermList = topicTerms[topicName];
                int density = allDensity[topicName];
                if(isStartPosHighlight(topicTermList,fileTermList, startIndex))
                {
                    toNext.Add(topicName,density - 1);
                }
                else
                {
                    toNext.Add(topicName,density);
                }
            }
            return toNext;
        }

        private bool isLastPosHighlight(List<string> topicTermList, List<string> fileTermList, int endIndex)
        {
            string lastPos = fileTermList[endIndex];
            foreach (string topicTerm in topicTermList)
            {
                int topicTermLength = topicTerm.Split(' ').Length;
                if (topicTermLength == 1)
                {
                    if (topicTerm.Equals(lastPos))
                    {
                        return true;
                    }
                }
                else if (topicTermLength > 1 && topicTerm.EndsWith(lastPos))
                {
                    string windowEndStr = "";
                    for (int i = endIndex - topicTermLength + 1; i <= endIndex; i++)
                    {
                        windowEndStr += fileTermList[i] + " ";
                    }
                    if (windowEndStr.Equals(topicTerm))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool isStartPosHighlight(List<string> topicTermList, List<string> fileTermList, int startIndex)
        {
            string startPos = fileTermList[startIndex];
            foreach (string topicTerm in topicTermList)
            {
                int topicTermLength = topicTerm.Split(' ').Length;
                if (topicTermLength == 1)
                {
                    if (topicTerm.Equals(startPos))
                    {
                        return true;
                    }
                }
                else if(topicTermLength > 1 && topicTerm.StartsWith(startPos))
                {
                    string windowStartStr = "";
                    for (int i = 0; i < topicTermLength; i++)
                    {
                        windowStartStr += fileTermList[startIndex + i] + " ";
                    }
                    if (windowStartStr.Equals(topicTerm))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private Dictionary<string, int> GetCurWindowUpdate(Dictionary<string, int> toNext, int endIndex, Dictionary<string, int> windowDensity, List<string> fileTermList)
        {
            Dictionary<string, int> curDensity = new Dictionary<string, int>();
            foreach (string topicName in topicTerms.Keys)
            {
                List<string> topicTermList = topicTerms[topicName];
                int density = toNext[topicName];
                if(isLastPosHighlight(topicTermList,fileTermList,endIndex))
                {
                    density += 1;
                }
                curDensity.Add(topicName, density);
                int maxDensity = windowDensity[topicName];
                if (density > maxDensity)
                {
                    windowDensity[topicName] = density;
                }
            }
            return curDensity;
        }



    }
}

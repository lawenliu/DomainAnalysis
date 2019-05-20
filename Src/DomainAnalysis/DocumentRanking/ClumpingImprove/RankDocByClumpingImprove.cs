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
    /*
     * by adding the impact of term variable on the ranking
     * density = freq * termType in a window
     * Because we use the same ranking algorithm for both the automated and manual model, we use topic to refer to component in source code.
     */
    class RankDocByClumpingImprove
    {
        private string topicTermsFilePath = @""; //the file includes the components and the related terms
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

         public RankDocByClumpingImprove(string topicTermsFilePath, string txtCleanFileDir,  string outputSimilarityFilePath)
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

          //   Dictionary<string, List<string>> topicTermsInWindow = new Dictionary<string, List<string>>(); // used to record the topic terms in a window
             Dictionary<string, int> topicMaxDensityEndindex = new Dictionary<string, int>(); //record the max density endIndex of each topic
             Dictionary<string, float> windowDensity = CalDensityInOneWindow(fileTermList, startIndex, endIndex, topicMaxDensityEndindex); //first window
             string firstWord = fileTermList[startIndex];
             Dictionary<string, float> density_toNext = GetDensityToNext(windowDensity, startIndex,fileTermList); //for next replace

             startIndex++;
             endIndex++;

             for (; endIndex <= docTermLength - 1; startIndex++, endIndex++)
             {
                 string lastPos = fileTermList[endIndex];
                 Dictionary<string, float> curDensity = GetCurWindowUpdate(density_toNext, endIndex, windowDensity,fileTermList, topicMaxDensityEndindex); //critical update
                 string firstPos = fileTermList[startIndex];
                 density_toNext = GetDensityToNext(curDensity, startIndex,fileTermList);
             }
             //update windowDensity
             Dictionary<string, float> updatedWindowDensity = new Dictionary<string, float>();
             Dictionary<string, float> termSizeInMaxWindow = GetTermsSize(fileTermList, topicMaxDensityEndindex);
             foreach (string topicName in windowDensity.Keys)
             {
                 float termSize = termSizeInMaxWindow[topicName];
                 float density = windowDensity[topicName] * termSize;
                 updatedWindowDensity.Add(topicName, density);
             }
             return updatedWindowDensity;
             //return windowDensity;
         }

         private Dictionary<string, float> GetTermsSize(List<string> fileTermList, Dictionary<string, int> topicMaxDensityEndindex)
         {
             Dictionary<string, float> termSizeInMaxwindow = new Dictionary<string, float>();
             foreach (string topicName in topicMaxDensityEndindex.Keys)
             {
                 int endIndex = topicMaxDensityEndindex[topicName];
                 int tmp = endIndex - _windowSize;
                 int startIndex = (tmp >= 0) ? tmp : 0;

                 string windowStr = GetWindowStr(startIndex, endIndex, fileTermList);

                 float count = 0;

                 Dictionary<string, float> curTopicTerms = topicTerms[topicName];

                 foreach (string topicTerm in curTopicTerms.Keys)
                 {
                     string filteredStr;
                     float freq = 0;

                    filteredStr = windowStr.Replace(" " + topicTerm + " ", " ");
                    // filteredStr = windowStr.Replace(topicTerm + " ", "");
                     freq = (windowStr.Length - filteredStr.Length) / (topicTerm.Length + 1);
                     if (freq > 0)
                     {
                         count += 1;
                     }
                     else
                     {
                         filteredStr = windowStr.Replace(" " + topicTerm + "s ", " ");
                        // filteredStr = windowStr.Replace(topicTerm + "s ", "");
                         freq = (windowStr.Length - filteredStr.Length) / (topicTerm.Length + 2);
                     }

                     windowStr = filteredStr; //reduce the length the string to save the match time
                 }
                 termSizeInMaxwindow.Add(topicName, count);
             }
             return termSizeInMaxwindow;
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

         private Dictionary<string, float> CalDensityInOneWindow(List<string> fileTermList, int startIndex, int endIndex,Dictionary<string,int> topicMaxdensityEndindex)
         {
             Dictionary<string, float> topicDensity_aWindow = new Dictionary<string,float>();
             Dictionary<string, float> termDensity_aWindow = new Dictionary<string, float>(); //term and the related frequency in one specific window
             string windowStr = GetWindowStr(startIndex, endIndex, fileTermList);
             //calculate the first circle

            foreach (string topicName in topicTerms.Keys)
            {
                
                Dictionary<string, float> curTopicTerms = topicTerms[topicName];

                float topic_density = 0;
                foreach (string topicTerm in curTopicTerms.Keys)
                {
                    float termWeight = curTopicTerms[topicTerm];
                    if (termDensity_aWindow.ContainsKey(topicTerm)) //if current term has been calculated
                    {
                        topic_density += termDensity_aWindow[topicTerm] * termWeight;
                    }
                    else //compute the freq of current term in the file
                    {
                        string filteredStr;
                        int freq = 0;
                        
                        filteredStr = windowStr.Replace(" " + topicTerm + " ", " ");
                       // filteredStr = windowStr.Replace(topicTerm + " ", "");
                        int freq_o = (windowStr.Length - filteredStr.Length) / (topicTerm.Length + 1);
                        
                        string secondFilter = filteredStr.Replace(" " + topicTerm + "s ", " ");
                       // string secondFilter = filteredStr.Replace(topicTerm + "s ", "");
                     
                        int freq_s = (filteredStr.Length - secondFilter.Length) / (topicTerm.Length + 2);
                        //if (freq_s > 0)
                        //{
                        //    Console.WriteLine(filteredStr);
                        //}
                       // freq = (windowStr.Length - filteredStr.Length) / (topicTerm.Length + 1);
                        freq = freq_o + freq_s;

                        topic_density += freq * termWeight;
                        termDensity_aWindow.Add(topicTerm, freq);

                        windowStr = secondFilter;
                      //  windowStr = filteredStr; //reduce the length the string to save the match time
                    }
                }
                topicDensity_aWindow.Add(topicName,topic_density);
                topicMaxdensityEndindex.Add(topicName, endIndex);
            }
                
             return topicDensity_aWindow;
         }

         Dictionary<string, float> GetDensityToNext(Dictionary<string, float> allDensity, int startIndex, List<string> fileTermList)
        {
            Dictionary<string, float> toNext = new Dictionary<string,float>();
            foreach(string topicName in topicTerms.Keys)
            {
                List<string> topicTermList = topicTerms[topicName].Keys.ToList();
                float density = allDensity[topicName];
                string hilightStartPos = isStartPosHighlight(topicTermList, fileTermList, startIndex);
                if(hilightStartPos.Length > 0)
                {
                    float weight = topicTerms[topicName][hilightStartPos];
                    float tmpDensity = density - weight;

                    if (tmpDensity < 0)
                    {
                        tmpDensity = 0;
                    }
                    toNext.Add(topicName, tmpDensity);
                }
                else
                {
                    toNext.Add(topicName,density);
                }
            }
            return toNext;
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
                         return startPos;
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

        private Dictionary<string, float> GetCurWindowUpdate(Dictionary<string, float> toNext, int endIndex, Dictionary<string, float> windowDensity, List<string> fileTermList, Dictionary<string, int> topicMaxdensityEndindex)
        {
            Dictionary<string, float> curDensity = new Dictionary<string, float>();
            foreach (string topicName in topicTerms.Keys)
            {
                Dictionary<string, float> curTopicDic = topicTerms[topicName];
                List<string> topicTermList = curTopicDic.Keys.ToList();
                float density = toNext[topicName];
                string hilightLastPos = isLastPosHighlight(topicTermList, fileTermList, endIndex);
                if (hilightLastPos.Length > 0)
                {
                    float weight = curTopicDic[hilightLastPos];
                    density += weight;
                }
                curDensity.Add(topicName, density);
                float maxDensity = windowDensity[topicName];
                if (density > maxDensity)
                {
                    windowDensity[topicName] = density;
                    topicMaxdensityEndindex[topicName] = endIndex;
                }
            }
            return curDensity;
        }
    }
}

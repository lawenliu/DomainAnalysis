using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DomainAnalysis.Utils;
using DomainAnalysis.DocumentRanking.ClumpingRankDoc;
using System.ComponentModel;

namespace DomainAnalysis.DocumentRanking.ClumpingRankDoc
{
    class RankDocByClumpingLessClumps
    {
        /*
         * Different from other versions, the document is divided into several windows. And there are no overlaps between these windows.
         * the density is equal to the average density per clump.
         * 
         * This is the final version of our ranking algorithm.
         * Because we use the same ranking algorithm for both the automated and manual model, we use topic to refer to component in source code.
         */
        private int _windowSize = 200; 

        private string txtCleanFileDir = "";
        private string topicTermsFilePath = "";
        private string rankResult = "";

        private int filePathLength;
        //store the terms and the weights of each topic
        private Dictionary<string, Dictionary<string, float>> topicTerms = new Dictionary<string, Dictionary<string, float>>();
       
        Dictionary<string, Dictionary<string, float>> docTopicDensityMap;//map between one docPath and all topics info including topicName and the density
        Dictionary<string, Dictionary<string, float>> fileNameTopicDensityMap;//map between one fileName and all topics info including topicName and the maxDensity
       //add the above two similar variables because there are redundant files in different directory

        //private int clumpIndex = 0; //record the clumpID, to store the relative density value
      //  private Dictionary<string, List<string>> termInCurWindow; //the terms appearing in the current window in the current document

        private Dictionary<string, Dictionary<int, float>> topicClumpDensity;
        public RankDocByClumpingLessClumps(string topicTermsPath, string fileDir, string similarStore)
        {
            this.txtCleanFileDir = fileDir;
          
            this.topicTermsFilePath = topicTermsPath;
            this.rankResult = similarStore;

            filePathLength = this.txtCleanFileDir.Length;
        }

        /*
         * calculate the density per document
         */
        //find the density of all components in each document.
        //files in two levels
         public void DoClumpingRank(BackgroundWorker backgroundWorker)
         {
            if (File.Exists(rankResult))
            {
                File.Delete(rankResult);
            }

            OutputMg.OutputContent(backgroundWorker, "Start parsing topic terms");
            string topicTermContent = FileOperators.ReadFileText(topicTermsFilePath).ToLower();
            topicTerms = ParseTopicTerms.GetTopicTermValueList(topicTermContent);
            OutputMg.OutputContent(backgroundWorker, "Finished parsing topic terms.");

            OutputMg.OutputContent(backgroundWorker, "Start ranking topic");
            docTopicDensityMap = new Dictionary<string, Dictionary<string, float>>();
            fileNameTopicDensityMap = new Dictionary<string, Dictionary<string, float>>();

             string[] fileEntities = Directory.GetFiles(txtCleanFileDir);
             CalDocDensityMap(fileEntities);
             string[] dirs = Directory.GetDirectories(txtCleanFileDir);
             foreach (string dir in dirs)
             {
                 string[] subFileEntities = Directory.GetFiles(dir);
                 //CalDocDensityMap(subFileEntities);
                 CalDocDensityMap(subFileEntities);
             }

            OutputMg.OutputContent(backgroundWorker, "Finished ranking topic");
            OutputMg.OutputContent(backgroundWorker, "Start writing ranking topic");
            WriteRankingResult();
            OutputMg.OutputContent(backgroundWorker, "Finished writing ranking topic");
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
                 string fileName = FileNameParse.GetFileName(fileEntity);
                 
                 if (!fileNameTopicDensityMap.ContainsKey(fileName))
                 {
                     string fileContent = FileOperators.ReadFileText(fileEntity).Replace("\n", " ").ToLower();
                     char[] delimiters = new char[] {' '};
                     string[] tmpList = fileContent.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                     List<string> fileTermList = new List<string>();
                     foreach (string tmpTerm in tmpList) //collect the terms in file
                     {
                         //if (tmpTerm.Length > 1)
                         //{
                         //    fileTermList.Add(tmpTerm);
                         //}
                         fileTermList.Add(tmpTerm);
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

         //private void CalDocDensityMapPara(string[] fileEntities)
         //{
         //    foreach (string fileEntity in fileEntities)
         //    {
         //        string fileName = FileNameParse.GetFileName(fileEntity);

         //        if (!fileNameTopicDensityMap.ContainsKey(fileName))
         //        {
         //            string[] lines = FileOperators.ReadFileLines(fileEntity);
         //            List<string> fileLines = new List<string>();
         //            foreach (string line in lines)
         //            {
         //                if (line.Trim().Length > 0)
         //                {
         //                    fileLines.Add(line);
         //                }
         //            }
         //            Dictionary<string, float> topicDensity = CalWindowDensityInOneFilePara(fileLines); //get the topic and the related density for one file

         //            fileNameTopicDensityMap.Add(fileName, topicDensity);
         //            docTopicDensityMap.Add(fileEntity, topicDensity);
         //        }
         //        else
         //        {
         //            Dictionary<string, float> topicDensity = fileNameTopicDensityMap[fileName];
         //            docTopicDensityMap.Add(fileEntity, topicDensity);
         //        }
         //    }
         //}

         private List<List<string>> SplitClumps(List<string> fileTermList)
         {
             List<List<string>> clumps = new List<List<string>>();
             int termIndex = 0;
             List<string> curClump = new List<string>();
             foreach (string term in fileTermList)
             {
                 curClump.Add(term);
                 if ((termIndex + 1) % _windowSize == 0)
                 {
                     clumps.Add(curClump);
                     curClump = new List<string>();
                 }

                 termIndex++;
             }
             if (curClump.Count > 0)
             {
                 clumps.Add(curClump);
             }
             return clumps;
         }


         //private Dictionary<string, float> CalWindowDensityInOneFilePara(List<string> fileLines)
         //{
         //    Dictionary<string, float> topicDensitys = new Dictionary<string, float>(); //the final density of all topics in the current file
         //    topicClumpDensity = new Dictionary<string, Dictionary<int, float>>();

         //    foreach (string aLine in fileLines)
         //    {
         //        foreach (string topicName in topicTerms.Keys)
         //        {
         //            Dictionary<string, float> termWeights = topicTerms[topicName];
         //            float density = 0f;
         //            int termType = 0;

         //            string tmpContent = aLine;
         //            string filteredContent = aLine;
         //            foreach (string term in termWeights.Keys)
         //            {
         //                int freq = 0;
         //                filteredContent = tmpContent.Replace(" " + term + " ", " ");
         //                int tmpGap = tmpContent.Length - filteredContent.Length;
         //                int freq_o = 0;
         //                if (tmpGap > 0)
         //                {
         //                    freq_o = tmpGap / (term.Length + 1);
         //                }
         //                //  int freq_o = (tmpContent.Length - filteredContent.Length) / (term.Length + 1);

         //                tmpContent = filteredContent;

         //                filteredContent = tmpContent.Replace(" " + term + "s ", " ");

         //                int freq_s = 0;
         //                tmpGap = 0;
         //                tmpGap = tmpContent.Length - filteredContent.Length;

         //                if (tmpGap > 0)
         //                {
         //                    freq_s = tmpGap / (term.Length + 2);
         //                }
         //                //  int freq_s = (tmpContent.Length - filteredContent.Length) / (term.Length + 2);

         //                tmpContent = filteredContent;
         //                filteredContent = tmpContent.Replace(" " + term + "es ", " ");
         //                int freq_es = 0;
         //                tmpGap = 0;
         //                tmpGap = tmpContent.Length - filteredContent.Length;
         //                if (tmpGap > 0)
         //                {
         //                    freq_es = tmpGap / (term.Length + 3);
         //                }
         //                freq = freq_o + freq_s + freq_es;

         //                if (freq > 0)
         //                {
         //                    termType++;
         //                    density += freq * termWeights[term];
         //                }
         //            }
         //            if (density > 0)
         //            {
         //                density = density * termType;
         //                if (topicClumpDensity.ContainsKey(topicName))
         //                {
         //                    topicClumpDensity[topicName].Add(clumpIndex, density);
         //                }
         //                else
         //                {
         //                    Dictionary<int, float> curTopicClumpDensity = new Dictionary<int, float>();
         //                    curTopicClumpDensity.Add(clumpIndex, density);
         //                    topicClumpDensity.Add(topicName, curTopicClumpDensity);
         //                }
         //            }
         //        }

         //        clumpIndex++;
         //    }

         //    foreach (string topic in topicClumpDensity.Keys)
         //    {
         //        Dictionary<int, float> clumpDensity = topicClumpDensity[topic];
         //        // float densitySum = clumpDensity.Values.Sum();
         //        float densitySum = 0f; //if a clump only contains one sub-term, eliminate it.
         //        int feasibleClump = 0;

         //        //foreach (int index in clumpDensity.Keys)
         //        //{
         //        //    float density = clumpDensity[index];
         //        //    if (density > 0.2) //A-L-1
         //        //    {
         //        //        densitySum += density;
         //        //        feasibleClump++;
         //        //    }
         //        //}

         //        densitySum = clumpDensity.Values.Sum();
         //        feasibleClump = clumpDensity.Count();

         //        float finalDensity = feasibleClump * densitySum / fileLines.Count();
         //        topicDensitys.Add(topic, finalDensity);
         //    }

         //    return topicDensitys;
         //}

         //find the density of all topics in one file, because I want reuse the topic content spliting result.
        //record the feasible clumps
         private Dictionary<string, float> CalWindowDensityInOneFile(List<string> fileTermList)
         {
             Dictionary<string, float> topicDensitys = new Dictionary<string, float>(); //the final density of all topics in the current file
             List<List<string>> clumps = SplitClumps(fileTermList);
             int clumpIndex = 0;

             int clumpNo = clumps.Count();

             topicClumpDensity = new Dictionary<string, Dictionary<int, float>>();

             foreach (List<string> curClump in clumps)
             {
                 string curClumpContent = string.Join(" ", curClump);

                 foreach (string topicName in topicTerms.Keys)
                 {

                     Dictionary<string, float> termWeights = topicTerms[topicName];
                     float density = 0f;
                     int termType = 0;

                     string tmpContent = curClumpContent;
                     string filteredContent = curClumpContent;
                     foreach (string term in termWeights.Keys)
                     {
                         string tmpTerm = term;
                         if (term.Contains("-"))
                         {
                             tmpTerm = term.Replace('-',' ');
                         }

                         int freq = 0;
                         filteredContent =  tmpContent.Replace(" " + tmpTerm + " ", " ");
                         int tmpGap = tmpContent.Length - filteredContent.Length;
                         int freq_o = 0;
                         if (tmpGap > 0)
                         {
                             freq_o = tmpGap / (tmpTerm.Length + 1);
                         }
                       //  int freq_o = (tmpContent.Length - filteredContent.Length) / (term.Length + 1);

                         tmpContent = filteredContent;

                         filteredContent = tmpContent.Replace(" " + tmpTerm + "s ", " ");

                         int freq_s = 0;
                         tmpGap = 0;
                         tmpGap = tmpContent.Length - filteredContent.Length;

                         if (tmpGap > 0)
                         {
                             freq_s = tmpGap / (tmpTerm.Length + 2);
                         }
                       //  int freq_s = (tmpContent.Length - filteredContent.Length) / (term.Length + 2);

                         tmpContent = filteredContent;
                         filteredContent = tmpContent.Replace(" " + tmpTerm + "es ", " ");
                         int freq_es = 0;
                         tmpGap = 0;
                         tmpGap = tmpContent.Length - filteredContent.Length;
                         if (tmpGap > 0)
                         {
                             freq_es = tmpGap / (tmpTerm.Length + 3);
                         }
                         freq = freq_o + freq_s + freq_es;

                         if (freq > 0)
                         {
                             termType++;
                             density += freq * termWeights[term];
                         }
                     }
                     if(density > 0)
                     {
                         density = density * termType;
                         if(topicClumpDensity.ContainsKey(topicName))
                         {
                             topicClumpDensity[topicName].Add(clumpIndex,density);
                         }
                         else
                         {
                             Dictionary<int, float> curTopicClumpDensity = new Dictionary<int,float>();
                             curTopicClumpDensity.Add(clumpIndex,density);
                             topicClumpDensity.Add(topicName, curTopicClumpDensity);
                         }
                     }
                 }

                 clumpIndex++;
             }

             foreach (string topic in topicClumpDensity.Keys)
             {
                 Dictionary<int, float> clumpDensity = topicClumpDensity[topic];
                // float densitySum = clumpDensity.Values.Sum();
                 float densitySum = 0f; //if a clump only contains one sub-term, eliminate it.
                 int feasibleClump = 0;

                 //foreach (int index in clumpDensity.Keys)
                 //{
                 //    float density = clumpDensity[index];
                 //    if (density > 0.2) //A-L-1
                 //    {
                 //        densitySum += density;
                 //        feasibleClump++;
                 //    }
                 //}

                 densitySum = clumpDensity.Values.Sum();
                 feasibleClump = clumpDensity.Count();

                 float finalDensity =  feasibleClump * densitySum / clumpNo;
                 topicDensitys.Add(topic, finalDensity);
             }

             return topicDensitys;
         }


        // private Dictionary<string, float> GetTermsSize(List<string> fileTermList, Dictionary<string, int> topicMaxDensityEndindex)
        // {
        //     Dictionary<string, float> termSizeInMaxwindow = new Dictionary<string, float>();
        //     foreach (string topicName in topicMaxDensityEndindex.Keys)
        //     {
        //         int endIndex = topicMaxDensityEndindex[topicName];
        //         int tmp = endIndex - _windowSize;
        //         int startIndex = (tmp >= 0) ? tmp : 0;

        //         string windowStr = GetWindowStr(startIndex, endIndex, fileTermList);

        //         float count = 0;

        //         Dictionary<string, float> curTopicTerms = topicTerms[topicName];

        //         foreach (string topicTerm in curTopicTerms.Keys)
        //         {
        //             string filteredStr;
        //             float freq = 0;

        //            filteredStr = windowStr.Replace(" " + topicTerm + " ", " ");
        //            // filteredStr = windowStr.Replace(topicTerm + " ", "");
        //             freq = (windowStr.Length - filteredStr.Length) / (topicTerm.Length + 1);
        //             if (freq > 0)
        //             {
        //                 count += 1;
        //             }
        //             else
        //             {
        //                 filteredStr = windowStr.Replace(" " + topicTerm + "s ", " ");
        //                // filteredStr = windowStr.Replace(topicTerm + "s ", "");
        //                 freq = (windowStr.Length - filteredStr.Length) / (topicTerm.Length + 2);
        //             }

        //             windowStr = filteredStr; //reduce the length the string to save the match time
        //         }
        //         termSizeInMaxwindow.Add(topicName, count);
        //     }
        //     return termSizeInMaxwindow;
        // }

        // private string GetWindowStr(int startIndex, int endIndex, List<string> fileTermList)
        // {
        //     string windowStr = "";
        //     for (int i = startIndex; i <= endIndex; i++)
        //     {
        //         windowStr += fileTermList[i] + " ";
        //     }
        //     return windowStr;
        // }

        // private Dictionary<string, float> CalDensityInOneWindow(List<string> fileTermList, int startIndex, int endIndex, Dictionary<string, Dictionary<int, float>> clumpDensity)
        // {
        //     Dictionary<string, float> topicDensity_aWindow = new Dictionary<string,float>();
        //     Dictionary<string, float> termDensity_aWindow = new Dictionary<string, float>(); //term and the related frequency in one specific window
        //     string windowStr = GetWindowStr(startIndex, endIndex, fileTermList);
        //     //calculate the first circle

        //    foreach (string topicName in topicTerms.Keys)
        //    {
        //        Dictionary<string, float> curTopicTerms = topicTerms[topicName];

        //        float topic_density = 0;
        //        int termTypes = 0; //add the impact of termTypes

        //        List<string> termInFirstWindow = new List<string>();
        //        foreach (string topicTerm in curTopicTerms.Keys)
        //        {
        //            float termWeight = curTopicTerms[topicTerm];
        //            if (termDensity_aWindow.ContainsKey(topicTerm)) //if current term has been calculated
        //            {
        //                topic_density += termDensity_aWindow[topicTerm] * termWeight;
        //                termTypes++;

        //                if(!termInFirstWindow.Contains(topicTerm))
        //                {
        //                    termInFirstWindow.Add(topicTerm);
        //                }
                        
        //            }
        //            else //compute the freq of current term in the file
        //            {
        //                string filteredStr;
        //                int freq = 0;
                        
        //                filteredStr = windowStr.Replace(" " + topicTerm + " ", " ");
        //                int freq_o = (windowStr.Length - filteredStr.Length) / (topicTerm.Length + 1);
                        
        //                string secondFilter = filteredStr.Replace(" " + topicTerm + "s ", " ");
                     
        //                int freq_s = (filteredStr.Length - secondFilter.Length) / (topicTerm.Length + 2);
        //                freq = freq_o + freq_s;

        //                if(freq > 0)
        //                {
        //                    topic_density += freq * termWeight;
                        
        //                    termDensity_aWindow.Add(topicTerm, freq);

        //                    windowStr = secondFilter;

        //                    termTypes++;

        //                    if(!termInFirstWindow.Contains(topicTerm))
        //                    {
        //                        termInFirstWindow.Add(topicTerm);
        //                    }
        //                }
                        
        //            }
        //        }

        //        termInCurWindow.Add(topicName, termInFirstWindow);

        //        topic_density = topic_density * termTypes;
        //        if (topic_density > 0 && termTypes == 0)
        //        {
        //            Console.WriteLine("except!");
        //        }

        //        if (topic_density > 0)
        //        {
        //            topicDensity_aWindow.Add(topicName, topic_density);
        //            Dictionary<int, float> curClumpDensity = new Dictionary<int, float>();
        //            curClumpDensity.Add(clumpIndex, topic_density);
        //            clumpDensity.Add(topicName, curClumpDensity);
        //        }
                
        //    }

        //    clumpIndex++;   
        //     return topicDensity_aWindow;
        // }

        // Dictionary<string, float> GetDensityToNext(Dictionary<string, float> allDensity, int startIndex, List<string> fileTermList)
        //{
        //    Dictionary<string, float> toNext = new Dictionary<string,float>();
        //    foreach(string topicName in topicTerms.Keys)
        //    {
        //        List<string> topicTermList = topicTerms[topicName].Keys.ToList();

        //        List<string> existingTermInTopic = new List<string>();
        //        if(termInCurWindow.ContainsKey(topicName))
        //        {
        //            existingTermInTopic = termInCurWindow[topicName];
        //        }

        //        float density = 0f;
        //        if (allDensity.ContainsKey(topicName))
        //        {
        //            density = allDensity[topicName];
        //        }
                
        //        string hilightStartPos = isStartPosHighlight(topicTermList, fileTermList, startIndex);
        //        if(hilightStartPos.Length > 0)
        //        {
        //            float weight = topicTerms[topicName][hilightStartPos];
        //            float tmpDensity = density - weight;

        //            if (tmpDensity < 0)
        //            {
        //                tmpDensity = 0;
        //            }
        //            toNext.Add(topicName, tmpDensity);

        //            if(existingTermInTopic.Contains(hilightStartPos))
        //            {
        //                existingTermInTopic.Remove(hilightStartPos);
        //            }
        //        }
        //        else
        //        {
        //            toNext.Add(topicName,density);
        //        }
        //        termInCurWindow[topicName] = existingTermInTopic;
        //    }
        //    return toNext;
        //}

        // private string isLastPosHighlight(List<string> topicTermList, List<string> fileTermList, int endIndex)
        // {
        //     string lastPos = fileTermList[endIndex];
        //     foreach (string topicTerm in topicTermList)
        //     {
        //         int topicTermLength = topicTerm.Split(' ').Length;
        //         if (topicTermLength == 1)
        //         {
        //             if (topicTerm.Equals(lastPos))
        //             {
        //                 return topicTerm;
        //             }
        //         }
        //         else if (topicTermLength > 1 && topicTerm.EndsWith(lastPos))
        //         {
        //             string windowEndStr = "";
        //             for (int i = endIndex - topicTermLength + 1; i <= endIndex; i++)
        //             {
        //                 windowEndStr += fileTermList[i] + " ";
        //             }
        //             if (windowEndStr.Trim().Equals(topicTerm) || windowEndStr.Trim().Equals(topicTerm + "s"))
        //             {
        //                 return topicTerm;
        //             }
        //         }
        //     }
        //     return "";
        // }

        // private string isStartPosHighlight(List<string> topicTermList, List<string> fileTermList, int startIndex)
        // {
        //     string startPos = fileTermList[startIndex];
        //     foreach (string topicTerm in topicTermList)
        //     {
        //         int topicTermLength = topicTerm.Split(' ').Length;
        //         if (topicTermLength == 1)
        //         {
        //             if (topicTerm.Equals(startPos))
        //             {
        //                 return startPos;
        //             }
        //         }
        //         else if (topicTermLength > 1 && topicTerm.StartsWith(startPos))
        //         {
        //             string windowStartStr = "";
        //             for (int i = 0; i < topicTermLength; i++)
        //             {
        //                 windowStartStr += fileTermList[startIndex + i] + " ";
        //             }
        //             if (windowStartStr.Trim().Equals(topicTerm) || windowStartStr.Trim().Equals(topicTerm + "s"))
        //             {
        //                 return topicTerm;
        //             }
        //         }
        //     }
        //     return "";
        // }

        ////record the density of all clumps
        //private Dictionary<string, float> GetCurWindowUpdate(Dictionary<string, float> toNext, int endIndex, Dictionary<string, float> windowDensity, List<string> fileTermList, Dictionary<string, Dictionary<int, float>> clumpDensity)
        //{
        //    Dictionary<string, float> curDensity = new Dictionary<string, float>();
        //    foreach (string topicName in topicTerms.Keys)
        //    {
        //        Dictionary<string, float> curTopicDic = topicTerms[topicName];
        //        List<string> topicTermList = curTopicDic.Keys.ToList();
        //        float density = toNext[topicName];

        //        List<string> existingTerms = new List<string>();
        //        if(termInCurWindow.ContainsKey(topicName))
        //        {
        //            existingTerms = termInCurWindow[topicName];
        //        }

        //        string hilightLastPos = isLastPosHighlight(topicTermList, fileTermList, endIndex);
        //        if (hilightLastPos.Length > 0)
        //        {
        //            float weight = curTopicDic[hilightLastPos];
        //            density += weight;

        //            if(!existingTerms.Contains(hilightLastPos))
        //            {
        //                existingTerms.Add(hilightLastPos);
        //            }

        //            if(termInCurWindow.ContainsKey(topicName))
        //            {
        //                termInCurWindow[topicName] = existingTerms;
        //            }
        //            else
        //            {
        //                termInCurWindow.Add(topicName,existingTerms);
        //            }

        //            density = density * existingTerms.Count;

        //            curDensity.Add(topicName, density);
        //            if (clumpDensity.ContainsKey(topicName))
        //            {
        //                clumpDensity[topicName].Add(clumpIndex, density);
        //            }
        //            else
        //            {
        //                Dictionary<int, float> curClumpDensity = new Dictionary<int, float>();
        //                curClumpDensity.Add(clumpIndex, density);
        //                clumpDensity.Add(topicName, curClumpDensity);
        //            }
        //        }
        //    }

        //    clumpIndex++;
        //    return curDensity;
        //}
    }
}

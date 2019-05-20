using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainAnalysis.Utils;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace DomainAnalysis.SummaryGeneration
{
    /* only the paragraph containing at least three keywords are deemed as relevant to one component.
     * I don't want to get too many useless terms
     */
    class IdentifyComParagraphs
    {
        private Dictionary<string, Dictionary<string, float>> compTerms = new Dictionary<string,Dictionary<string,float>>();
        private Dictionary<string, List<string>> compFiles = new Dictionary<string, List<string>>();

        //searchTerms:C:\Users\xlian\MyPapers\Simmons\qualityRequirements\ProgramImpl\TopicManualTerms.txt
        //compRelatedFile:C:\Users\xlian\MyPapers\Simmons\qualityRequirements\ProgramImpl\TOpicManualRelatedFiles.txt
        public void IdentifyComponentPara(BackgroundWorker backgroundWorker, string searchTerms, string compRelatedFile, string paras, string storeFile)
        {
            OutputMg.OutputContent(backgroundWorker, "Start parsing component terms");
            ParseCompTerms(searchTerms);
            OutputMg.OutputContent(backgroundWorker, "Parsing component terms has been done");
            OutputMg.OutputContent(backgroundWorker, "Start parsing component files");
            ParseCompFiles(compRelatedFile);
            OutputMg.OutputContent(backgroundWorker, "Parsing component file has been done");
            OutputMg.OutputContent(backgroundWorker, "Start extracting component paragraphs");
            ExtractCompParagraphs(compRelatedFile, paras, storeFile);
            OutputMg.OutputContent(backgroundWorker, "Extracting component paragraphs has been done");
        }

        /*
         * extract the component-related paragraphs according to the component-related files
         * 
         *  
         */
        private void ExtractCompParagraphs(string compRelatedFile, string paras, string storePath)
        {
            Regex regex = new Regex(@"[^a-zA-Z]");
            foreach (string comp in compFiles.Keys)
            {
                string compParagraphs = "";
                List<string> relatedFiles = compFiles[comp];
                List<string> relatedContent = new List<string>();
                if (compTerms.ContainsKey(comp))
                {
                    Dictionary<string, float> compTermWeight = compTerms[comp];
                    foreach (string file in relatedFiles)
                    {
                        string paraFile = paras + "\\" + file;
                        string realStorePath = paraFile;
                        if(File.Exists(paraFile))
                        {
                            //do nothing
                        }
                        else
                        {
                            string tmpPath = paraFile.Replace(" ","-");
                            if(File.Exists(tmpPath))
                            {
                                realStorePath = tmpPath;
                            }
                        }
                        if (File.Exists(realStorePath))
                        {
                            string fileContent = FileOperators.ReadFileText(realStorePath);
                            string[] seperators = new string[] { "\r\n\r\n" };
                            string[] paraChunks = fileContent.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
                         //   string[] fileLines = FileOperators.ReadFileLines(realStorePath);
                            foreach (string tmpPara in paraChunks)
                            {
                                if (tmpPara.Trim().Length == 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    string lowerCaseLine = tmpPara.ToLower();
                                    if (IsCompRelated(lowerCaseLine, compTermWeight))
                                    {

                                        string pureWords = regex.Replace(lowerCaseLine, "");
                                        if (!relatedContent.Contains(pureWords))
                                        {
                                            relatedContent.Add(pureWords);
                                            lowerCaseLine = lowerCaseLine.Replace("\r\n", "");
                                            compParagraphs += lowerCaseLine + "\r\n";
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Please check the existence of file: " + file);
                        }
                    }
                    FileOperators.FileWrite(storePath + comp + ".txt", compParagraphs);
                }
                
            }
        }
     
        


        private bool IsCompRelated(string aParagraph, Dictionary<string, float> termWeight)
        {
            float weight = 0f;
            foreach (string term in termWeight.Keys)
            {
                if (aParagraph.Contains(" " + term + " ") || aParagraph.Contains(" " + term + "s") || aParagraph.Contains(" " + term + "es"))
                {
                    weight += termWeight[term];
                }
                if (weight >= 0.6)
                {
                    return true;
                }
            }
            return false;
        }
        /* Parse the terms and their weights from the TopicManualTerms.txt
         */
        private void ParseCompTerms(string searchTerms)
        {
            string[] compLines = FileOperators.ReadFileLines(searchTerms);
            foreach (string aLine in compLines)
            {
                if (aLine.Contains(":"))
                {
                    int commaIndex = aLine.IndexOf(":");
                    string compName = aLine.Substring(0, commaIndex);
                    string termParts = aLine.Substring(commaIndex + 1);
                    string[] termWeights = termParts.Split(';');
                    Dictionary<string, float> termWeightDic = new Dictionary<string, float>();
                    foreach (string termWeight in termWeights)
                    {
                        if (termWeight.Contains(","))
                        {
                            string[] tmpTerms = termWeight.Split(',');
                            string term = tmpTerms[0];
                            string weightStr = tmpTerms[1];
                            float weight = float.Parse(weightStr);
                            termWeightDic.Add(term, weight);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    compTerms.Add(compName, termWeightDic);
                }
                else
                {
                    continue;
                }
            }
        }


        /*
         * parse component and the related files from the results of ranking
         */
        private void ParseCompFiles(string compRelatedFiles)
        {
            string fileContent = FileOperators.ReadFileText(compRelatedFiles);
            string[] seperators = new string[]{"\r\n\r\n"};
            string[] compChunks = fileContent.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
            foreach(string compChunk in compChunks)
            {
                string[] lineSeperator = new string[]{"\r\n"};
                string[] chunkLines = compChunk.Split(lineSeperator,StringSplitOptions.RemoveEmptyEntries);
                string firstLine = chunkLines[0];
                string compName = "";
                List<string> relatedFiles = new List<string>();
                if (firstLine.Contains('\t'))
                {
                    string[] lineTerms = firstLine.Split('\t');
                    compName = lineTerms[0];
                    string fileName = lineTerms[1];
                    if (fileName.Contains(@"\"))
                    {
                        int fileNameIndex = fileName.LastIndexOf(@"\");
                        fileName = fileName.Substring(fileNameIndex + 1);
                    }
                    relatedFiles.Add(fileName);
                }
                else
                {
                    Console.WriteLine("wrong in the compFile! " + firstLine);
                }
                int lineScale = chunkLines.Count();
                for (int i = 1; i < lineScale; i++)
                {
                    string curLine = chunkLines[i];
                    if (curLine.Contains('\t'))
                    {
                        string[] lineTerms = curLine.Split('\t');
                        string fileName = lineTerms[1];
                        if (fileName.Contains(@"\"))
                        {
                            int fileNameIndex = fileName.LastIndexOf(@"\");
                            fileName = fileName.Substring(fileNameIndex + 1);
                        }
                        relatedFiles.Add(fileName);
                    }
                    else
                    {
                        continue;
                    }
                }
                compFiles.Add(compName, relatedFiles);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DomainAnalysis.Utils;

namespace DomainAnalysis.ComputeDocRank
{
    class DocSeqInRankingResult
    {

        /*
         * oriPDFPath: the pdf documents are stored in a directory
         */
        public void CountDocRelativeSeq(string resultFile, string oriPDFPath)
        {
            //parse component and the related files
            Dictionary<string, List<string>> compFiles = new Dictionary<string, List<string>>();
            foreach (string compDir in Directory.GetDirectories(oriPDFPath))
            {
                string compName = FileNameParse.GetFileName(compDir);
                List<string> fileList = new List<string>();
                foreach (string filePath in Directory.GetFiles(compDir))
                {
                    string fileName = FileNameParse.GetFileName(filePath);
                    fileList.Add(fileName);
                }
                compFiles.Add(compName, fileList);
            }

            //read the result file
            string resultFileContent = FileOperators.ReadFileText(resultFile);
            string[] separators = new string[]{"\r\n\r\n"};
            string[] compChunks = resultFileContent.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (string compChunk in compChunks)
            {
                string[] lineSeparator = new string[] {"\r\n"};
                string[] lines = compChunk.Split(lineSeparator, StringSplitOptions.RemoveEmptyEntries);
                string firstLine = lines[0];
                if (firstLine.Contains("\t"))
                {
                    string[] lineTerms = firstLine.Split('\t');
                    string compName = lineTerms[0];

                    if (compName.Contains(","))
                    {
                        int commaIndex = compName.IndexOf(',');
                        string[] candidateComps = compName.Split(',');
                        int candidateScale = candidateComps.Count();

                        bool matched = false;
                        int candidateIndex = 0;
                        while (!matched && candidateIndex < candidateScale)
                        {
                            string candidate = candidateComps[candidateIndex].Trim();
                            foreach (string fileComp in compFiles.Keys)
                            {
                                if (candidate.Equals(fileComp.ToLower()))
                                {
                                    List<string> targetFiles = compFiles[fileComp];
                                    ReadResultFile(fileComp, lines, targetFiles);
                                    matched = true;
                                    break;
                                }
                            }
                            candidateIndex++;
                        }
                    }
                    else
                    {
                        foreach (string fileComp in compFiles.Keys)
                        {
                            if (fileComp.Equals(compName))
                            {
                                List<string> targetFiles = compFiles[fileComp];
                                ReadResultFile(fileComp, lines, targetFiles);
                                break;
                            }
                        }
                    }
                }
            }
        }

        //parse the result, and write the output to console
        private void ReadResultFile(string compName, string[] lines, List<string> fileList)
        {
            Dictionary<string, float> fileWeights = new Dictionary<string, float>();
            foreach (string curLine in lines)
            {
                if (curLine.Contains("\t"))
                {
                    string[] lineTerms = curLine.Split('\t');
                    string fileRelativePath = lineTerms[1];
                    string fileName = ParseFileName(fileRelativePath);
                    if (fileName.EndsWith(".txt"))
                    {
                        fileName = fileName.Substring(0, fileName.Length - 4);
                    }
                    if (fileList.Contains(fileName))
                    {
                        string weight = lineTerms[2];
                        if (!fileWeights.ContainsKey(fileName))
                        {
                            fileWeights.Add(fileName, float.Parse(weight));
                        }
                    }
                }
            }
            fileWeights.OrderByDescending(x => x.Value);
            string content = compName + "\r\n";
            foreach (string file in fileWeights.Keys)
            {
                content += file + "," + fileWeights[file] + "\r\n";
            }
            content += "\r\n";
            Console.Write(content);

            Console.WriteLine("====================document order");
            content = "";
            foreach (string oriFile in fileList)
            {
                int order = 1;
                foreach (string orderedFile in fileWeights.Keys)
                {
                    if (orderedFile.Equals(oriFile))
                    {
                        content += order + "\r\n";
                    }
                    else
                    {
                        order++;
                    }
                }
            }
            content += "\r\n\r\n";
            Console.Write(content);
        }

        private string ParseFileName(string fileRelativePath)
        {
            string fileName = fileRelativePath;
            if (fileRelativePath.Contains(@"\"))
            {
                int slashIndex = fileRelativePath.IndexOf(@"\");
                fileName = fileRelativePath.Substring(slashIndex + 1);
            }
            if (fileName.EndsWith(".txt"))
            {
                fileName = fileName.Substring(0, fileName.Length - 4);
            }
            return fileName;
        }
    }
}

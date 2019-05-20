using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using DomainAnalysis.Utils;
using System.Text.RegularExpressions;

namespace DomainAnalysis.ExtractContent.Preprocess
{
    /* clean the text, keep the paragraph and sentences info.
     */
    public class SemiClean
    {
        /*
         * delete: content, list of tables, list of figures, appendix, Revision History
         * split as paragraphs
         * filter: number-> "", camel split, lowercase,  more than two empty lines, 
         */
        public void CleanDir(string originalText, string cleanedText)
        {
             FileAttributes att = File.GetAttributes(originalText);
             if ((att & FileAttributes.Directory) == FileAttributes.Directory)
             {
                 string[] files = Directory.GetFiles(originalText);
                 foreach (string filePath in files)
                 {
                     string fileName = FileNameParse.GetFileName(filePath);
                     string destFilePath = cleanedText + "\\" + fileName + ".txt";
                     CleanSingleFile(filePath, destFilePath);
                 }
                 string[] dirs = Directory.GetDirectories(originalText);
                 foreach (string subDir in dirs)
                 {
                     string[] subFiles = Directory.GetFiles(subDir);
                     string subDirName = FileNameParse.GetFileName(subDir);
                     foreach (string subFile in subFiles)
                     {
                         string fileName = FileNameParse.GetFileName(subFile);
                         string destFilePath = cleanedText + "\\" + subDirName + "\\" + fileName + ".txt";
                         CleanSingleFile(subFile, destFilePath);
                     }
                 }
             }
             else
             {
                //  string fileName = FileNameParse.GetFileName(originalText);
                //  CleanSingleFile(originalText, cleanedText + "\\" + fileName + ".txt");
                CleanSingleFile(originalText, cleanedText);
            }
          
        }

        private void CleanSingleFile(string sourceFilePath, string destFileName)
        {
            string fileName = FileNameParse.GetFileName(sourceFilePath);
            
            string[] oriLines = FileOperators.ReadFileLines(sourceFilePath);
            List<string> filterDecorate = DeleteDecorate(oriLines);//contents, list of figures, list of tables, appendix

            //SplitParagraph(filterDecorate);

            List<string> cleanedText = DetailClean(filterDecorate);
            List<string> furtherClean = CleanMoreSpace(cleanedText);//filter the more consecutive empty space in each line
            List<string> mergedContent = MergeContent(furtherClean);

            string filteredContent = String.Join("\r\n", mergedContent);

            FileOperators.FileWrite(destFileName, filteredContent);
        }

        //if one line (et al.one paragraph) doesn't end with '.' or ":", merge the next few lines until find the lines ending with "." or starting with number. The lines starting with number don't taken into account.
        //another factor taken into consideration is the special character, the bullet terms
        private List<string> MergeContent(List<string> originalContent)
        {
            List<string> mergedContent = new List<string>();

            int originalScale = originalContent.Count;
            Regex startsRegex = new Regex(@"^\W");
            Regex filterRegex = new Regex(@"\W");
            for (int i = 0; i < originalScale; i++)
            {
                string curLine = originalContent[i].Trim();
                if (curLine.Trim().Length == 0) //
                {
                    mergedContent.Add(curLine);
                }
                else if (curLine.StartsWith("acronym definition") || curLine.StartsWith("acronym meaning") || curLine.StartsWith("term definition") || curLine.StartsWith("acronym explanation") || curLine.StartsWith("acronym description") || curLine.StartsWith("acronyms acronym description"))
                {
                    continue;
                }
                else if (ValidTerms(curLine) <= 5)
                {
                    continue;
                }
                else if (IsHeading(curLine))
                {
                    continue;
                }
                else if (curLine.EndsWith(":"))
                {
                    string tmpMerged = curLine;
                    i++;
                    i++;
                    if (i < originalScale)
                    {
                        curLine = originalContent[i].Trim();
                    }
                    else
                    {
                        continue;
                    }

                    while (startsRegex.Match(curLine).Success)
                    {
                        if (curLine.EndsWith(".") || curLine.EndsWith(","))
                        {
                            curLine = curLine.Substring(0, curLine.Length - 1);
                        }
                        tmpMerged += " " + curLine.Substring(1) + ",";
                        i++;
                        i++;
                        if (i < originalScale)
                        {
                            curLine = originalContent[i].Trim();
                        }
                        else
                        {
                            break;
                        }
                    }
                    i--;
                    tmpMerged = tmpMerged.Substring(0, tmpMerged.Length - 1);
                    mergedContent.Add(tmpMerged + "\r\n");
                }
                
                else
                {
                    mergedContent.Add(curLine);
                }
            }
                return mergedContent;
        }

        //if a line starts with seq, such as 5.4.4
        private bool IsHeading(string aLine)
        {
            Regex seqRegex = new Regex(@"^([0-9]+\.)+[0-9]+");

            if (seqRegex.Match(aLine).Success)
            {
                string[] terms = aLine.Split(' ');
                if (terms.Count() < 15)
                {
                    return true;
                }
            }
            return false;
        }

        //do additional filtering, including a. system req. 11], b. Includes centralized traffic control  method, c.K-ii[m-9154a]20appendix d icc routing sequence diagram
        //d.diagram messaging definitions: 1=... 2=... 
        private int ValidTerms(string sentence)
        {
            if (sentence.StartsWith("includes") || sentence.StartsWith("diagram messaging definitions:"))
            {
                return 0;
            }
            else if (sentence.StartsWith("system req ."))
            {
                int separatorIndex = sentence.IndexOf(']');
                string tmpSentence = sentence.Substring(separatorIndex + 1).Trim();
                string[] tmpTerms = tmpSentence.Split(' ');
                int termCount = 0;
                Regex wordRegex = new Regex(@"\w", RegexOptions.IgnorePatternWhitespace);
                foreach (string tmpTerm in tmpTerms)
                {
                    if (wordRegex.Match(tmpTerm).Success)
                    {
                        termCount++;
                    }
                }
                return termCount;
            }
            string[] oriTerms = sentence.Split(' ');
            return oriTerms.Count();
        }

        
        ////almost
        //private bool IsHeading(string curLine)
        //{
        //    Regex regex = new Regex(@"^([0-9]\.)+[0-9]*");
        //    string[] terms = curLine.Split(' ');
            
        //    string firstTerm = terms[0];
        //    if (terms.Count() <= 7 && regex.Match(firstTerm).Success)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        /*delete the content, list of tables, list of figures, appendix, revision history
         * Table of contents: contain more than three '.' (starts with number or letter, follows with more than two '.', and ends with number or -number-)
         */
        private List<string> DeleteDecorate(string[] fileLines)
        {
            List<string> filtered = new List<string>();
            string[] targetHeads = { "Table of Contents", "Table of Figures", "contents"};
            int lineScale = fileLines.Count();
            for (int i = 0; i < lineScale; i++)
            {
                string fileLine = fileLines[i];
                string tmpLower = fileLine.ToLower();
                if (tmpLower.Equals("references"))
                {
                    break;
                }
                if (fileLine.Trim().Length == 0)
                {
                    filtered.Add(fileLine);
                    continue;
                }

                Regex regex = new Regex(@"(\.\s{0,1}){5,6}(\.\s{0,1})"); //content format, contains more than 5 dot
                Regex wordRegex = new Regex(@"[^a-zA-Z]");
                Regex referenceRegex = new Regex(@"^[0-9]+\s+references$");
                string numberFilter = wordRegex.Replace(tmpLower,"");

               // if (tmpLower.Contains("table of contents") || tmpLower.Contains("list of figures") || tmpLower.Contains("list of tables") || regex.IsMatch(tmpLower))
                if(regex.IsMatch(tmpLower))
                {
                    if (filtered.Count > 0)
                    {
                        filtered.Clear(); //remove the content before the content
                    }
                    continue;
                }
                else if (tmpLower.StartsWith("appendix"))
                {
                    continue;
                }
                else if (!tmpLower.Contains(" ")) //if one line only contains one word, ignore it
                {
                    continue;
                }
                else if (referenceRegex.Match(tmpLower).Success) //ignore all of the references. Sometimes, the reference list is listed before the main body
                {
                    int spaceIndex = tmpLower.IndexOf(" ");
                    string seqID = tmpLower.Substring(0, spaceIndex);
                    while(IsReference(i, lineScale, fileLines, seqID))
                    {
                        i = i + 2;
                    }
                }

                else if (!wordRegex.Match(tmpLower).Success) //If one line doesn't contain any word, ignore this line
                {
                    continue;
                }
                else if (tmpLower.StartsWith("figure") || tmpLower.StartsWith("fig.")) //if one line is the figure title, ignore this line, and delete the line before 
                {
                    if (filtered.Count > 1)
                    {
                        string lastLine = filtered[filtered.Count() - 2];
                        if (lastLine.Length > 0 && !lastLine.Contains(".") && !lastLine.Contains(","))
                        {
                            filtered.RemoveAt(filtered.Count() - 1);
                            filtered.RemoveAt(filtered.Count() - 1);
                        }
                    }

                    continue;
                }
                //  else if ((tmpLower.StartsWith("table") || numberFilter.StartsWith("table")) && !tmpLower.Contains(".") && !tmpLower.Contains(",")) //if one line is the table head, ignore this line and the next line
                else if (tmpLower.StartsWith("table") || numberFilter.StartsWith("table"))
                {

                    //ignore the line directly following table head line
                    int jumpGap = JumpDirectNoTable(fileLines, i);
                    i = jumpGap + i;
                    //ignore the lines following without any punctation
                    jumpGap = JumpTableContent(fileLines, i);
                    i = jumpGap + i - 1;
                }
                else
                {
                    Regex numRegex = new Regex("\\d+");
                    string filterNum = numRegex.Replace(tmpLower, "").Trim();
                    if (filterNum.Length > 0)
                    {
                        filtered.Add(fileLine);
                    }
                    else
                    {
                        filtered.Add("\r\n");
                    }
                }
          }
            return filtered;
        }

        private bool IsReference(int i, int lineScale, string[] fileLines, string seqID)
        {
            i = i + 2;
            if (i < lineScale)
            {
                string curLine = fileLines[i];
                if (curLine.StartsWith("•"))
                {
                    return true;
                }
                else
                {
                    int spaceIndex = curLine.IndexOf(" ");
                    string nextSeqID = curLine.Substring(0, spaceIndex);
                    if (nextSeqID.StartsWith(seqID))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //ignore the consecutive line following with the table head. The ith line is the table head.
        private int JumpDirectNoTable(string[] fileLines, int i)
        {
            int jumpGap = 0;
            string nextLine = "";
            do
            {
                jumpGap++;
                nextLine = fileLines[i + jumpGap];
            } while (nextLine.Trim().Length == 0);

            return jumpGap;
        }

        //ignore the content of tables. The contents are identified by the punctations. Sometimes, the first line after the table doesn't contain punctation either.
        //For safety, keep the last line without punctations.
        private int JumpTableContent(string[] fileLines, int i)
        {
            int jumpGap = 0;
            int fileLineScale = fileLines.Count();
            string nextLine = "";
            do
            {
                jumpGap++;
                if (i + jumpGap < fileLineScale)
                {
                    nextLine = fileLines[i + jumpGap];
                }
                
            } while(!IsContainPunct(nextLine) && jumpGap + i < fileLineScale - 1);

            int finalGap = jumpGap > 0 ? jumpGap - 1 : 0;
            return finalGap;
        }

        //if a line contains punctations
        private bool IsContainPunct(string line)
        {
            string[] separators = {" "};
            string[] terms = line.Split(separators, StringSplitOptions.None);
            foreach (string term in terms)
            {
                if (term.EndsWith(".") || term.EndsWith(","))
                {
                    return true;
                }
            }
            return false;
        }

        private List<string> CleanMoreSpace(List<string> lines)
        {
            List<string> cleanedLines = new List<string>();
            foreach (string line in lines)
            {
                RegexOptions options = RegexOptions.None;
                //Regex wordRegex = new Regex(@"[^a-zA-Z]");
                //string tmpLine = wordRegex.Replace(line, " ");

                Regex regex = new Regex(@"[ ]{2,}", options);
                string tmpLine = regex.Replace(line, @" ");
                
                cleanedLines.Add(tmpLine);
            }
            return cleanedLines;
        }
        //split on '.'.
        //private void SplitParagraph(List<string> lines)
        //{
        //    int lineCount = lines.Count;

        //    for (int i = 0; i < lineCount; i++)
        //    {
        //        string curLine = lines[i];
        //        if (curLine.Trim().EndsWith("."))
        //        {
        //            int j = i + 1;
        //            if (j < lineCount)
        //            {
        //                string nextLine = lines[j];
        //                if (nextLine.Trim().Length > 0)
        //                {
        //                    lines.Insert(i + 1, " ");
        //                    lineCount++;
        //                    i++;
        //                }
        //            }
                    
        //        }

        //            //start with 1.2.1 and so on
        //        else if (Regex.IsMatch(curLine, @"^\d(\.\d)+"))
        //        {
        //            lines[i] = Regex.Replace(curLine, @"^\d(\.\d)+", "");
        //        }

        //            //here, I lost something
        //        else
        //        {
 
        //        }

        //    }

        //    //string tmpContent = String.Join("\r\n", lines);

        //    //string tmpPath = @"C:\Users\xlian\MyPapers\Simmons\qualityRequirements\ProgramImpl\Summary\paragraph.txt";
        //    //FileOperator.FileWrite(tmpPath, tmpContent);
        //}

        /* camelSplit, more than two empty lines, toLowerCase
         */
        private List<string> DetailClean(List<string> lines)
        {
            List<string> newLines = new List<string>();
            int lineCount = lines.Count;
            int consecutiveEmptyLine = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                string curLine = lines[i];
                if (curLine.Trim().Length == 0)// filter more than three empty lines
                {
                    if (consecutiveEmptyLine == 0)
                    {
                        if (i > 0)
                        {
                            string tmpLine = lines[i - 1];
                            if (!tmpLine.Trim().EndsWith(":"))
                            {
                                newLines.Add("\r\n");
                                consecutiveEmptyLine++;
                            }
                        }
                    }
                    else if (consecutiveEmptyLine > 0)
                    {
                        continue;
                    }
                }
                else
                {
                    consecutiveEmptyLine = 0;
                    if (curLine.Trim().Length > 0)
                    {
                        if(NeedCamelSplit(curLine.Trim()))
                        {
                            string camelSplitted = SplitCamelCase(curLine);
                            newLines.Add(camelSplitted.ToLower());
                        }
                        else
                        {
                            newLines.Add(curLine.ToLower());
                        }
                        
                    }
                    else
                    {
                        newLines.Add(curLine.ToLower());
                    }
                }
            }
            return newLines;
        }

        private string SplitCamelCase(string str)
        {
            string camelSplit = Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
            return Regex.Replace(camelSplit, @"\\s+", " ");
        }

        private bool NeedCamelSplit(string str)
        {
            Regex pattern = new Regex(@"[a-z][A-Z]");
            bool flag = pattern.IsMatch(str);
            return flag;
        }
    }
}

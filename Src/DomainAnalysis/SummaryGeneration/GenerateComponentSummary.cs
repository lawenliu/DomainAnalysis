using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using DomainAnalysis.Utils;

namespace DomainAnalysis.SummaryGeneration
{
    class GenerateComponentSummary
    {

        /* intput: the component that the summary is related with.
         * compName: the target component that can be a single component or a component\\subcomponent if the target is a subcomponent
         */
        public void GenerateSummary(string compTextPath, string compName, List<string> subcompTerms, string summaryStore)
        {
            string compPath = compTextPath + "\\" + compName;
            List<string> subcompNames = new List<string>();
            List<string> candidateSentences = SplitSentences(compPath); //split the paragraph into sentences
            string summary = "";
            if (IsDirectory(compPath))//if the target component is a 'component'
            {
                //1. create the subcomponent set. read all of the subcomponents! Here we identify the subcomponent from folders. To the subcomponent that the folders don't contain, they must be not in the critical content of components
                //2. write all of the related sentences in one document. calculate the scores of sentences in all of the related documents
                string[] subcomps = Directory.GetFiles(compPath);
                
                foreach (string subcomp in subcomps)
                {
                    string subcompName = FileNameParse.GetFileName(subcomp);
                    subcompNames.Add(subcompName);
                }
            }
            else //if the target component is a 'subcomponent'. Calculate the scores of the sentences in one file
            {
                string subcompName = FileNameParse.GetFileName(compPath);
                subcompNames.Add(subcompName);
                foreach (string acronym in subcompTerms)
                {
                    subcompNames.Add(acronym);
                }
            }

            MMRSummary aSummary = new MMRSummary();
            summary = aSummary.GenerateSummary(subcompNames, candidateSentences);
            if (!string.IsNullOrEmpty(summary))
            {
                FileOperators.FileWrite(summaryStore, summary);
            }
            else
            {
                Console.WriteLine("summary is empty:" + summary + ":" + compName);
            }
        }

        //split the content of file or folder into several sentences
        //the filePath can be a directory or a direct file
        private List<string> SplitSentences(string filePath)
        {
            List<string> sentences = new List<string>();
            if (IsDirectory(filePath))
            {
                string[] fileEntities = Directory.GetFiles(filePath);
                foreach (string fileEntity in fileEntities)
                {
                    List<string> curSentences = SplitSingleFileSentence(fileEntity);
                    foreach (string curSentence in curSentences)
                    {
                        if (!sentences.Contains(curSentence))
                        {
                            sentences.Add(curSentence);
                        }
                    }
                }
            }
            else
            {
                sentences = SplitSingleFileSentence(filePath);
            }
            return sentences;
        }

        //the sentence should contain at least 7 words
        private bool ContainEnoughInfo(string curSentence)
        {
            string[] terms = curSentence.Split(' ');
            int count = 0;
            Regex wordRegex = new Regex(@"[a-zA-Z]+");
            foreach (string term in terms)
            {
                if (wordRegex.Match(term).Success)
                {
                    count++;
                }
            }
            if (count > 7)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public List<string> SplitSingleParaRemoveUseless(string filePath)
        {
            string[] fileLines = FileOperators.ReadFileLines(filePath);
            List<string> paras = new List<string>();
            foreach (string aLine in fileLines)
            {
                string curPara = "";
                string curLine = aLine;
                if (aLine.Contains("e.g."))
                {
                    curLine = aLine.Replace("e.g.", "");
                }
                if (aLine.Contains("i.e.,"))
                {
                    curLine = aLine.Replace("i.e.,", "");
                }
                string[] seperator = {". "};
                string[] pieces = curLine.Split(seperator,StringSplitOptions.RemoveEmptyEntries);
                foreach (string piece in pieces)
                {
                    if (piece.Contains("appendix") || piece.Contains("figure") || piece.Contains("table") || piece.Contains("section") || piece.Contains("standard") || piece.Contains("version") || piece.Contains("http") || piece.Contains("www"))
                    {
                        continue;
                    }
                    string filteredPiece = piece.Trim();
                    for(int i = 0; i < piece.Length; i++)
                    {
                        char curChar = piece[i];
                        if (curChar > 'z' || curChar < 'a')
                        {
                            continue;
                        }
                        else
                        {
                            filteredPiece = piece.Substring(i).Trim();
                            break;
                        }
                    }

                    char endChar = filteredPiece[filteredPiece.Length - 1];
                    if (endChar >= '0' && endChar <= '9')
                    {
                        continue;
                    }

                    if (IsSynonym(filteredPiece)) //if the sentence is to define the synonym, ignore
                    {
                        continue;
                    }

                    if (IsSectionHead(filteredPiece))
                    {
                        continue;
                    }


                    if (ContainEnoughInfo(filteredPiece) && !filteredPiece.Contains("the following") && !filteredPiece.StartsWith("commentary") && !filteredPiece.StartsWith("see") && !filteredPiece.StartsWith("of") && !filteredPiece.StartsWith("description:") && !filteredPiece.StartsWith("content"))
                    {
                        curPara += filteredPiece + ". ";
                    }
                }
                if (curPara.Length > 0)
                {
                    paras.Add(curPara);
                }
                
              }
            
            return paras;
        }

        public List<string> SplitSingleFileSentence(string filePath)
        {
            //result sentences
            List<string> sentences = new List<string>();

            Regex funnySign = new Regex("[^\\w\\d\\p{P}\\s]");
            //split file into paragraphs firstly
            string fileContent = FileOperators.ReadFileText(filePath);
            string[] paras = fileContent.Split(new char[] { '\r', '\n' });

            foreach (string tmpPara in paras)
            {
                if (tmpPara.Trim().Length == 0)
                {
                    continue;
                }
                else if (!tmpPara.Contains(",") && !tmpPara.Contains(".") && !tmpPara.Contains("!") && !tmpPara.Contains(";") && !tmpPara.Contains(":"))
                {
                    continue;
                }
                string[] pieces = Regex.Split(tmpPara, "(?<=[.?!])\\s+(?=[a-zA-Z])");
                // string[] pieces = curLine.Split(seperator,StringSplitOptions.RemoveEmptyEntries);
                foreach (string piece in pieces)
                {
                    if (piece.Contains("appendix") || piece.Contains("standard") || piece.Contains("figure") || piece.Contains("table") || piece.Contains("section") || piece.Contains("version") || piece.Contains("http") || piece.Contains("www") || piece.Contains("error! reference"))
                    {
                        continue;
                    }
                    else if (piece.StartsWith("acronym") || piece.StartsWith("definition") || piece.EndsWith("as shown in fig") || piece.StartsWith("chapter"))
                    {
                        continue;
                    }

                    //if filteredPiece is the substring of piece, starting of which is a character, actually I can use regex to replace it
                    /////////////////////////////////////////////////////////////////////////////////////////////////////
                    //add for the content without clean operation beforehand
                    string filteredPiece = piece.Trim();
                    for (int i = 0; i < piece.Length; i++)
                    {
                        char curChar = piece[i];
                        if ((curChar > 'z' || curChar < 'a') && (curChar > 'Z' || curChar < 'A'))
                        {
                            continue;
                        }
                        else
                        {
                            filteredPiece = piece.Substring(i).Trim();
                            break;
                        }
                    }

                    if (filteredPiece.Length > 1)
                    {
                        char endChar = filteredPiece[filteredPiece.Length - 1];
                        if (endChar >= '0' && endChar <= '9')
                        {
                            continue;
                        }
                    }

                    if (funnySign.Match(filteredPiece).Success)
                    {
                        filteredPiece = funnySign.Replace(filteredPiece, " ");
                    }

                    if (IsSynonym(filteredPiece)) //if the sentence is to define the synonym, ignore it
                    {
                        continue;
                    }

                    if (IsSectionHead(filteredPiece))
                    {
                        continue;
                    }



                    if (ContainEnoughInfo(filteredPiece) && !filteredPiece.Contains("the following") && !filteredPiece.StartsWith("commentary") && !filteredPiece.StartsWith("see") && !filteredPiece.StartsWith("of") && !filteredPiece.StartsWith("description:") && !filteredPiece.StartsWith("content"))
                    {
                        sentences.Add(filteredPiece);
                    }
                }
            }

        
        return sentences;

            /*
            string[] fileLines = FileOperators.ReadFileLines(filePath);
            
          //  Regex funnySign = new Regex("[^a-zA-Z0-9.,;:()\"\'/-]");
            Regex funnySign = new Regex("[^\\w\\d\\p{P}\\s]");
            if (fileLines == null)
            {
                return null;
            }

            foreach (string aLine in fileLines)
            {
                if (aLine.Contains("&"))
                {
                    continue;
                }

                if (!aLine.Contains(",") && !aLine.Contains(".") && !aLine.Contains("!") && !aLine.Contains(";") && !aLine.Contains(":"))
                {
                    continue;
                }

                string curLine = aLine;
               
                //if (curLine.Contains("e.g."))
                //{
                //    curLine = curLine.Replace("e.g.", "e,g,");
                //}
                //if (curLine.Contains("e .g ."))
                //{
                //    curLine = curLine.Replace("e .g .", "e,g,");
                //}
                //if (curLine.Contains("i.e.") )
                //{
                //    curLine = curLine.Replace("i.e.", "i,e,");
                //}
                //if (curLine.Contains("i .e ."))
                //{
                //    curLine = curLine.Replace("i .e .", "i,e,");
                //}
                //if (curLine.Contains("vs."))
                //{
                //    curLine = curLine.Replace("vs.","vs,");
                //}
               // Regex sentencePattern = new Regex("(?<=[.?!])\\s+(?=[a-zA-Z])");
                
                
               // string[] seperator = {"."};
                string[] pieces = Regex.Split(curLine, "(?<=[.?!])\\s+(?=[a-zA-Z])");
               // string[] pieces = curLine.Split(seperator,StringSplitOptions.RemoveEmptyEntries);
                foreach (string piece in pieces)
                {
                    if (piece.Contains("appendix") || piece.Contains("standard") || piece.Contains("figure") || piece.Contains("table") || piece.Contains("section")  || piece.Contains("version") || piece.Contains("http") || piece.Contains("www") || piece.Contains("error! reference"))
                    {
                        continue;
                    }
                    else if (piece.StartsWith("acronym") || piece.StartsWith("definition") || piece.EndsWith("as shown in fig") || piece.StartsWith("chapter"))
                    {
                        continue;
                    }

                    //if filteredPiece is the substring of piece, starting of which is a character, actually I can use regex to replace it
                    /////////////////////////////////////////////////////////////////////////////////////////////////////
                    //add for the content without clean operation beforehand
                    string filteredPiece = piece.Trim();
                    for (int i = 0; i < piece.Length; i++)
                    {
                        char curChar = piece[i];
                        if ((curChar > 'z' || curChar < 'a') && (curChar > 'Z' || curChar < 'A'))
                        {
                            continue;
                        }
                        else
                        {
                            filteredPiece = piece.Substring(i).Trim();
                            break;
                        }
                    }

                    if (filteredPiece.Length > 1)
                    {
                        char endChar = filteredPiece[filteredPiece.Length - 1];
                        if (endChar >= '0' && endChar <= '9')
                        {
                            continue;
                        }
                    }

                    if(funnySign.Match(filteredPiece).Success)
                    {
                        filteredPiece = funnySign.Replace(filteredPiece, " ");
                    }

                    if (IsSynonym(filteredPiece)) //if the sentence is to define the synonym, ignore it
                    {
                        continue;
                    }

                    if (IsSectionHead(filteredPiece))
                    {
                        continue;
                    }

       

                    if (ContainEnoughInfo(filteredPiece) && !filteredPiece.Contains("the following") && !filteredPiece.StartsWith("commentary") && !filteredPiece.StartsWith("see") && !filteredPiece.StartsWith("of") && !filteredPiece.StartsWith("description:") && !filteredPiece.StartsWith("content"))
                    {
                        sentences.Add(filteredPiece);
                    }
                }
            }
            return sentences;*/
        }

        /// <summary>
        /// check if one sentence is a section head
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        private bool IsSectionHead(string sentence)
        {
            Regex seqIdRegex = new Regex(@"[0-9]{1,3}(\.[0-9]{1,3})+");
            if (seqIdRegex.Match(sentence).Success)
            {
                return true;
            }
            return false;
        }

        private bool IsSynonym(string sentence)
        {
            string acronymStart = "acronym explanation";
            if (sentence.StartsWith(acronymStart))
            {
                int startPos = acronymStart.Length;
                sentence = sentence.Substring(startPos).Trim();
            }

            string[] terms = sentence.Split(' ');
            int termLength = terms.Count();
            int acronymPair = 0;

            string lastAconym = "";
            for (int i = 0; i < termLength; i++)
            {
                string tmpTerm = terms[i];
                if (tmpTerm.Equals("system"))
                {
                    continue;
                }
                else if (tmpTerm.Equals("or"))
                {
                    tmpTerm = lastAconym;
                }
                else
                {
                    lastAconym = tmpTerm;
                }

                int nextIndex = i + 1;
                if (nextIndex == termLength)
                {
                    return false;
                }
                string nextTerm = terms[i + 1];
                int acronymOffset = IsAcronym(tmpTerm, i, terms);
                if (acronymOffset > 0)
                {
                    i += acronymOffset;
                    acronymPair++;
                }
                else if (IsAbbreviate(tmpTerm, nextTerm))
                {
                    i += 1;
                    acronymPair++;
                }
                else
                {
                    return false;
                }

                if (acronymPair == 2)
                {
                    return true;
                }
            }
            return true;
        }

        private int IsAcronym(string acronym, int pos, string[] terms)
        {
            string oriTerm = acronym;
            if (oriTerm.Contains("-"))
            {
                oriTerm = oriTerm.Replace("-", "");
            }
            int tmpTermLen = oriTerm.Length;
            int count = 1;
            int checkTermOffset = 0;
            int termLength = terms.Count();
            int shiftRight = 0;
            while (count <= tmpTermLen && pos + count + checkTermOffset < termLength)
            {
                string curCheckedTerm = terms[pos + count + checkTermOffset].Trim();
                char char_count = oriTerm[count - 1];
                if (curCheckedTerm.StartsWith("-") || curCheckedTerm.StartsWith("–") || curCheckedTerm.Equals("of"))
                {
                    checkTermOffset++;
                    shiftRight++;
                    continue;
                }
                else if (curCheckedTerm.Equals("and") && char_count.Equals('&'))
                {
                    count++;
                    shiftRight++;
                    continue;
                }
                else if (curCheckedTerm.StartsWith(char_count.ToString()))
                {
                    count++;
                    shiftRight++;
                    continue;
                }
                else
                {
                    return 0;
                }
            }
            return shiftRight;
        }

        //the original should contain all of the chars in abbre by sequence
        private bool IsAbbreviate(string abbre, string original)
        {
            if (abbre.Length == 1)
            {
                return false;
            }
            int index_before = 0; //record the index of char in the before position in abbre.
            foreach (char tmp_c in abbre)
            {
                if (original.Contains(tmp_c))
                {
                    int cur_index = original.IndexOf(tmp_c, index_before);
                    if (cur_index > -1)
                    {
                        index_before = cur_index;
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsDirectory(string path)
        {
            return ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory);
        }

        /// <summary>
        /// merge the content of multiple files into a single one
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="mergedFile"></param>
        public void MergeMultiFiles(String dir, String mergedFile)
        {
            string[] inputFilePaths = Directory.GetFiles(dir);
            using(var outputStream = File.Create(mergedFile))
            {
                foreach(var inputFilePath in inputFilePaths)
                {
                    using (var inputStream = File.OpenRead(inputFilePath))
                    {
                        inputStream.CopyTo(outputStream);
                    }
                }
                
            }
        }

        /// <summary>
        /// merge the content of multiple files into a single one. The difference with the above function is that there are a candidate file list.
        /// </summary>
        /// <param name="oriDir"></param>
        /// <param name="candidateFiles">the name of the candidate files</param>
        /// <param name="mergedFile"></param>
        public void MergeMultiFiles(String oriDir, List<string> candidateFiles, string mergedFile)
        {
            string[] inputFilePaths = Directory.GetFiles(oriDir);
            Regex wordRegex = new Regex("[^a-zA-Z0-9]");
            using (var outputStream = File.Create(mergedFile))
            {
                foreach (var inputFilePath in inputFilePaths)
                {
                    string fileName = GetFileName(inputFilePath);
                    fileName = wordRegex.Replace(fileName, ""); // add this, because there may be mojibake in the name of the file name
                    if (candidateFiles.Contains(fileName))
                    {
                        using (var inputStream = File.OpenRead(inputFilePath))
                        {
                            inputStream.CopyTo(outputStream);
                        }
                    }
                }
            }
        }

        private string GetFileName(string filePath)
        {
            int slashIndex = filePath.LastIndexOf("\\");
            if (slashIndex > 0)
            {
                string fileName = filePath.Substring(slashIndex + 1);
                return fileName;
            }
            return filePath;
        }

    }
}

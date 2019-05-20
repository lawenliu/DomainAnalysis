using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using DomainAnalysis.Utils;
using DomainAnalysis.ExtractContent.Preprocess;

namespace ExtractContent.Preprocess
{
    class StemFileDir
    {
        /* store the stemmed file as the original hierarchy
         */
        public void FileDirStemming(string sourceFile, string stemmedPath)
        {
            if (IsDirectory(sourceFile))
            {
                string[] subDirs = Directory.GetDirectories(sourceFile);
                foreach (string subDir in subDirs)
                {
                    string subDirName = FileNameParse.GetFileName(subDir);
                    string storePath = stemmedPath + "\\" + subDirName;
                    FileDirStemming(subDir, storePath);
                }
                string[] subFiles = Directory.GetFiles(sourceFile);
                foreach (string subFile in subFiles)
                {
                    string[] fileLines = FileOperators.ReadFileLines(subFile);
                    string fileName = FileNameParse.GetFileName(subFile);
                    string stemmedContent = "";
                    foreach (string fileLine in fileLines)
                    {
                        string stemmedLine = "";
                        string[] separators = { " ", "," };
                        string[] terms = fileLine.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        Porter2 porter = new Porter2();
                        foreach (string term in terms)
                        {
                            string stemmedTerm = porter.stem(term);
                            stemmedLine += stemmedTerm + " ";
                        }
                        stemmedContent += stemmedLine + "\r\n";
                    }
                    FileOperators.FileWrite(stemmedPath + "\\" + fileName, stemmedContent);
                }
            }
        }

        private bool IsDirectory(string path)
        {
            return ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory);
        }
    }
}

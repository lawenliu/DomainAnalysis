using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.PowerPoint;
using System.ComponentModel;
using System.Text.RegularExpressions;
using DomainAnalysis.DataPrepare;

namespace DomainAnalysis.ExtractContent
{
    public class ExtractMg
    {
        private static int curDealFileIndex = 0;
        private static int curCleanFileIndex = 0;

        public static int ExtractFile(string sourceDirName, string destDirName, string cleanDirName, string semiCleanDirName,
            string tmtInputFilePath = null, BackgroundWorker backgroundWorker = null)
        {
            curDealFileIndex = 0;
            curCleanFileIndex = 0;
            
            if (!string.IsNullOrEmpty(tmtInputFilePath) && File.Exists(tmtInputFilePath))
            {
                File.Delete(tmtInputFilePath);
            }

            return ExecuteExtract(sourceDirName, destDirName, cleanDirName, semiCleanDirName, tmtInputFilePath, backgroundWorker);
        }

        private static int ExecuteExtract(string sourceDirName, string destDirName, string cleanDirName, string semiCleanDirName,
            string tmtInputFilePath, BackgroundWorker backgroundWorker = null)
        {
            if (backgroundWorker != null)
            {
                OutputMg.OutputContent(backgroundWorker, "Extrating file from " + sourceDirName + " to " + destDirName);
            }

            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }
            
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            if (!Directory.Exists(cleanDirName))
            {
                Directory.CreateDirectory(cleanDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                curDealFileIndex++;
                if (backgroundWorker != null)
                {
                    OutputMg.OutputContent(backgroundWorker, "Extrating file " + file.Name, curCleanFileIndex);
                }

                try {
                    if (file.Name.EndsWith(".pdf")|| file.Name.EndsWith(".doc") || file.Name.EndsWith(".docx"))
                    {
                        string tempDestFileName = file.Name + ".txt";
                        string tempDestPath = System.IO.Path.Combine(destDirName, tempDestFileName);
                        string tempCleanPath = System.IO.Path.Combine(cleanDirName, tempDestFileName);
                        string tempSemiCleanPath = System.IO.Path.Combine(semiCleanDirName, tempDestFileName);
                        if (!File.Exists(tempDestPath))
                        {

                            if (file.Name.EndsWith(".pdf"))
                            {
                                ExtractPDF.ExecuteExtraction(file.FullName, tempDestPath);
                            }
                            else if (file.Name.EndsWith(".doc") || file.Name.EndsWith(".docx"))
                            {
                                ExtractWord.ExecuteWordExtraction(file.FullName, tempDestPath);
                            }
                            else if (file.Name.EndsWith(".ppt") || file.Name.EndsWith(".pptx"))
                            {
                                ExtractPPT.ExecuteExtraction(file.FullName, tempDestPath);
                            }
                            else if (file.Name.EndsWith(".xls") || file.Name.EndsWith(".xlsx"))
                            {
                                ExtractExcel.ExecuteExtraction(file.FullName, tempDestPath);
                            }
                        }

                        //clean
                        ExtractContent.Preprocess.SemiClean cleaner = new Preprocess.SemiClean();
                        cleaner.CleanDir(tempDestPath,tempCleanPath);

                        //generate tmtInputFile.csv
                        GenerateTmtInputFile(tempCleanPath, tmtInputFilePath);
                        

                        if (backgroundWorker != null)
                        {
                            OutputMg.OutputContent(backgroundWorker, "Successfuly Extrated file " + file.Name, curDealFileIndex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OutputMg.OutputContent(backgroundWorker, "Failed to extract file " + file.Name + " with Exception: " + ex.Message, curDealFileIndex);
                }
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempDestPath = System.IO.Path.Combine(destDirName, subdir.Name);
                string tempCleanPath = System.IO.Path.Combine(cleanDirName, subdir.Name);
                string tempSemiCleanPath = System.IO.Path.Combine(semiCleanDirName, subdir.Name);
                ExecuteExtract(subdir.FullName, tempDestPath, tempCleanPath, tempSemiCleanPath, tmtInputFilePath, backgroundWorker);
            }

            return curDealFileIndex;
        }

        private static void GenerateTmtInputFile(string tempCleanPath, string tmtInputFilePath)
        {
            StreamReader sr = new StreamReader(tempCleanPath);
            StringBuilder sb = new StringBuilder();
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                Regex rgx = new Regex("( )+");
                line = rgx.Replace(line, " ");
                rgx = new Regex("[^a-zA-Z ]");
                line = rgx.Replace(line, " ");
                line = line.Replace("Page", " ");
                //line = line.Replace("\n", " ");
                line = line.Replace("\t", " ");
                rgx = new Regex("[ ]{2,}");
                line = rgx.Replace(line, " ");//if a line has more than two consecutive blank space, then replace them with one blank space
                line = line.Trim();

                //if (line.Contains(" "))
                //{
                //    string[] lineTerms = line.Split(' ');
                //    if (lineTerms.Count() > 2)
                //    {
                //        sb.Append(line);
                //        sb.Append("\n");
                //    }
                //}
                //else
                //{
                //    continue;
                //}
                if (line.Length > 2)
                {
                    sb.Append(line);
                    sb.Append("\n");
                }
            }

            sr.Close();

            string result = sb.ToString().Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(result))
            {
                // Clean File
                StreamWriter sw;

                // CSV for tmp input
                if (!string.IsNullOrEmpty(tmtInputFilePath))
                {
                    sw = new StreamWriter(tmtInputFilePath, true);
                    result = result.Replace("\n", " ");
                    sw.WriteLine(curCleanFileIndex + "," + result);
                    curCleanFileIndex++;
                    sw.Close();
                }
                else
                {
                    Console.WriteLine("test if some files are filtered here");
                }
            }
            return;
        }
      /*  private static void CleanExtractText(string sourceFileName, string destFileName, string tmtInputFilePath)
        {
            
            StreamReader sr = new StreamReader(sourceFileName);
            StringBuilder sb = new StringBuilder();
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                Regex rgx = new Regex("( )+");
                line = rgx.Replace(line, " ");
                rgx = new Regex("[^a-zA-Z ]");
                line = rgx.Replace(line, " ");
                line = line.Replace("Page", " ");
                //line = line.Replace("\n", " ");
                line = line.Replace("\t", " ");
                rgx = new Regex("[ ]{2,}");
                line = rgx.Replace(line, " ");//if a line has more than two consecutive blank space, then replace them with one blank space
                line = line.Trim();

                //if (line.Contains(" "))
                //{
                //    string[] lineTerms = line.Split(' ');
                //    if (lineTerms.Count() > 2)
                //    {
                //        sb.Append(line);
                //        sb.Append("\n");
                //    }
                //}
                //else
                //{
                //    continue;
                //}
                if (line.Length > 2)
                {
                    sb.Append(line);
                    sb.Append("\n");
                }
            }

            sr.Close();

            string result = sb.ToString().Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(result))
            {
                // Clean File
                StreamWriter sw = new StreamWriter(destFileName);
                sw.Write(result);
                sw.Close();

                // CSV for tmp input
                if (!string.IsNullOrEmpty(tmtInputFilePath))
                {
                    sw = new StreamWriter(tmtInputFilePath, true);
                  //  result = result.Replace("\n", " ");
                    sw.WriteLine(curCleanFileIndex + "," + result);
                    curCleanFileIndex++;
                    sw.Close();
                }
                else
                {
                    Console.WriteLine("test if some files are filtered here");
                }
            }
        }*/



        private static void SemiCleanExtractText(string sourceFileName, string destFileName, string tmtInputFilePath)
        {

            StreamReader sr = new StreamReader(sourceFileName);
            StringBuilder sb = new StringBuilder();
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                Regex rgx = new Regex("( )+");
                //line = rgx.Replace(line, " ");
                rgx = new Regex("[^a-zA-Z ]");
                //line = rgx.Replace(line, " ");
                line = line.Replace("Page", " ");
                //line = line.Replace("\n", " ");
                line = line.Replace("\t", " ");
                rgx = new Regex("[ ]{2,}");
                line = rgx.Replace(line, " ");//if a line has more than two consecutive blank space, then replace them with one blank space
                line = line.Trim();

                //if (line.Contains(" "))
                //{
                //    string[] lineTerms = line.Split(' ');
                //    if (lineTerms.Count() > 2)
                //    {
                //        sb.Append(line);
                //        sb.Append("\n");
                //    }
                //}
                //else
                //{
                //    continue;
                //}
                if (line.Length > 2)
                {
                    sb.Append(line);
                    sb.Append("\n");
                }
            }

            sr.Close();

            string result = sb.ToString().Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(result))
            {
                // Clean File
                StreamWriter sw = new StreamWriter(destFileName);
                sw.Write(result);
                sw.Close();

                // CSV for tmp input
                if (!string.IsNullOrEmpty(tmtInputFilePath))
                {
                    sw = new StreamWriter(tmtInputFilePath, true);
                    result = result.Replace("\n", " ");
                    sw.WriteLine(curCleanFileIndex + "," + result);
                    curCleanFileIndex++;
                    sw.Close();
                }
                else
                {
                    Console.WriteLine("test if some files are filtered here");
                }
            }
        }

        /*NOT USED NOW!*/
        private string SplitCamelCase(string str)
        {
            return Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        }
    }
}

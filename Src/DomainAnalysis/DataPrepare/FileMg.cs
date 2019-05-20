using DomainAnalysis.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis.DataPrepare
{
    class FileMg
    {
        public static void ClearAutoFolder(bool delSubDir = true)
        {            
            DirectoryDelete(AutoSourceFileDir, delSubDir);
            DirectoryDelete(AutoCleanTextFileDir, delSubDir);
            DirectoryDelete(AutoExtractTextFileDir, delSubDir);
            DirectoryDelete(AutoTmtOutputFileDir, delSubDir);
            DirectoryDelete(AutoTopicLabelFileDir, delSubDir);
            DirectoryDelete(AutoRDataFileDir, delSubDir);
            DirectoryDelete(AutoJNSPDataFileDir, delSubDir);
            DirectoryDelete(AutoTmtDataFileDir, delSubDir);
            DirectoryDelete(AutoHighlightFileDir, delSubDir);

            InitDataFolder();
        }

        public static void ClearManualFolder(bool delSubDir = true)
        {
            DirectoryDelete(ManualSourceFileDir, delSubDir);
            DirectoryDelete(ManualCleanTextFileDir, delSubDir);
            DirectoryDelete(ManualExtractTextFileDir, delSubDir);            
            DirectoryDelete(ManualTopicLabelFileDir, delSubDir);
            DirectoryDelete(ManualHighlightFileDir, delSubDir);
            DirectoryDelete(ManualSemiCleanTextFileDir, delSubDir);
            DirectoryDelete(ManualCleanComponentFileDir, delSubDir);

            InitDataFolder();
        }

        public static void DeleteTmtCacheFile(string dirName)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(dirName);

            if (!dir.Exists)
            {
                return;
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Name.EndsWith(".gz"))
                {
                    try
                    {
                        file.Delete();
                    }
                    catch
                    { }
                }
            }
        }

        public static int CountFileNumber(string sourceDirName)
        {
            int count = 0;
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                return count;
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                count++;
            }

            // If copying subdirectories, copy them and their contents to new location.
            foreach (DirectoryInfo subdir in dirs)
            {
                count += CountFileNumber(subdir.FullName);
            }

            return count;
        }

        /*copy the subdirectories and the related files*/
        public static int ToolDirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool createDirs,
            BackgroundWorker backgroundWorker = null, int currentFileIndex = 0)
        {
            if (backgroundWorker != null)
            {
                OutputMg.OutputContent(backgroundWorker, "Copying file from " + sourceDirName + " to " + destDirName);
            }

            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                try
                {
                    currentFileIndex++;
                    OutputMg.OutputContent(backgroundWorker, "Copying file " + file.Name, currentFileIndex);
                    string temppath = Path.Combine(destDirName, file.Name);
                    if (!File.Exists(temppath))
                    {
                        file.CopyTo(temppath, false);
                    }

                    //if (backgroundWorker != null)
                    //{
                    //    OutputMg.OutputContent(backgroundWorker, "Copying file " + file.Name, currentFileIndex);
                    //}
                }
                catch (Exception ex)
                {
                    if (backgroundWorker != null)
                    {
                        OutputMg.OutputContent(backgroundWorker, "Copying file failed with exception: " + ex.Message);
                    }
                }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    currentFileIndex = DirectoryCopy(subdir.FullName, temppath, copySubDirs, createDirs, backgroundWorker, currentFileIndex);
                }
            }

            return currentFileIndex;
        }

        /*only copy the files, and remove the replicated files*/
        public static int DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool createDirs,
            BackgroundWorker backgroundWorker = null, int currentFileIndex = 0)
        {
            if (backgroundWorker != null)
            {
                OutputMg.OutputContent(backgroundWorker, "Copying file from " + sourceDirName + " to " + destDirName, currentFileIndex);
            }

            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                try
                {
                    currentFileIndex++;
                    OutputMg.OutputContent(backgroundWorker, "Copying file " + file.Name, currentFileIndex);
                    string temppath = Path.Combine(destDirName, file.Name);
                    if (!File.Exists(temppath))
                    {
                        file.CopyTo(temppath, false);
                    }
                }
                catch (Exception ex)
                {
                    if (backgroundWorker != null)
                    {
                        OutputMg.OutputContent(backgroundWorker, "Copying file failed with exception: " + ex.Message);
                    }
                }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempDestDir = destDirName;
                    if (createDirs)
                    {
                        tempDestDir = Path.Combine(destDirName, subdir.Name);
                    }
                   
                    currentFileIndex = DirectoryCopy(subdir.FullName, tempDestDir, copySubDirs, createDirs, backgroundWorker, currentFileIndex);
                }
            }

            return currentFileIndex;
        }

        public static bool DirectoryDelete(string dirName, bool delSubDirs)
        {
            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(dirName);
                if (!dir.Exists)
                {
                    return true;
                }

                dir.Delete(true);

                return true;
            }
            catch
            {
                return true;
            }
        }

        public static string MapRelToAbsFilePath(string relatedFilePath)
        {
            string tempFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            tempFilePath = tempFilePath.Substring(0, tempFilePath.LastIndexOf('\\'));
            if (relatedFilePath.StartsWith(".\\"))
            {
                relatedFilePath = relatedFilePath.Substring(2);
            }

            if (tempFilePath.EndsWith("\\"))
            {
                tempFilePath += relatedFilePath;
            }
            else
            {
                tempFilePath += "\\" + relatedFilePath;
            }

            return tempFilePath;
        }

        public static void InitDataFolder()
        {
            if (!Directory.Exists(AppDataFolder))
            {
                Directory.CreateDirectory(AppDataFolder);
            }

            if (!Directory.Exists(AutoDataFolder))
            {
                Directory.CreateDirectory(AutoDataFolder);
            }

            if (!Directory.Exists(ManualDataFolder))
            {
                Directory.CreateDirectory(ManualDataFolder);
            }

            if (!Directory.Exists(AutoSourceFileDir))
            {
                Directory.CreateDirectory(AutoSourceFileDir);
            }

            if (!Directory.Exists(AutoExtractTextFileDir))
            {
                Directory.CreateDirectory(AutoExtractTextFileDir);
            }

            if (!Directory.Exists(AutoCleanTextFileDir))
            {
                Directory.CreateDirectory(AutoCleanTextFileDir);
            }

            if (!Directory.Exists(AutoTmtDataFileDir))
            {
                Directory.CreateDirectory(AutoTmtDataFileDir);
            }

            if (!Directory.Exists(AutoHighlightFileDir))
            {
                Directory.CreateDirectory(AutoHighlightFileDir);
            }

            if (!Directory.Exists(AutoTopicLabelFileDir))
            {
                Directory.CreateDirectory(AutoTopicLabelFileDir);
            }

            if (!Directory.Exists(AutoTmtOutputFileDir))
            {
                Directory.CreateDirectory(AutoTmtOutputFileDir);
            }

            if (!Directory.Exists(AutoRDataFileDir))
            {
                Directory.CreateDirectory(AutoRDataFileDir);
            }

            if (!Directory.Exists(AutoJNSPDataFileDir))
            {
                Directory.CreateDirectory(AutoJNSPDataFileDir);
            }

            if (!Directory.Exists(ManualSourceFileDir))
            {
                Directory.CreateDirectory(ManualSourceFileDir);
            }

            if (!Directory.Exists(ManualExtractTextFileDir))
            {
                Directory.CreateDirectory(ManualExtractTextFileDir);
            }

            if (!Directory.Exists(ManualCleanTextFileDir))
            {
                Directory.CreateDirectory(ManualCleanTextFileDir);
            }

            if (!Directory.Exists(ManualSemiCleanTextFileDir))
            {
                Directory.CreateDirectory(ManualSemiCleanTextFileDir);
            }

            if (!Directory.Exists(ManualCleanComponentFileDir))
            {
                Directory.CreateDirectory(ManualCleanComponentFileDir);
            }

            if (!Directory.Exists(ManualHighlightFileDir))
            {
                Directory.CreateDirectory(ManualHighlightFileDir);
            }

            if (!Directory.Exists(ManualTopicLabelFileDir))
            {
                Directory.CreateDirectory(ManualTopicLabelFileDir);
            }

            if (!Directory.Exists(ExternalToolDir))
            {
                Directory.CreateDirectory(ExternalToolDir);
                CopyToolFiles();
            }
        }

        public static void CopyToolFiles()
        {
            ToolDirectoryCopy(Constants.LocalExternalToolDir, ExternalToolDir, true, true);
        }

        public static string AppDataFolder
        {
            get { return Constants.DefaultDataDir + Constants.DefaultApplicationDir; }
        }
        
        public static string AutoDataFolder
        {
            get { return AppDataFolder + Constants.DefaultAutoDataDir; }
        }

        public static string ManualDataFolder
        {
            get { return AppDataFolder + Constants.DefaultManualDataDir; }
        }

        public static string ExternalToolDir
        {
            get { return AutoDataFolder + Constants.DefaultExternalToolDir; }
        }        

        public static string AutoSourceFileDir
        {
            get { return AutoDataFolder + Constants.SourceFileDir; }
        }

        public static string AutoExtractTextFileDir
        {
            get { return AutoDataFolder + Constants.ExtractTextFileDir; }
        }

        public static string AutoCleanTextFileDir
        {
            get { return AutoDataFolder + Constants.CleanTextFileDir; }
        }

        public static string AutoSemiCleanTextFileDir
        {
            get { return AutoDataFolder + Constants.SemiCleanTextFileDir; }
        }

        public static string AutoTmtDataFileDir
        {
            get { return AutoDataFolder + Constants.TMTDataFileDir; }
        }
        
        public static string AutoHighlightFileDir
        {
            get { return AutoDataFolder + Constants.HighlightFileDir; }
        }

        public static string AutoTopicLabelFileDir
        {
            get { return AutoDataFolder + Constants.TopicLabelFileDir; }
        }

        public static string AutoTmtOutputFileDir
        {
            get { return AutoDataFolder + Constants.TMTOutputFileDir; }
        }

        public static string AutoRDataFileDir
        {
            get { return AutoDataFolder + Constants.RDataFileDir; }
        }

        public static string AutoJNSPDataFileDir
        {
            get { return AutoDataFolder + Constants.JNSPDataFileDir; }
        }

        public static string ManualSourceFileDir
        {
            get { return ManualDataFolder + Constants.SourceFileDir; }
        }

        public static string ManualExtractTextFileDir
        {
            get { return ManualDataFolder + Constants.ExtractTextFileDir; }
        }

        public static string ManualCleanTextFileDir
        {
            get { return ManualDataFolder + Constants.CleanTextFileDir; }
        }

        public static string ManualSemiCleanTextFileDir
        {
            get { return ManualDataFolder + Constants.SemiCleanTextFileDir; }
        }

        public static string ManualCleanComponentFileDir
        {
            get { return ManualDataFolder + Constants.CleanComponentFileDir; }
        }

        public static string ManualHighlightFileDir
        {
            get { return ManualDataFolder + Constants.HighlightFileDir; }
        }

        public static string ManualTopicLabelFileDir
        {
            get { return ManualDataFolder + Constants.TopicLabelFileDir; }
        }

        public static string TmtToolDir
        {
            get { return ExternalToolDir + Constants.TmtToolDir; }
        }

        public static string JnspToolDir
        {
            get { return ExternalToolDir + Constants.JnspToolDir; }
        }

        public static string RToolDir
        {
            get { return ExternalToolDir + Constants.RToolDir; }
        }
    }
}

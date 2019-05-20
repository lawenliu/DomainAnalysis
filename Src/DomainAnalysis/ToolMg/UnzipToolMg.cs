using DomainAnalysis.DataPrepare;
using DomainAnalysis.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis
{
    class UnzipToolMg
    {
        private static void GenerateBat(string inputFilePath, string ouputFileDir)
        {
            string commandFilePath = GetCommandFilePath()  + "7z";
            StreamWriter sw = new StreamWriter(FileMg.AutoRDataFileDir + Constants.UnzipToolBatFileName);
            sw.WriteLine(@"""{0}"" e {1} -o""{2}"" -aoa", commandFilePath, inputFilePath, ouputFileDir);
            sw.Close();
        }
        
        public static bool RunUnzipTool(string inputFilePath, string ouputFileDir)
        {
            try
            {
                FileStream originalFileStream = new FileStream(inputFilePath, FileMode.Open);
                string newFileName = inputFilePath.Remove(inputFilePath.Length - 3);
                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                    }
                }
            }
            catch
            {
            }

            if (!File.Exists(FileMg.AutoRDataFileDir + Constants.RInputFileName))
            {
                return UnzipWith7Zip(inputFilePath, ouputFileDir);
            }

            return true;
        }

        public static bool UnzipWith7Zip(string inputFilePath, string ouputFileDir)
        {
            GenerateBat(inputFilePath, ouputFileDir);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = FileMg.AutoRDataFileDir + Constants.UnzipToolBatFileName;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using-statement will close.
                Process exeProcess = Process.Start(startInfo);
                {
                    exeProcess.WaitForExit();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GetCommandFilePath()
        {
            try
            {
                // Get path to 7zip
                RegistryKey localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                var rCore = localMachine64.OpenSubKey(@"SOFTWARE\7-Zip") ??
                    Registry.LocalMachine.OpenSubKey(@"SOFTWARE\7-Zip") ??
                    Registry.CurrentUser.OpenSubKey(@"SOFTWARE\7-Zip");
                var is64Bit = Environment.Is64BitProcess;
                if (rCore != null)
                {
                    var installPath = (string)rCore.GetValue("Path");
                    rCore.Close();

                    return installPath;
                }
            }
            catch
            { }

            return null;
        }
    }
}

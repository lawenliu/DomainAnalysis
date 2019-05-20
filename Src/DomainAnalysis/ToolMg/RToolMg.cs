using DomainAnalysis.DataPrepare;
using DomainAnalysis.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis
{
    class RToolMg
    {
        private static bool ConfigureOptionFile()
        {
            try
            {
                StreamReader sr = new StreamReader(FileMg.RToolDir + Constants.RScriptTemplateFileName);
                string optionContent = sr.ReadToEnd();
                sr.Close();

                optionContent = optionContent.Replace(Constants.RInputFileNamePlaceHolder, FileMg.AutoRDataFileDir.Replace(@"\", @"\\") + Constants.RInputFileName);
                optionContent = optionContent.Replace(Constants.ROutputFileNamePlaceHolder, FileMg.AutoRDataFileDir.Replace(@"\", @"\\") + Constants.ROutputFileName);

                StreamWriter sw = new StreamWriter(FileMg.AutoRDataFileDir + Constants.RScriptGeneratedFileName);
                sw.Write(optionContent);
                sw.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void GenerateBat()
        {
            string commandFilePath = GetCommandFilePath();
            StreamWriter sw = new StreamWriter(FileMg.AutoRDataFileDir + Constants.RToolBatFileName);
            sw.WriteLine(@"""{0}"" {1}", commandFilePath, FileMg.AutoRDataFileDir + Constants.RScriptGeneratedFileName);
            sw.Close();
        }
        
        public static bool RunRTool()
        {
            if (ConfigureOptionFile())
            {
                GenerateBat();

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = FileMg.AutoRDataFileDir + Constants.RToolBatFileName;
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

            return false;
        }

        private static string GetCommandFilePath()
        {
            try
            {                    
                // Get path to R
                RegistryKey localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                var rCore = localMachine64.OpenSubKey(@"SOFTWARE\R-core") ??
                    Registry.LocalMachine.OpenSubKey(@"SOFTWARE\R-core") ??
                            Registry.CurrentUser.OpenSubKey(@"SOFTWARE\R-core");
                var is64Bit = Environment.Is64BitProcess;
                if (rCore != null)
                {
                    var r = rCore.OpenSubKey(is64Bit ? "R64" : "R");
                    var installPath = (string)r.GetValue("InstallPath");
                    var binPath = Path.Combine(installPath, "bin");
                    binPath = Path.Combine(binPath, "Rscript");

                    return binPath;
                }

                return null;
            }
            catch
            { }

            return null;
        }
    }
}

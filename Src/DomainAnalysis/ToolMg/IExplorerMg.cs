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

namespace DomainAnalysis.ToolMg
{
    class IExplorerMg
    {
        private static void GenerateBat(string htmlFile)
        {
            string commandFilePath = GetCommandFilePath();
            StreamWriter sw = new StreamWriter(FileMg.AutoRDataFileDir + Constants.UnzipToolBatFileName); /**** Fix this path *****/
            sw.WriteLine(@"""{0}"" ""{1}""", commandFilePath, htmlFile);
            sw.Close();
        }

        public static bool RunIExplorerTool(string htmlFile)
        {
            GenerateBat(htmlFile);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = FileMg.AutoRDataFileDir + Constants.UnzipToolBatFileName; /**** Fix this path *****/
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
                string path = @"\http\shell\open\command";

                using (RegistryKey reg = Registry.ClassesRoot.OpenSubKey(path))
                {
                    if (reg != null)
                    {
                        string webBrowserPath = reg.GetValue(String.Empty) as string;

                        if (!String.IsNullOrEmpty(webBrowserPath))
                        {
                            if (webBrowserPath.First() == '"')
                            {
                                return webBrowserPath.Split('"')[1];
                            }

                            return webBrowserPath.Split(' ')[0];
                        }
                    }

                    return null;
                }
            }
            catch
            { }

            return null;
        }
    }
}

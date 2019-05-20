using DomainAnalysis.DataPrepare;
using DomainAnalysis.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis
{
    public class TmtToolMg
    {
        private static bool ConfigureScalaFile(string topicNumberArray, string maxIteration)
        {
            try
            {
                StreamReader sr = new StreamReader(FileMg.TmtToolDir + Constants.TmtScalaTemplateFileName);
                string scalaContent = sr.ReadToEnd();
                sr.Close();

                scalaContent = scalaContent.Replace(Constants.TmtInputFilePathPlaceHolder, FileMg.AutoTmtDataFileDir.Replace(@"\", @"\\") + Constants.TmtInputFileName);
                scalaContent = scalaContent.Replace(Constants.TmtInfoFilePathPlaceHolder, FileMg.AutoTmtOutputFileDir.Replace(@"\", @"\\") + Constants.TmtOutputInfoFileName);
                scalaContent = scalaContent.Replace(Constants.TmtTopicNumberArraryPlaceHolder, topicNumberArray);
                scalaContent = scalaContent.Replace(Constants.TmtMaxIterationPlaceHolder, maxIteration);
                scalaContent = scalaContent.Replace(Constants.TmtMinimumTopicPlaceHolder, Constants.TmtMinimumTopicKey);
                scalaContent = scalaContent.Replace(Constants.TmtOutputFileDirPlaceHolder, Constants.TmtOutputFileDirName);

                StreamWriter sw = new StreamWriter(FileMg.AutoTmtDataFileDir + Constants.TmtScalaGeneratedFileName);
                sw.Write(scalaContent);
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
            StreamWriter sw = new StreamWriter(FileMg.AutoTmtDataFileDir + Constants.TmtToolBatFileName);
            sw.WriteLine(string.Format("cd " + FileMg.AutoDataFolder));
            sw.WriteLine(string.Format(FileMg.AutoDataFolder.TrimStart().Substring(0, 1)) + ":");
            sw.WriteLine(string.Format("java -jar {0} {1}", FileMg.TmtToolDir + Constants.TmtToolFileName, FileMg.AutoTmtDataFileDir + Constants.TmtScalaGeneratedFileName));
            sw.Close();
        }

        public static bool RunTmtTool(string topicNumberArray, string maxIteration)
        {
            if (ConfigureScalaFile(topicNumberArray, maxIteration))
            {
                GenerateBat();

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = FileMg.AutoTmtDataFileDir + Constants.TmtToolBatFileName;
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
    }
}

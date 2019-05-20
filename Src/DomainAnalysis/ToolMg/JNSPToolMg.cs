using DomainAnalysis.DataPrepare;
using DomainAnalysis.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis
{
    class JNSPToolMg
    {
        private static bool ConfigureOptionFile()
        {
            try
            {
                StreamReader sr = new StreamReader(FileMg.JnspToolDir + Constants.JnspOptionTemplateFileName);
                string optionContent = sr.ReadToEnd();
                sr.Close();

                optionContent = optionContent.Replace(Constants.JnspWindowNumberPlaceHolder, Constants.JnspOptionWindowNumber);
                optionContent = optionContent.Replace(Constants.JnspTxtCleanDataDirPlaceHolder, FileMg.AutoCleanTextFileDir);
                optionContent = optionContent.Replace(Constants.JnspCNTFileNamePlaceHolder, FileMg.AutoJNSPDataFileDir + Constants.JnspOptionCNTFileName);
                optionContent = optionContent.Replace(Constants.JnspFreqComboPlaceHolder, FileMg.JnspToolDir + Constants.JnspOptionFreqCombo);
                optionContent = optionContent.Replace(Constants.JnspStopWordPlaceHolder, FileMg.JnspToolDir + Constants.JnspOptionStopWord);

                StreamWriter sw = new StreamWriter(FileMg.AutoJNSPDataFileDir + Constants.JnspOptionGeneratedFieName);
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
            StreamWriter sw = new StreamWriter(FileMg.AutoJNSPDataFileDir + Constants.JnspToolBatFileName);
            sw.WriteLine(@"java -cp {0};{1}lib\tokenfilter.jar jnsp.Counter {2}", FileMg.JnspToolDir, FileMg.JnspToolDir, FileMg.AutoJNSPDataFileDir + Constants.JnspOptionGeneratedFieName);
            sw.Close();
        }
        
        public static bool RunJNSPTool()
        {
            if (ConfigureOptionFile())
            {
                GenerateBat();

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = FileMg.AutoJNSPDataFileDir + Constants.JnspToolBatFileName;
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

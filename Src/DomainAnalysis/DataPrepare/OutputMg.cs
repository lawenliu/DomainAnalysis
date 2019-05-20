using DomainAnalysis.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis
{
    class OutputMg
    {
        public static void OutputHeader1(BackgroundWorker backgroundWorker, string step, string header, int pValue = 0)
        {
            if(backgroundWorker != null)
            {
                string output = string.Format(Constants.OutputHeader1Temp, step, header);
                backgroundWorker.ReportProgress(pValue, output);
            }            
        }

        public static void OutputContent(BackgroundWorker backgroundWorker, string content, int pValue = 0)
        {
            if(backgroundWorker != null)
            {
                string output = string.Format(Constants.OutputContentTemp, content);
                backgroundWorker.ReportProgress(pValue, output);
            }            
        }
    }
}

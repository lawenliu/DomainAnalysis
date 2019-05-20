using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Office.Interop.Word;
using DomainAnalysis.Highlight;
using System.ComponentModel;
using System.Windows.Forms;
using DomainAnalysis.Utils;
using System.IO;

namespace DomainAnalysis.Highlight.HighlightOffice
{
    class HighlightDoc
    {
        string originalFilePath = "";
        string topicTermPath = "";
        string targetTopicName = "";
        string highlightedFilePath = "";
        Dictionary<int, string> pageContents = new Dictionary<int, string>();

        public HighlightDoc(string originalFilePath, string topicTermsPath, string targetTopic, string highlightedFile)
        {
            this.originalFilePath = originalFilePath;
            this.topicTermPath = topicTermsPath;
            this.targetTopicName = targetTopic;
            this.highlightedFilePath = highlightedFile;
        }

        public Dictionary<int, string> ExecuteHighlight(BackgroundWorker backgroundWorker)
        {
            OutputMg.OutputContent(backgroundWorker, "Starting highlight file " + originalFilePath);

            //if the document is open, close it firstly
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("WINWORD");
            if (processes != null)
            {
                if (processes.Length > 0)
                {
                    string targetDocPath = "";
                    int dirIndex = highlightedFilePath.LastIndexOf("\\");
                    if (dirIndex > 0)
                    {
                        targetDocPath = highlightedFilePath.Substring(dirIndex + 2);
                    }
                    foreach (System.Diagnostics.Process process in processes)
                    {
                        string temp = process.MainWindowTitle.ToString();
                        if (temp.Length == 0)
                        {
                            process.Kill();
                        }
                        else if (temp.Contains(targetDocPath))
                        {
                            process.Kill();
                            System.IO.File.Delete(highlightedFilePath);
                        }
                        
                    }
                }
            }

            Dictionary<int, string> pageContents = new Dictionary<int, string>();
            var app = new Microsoft.Office.Interop.Word.Application();

            app.Visible = false;
            object readOnly = false;
            object missing = System.Reflection.Missing.Value;
            var doc = app.Documents.Open(this.originalFilePath, missing, readOnly);

            int pageNum = doc.Content.ComputeStatistics(Microsoft.Office.Interop.Word.WdStatistic.wdStatisticPages); //doc page

            List<string> topicTerms = ReadTargetTopicTerms.ParseTopicTerms(this.topicTermPath, this.targetTopicName);

            AddUserSearchTerms(topicTerms);

            //identify each word
            for (int p = 1; p <= pageNum; p++)
            {
                OutputMg.OutputContent(backgroundWorker, "Parsing page: " + p);

                string pageHighlight = "";

                object what = WdGoToItem.wdGoToPage;
                object which = WdGoToDirection.wdGoToAbsolute;
                object nextPage = p + 1;
                Range startRange;
                Range endRange;

                try
                {
                    startRange = app.Selection.GoTo(ref what, ref which, p, ref missing);
                    endRange = app.Selection.GoTo(what, which, nextPage, missing);
                }
                catch (Exception)
                {
                    doc.Close();
                    app.Quit();
                    MessageBox.Show("This document is locked by author. We cannot execute highlight", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                }
                if (startRange.Start == endRange.Start)
                {
                    which = WdGoToDirection.wdGoToLast;
                    what = WdGoToItem.wdGoToLine;
                    endRange = app.Selection.GoTo(what, which, nextPage, missing);
                }

                endRange.SetRange(startRange.Start, endRange.End);

                foreach (Paragraph field in endRange.Paragraphs)
                {
                    Range fieldRange = field.Range;
                    string paraText = fieldRange.Text.ToLower();

                    if (paraText.Length == 0)
                    {
                        continue;
                    }
                    else
                    {
                        foreach (string topicTerm in topicTerms)
                        {
                            if (paraText.Contains(topicTerm) || paraText.Contains(topicTerm + "s"))
                            {
                                fieldRange.HighlightColorIndex = WdColorIndex.wdYellow;
                                pageHighlight += paraText + "\t";
                                break;
                            }
                        }
                    }
                }
                pageContents.Add(p, pageHighlight);
            }

            doc.SaveAs2(this.highlightedFilePath);
            doc.Close();
            app.Quit();

            return pageContents;
        }

        private void AddUserSearchTerms(List<string> topicTerms)
        {
            string extraSearchTermPath = Configures.GetManualSearchTermPath();
            if (!File.Exists(extraSearchTermPath))
            {
                return;
            }
            else
            {
                string[] lines = FileOperators.ReadFileLines(extraSearchTermPath);
                foreach (string line in lines)
                {
                    if (line.Contains(":"))
                    {
                        int commaIndex = line.IndexOf(":");
                        string compName = line.Substring(0, commaIndex);
                        if (compName.Equals(targetTopicName))
                        {
                            string extraTermStr = line.Substring(commaIndex + 1);
                            string[] extraTerms = extraTermStr.Split(',');
                            foreach (string extraTerm in extraTerms)
                            {
                                string trimmedTerm = extraTerm.Trim();
                                if (!topicTerms.Contains(trimmedTerm))
                                {
                                    topicTerms.Add(trimmedTerm);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

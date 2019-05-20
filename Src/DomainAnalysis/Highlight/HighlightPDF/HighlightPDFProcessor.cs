using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.text;

using DomainAnalysis.Utils;
using System.ComponentModel;
using DomainAnalysis.Highlight;

namespace DomainAnalysis.HighlightPDF
{
    /*entry is in the form!
     */
    public class HighlightPDFProcessor
    {
        string pdfFilePath = "";
        string topicTermPath = "";
        string targetTopicName = "";
        string highlightedPDFPath = "";
        Dictionary<int, string> pageContents = new Dictionary<int, string>();

        //Here, the tacticterms are from the stored files
        public HighlightPDFProcessor(string paraPdfPath, string topicTermsPath, string targetTopic, string highlightedPDF)
        {
            this.pdfFilePath = paraPdfPath;
            this.topicTermPath = topicTermsPath;
            this.targetTopicName = targetTopic;
            this.highlightedPDFPath = highlightedPDF;
        }

        public Dictionary<int, string> ExecuteHighlight(BackgroundWorker backgroundWorker)
        {
            OutputMg.OutputContent(backgroundWorker, "Starting highlight file " + pdfFilePath);

            List<string> topicTerms = ReadTargetTopicTerms.ParseTopicTerms(this.topicTermPath, this.targetTopicName);

            AddUserSearchTerms(topicTerms);

            string origiFile = pdfFilePath;

            //Create a new file from our test file with highlighting
            string highLightFile = highlightedPDFPath;

            int pdfNum = 0;

            PdfReader reader = new PdfReader(origiFile);

            using (FileStream fs = new FileStream(highLightFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (PdfStamper stamper = new PdfStamper(reader, fs))
                {
                    using (var r = new PdfReader(origiFile))
                    {
                        pdfNum = r.NumberOfPages;
                        string ex = "";
                        ITextExtractionStrategy strategy;

                        for (int i = 1; i <= pdfNum; i++)
                        {
                            OutputMg.OutputContent(backgroundWorker, "Parsing page: " + i);

                            Rectangle pageRect = r.GetPageSize(i);

                            Document doc = new Document(pageRect);

                            float leftMargin = doc.LeftMargin;
                            float rightMargin = doc.RightMargin;
                            float lineWidth = pageRect.Width;

                            var textPos = new FutherLocationTextExtractionStrategy(topicTerms);

                            //Create an instance of our strategy
                            ex = PdfTextExtractor.GetTextFromPage(r, i, textPos); //store the text and the position info in textPos
                            List<iTextSharp.text.Rectangle> quadList = new List<iTextSharp.text.Rectangle>();

                            foreach (var p in textPos.myPoints)
                            {
                                string p_text = p.Text;

                                iTextSharp.text.Rectangle rect = p.Rect;


                                quadList.Add(rect);//collect the coordination of keywords
                            }

                            List<string> pageContent = new List<string>();

                            if (quadList.Count > 0)
                            {
                                List<iTextSharp.text.Rectangle> orderedRect = orderRectByBottom(quadList);
                                //merge and adjust the rectangle, highlight the adjusted rect
                                List<iTextSharp.text.Rectangle> adjustedRect = adjustRect(orderedRect, lineWidth, leftMargin);
                                foreach (Rectangle rect in adjustedRect)
                                {
                                    //Create an array of quad points based on that rectangle. NOTE: The order below doesn't appear to match the actual spec but is what Acrobat produces
                                    //the co-ordination of four points
                                    float[] quad = { rect.Left, rect.Bottom, rect.Right, rect.Bottom, rect.Left, rect.Top, rect.Right, rect.Top };

                                    ////Create our hightlight
                                    PdfAnnotation highlight = PdfAnnotation.CreateMarkup(stamper.Writer, rect, null, PdfAnnotation.MARKUP_HIGHLIGHT, quad);

                                    ////Set the color
                                    highlight.Color = BaseColor.YELLOW;

                                    stamper.AddAnnotation(highlight, i); // i is the page

                                    //get the text of highlighting
                                    RenderFilter[] filter = { new RegionTextRenderFilter(rect) };
                                    strategy = new MyFilteredTextRenderListener(new LocationTextExtractionStrategy(), filter);
                                    string text = PdfTextExtractor.GetTextFromPage(reader, i, strategy).Trim();
                                    if (!pageContent.Contains(text))
                                    {
                                        pageContent.Add(text);
                                    }

                                }
                                StringBuilder sb = new StringBuilder();

                                foreach (string tmp in pageContent)
                                {
                                    sb.AppendLine(tmp);
                                }

                                pageContents.Add(i, sb.ToString());
                            }

                        }
                    }
                }
            }

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

        private List<iTextSharp.text.Rectangle> orderRectByBottom(List<iTextSharp.text.Rectangle> quadList)
        {
            List<Rectangle> orderedRectList = new List<Rectangle>();
            orderedRectList = quadList.OrderByDescending(x => x.Bottom).ThenBy(x => x.Left).ToList();
            return orderedRectList;
        }

        /* we assume that the line spacing is 1.5 times of font size
         * if two keywords are in the same line, merge the rect
         * if two keywords are in the consecutive lines, adjust the two rect
         */
        private List<iTextSharp.text.Rectangle> adjustRect(List<iTextSharp.text.Rectangle> rectList, float lineWidth, float leftMargin)
        {
            List<iTextSharp.text.Rectangle> adjustedRectList = new List<Rectangle>();
            int existingRectScale = rectList.Count;
            adjustedRectList.Add(rectList[0]);
            float fontHeight = rectList[0].Top - rectList[0].Bottom;

            for (int i = 1; i < existingRectScale; i++)
            {
                Rectangle curRect = rectList[i];
                int lastPos = adjustedRectList.Count - 1;
                Rectangle nearestRect = adjustedRectList[lastPos];
                bool sameLine = isInSameLine(curRect, nearestRect);//whether two rects are in the same line
                if (!sameLine)//if they are not in the same line
                {
                    bool consecutiveline = isConsecutiveLine(curRect, nearestRect, fontHeight);
                    if (consecutiveline) //if the two rects are in the consecutive lines
                    {
                        var updateNearestRect = new iTextSharp.text.Rectangle(nearestRect.Left, nearestRect.Bottom, lineWidth - nearestRect.Left, nearestRect.Top);
                        var newNearestRect = new iTextSharp.text.Rectangle(leftMargin, curRect.Bottom, curRect.Right, curRect.Top);
                        adjustedRectList[lastPos] = updateNearestRect;
                        adjustedRectList.Add(newNearestRect);
                    }
                    else //if they are not in the consecutive line
                    {
                        adjustedRectList.Add(curRect);
                    }
                }
                else //if they are in the same line
                {
                    var newRect = new iTextSharp.text.Rectangle(nearestRect.Left, nearestRect.Bottom, curRect.Right, curRect.Top);
                    adjustedRectList[lastPos] = newRect;
                }
            }
            //    Console.WriteLine("calculate the rect, done!");
            return adjustedRectList;
        }

        //if in the same line
        private bool isInSameLine(Rectangle A, Rectangle B)
        {
            bool result = false;
            result = (B.Bottom == A.Bottom) ? true : false;
            return result;
        }

        //whether in the consecutive lines
        private bool isConsecutiveLine(Rectangle A, Rectangle B, float fontHeight)
        {
            bool result = false;
            float h_dis = B.Top - A.Bottom;
            result = (h_dis / fontHeight > 1.5) ? false : true;
            return result;
        }

    }
}

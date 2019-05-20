using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using iTextSharp.text;

using DomainAnalysis.Utils;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace DomainAnalysis.ExtractContent
{
    //the work of extracting doc, ppt and .xls has been done in eclipse
    public static class ExtractPDF
    {   
        public static void ExecuteExtraction(string sourceFileName, string destFileName, BackgroundWorker backgroundWorker = null)
        {
            PdfReader reader = new PdfReader(sourceFileName);

            int pdfNum = reader.NumberOfPages;
            for (int i = 1; i <= pdfNum; i++)
            {
                var textPos = new MyLocationExtractionStrategy();

                //Create an instance of our strategy
                string ex = PdfTextExtractor.GetTextFromPage(reader, i, textPos); //store the text and the position info in textPos
                FileOperators.FileAppend(destFileName, ex);
            }
        }
    }
}
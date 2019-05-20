using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using DomainAnalysis.Utils;
using Code7248.word_reader;

namespace DomainAnalysis.ExtractContent
{
    class ExtractWord
    {
        //too slow, deprecated
        public static void ExecuteExtraction(string sourceFileName, string destFileName)
        {
            Application _application = new Application();
            object docFilenameAsObject = sourceFileName;
            Document _document = _application.Documents.Open(ref docFilenameAsObject);
            string docContent = "";

            try
            {
                int paraNum = _document.Paragraphs.Count;
                foreach (Paragraph para in _document.Paragraphs)
                {
                    Range paraRange = para.Range;
                    docContent += paraRange.Text;
                }


                FileOperators.FileWrite(destFileName, docContent);
                ((_Document)_document).Close();
                ((_Application)_application).Quit();
            }
            catch (Exception)
            {
                ((_Document)_document).Close();
                ((_Application)_application).Quit();
            }
            
        }

        public static void ExecuteWordExtraction(string sourceFileName, string destFileName)
        {
            TextExtractor extractor = new TextExtractor(sourceFileName);
            string wordText = extractor.ExtractText();
            FileOperators.FileWrite(destFileName,wordText);
        }

     
    }    
}

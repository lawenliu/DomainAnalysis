using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using DomainAnalysis.Utils;

namespace DomainAnalysis.ExtractContent
{   
    class ExtractPPT
    {
        public static void ExecuteExtraction(string sourceFileName, string destFileName)
        {
            Application _application = new Microsoft.Office.Interop.PowerPoint.Application();
            var pres = _application.Presentations;
            Presentation pptFile = pres.Open(sourceFileName, MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);

            string storeContent = "";
            int slideCount = pptFile.Slides.Count;
            for (int i = 1; i <= slideCount; i++)
            {
                Slide slide = pptFile.Slides[i];
                slide.FollowMasterBackground = MsoTriState.msoFalse;
                foreach (var item in slide.Shapes)
                {
                    var shape = (Microsoft.Office.Interop.PowerPoint.Shape)item;
                    if (shape.HasTextFrame == MsoTriState.msoTrue)
                    {

                        //shape.Fill.ForeColor.RGB = System.Drawing.ColorTranslator.ToWin32(Color.Red);
                        var textRange = shape.TextFrame.TextRange;
                        var text = textRange.Text;
                        storeContent += text + " ";
                    }

                }
            }

            FileOperators.FileWrite(destFileName, storeContent);
            pptFile.Close();
            _application.Quit();          
        }
    }
}

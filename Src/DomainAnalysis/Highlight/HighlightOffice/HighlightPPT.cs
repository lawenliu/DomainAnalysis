using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using System.Drawing;

namespace DomainAnalysis.Highlight
{
    class HighlightPPT
    {
        string fileName = @"C:\Users\xlian\MyPapers\Simmons\test.pptx";

        string textToFind = "Chicago";

        public void Hightlight()
        {
            try
            {
                var app = new Microsoft.Office.Interop.PowerPoint.Application();
                var pres = app.Presentations;
                Presentation file = pres.Open(fileName, MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);
                //Presentation presentation = Globals.ThisAddIn.Application.ActivePresentation;
                // file.SaveCopyAs(copyFileName, Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsJPG, MsoTriState.msoTrue);
                try
                {
                    int slideCount = file.Slides.Count;
                    for (int i = 1; i <= slideCount; i++)
                    {
                        Slide slide = file.Slides[i];
                        slide.FollowMasterBackground = MsoTriState.msoFalse;
                        foreach (var item in slide.Shapes)
                        {
                            var shape = (Microsoft.Office.Interop.PowerPoint.Shape)item;
                            if (shape.HasTextFrame == MsoTriState.msoTrue)
                            {

                                //shape.Fill.ForeColor.RGB = System.Drawing.ColorTranslator.ToWin32(Color.Red);
                                var textRange = shape.TextFrame.TextRange;
                                var text = textRange.Text;
                                Console.WriteLine("text:" + text);
                                int index = text.IndexOf(textToFind);
                                if (index >= 0)
                                {
                                    Console.WriteLine("found");
                                    shape.TextFrame.TextRange.Font.Color.RGB = BGR(Color.Green);
                                }

                            }

                        }
                    }
                    file.Save();
                    file.SaveAs(@"C:\Users\xlian\MyPapers\Simmons\tested.pptx", PpSaveAsFileType.ppSaveAsDefault, MsoTriState.msoTrue);
                    file.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    file.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private int BGR(Color color)
        {

            // PowerPoint's color codes seem to be reversed (i.e., BGR) not RGB, so we have  to  produce the color in reverse

            int iColor = (color.A << 24) | (color.B << 16) | (color.G << 8) | color.R;
            return iColor;
        }
    }
}

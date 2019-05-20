using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Office.Interop.Excel;

namespace DomainAnalysis.Highlight
{
    class HighlightXLS
    {
        string fileName = @"C:\Users\xlian\MyPapers\Simmons\analysis.xlsx";

        string copyFileName = @"C:\Users\xlian\MyPapers\Simmons\analysisCopy.xlsx";


        string textToFind = "PTC";

        public void Highlight()
        {
            if (Directory.Exists(copyFileName))
            {
                Directory.Delete(copyFileName);
            }

            var app = new Application();
            Workbook oriWorkBook = app.Workbooks.Open(fileName);
            oriWorkBook.SaveCopyAs(copyFileName);

            Workbook xlWorkBook = app.Workbooks.Open(copyFileName);
            
            int sheetNum = xlWorkBook.Worksheets.Count;

            for (int i = 1; i <= sheetNum; i++)
            {
                Worksheet xlWorkSheet = xlWorkBook.Worksheets[i];

                // Detect Last used Row - Ignore cells that contains formulas that result in blank values
                int lastRowIgnoreFormulas = xlWorkSheet.Cells.Find(
                                "*",
                                System.Reflection.Missing.Value,
                                Microsoft.Office.Interop.Excel.XlFindLookIn.xlValues,
                                Microsoft.Office.Interop.Excel.XlLookAt.xlWhole,
                                Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                Microsoft.Office.Interop.Excel.XlSearchDirection.xlPrevious,
                                false,
                                System.Reflection.Missing.Value,
                                System.Reflection.Missing.Value).Row;
                // Detect Last Used Column  - Ignore cells that contains formulas that result in blank values
                int lastColIgnoreFormulas = xlWorkSheet.Cells.Find(
                                "*",
                                System.Reflection.Missing.Value,
                                System.Reflection.Missing.Value,
                                System.Reflection.Missing.Value,
                                Microsoft.Office.Interop.Excel.XlSearchOrder.xlByColumns,
                                Microsoft.Office.Interop.Excel.XlSearchDirection.xlPrevious,
                                false,
                                System.Reflection.Missing.Value,
                                System.Reflection.Missing.Value).Column;

                // Detect Last used Row / Column - Including cells that contains formulas that result in blank values
                for (int j = 1; j <= lastRowIgnoreFormulas; j++)
                {
                    for (int k = 1; k <= lastColIgnoreFormulas; k++)
                    {
                        Range usedRange = xlWorkSheet.Cells[j, k];
                        string rangeText = usedRange.Text;
                        Console.WriteLine(rangeText);

                        int index = rangeText.IndexOf(textToFind);
                        if (index >= 0)
                        {
                            usedRange.Interior.Color = 255;
                        }
                        
                    }
                }
            }

            xlWorkBook.Save();
            xlWorkBook.Close();

        }

    }
}

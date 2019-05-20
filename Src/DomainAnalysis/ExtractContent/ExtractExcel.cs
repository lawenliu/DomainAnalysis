using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Office.Interop.Excel;
using DomainAnalysis.Utils;

namespace DomainAnalysis.ExtractContent
{
    class ExtractExcel
    {
        public static void ExecuteExtraction(string sourceFileName, string destFileName)
        {
            Application _application = new Application();
            Workbook oriWorkBook = _application.Workbooks.Open(sourceFileName);

            int sheetNum = oriWorkBook.Worksheets.Count;

            for (int i = 1; i <= sheetNum; i++)
            {
                string fileContent = "";
                Worksheet xlWorkSheet = oriWorkBook.Worksheets[i];

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
                        fileContent += rangeText + " ";
                    }
                }

                FileOperators.FileAppend(destFileName, fileContent);
            }

            oriWorkBook.Close(XlSaveAction.xlDoNotSaveChanges);
            _application.Application.Quit();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis.SummaryGeneration
{
    class Matrix
    {
        public int rows;
        public int cols;
        public float[] mat;

        public Matrix(int iRows, int iCols)
        {
            rows = iRows;
            cols = iCols;
            mat = new float[rows * cols];
        }

        public float GetRowCol(int iRow, int iCol)
        {
            return mat[iRow * cols + iCol];
        }

        public void SetRowCol(int iRow, int iCol, float value)
        {
            mat[iRow * cols + iCol] = value;
        }
    }
}

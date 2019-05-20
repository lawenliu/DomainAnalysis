using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis.Utils
{
    class FileNameParse
    {
        public static string GetFileName(string filePath)
        {
            int lastSlashIndex = filePath.LastIndexOf("\\");
            string fileName = filePath.Substring(lastSlashIndex + 1);
            return fileName;
        }
    }
}

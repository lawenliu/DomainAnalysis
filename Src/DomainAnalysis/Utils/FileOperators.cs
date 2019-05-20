using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis.Utils
{
    public class FileOperators
    {
        public static string ReadFileText(string filePath)
        {
            string content = null;
            if (File.Exists(filePath))
            {
                content = File.ReadAllText(filePath);
            }

            return content;
        }

        public static string[] ReadFileLines(string filePath)
        {
            string[] lines = null;
            if (File.Exists(filePath))
            {
                lines = File.ReadAllLines(filePath);
            }
            
            return lines;
        }

        public static void FileWrite(string filePath, string content)
        {
            int slashIndex = filePath.LastIndexOf('\\');
            if (slashIndex > 0)
            {
                string dir = filePath.Substring(0, slashIndex);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            //System.IO.File.WriteAllText(filePath,content);
            StreamWriter sw = new StreamWriter(filePath, true);
            sw.Write(content);
            sw.WriteLine();
            sw.Close();
        }

        public static void FileAppend(string filePath, string appendContent)
        {
            int slashIndex = filePath.LastIndexOf('\\');
            if (slashIndex > 0)
            {
                string dir = filePath.Substring(0, slashIndex);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }

            StreamWriter sw = new StreamWriter(filePath, true);
            sw.Write(appendContent);
            sw.WriteLine();
            sw.Close();

            //if (!File.Exists(filepath))
            //{
            //    File.WriteAllText(filepath, appendcontent);
            //}
            //else
            //{
            //    file.appendalltext(filepath, appendcontent);
            //}
            //using (system.io.streamwriter file = new streamwriter(filepath, true))
            //{
            //    file.writeline(appendcontent);
            //}
        }

        internal static object DirectoryCopy(string text, string defaultSourceFileDir, bool v, BackgroundWorker backgroundWorker)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using DomainAnalysis.Utils;

namespace DomainAnalysis.DocumentRanking.VSMRankDoc
{
    /*the tfidf for all terms in all files
     * write into a .csv file
     * filePath: the path of all project files
     * tacticTerms: the terms of tactic-related
     * storePath: store the tfidf
     * 
     * NOTE:for all documents and one topic
     */
    class TFIDF
    {
        string fileDir;
        List<string> tacticTerms;
        string storePath;

        Dictionary<string, int> termRelateDocNum = new Dictionary<string, int>(); //the number of documents that includes specific terms
        Dictionary<string, double> terms_idf;
        int docCount = 0; //count the number of project files

        public TFIDF(List<string> terms, string fileDir, string storePath)
        {
            this.fileDir = fileDir;
            tacticTerms = terms;
            this.storePath = storePath;
        }
       
        /*generate and store the tf-idf of each file
         */
        public void calTfidf()
        {
            string titleContent = "fileName,";
            foreach (string term in tacticTerms)
            {
                titleContent += term + ";";
            }
            titleContent = titleContent.Remove(titleContent.Length - 1);
            FileOperators.FileWrite(this.storePath, titleContent);

            terms_idf = idf(); //calculate idf
            string[] fileEntries = Directory.GetFiles(fileDir);
            updateTfidf(fileEntries);
            string[] directories = Directory.GetDirectories(fileDir);
            foreach (string directory in directories)
            {
                string[] subFileEntries = Directory.GetFiles(directory);
                updateTfidf(subFileEntries);
            }
           
        }

        private void updateTfidf(string[] fileEntries)
        {

            foreach (string fileName in fileEntries)
            {
                //string storeFilePath = this.storePath + "/" + fileName;
                //int slashIndex = fileName.LastIndexOf('\\');
                //string filteredFileName = fileName.Substring(0, slashIndex);

                string tfidfContent = fileName + ";";
                foreach (string term in tacticTerms)
                {
                    int tf_value = tf(term, fileName);
                    double term_idf = terms_idf[term];
                    double tf_idf = tf_value * term_idf;
                    tfidfContent += tf_idf + ";";
                }
                tfidfContent = tfidfContent.Remove(tfidfContent.Length - 1);

                FileOperators.FileAppend(this.storePath, tfidfContent);
            }
        }

        /*the number of times a term occurs in a document
         * for one term and one doc.
         */
        private int tf(string term, string filePath)
        {
            int freq = 0;
            string fileContent = FileOperators.ReadFileText(filePath);
            freq = (fileContent.Length - fileContent.Replace(term, "").Length) / term.Length;
            return freq;
        }


        //for all terms related with a topic
        /* find out the relevant documents match the query
         * return the idf of all terms in one topic, <term, idf>
         * IDF(game) = 1 + log_e(Total Number Of Documents / Number Of Documents with term game in it)
         */
        private Dictionary<string, double> idf()
        {
            Dictionary<string, double> term_idf = new Dictionary<string, double>();

            foreach (string term in tacticTerms)
            {
                termRelateDocNum.Add(term, 0);
            }
            
            string[] fileEntries = Directory.GetFiles(fileDir);
            updateTermDocNum(fileEntries);
            docCount = fileEntries.Count();//update

            string[] directories = Directory.GetDirectories(fileDir);
            foreach (string directory in directories)
            {
                string[] subDirectoryFiles = Directory.GetFiles(directory);
                updateTermDocNum(subDirectoryFiles);
                docCount += subDirectoryFiles.Count();
            }

            foreach (string key in termRelateDocNum.Keys)
            {
                int value = termRelateDocNum[key]; //the number of doc that include key
                if (value == 0)
                {
                    value = 1;
                }
                double cur_idf = 1 + Math.Log(docCount / value);
                term_idf.Add(key, cur_idf);
            }
            return term_idf;
        }

        private void updateTermDocNum(string[] fileEntries)
        {
            foreach (string fileName in fileEntries) //fullfill the termRelateDocNum
            {
                string fileContent = Utils.FileOperators.ReadFileText(fileName);
                foreach (string term in tacticTerms)
                {
                    if (fileContent.Contains(term))
                    {
                        termRelateDocNum[term]++;
                       // Console.WriteLine(fileName + ":" + term);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
    }
}

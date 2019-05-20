using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis.DocumentRanking.VSMRankDoc
{
    /*
     * calculate the similarity between document and query
     * firstly, calculate the term weight in each document (tf-idf)
     * second, calculate the cosin similarity between document and query
     */
    class VSM
    {
        /*
         * calculate the similarity between one document and the topic which are modeled as the vector of key terms
         */
        public double calSimilarity(List<float> docVector, List<float> queryVector)
        {
            double sim = 0;
            int vectorSize = docVector.Count;
            double doc_mod = 0;
            double query_mod = 0;
            double dotProduct = 0;

            for (int i = 0; i < vectorSize; i++)
            {
                double doc_elem = docVector.ElementAt(i);
                double query_elem = queryVector.ElementAt(i);
                dotProduct += doc_elem * query_elem;
                doc_mod += doc_elem * doc_elem;
                query_mod += query_elem * query_elem;
            }
            if (doc_mod == 0 || query_mod == 0)
            {
                sim = 0;
            }
            else
            {
                sim = dotProduct / (Math.Sqrt(doc_mod) * Math.Sqrt(query_mod));
            }
            return sim;
        }
    }
}

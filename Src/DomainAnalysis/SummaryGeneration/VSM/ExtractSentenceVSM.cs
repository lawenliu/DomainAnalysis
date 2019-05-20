using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis.SummaryGeneration.VSM
{
    class ExtractSentenceVSM
    {
        private float threshold = 0f;
        public string ExtractSentenceWithVSM(Dictionary<string, float> qualityTerms,  List<string> candidateSentences)
        {
            string selectedSentence = "";

            Dictionary<string, List<float>> docTermProps = new Dictionary<string, List<float>>(); //record the term props in sentence

            List<string> termList = qualityTerms.Keys.ToList();
            List<float> weightList = qualityTerms.Values.ToList();

            TF_IDF docTFIDF = new TF_IDF();
            docTermProps = docTFIDF.CalculateTFIDF(candidateSentences, termList);

            //calculate the cosine similarity
            foreach (string sentence in docTermProps.Keys)
            {
                List<float> termWeightList = docTermProps[sentence];
                double simialrity = CosineSimilarity(termWeightList, weightList);
                if (simialrity > threshold)
                {
                    //selectedSentence += sentence + "\t" + simialrity + "\r\n";
                    selectedSentence += sentence + "\r\n";
                }
            }


            return selectedSentence;
        }

        private double CosineSimilarity(List<float> sentenceList, List<float> qualityList)
        {
            int listScale = sentenceList.Count;
            float product = 0f;
            float reqModule = 0f;
            float topicModule = 0f;
            double similarity = 0f;
            for (int i = 0; i < listScale; i++)
            {
                product += sentenceList[i] * qualityList[i];
                reqModule += sentenceList[i] * sentenceList[i];
                topicModule += qualityList[i] * qualityList[i];
            }
            similarity = product / (Math.Sqrt(reqModule) * Math.Sqrt(topicModule));
            return similarity;
        }

    }
}

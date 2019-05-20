using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis.DocumentRanking.VSMRankDoc
{
    class NormalizeTopicRelevance
    {
        public Dictionary<string, Dictionary<string, float>> DoNormalize(Dictionary<string, Dictionary<string, string>> originalDic)
        {
            Dictionary<string, Dictionary<string, float>> normalized = new Dictionary<string, Dictionary<string, float>>();
            foreach (string topicName in originalDic.Keys)
            {
                Dictionary<string,string> termValues = originalDic[topicName];
                float valueSum = 0;
                foreach (string relevance in termValues.Values)
                {
                    valueSum += float.Parse(relevance);
                }

                Dictionary<string, float> afterNormalizedList = new Dictionary<string, float>();

                foreach (string term in termValues.Keys)
                {
                    string oriValueStr = termValues[term];
                    float afterNormalized = float.Parse(oriValueStr) / valueSum;
                    afterNormalizedList.Add(term,afterNormalized);
                }
                normalized.Add(topicName,afterNormalizedList);
            }
            return normalized;
        }
    }
}

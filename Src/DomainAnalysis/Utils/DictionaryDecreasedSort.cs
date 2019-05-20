using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis.Utils
{
    class DictionaryDecreasedSort
    {
        public static Dictionary<string, double> DecreasedByValue(Dictionary<string, double> originalDic)
        {
            var result = originalDic.OrderByDescending(i => i.Value);

            Dictionary<string, double> sortedDic = new Dictionary<string, double>();

            foreach (KeyValuePair<string, double> kvp in result)
            {
                sortedDic.Add(kvp.Key, kvp.Value);
            }
            return sortedDic;
        }

        public static Dictionary<string, int> DecreasedByValue(Dictionary<string, int> originalDic)
        {
            var result = originalDic.OrderByDescending(i => i.Value);

            Dictionary<string, int> sortedDic = new Dictionary<string, int>();

            foreach (KeyValuePair<string, int> kvp in result)
            {
                sortedDic.Add(kvp.Key, kvp.Value);
            }
            return sortedDic;
        }

        public static Dictionary<string, float> DecreasedByValue(Dictionary<string, float> originalDic)
        {
            var result = originalDic.OrderByDescending(i => i.Value);

            Dictionary<string, float> sortedDic = new Dictionary<string, float>();

            foreach (KeyValuePair<string, float> kvp in result)
            {
                sortedDic.Add(kvp.Key, kvp.Value);
            }
            return sortedDic;
        }
    }
}

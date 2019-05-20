using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DomainAnalysis.ExtractContent.Preprocess;

namespace DomainAnalysis.SummaryGeneration
{
    class RelevantSummary
    {
        private int SENTENCE_NUM = 10; //this is a threshold. it allows that there are 5 sentences in the final summary
        private int SENTENCE_LEN = 200;// alway the length of valid sentence is less than 40. we set the threshold 100.
        //calculate the scores of sentences according to the equation
        public string GenerateSummary(List<string> compTerms, List<string> candidateSentences)
        {
            string summary = "";
            //store the score of each candidate sentence
            Dictionary<string, float> sentenceScores = new Dictionary<string, float>();

            foreach (string sentence in candidateSentences)
            {
               // Console.WriteLine("Calculate the score of sentence!");
                float score = SentenceScore(sentence, compTerms);
                if (score > 0)
                {
                    sentenceScores.Add(sentence, score);
                }
            }

            sentenceScores = sentenceScores.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            List<string> keys = sentenceScores.Keys.ToList();

            int summaryScale = keys.Count > SENTENCE_NUM ? SENTENCE_NUM : keys.Count;

            List<string> pureWords = new List<string>(); //store the pure words of each sentence to filter the sentence replicators

            for (int i = 0, j = 0; i < keys.Count && j < summaryScale; i++)
            {
                string candidateSentence = keys[i];
                string pureWord = GetPureWords(candidateSentence);
                if (!pureWords.Contains(pureWord))
                {
                    summary += keys[i] + "\r\n";
                    pureWords.Add(pureWord);
                    j++;
                }
            }

            Console.WriteLine("generate summary, finish!");
            return summary;
        }

        public float SentenceScore(string sentence, Dictionary<string, float> compTerms)
        {
            float score = 0.0f;
            List<string> sentenceTerms = sentence.Split(' ').ToList();
            int sentenceLength = 0;
            RegexOptions option = RegexOptions.None;
            Regex regex = new Regex(@"[a-zA-Z]", option);
            Regex wordRex = new Regex(@"\W", option);
            List<string> qualityTermList = new List<string>();

            float numerator = 0.0f;

            if (sentenceTerms.Count > SENTENCE_LEN) //if the sentence is too long, we ignore it. because there must be some extraction problems, table or diagram.
            {
                return score;
            }

            string stemmedSentence = "";
            Porter2 porter = new Porter2();
            foreach (string term in sentenceTerms) //calculate the number of valid words
            {
                if (regex.Match(term).Success)
                {
                    string filteredTerm = wordRex.Replace(term, "");
                    string stemmedTerm = porter.stem(filteredTerm);
                    stemmedSentence += stemmedTerm + " ";
                    sentenceLength++;
                }
            }

            string filteredStr = stemmedSentence;

            foreach (string compTerm in compTerms.Keys) //test if all of the terms are stemmed
            {
                float weight = compTerms[compTerm];

                string filteredTerm = compTerm.Replace("-", "");

                filteredTerm = porter.stem(filteredTerm);

                string tmpFilteredStr = filteredStr.Replace(filteredTerm + " ", " ");
                int freq = (filteredStr.Length - tmpFilteredStr.Length) / (filteredTerm.Length);
                if (freq > 0)
                {
                    //edit for component summary (without quality)

                    numerator += freq * weight; //adjust here, if a sentence contains more quality info, it should be more relevant to a quality. But it should consist of component info.
                }

                filteredStr = tmpFilteredStr;

                tmpFilteredStr = filteredStr.Replace(filteredTerm + "s ", " ");
                freq = (filteredStr.Length - tmpFilteredStr.Length) / (filteredTerm.Length + 1);
                if (freq > 0)
                {
                    numerator += freq * weight;
                }
                filteredStr = tmpFilteredStr;

                tmpFilteredStr = filteredStr.Replace(filteredTerm + ")", " ");
                freq = (filteredStr.Length - tmpFilteredStr.Length) / filteredTerm.Length;
                if (freq > 0)
                {
                    numerator += freq * weight;
                }

                filteredStr = tmpFilteredStr;
            }
            score = numerator / sentenceLength;

            return score;

        }

        public float SentenceScore(string sentence, List<string> compTerms)
        {
           
            float score = 0.0f;

            
            int sentenceLength = 0;
            RegexOptions option = RegexOptions.None;
            Regex regex = new Regex(@"[a-zA-Z]", option);
            Regex wordRex = new Regex(@"\W", option);
            string filteredSentence = wordRex.Replace(sentence, " ");
            List<string> sentenceTerms = filteredSentence.Split(' ').ToList();
            List<string> qualityTermList = new List<string>();
            
            float numerator = 0.0f;

            //bool componentSign = false; //the final sentence should contain quality and component terms at the same time. Under this condition, the value should be 2

        //    int qualityTermCount = 0;
            if (sentenceTerms.Count > SENTENCE_LEN) //if the sentence is too long, we ignore it. because there must be some extraction problems, table or diagram.
            {
                return score;
            }

            string stemmedSentence = "";
            Porter2 porter = new Porter2();
            foreach (string term in sentenceTerms) //calculate the number of valid words
            {
                if (regex.Match(term).Success)
                {
                    string filteredTerm = wordRex.Replace(term, "");
                    string stemmedTerm = porter.stem(filteredTerm);
                    stemmedSentence += stemmedTerm + " ";
                    sentenceLength++;
                }
            }

            int qualityTermCovered = 0; //try to add the impact of the diversity of quality terms
            string filteredStr = stemmedSentence;

            filteredStr = stemmedSentence;

            ////////////////////////////////////////////////////////////////////////////////
            //if generate the summary for component only
            ////////////////////////////////////////////////////////////////////////////////
            foreach (string compTerm in compTerms) //test if all of the terms are stemmed
            {
                string filteredTerm = compTerm.Replace("-", "");

                filteredTerm = porter.stem(filteredTerm);

                string tmpFilteredStr = filteredStr.Replace(filteredTerm + " ", " ");
                int freq = (filteredStr.Length - tmpFilteredStr.Length) / (filteredTerm.Length);
                if (freq > 0)
                {
                   
                    numerator += freq; 
                }

                filteredStr = tmpFilteredStr;

                tmpFilteredStr = filteredStr.Replace(filteredTerm + "s ", " ");
                freq = (filteredStr.Length - tmpFilteredStr.Length) / (filteredTerm.Length + 1);
                if (freq > 0)
                {
                    numerator += freq;
                }
                filteredStr = tmpFilteredStr;

                tmpFilteredStr = filteredStr.Replace(filteredTerm + ")", " ");
                freq = (filteredStr.Length - tmpFilteredStr.Length) / filteredTerm.Length;
                if (freq > 0)
                {
                    numerator += freq;
                }

                filteredStr = tmpFilteredStr;
            }
            score = numerator / sentenceLength;

            return score;
        }

        


        private string GetPureWords(string sentence)
        {
            RegexOptions option = RegexOptions.None;
            Regex regex = new Regex(@"[^a-zA-Z]", option);
            string filtered = regex.Replace(sentence, "");
            return filtered;
        }
    }
}

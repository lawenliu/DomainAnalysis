using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DomainAnalysis.Utils;
using DomainAnalysis.ExtractContent.Preprocess;

namespace DomainAnalysis.SummaryGeneration
{
    class MMRSummary
    {
        private int SENTENCE_NUM = 10; //we generate 10 sentences in the summary
        List<string> ignoreSentence = new List<string>();//collect the sentences which are ignored during the summary selection. The reason includes: too long, same sentence has been selected
        private Dictionary<string, float> sim_sen_query = new Dictionary<string, float>(); //string: sentence id in the list of candidateSentence

        /* calculate the scores of sentences
         * the scores are calculated considering the similarity between the sentence with the query, and the dissimilarity betweent the selected sentence and the unselected ones
         * 
         * compTerms: the related terms of component
         * candidateSentences: all of the component-related sentences
         * 
         * step one: calculate the similarity between each sentence and the query (the component terms)
         * step two: calculate the pairwise dissimilarity between different sentences
         */
        public string GenerateSummary(List<string> compTerms, List<string> candidateSentences)
        {
            if (candidateSentences == null)
            {
                return null;
            }

            CalSimilaritySentenceQuery(compTerms, candidateSentences);//similarity between sentence and query
            string summary = GenerateFinalSummary(candidateSentences);
            return summary;
        }

        public string GenerateSummary( Dictionary<string, float> compTerms, List<string> candidateSentences)
        {
            CalSimilaritySentenceQuery(compTerms, candidateSentences);
            string summary = GenerateFinalSummary(candidateSentences);
            return summary;
        }

        //calculate the similarity between the components and component queries
        private void CalSimilaritySentenceQuery(List<string> compTerms, List<string> candidateSentences)
        {
            int sentenceScale = candidateSentences.Count;
            for (int i = 0; i < sentenceScale; i++ )
            {
                string sentence = candidateSentences[i];

                RelevantSummary relevantSummary = new RelevantSummary();
                float score = relevantSummary.SentenceScore(sentence, compTerms);
                if (score > 0)
                {
                    sim_sen_query.Add(i + "", score);
                }
                else
                {
                    ignoreSentence.Add(i + "");
                }
            }
        }

        private void CalSimilaritySentenceQuery(Dictionary<string, float> compTerms, List<string> candidateSentences)
        {
            int sentenceScale = candidateSentences.Count;
            for (int i = 0; i < sentenceScale; i++)
            {
                string sentence = candidateSentences[i];

                RelevantSummary relevantSummary = new RelevantSummary();
                float score = relevantSummary.SentenceScore(sentence, compTerms);
                if (score > 0)
                {
                    sim_sen_query.Add(i + "", score);
                }
                else
                {
                    ignoreSentence.Add(i + "");
                }
            }
        }

        private float CalDissimilarity(string sentence_i, string sentence_j)
        {
           
            Dictionary<string, int> terms_i = SplitTerms(sentence_i);
            if (terms_i.Count == 0)
            {
                return -1;
            }
                
            Dictionary<string, int> terms_j = SplitTerms(sentence_j);
            if (terms_j.Count == 0) //the sentence_j should not be empty
            {
                return -1;
            }

            float dissim = CalDissimilaritySentence(terms_i, terms_j);
            return dissim;
        }

        private bool Isomorphic(string sentence_iID, List<string> sentences, List<string> candidateSentences)
        {
            string sentence_i = candidateSentences[int.Parse(sentence_iID)];
            string pure_i = GetPureWords(sentence_i);
            
            foreach (string sentence_jID in sentences)
            {
                string sentence_j = candidateSentences[int.Parse(sentence_jID)];
                string pure_j = GetPureWords(sentence_j);
                if (pure_i.Equals(pure_j) || pure_i.Contains(pure_j) || pure_j.Contains(pure_i))
                {
                    return true;
                }
                else if (OverlayScaleTerms(sentence_i, sentence_j) > 0.7 || OverlayScale(pure_i, pure_j) > 0.70)
                {
                    return true;
                }
            }
            
            return false;
        }

        //calculate the fraction of overlapping substring. Not in the term of terms
        //max consecutive overlay, "abcdefgh" and "adcdmngh", max consecutive overlay is "adcd".
        private float OverlayScale(string candidate, string selected)
        {
            int candidateLen = candidate.Length;
            int selecteLen = selected.Length;
            int maxOverlap = 0;
            for (int i = 0; i < candidateLen; i++ )
            {
                char cur_i = candidate[i];
                for (int j = 0; j < selecteLen; j++)
                {
                    char cur_j = selected[j];
                    if (cur_i.Equals(cur_j))
                    {
                        int cur_overlap = 1;
                        int k = i + 1;
                        int m = j + 1;
                        for (; k < candidateLen && m < selecteLen; k++, m++)
                        {
                            char tmp_candidate = candidate[k];
                            char tmp_select = selected[m];
                            if (tmp_candidate.Equals(tmp_select))
                            {
                                cur_overlap++;
                            }
                            else
                            {
                                if (cur_overlap > maxOverlap)
                                {
                                    maxOverlap = cur_overlap;
                                }
                                break;
                            }
                        }
                        if (cur_overlap > maxOverlap)
                        {
                            maxOverlap = cur_overlap;
                        }
                    }
                }
            }
            float can_overlay = (float) maxOverlap / candidateLen;
            float sel_overlay = (float) maxOverlap / selecteLen;
            float overlay = can_overlay > sel_overlay ? can_overlay : sel_overlay;
            return overlay;
        }

        //count the max consecutive overlay, "abcdefgh" and "adcdmngh", max consecutive overlay is "adcd" + "gh".
        //it is time-consuming. i just give it up.
        public int AllOverlayScale(string candidate, string selected)
        {
            int maxOverlap = 0;
            int candidateLen = candidate.Length;
            int selectedLen = selected.Length;
            for (int i = 0; i < candidateLen; i++)
            {
                char candidateTmp = candidate[i];

                for (int j = 0; j < selectedLen; j++)
                {
                    char selectedTmp = selected[j];
                    if (candidateTmp.Equals(selectedTmp))
                    {
                        int tmpOverlap = 1;

                        int candidateIndex = i + 1;
                        int selectedIndex = j + 1;

                        while (candidateIndex < candidateLen && selectedIndex < selectedLen)
                        {
                            candidateTmp = candidate[candidateIndex];
                            selectedTmp = selected[selectedIndex];
                            if (candidateTmp.Equals(selectedTmp))
                            {
                                tmpOverlap++;
                                candidateIndex++;
                                selectedIndex++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        string candidateLast = candidate.Substring(candidateIndex);
                        string selectedLast = selected.Substring(selectedIndex);
                        tmpOverlap += AllOverlayScale(candidateLast, selectedLast);
                        if (tmpOverlap > maxOverlap)
                        {
                            maxOverlap = tmpOverlap;
                        }
                    }
                }
            }
            return maxOverlap;
        }


        private float OverlayScaleTerms(string candidate, string selected)
        {
            string[] selectedTerms = selected.Split(' ');
            string[] candidateTerms = candidate.Split(' ');
            int selectedTermScale = selectedTerms.Count();
            int overlay = 0;
            for (int i = 0; i < selectedTermScale; i++)
            {
                string curTerm = selectedTerms[i];
                if (candidateTerms.Contains(curTerm))
                {
                    overlay++;
                }
            }

            //edit in 11/19, because sometimes a longer sentence may contain much more information, although it covers more than 70% of the content in the shorter sentence
          //  return (float) overlay / selectedTermScale;
            float overlayInSelected = (float)overlay / selectedTermScale;
            float overlayInCandidate = (float)overlay / candidateTerms.Count();
            return (overlayInSelected >= overlayInCandidate ? overlayInCandidate : overlayInSelected);
        }

        private string GetPureWords(string sentence)
        {
            RegexOptions option = RegexOptions.None;
            Regex regex = new Regex(@"[^a-zA-Z]", option);
            string filtered = regex.Replace(sentence, "");
            return filtered;
        }

        private Dictionary<string, int> SplitTerms(string sentence)
        {
            List<string> tmpTerms = sentence.Split(' ').ToList();
            Dictionary<string, int> terms = new Dictionary<string, int>();
            RegexOptions option = RegexOptions.None;
            Regex regex = new Regex(@"[a-zA-Z]", option);
            foreach (string term in tmpTerms)
            {
                if (regex.Match(term).Success)
                {
                    if (terms.ContainsKey(term))
                    {
                        terms[term]++;
                    }
                    else
                    {
                        terms.Add(term, 1);
                    }
                }
            }
            return terms;
        }

        /* the two sentences should not be empty
         */
        private float CalDissimilaritySentence(Dictionary<string, int> terms_i, Dictionary<string, int> terms_j)
        {
            float dissim = 0.0f;
            foreach (string term in terms_i.Keys)
            {
                if (!terms_j.ContainsKey(term))
                {
                    dissim += terms_i[term];
                }
            }
            List<int> values = terms_i.Values.ToList();
            int senLength = values.Sum();
            dissim = dissim / senLength;
            return dissim;
        }

        /* MMR-based summary generation
         */
        private string GenerateFinalSummary(List<string> candidateSentences)
        {
            
            string summary = "";
            List<string> selectedSentence = new List<string>();
            
            Dictionary<string, float> sen_MMR = new Dictionary<string, float>();
            string selectElem = "";
            int summaryScale = sim_sen_query.Count > SENTENCE_NUM ? SENTENCE_NUM : sim_sen_query.Count;
            for (int i = 0, j = 0; i < sim_sen_query.Count && j < summaryScale; i++)
            {
                selectElem = SelectMaxScoreSentence(selectedSentence, ignoreSentence, candidateSentences); //calculate MMR scores, and select the maximal score
                float score = sim_sen_query[selectElem];
                //if (score < 0.08)
                //{
                //    break;
                //}

                string selectSentence = candidateSentences[int.Parse(selectElem)];

                if (!IsSynonym(selectSentence) && !Isomorphic(selectElem, selectedSentence, candidateSentences) && IsInformative(selectSentence))
                {
                    selectedSentence.Add(selectElem);
                    j++;
                }
                else
                {
                    ignoreSentence.Add(selectElem);
                }
            }

            foreach (string sentenceIdStr in selectedSentence)
            {
                int sentenceId = int.Parse(sentenceIdStr);
             //   string cleanedSen = CleanStartEnd(candidateSentences[sentenceId]);
             //   float score = sim_sen_query[sentenceIdStr];

             //   ///////for summary on paragraph
             ////   cleanedSen = RemoveUselessSentenses(cleanedSen, qualityTerms);
             //   summary += cleanedSen + "\t" + sim_sen_query[sentenceIdStr] + "\r\n";

                // for the purpose of comparing with the original sentences
                string tmpSelectedSentence = candidateSentences[sentenceId];
              //  summary += tmpSelectedSentence + "\t" + sim_sen_query[sentenceIdStr] + "\r\n";

                float score = sim_sen_query[sentenceIdStr];

                //if (score < 0.11)
                //{
                //    continue;
                //}
                // summary += tmpSelectedSentence + "\r\n";
                summary += tmpSelectedSentence + "\r\n";// + "\t" + score + "\r\n";
            }

            return summary;
        }

        //test for summary on paragraph. Remove the sentences that don't contain any quality term in a paragraph.
        private string RemoveUselessSentenses(string oriParas, List<string> qualityTerms)
        {
            string filteredSen = "";
            string[] seperators = {". "};
            string[] oriSentences = oriParas.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string oriSentence in oriSentences)
            {
                string stemmedSentence = StemSentence(oriSentence);
                foreach (string qualityTerm in qualityTerms)
                {
                    if (stemmedSentence.Contains(qualityTerm + " "))
                    {
                        filteredSen += oriSentence + ". ";
                        break;
                    }
                }
            }
            return filteredSen;
        }
        private string StemSentence(string oriSentence)
        {
            string stemmedSentence = "";
            List<string> sentenceTerms = oriSentence.Split(' ').ToList();
            Porter2 porter = new Porter2();
            foreach (string term in sentenceTerms) //calculate the number of valid words
            {
                string stemmedTerm = porter.stem(term);
                stemmedSentence += stemmedTerm + " ";
            }
            return stemmedSentence;
        }
        ////////////end for temperary summary on paragraph


        //some sentence starts with ieee, and they don't contain enough information
        //for instance:else if (curLine.StartsWith("ieee "))
                
        private bool IsInformative(string curLine)
        {
          //  Regex figureRegex = new Regex(@"figure [0-9]{1,2}\-{0,1}[0-9]{1,2}:");

            if(curLine.StartsWith("ieee "))
            {
                string tmpLine = curLine.Substring(5);
                Regex numRegex = new Regex(@"[0-9]+\.[0-9]+\—{0,1}[0-9]*");
                if (numRegex.Match(tmpLine).Success)
                {
                    return false ;
                }
            }
            else if (curLine.StartsWith("figure"))
            {
                return false;
            }
            //else if (figureRegex.Match(curLine).Success)
            //{
            //    return false;
            //}
            else if (curLine.Contains("="))
            {
                return false;
            }
            
            return true;
        }

        //if two consecutive acronyms, return true. I will say this paragraph is about acronym definition.
        private bool IsSynonym(string sentence)
        {
            string acronymStart = "acronym explanation";
            if (sentence.StartsWith(acronymStart))
            {
                int startPos = acronymStart.Length;
                sentence = sentence.Substring(startPos).Trim();
            }

            string[] terms = sentence.Split(' ');
            int termLength = terms.Count();
            int acronymPair = 0;

            string lastAconym = "";
            for (int i = 0; i < termLength; i++)
            {
                string tmpTerm = terms[i];
                if (tmpTerm.Equals("system"))
                {
                    continue;
                }
                else if (tmpTerm.Equals("or"))
                {
                    tmpTerm = lastAconym;
                }
                else
                {
                    lastAconym = tmpTerm;
                }

                int nextIndex = i + 1;
                if (nextIndex == termLength)
                {
                    return false;
                }
                string nextTerm = terms[i + 1];
                int acronymOffset = IsAcronym(tmpTerm, i, terms);
                if (acronymOffset > 0)
                {
                    i += acronymOffset;
                    acronymPair++;
                }
                else if (IsAbbreviate(tmpTerm, nextTerm))
                {
                    i += 1;
                    acronymPair++;
                }
                else
                {
                    return false;
                }

                if (acronymPair == 3)
                {
                    return true;
                }
            }
            return true;
        }

        private int IsAcronym(string acronym, int pos, string[] terms)
        {
            string oriTerm = acronym;
            if (oriTerm.Contains("-"))
            {
                oriTerm = oriTerm.Replace("-", "");
            }
            int tmpTermLen = oriTerm.Length;
            int count = 1;
            int checkTermOffset = 0;
            int termLength = terms.Count();
            int shiftRight = 0;
            while (count <= tmpTermLen && pos + count + checkTermOffset < termLength)
            {
                string curCheckedTerm = terms[pos + count + checkTermOffset].Trim();
                char char_count = oriTerm[count - 1];
                if (curCheckedTerm.StartsWith("-") || curCheckedTerm.StartsWith("–") || curCheckedTerm.Equals("of"))
                {
                    checkTermOffset++;
                    shiftRight++;
                    continue;
                }
                else if (curCheckedTerm.Equals("and") && char_count.Equals('&'))
                {
                    count++;
                    shiftRight++;
                    continue;
                }
                else if (curCheckedTerm.StartsWith(char_count.ToString()))
                {
                    count++;
                    shiftRight++;
                    continue;
                }
                else
                {
                    return 0;
                }
            }
            return shiftRight;
        }

        //the original should contain all of the chars in abbre by sequence
        private bool IsAbbreviate(string abbre, string original)
        {
            if (abbre.Length == 1)
            {
                return false;
            }
            int index_before = 0; //record the index of char in the before position in abbre.
            foreach (char tmp_c in abbre)
            {
                if (original.Contains(tmp_c))
                {
                    int cur_index = original.IndexOf(tmp_c, index_before);
                    if (cur_index > -1)
                    {
                        index_before = cur_index;
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        //to filter the remaining table content, we adopt the sentence length as one filter. Normaly, the sentence length is less than 100.
        //usually the sentence length should be less than 50.
        private bool IsSatisfyLength(string selectElem, List<string> candidateSentence)
        {
            string sentence = candidateSentence[int.Parse(selectElem)];
            Regex wordRegex = new Regex(@"\W");
            string filtered = wordRegex.Replace(sentence, "");
            if (filtered.Length >= 100)
            {
                Console.WriteLine(sentence);
            }
            return filtered.Length >= 100 ? false : true;
        }

        //if the selected sentence, like "the eic prt application detects recovery of wlan communication,a.5", filter "a.5".
        //if the selected sentence, like "4.6.1 the operational output of the tds is the track database file", filter "4.6.1"
        private string CleanStartEnd(string selectedSen)
        {
            if (selectedSen.EndsWith("as follows"))
            {
                selectedSen = selectedSen.Substring(0, selectedSen.Length - 10);
            }
            else if (selectedSen.EndsWith("()"))
            {
                selectedSen = selectedSen.Substring(0, selectedSen.Length - 2);
            }
            string filteredStr = "";
            Regex regex = new Regex(@"\b(\w\.)+(\w)*\b"); //1.1.2
            string[] terms = selectedSen.Split(' ');
            int termScale = terms.Count();
            string firstTerm = terms[0];
            string endTerm = terms[termScale - 1];
            bool firstMatch = false;
            bool endMatch = false;
            if (regex.Match(firstTerm).Success)
            {
                Regex wordRegex = new Regex("[a-zA-Z]");

                firstTerm = regex.Replace(firstTerm, "");
                if (firstTerm.Length > 0 && !wordRegex.Match(firstTerm).Success)
                {
                    firstTerm = "";
                }
                firstMatch = true;
            }
            if (regex.Match(endTerm).Success)
            {
                endTerm = regex.Replace(endTerm, "");
                endMatch = true;
            }
            else
            {
                if (!endTerm.EndsWith(".") && endTerm.Contains("."))
                {
                    int lastCommaIndex = endTerm.LastIndexOf('.');
                    endMatch = true;
                    endTerm = endTerm.Substring(0, lastCommaIndex + 1);
                }
            }

            if (firstMatch || endMatch)
            {
                filteredStr += firstTerm + " ";
                for (int i = 1; i < termScale - 1; i++)
                {
                    filteredStr += terms[i] + " ";
                }
                filteredStr += endTerm;

                if (filteredStr.Contains("•"))
                {
                    int dotIndex = filteredStr.IndexOf("•");
                    if (dotIndex > 1)
                    {
                        char beforeChar = filteredStr[dotIndex - 1];
                        if (beforeChar.Equals('.'))
                        {
                            filteredStr = filteredStr.Substring(0,dotIndex - 1);
                        }
                    }
                }
                return filteredStr.Trim();
            }
            else
            {
                if (selectedSen.Contains("•"))
                {
                    int dotIndex = selectedSen.IndexOf("•");
                    if (dotIndex > 1)
                    {
                        char beforeChar = selectedSen[dotIndex - 1];
                        if (beforeChar.Equals('.'))
                        {
                            filteredStr = selectedSen.Substring(0,dotIndex - 1);
                        }
                    }
                }
                return selectedSen;
            }
        }

        /* first calculate the sentence scores. then, select the maximal one
         */
        private string SelectMaxScoreSentence(List<string> selectedSentence, List<string> ignoreSentence, List<string> candidateSentences)
        {
            Dictionary<string, float> sentenceScores = new Dictionary<string, float>();
            int sentenceScale = candidateSentences.Count;

            //improve by xlian.
            //we don't need to retrieve all of the terms in candidateSentences.
            //for (int i = 0; i < sentenceScale; i++)
            //{
            //    if (!selectedSentence.Contains(i + "") && !ignoreSentence.Contains(i + "") && sim_sen_query.ContainsKey(i + ""))
            //    {
            //        float score = 0.6f * sim_sen_query[i + ""] + 0.4f * MaxDissimilarity(selectedSentence,i, candidateSentences);
            //        sentenceScores.Add(i + "", score);
            //    }
            //}

            List<string> id_nozeroList = sim_sen_query.Keys.ToList();
            foreach (string noZeroId in id_nozeroList)
            {
                if (!selectedSentence.Contains(noZeroId) && !ignoreSentence.Contains(noZeroId))
                {
                    float score = 0.6f * sim_sen_query[noZeroId] + 0.4f * MaxDissimilarity(selectedSentence, int.Parse(noZeroId), candidateSentences);
                    sentenceScores.Add(noZeroId,score);
                }
            }
            Dictionary<string, float> sortedSentences = sentenceScores.OrderByDescending(x => x.Value).ToDictionary(x=>x.Key,x=>x.Value);
            return sortedSentences.Keys.ToArray()[0];
        }

        //Obtain the max dissimilarity between the sentence with the selected sentences
        private float MaxDissimilarity(List<string> selectedSentences, int curID, List<string> candidateSentences)
        {
            float maxDis = 0.0f;
            int selectedScale = selectedSentences.Count;
            string curStr = candidateSentences[curID];
            foreach (string selectedID in selectedSentences)
            {
                int selectedIDV = int.Parse(selectedID);
                string selectedStr = candidateSentences[selectedIDV];
                
                float curValue = CalDissimilarity(selectedStr, curStr);
                if (curValue > maxDis)
                {
                    maxDis = curValue;
                }
            }
            return maxDis;
        }
    }
}

using System;
using System.Collections.Generic;

namespace MiniProject1
{
    /// <summary>
    /// Description of Ranker
    /// </summary>
    public sealed class Ranker
    {
        #region
        private static Ranker instance = new Ranker();

        public static Ranker Instance
        {
            get
            {
                return instance;
            }
        }

        private Ranker()
        {
        }
        #endregion

        const int pageCount = 1000;
        Dictionary<string, float> normQ = new Dictionary<string, float>();

        
        public List<int> GetSites(List<string> query)
        {
            List<int> returnList = GetSitesHelper(ref query, true);
            if (returnList.Count <= 500000)
            {
                Console.WriteLine("without champion");
                returnList = GetSitesHelper(ref query, false);
            }
            return returnList;
        }

        private List<int> GetSitesHelper(ref List<string> query, bool withChampion)
        {
            query = Stemmer.DoStemmer(query.ToArray());
            float[] scores = new float[pageCount];
            float[] length = new float[pageCount];

            ConstructNormQ(query);

            foreach (string term in query)
            {
                if (Indexer.Instance.invertedIndex.ContainsKey(term))
                {
                    var node = Indexer.Instance.invertedIndex[term].PostingList.First;
                    if (withChampion)
                    {
                        node = Indexer.Instance.invertedIndex[term].ChampionList.First;
                    }
                    while (node != null)
                    {
                        int docId = node.Value.DocID;
                        float tf_raw = node.Value.Frequency;
                        float tf_star = 1.0f + (float)Math.Log10(tf_raw);
                        float wt = tf_star;
                        length[docId] += (float)Math.Pow(wt, 2);
                        scores[docId] += wt * normQ[term];
                        node = node.Next;
                    }
                }
            }

            List<SortType> sortedList = new List<SortType>();
            for (int i = 0; i < pageCount; i++)
            {
                if (length[i] != 0)
                {
                    scores[i] /= (float)Math.Sqrt(length[i]);
                    sortedList.Add(new SortType(i, scores[i]));
                }
            }

            sortedList.Sort();
            int count = sortedList.Count > 10 ? 10 : sortedList.Count;
            List<int> returnList = new List<int>();
            foreach (var x in sortedList.GetRange(0, count))
            {
                returnList.Add(x.DocID);
            }
            return returnList;
        }

        private void ConstructNormQ(List<string> query)
        {
            //Query normalised vector calc.
            float wt_query_length = 0.0f;
            foreach (string term in query)
            {
                if (!Indexer.Instance.invertedIndex.ContainsKey(term))
                {
                    normQ[term] = 0.0f;
                }
                else
                {
                    float tfstar = 1.0f;
                    int df = Indexer.Instance.invertedIndex[term].DocFreq;
                    float idf = (float)Math.Log10((float)pageCount / (float)df);
                    float wt = tfstar * idf;
                    normQ[term] = wt;
                    wt_query_length += (float)Math.Pow(wt, 2);
                }
            }
            wt_query_length = (float)Math.Sqrt(wt_query_length);
            foreach (string term in query)
            {
                normQ[term] /= wt_query_length;
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer
{

    class Ranker
    {
        private Indexer _index;
        private int _amountOfDocuments;
        private List<int> _docIds;

        public Ranker(Indexer index, List<int> docIds, int amountOfDocuments) 
        {
            
            _index = index;            
            _amountOfDocuments = amountOfDocuments;
            _docIds = docIds;
        }

        public List<KeyValuePair<int, float>> GetKSites(string query, int K = 10) 
        {

            Dictionary<int, float> scores = new Dictionary<int, float>();
            Dictionary<int, float> length = new Dictionary<int, float>();

            foreach (int docId in _docIds) 
            {
                scores.Add(docId, 0.0f);
                length.Add(docId, 0.0f);
            }
            List<string> tokens = Tokenizer.ExtractText(query);
            List<string> terms = FeatureConstructor.RemoveAndStem(tokens);
            Dictionary<string, float> normalizedQueryWeights = ConstructQueryWeights(terms);

            foreach (string term in terms) 
            {
                if (_index.invertedIndex.ContainsKey(term))
                {
                    foreach (var item in _index.invertedIndex[term].Postings)
                    {
                        int docId = item.docId;
                        float tf_raw = item.frequencyInDoc;
                        float tf_star = 1.0f + (float)Math.Log10(tf_raw);
                        float wt = tf_star;
                        length[docId] += (float)Math.Pow(wt, 2);
                        scores[docId] += wt * normalizedQueryWeights[term];
                    }
                }
            }

            Dictionary<int, float> result = new Dictionary<int, float>();

            foreach(int docId in _docIds) 
            {
                float actualLength = (float)Math.Sqrt(length[docId]);
                
                result.Add(docId, scores[docId]);
                
            }
            

            var sortedResult = (from entry in result
                                orderby entry.Value descending
                                select entry).ToList().Take(K+1).ToList();

            return sortedResult;
        }

        public Dictionary<string, float> ConstructQueryWeights(List<string> query) 
        {

            Dictionary<string, float> normalizedWeights = new Dictionary<string, float>();

            foreach (string term in query)
            {
                float tfstar = 1.0f;
                int df = _index.getDocumentFrequency(term);
                float idf = 0.0f;
                if (df != 0) {
                    idf = (float)Math.Log10((float)_amountOfDocuments / df);
                }

                
                float wt = tfstar * idf;
                normalizedWeights[term] = wt;
            }

            float length = (float)Math.Sqrt(normalizedWeights.Values.ToList().Select(x => (float)Math.Pow(x, 2)).Sum());

            foreach (string term in query) 
            {
                normalizedWeights[term] /= length;
            }

            return normalizedWeights;
        }
    }
}

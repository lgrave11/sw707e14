using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer
{
    public class Indexer
    {
        public Dictionary<string, PostingList> invertedIndex;

        public Indexer() 
        {
            invertedIndex = new Dictionary<string, PostingList>();
        }

        public int getDocumentFrequency(string term) 
        {
            if (invertedIndex.ContainsKey(term))
            {
                return invertedIndex[term].Frequency;
            }
            else 
            {
                return 0;
            }
        }

        public PostingList getIncidenceVector(string term) 
        {
            if (invertedIndex.ContainsKey(term))
            {
                return invertedIndex[term];
            }
            else 
            {
                return null;
            }
        }

        public int getTermFrequency(string term, int docid) 
        {
            if (invertedIndex.ContainsKey(term))
            {
                return invertedIndex[term].Postings.Where(x => x.docId == docid).FirstOrDefault().frequencyInDoc;
            }
            else 
            {
                return 0;
            }
        }

        public void addIncidence(string term, int docid) 
        {
            if (!invertedIndex.ContainsKey(term))
            {
                List<Posting> DocsWithTerm = new List<Posting>();
                DocsWithTerm.Add(new Posting(docid));
                invertedIndex.Add(term, new PostingList { Postings = DocsWithTerm });

            }
            else 
            {
                if (invertedIndex[term].Postings.Where(x => x.docId == docid).Count() == 0)
                {

                    invertedIndex[term].Postings.Add(new Posting(docid));
                    invertedIndex[term].Postings.Sort();
                }
                else 
                {
                    invertedIndex[term].Postings.Where(x => x.docId == docid).FirstOrDefault().frequencyInDoc++;
                }
            }
        }

    }
}

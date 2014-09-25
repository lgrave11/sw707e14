using System;
using System.Collections.Generic;
namespace MiniProject1
{
    /// <summary>
    /// Description of Indexer
    /// </summary>
    public sealed class Indexer
    {
        #region singletonpart
        private static Indexer instance = new Indexer();
        int id = 0;
        public static Indexer Instance
        {
            get
            {
                return instance;
            }
        }

        private Indexer()
        {
        }
        #endregion

        List<IndexWrapper> pairSequence = new List<IndexWrapper>();

        public void AddDoc(string content){
            List<string> terms = TermMaker.Instance.GetTerms(content);
            foreach (string s in terms)
            {
                pairSequence.Add(new IndexWrapper(s, id));
            }
            if((id +1)  % 100 == 0)
                Console.WriteLine(id +1);
            id++;
        }

        public Dictionary<string, Postings> invertedIndex = new Dictionary<string, Postings>();
        public void ConstructIndex()
        {
            pairSequence.Sort();
            foreach (IndexWrapper pair in pairSequence)
            {
                if(!invertedIndex.ContainsKey(pair.Term))
                {
                    invertedIndex[pair.Term] = new Postings();
                }
                invertedIndex[pair.Term].AddPosting(pair.DocId);
            }
            pairSequence.Clear();
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer
{
    // Man ligger ikke normalt frequency her.
    public class PostingList
    {
        
        public List<Posting> Postings;
        public List<Posting> Champions;
        public int Frequency { get { return Postings.Count(); } }

        public void makeChampionsList()
        {
            int r = 5;
            Champions = Postings.OrderByDescending(x => x.frequencyInDoc).Take(r).ToList();
        }
    }

    public class Posting : IComparable
    {
        public int docId;
        public int frequencyInDoc = 0;

        public Posting(int _docId) 
        {
            this.docId = _docId;
            this.frequencyInDoc = 1;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            Posting otherDoc = obj as Posting;
            if (otherDoc != null)
            {
                return this.docId.CompareTo(otherDoc.docId);
            }
            else 
            {
                throw new ArgumentException("Object is not a Posting");
            }
        }
    }
}

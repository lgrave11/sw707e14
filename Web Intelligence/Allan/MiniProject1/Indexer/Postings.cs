using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject1
{
    public class Postings
    {
        const int champion_size = 5;
       
        public LinkedList<PostingType> PostingList = new LinkedList<PostingType>();
        public LinkedList<PostingType> ChampionList = new LinkedList<PostingType>();
        public int DocFreq { get { return PostingList.Count; } }

        public void GenerateChampionList()
        {
            List<PostingType> intermediateList = new List<PostingType>();
            var node = PostingList.First;
            while (node != null)
            {
                intermediateList.Add(node.Value);
                if (intermediateList.Count > champion_size)
                {
                    intermediateList.Remove(intermediateList.Min());
                }

                node = node.Next;
            }
            foreach (var x in intermediateList.OrderByDescending(y=> y.Frequency))
            {
                ChampionList.AddLast(x);
            }
        }
        public void AddPosting(int DocID)
        {
            PostingType toBeAdded = new PostingType(DocID);
                      
            if (PostingList.Count == 0)
            {
                PostingList.AddFirst(toBeAdded);
            }
            else
            {
                var p = PostingList.First;
                while (p != null &&p.Value.DocID < DocID)
                {
                    p = p.Next;
                }
                if (p == null)
                {
                    PostingList.AddLast(toBeAdded);
                }
                else if (p.Value.DocID == DocID)
                {
                    p.Value.Frequency++;
                }
                else
                {
                    PostingList.AddBefore(p, toBeAdded);
                }
            }
        }
        
    }
}

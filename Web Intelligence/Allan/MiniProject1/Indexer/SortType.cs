using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject1
{
    class SortType:IComparable
    {
        public int DocID { get; set; }
        public float Score { get; set; }
        public SortType(int docID, float score)
        {
            DocID = docID;
            Score = score;
        }




        public int CompareTo(object obj)
        {
            return ((SortType)obj).Score.CompareTo(Score);
        }
    }
}

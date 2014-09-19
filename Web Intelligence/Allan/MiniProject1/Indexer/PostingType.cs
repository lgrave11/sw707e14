using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject1
{
    class PostingType:IComparable
    {
        public int DocID { get; set; }
        public int Frequency { get;set; }

        public PostingType(int docId)
        {
            DocID = docId;
            Frequency = 1;
        }

        public int CompareTo(object obj)
        {
            return DocID.CompareTo(((PostingType)obj).DocID);
        }
    }
}

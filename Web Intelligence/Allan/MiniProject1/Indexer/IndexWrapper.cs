using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject1
{
    class IndexWrapper:IComparable
    {
        public string Term { get; set; }
        public int DocId { get; set; }

        public IndexWrapper(string term, int docId)
        {
            Term = term ?? "";
            
            DocId = docId;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return -1;
            }
            IndexWrapper otherObject = (IndexWrapper)obj;
            if (Term.CompareTo(otherObject.Term) == 0)
            {
                return DocId.CompareTo(otherObject.DocId);
            }
            else
                return Term.CompareTo(otherObject.Term);
        }
    }
}

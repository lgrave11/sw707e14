using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject2
{
    class SortClass:IComparable
    {
        public User User { get; set; }
        public double EVDValue { get; set; }
        public SortClass(User user, double evdValue)
        {
            User = user;
            EVDValue = evdValue;
            
        }

        public int CompareTo(object obj)
        {
            return EVDValue.CompareTo(((SortClass)obj).EVDValue);
        }
    }
}

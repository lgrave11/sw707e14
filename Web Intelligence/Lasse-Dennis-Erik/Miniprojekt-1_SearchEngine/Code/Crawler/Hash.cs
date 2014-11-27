using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerNamespace
{
    public class Hash
    {
        static public Int64 HashA(String str)
        {
            const Int32 b = 378551;
            Int32 a = 63689;
            Int64 hash = 0;
            for (Int32 i = 0; i < str.Length; i++)
            {
                hash = hash * a + str[i];
                a = a * b;
            }
            return hash;
        }

        static public Int64 HashB(String str)
        {
            Int64 hash = 1315423911;
            for (Int32 i = 0; i < str.Length; i++)
            {
                hash ^= ((hash << 5) + str[i] + (hash >> 2));
            }
            return hash;
        }

        static public Int64 HashC(String str)
        {
            Int64 hash = 0;
            Int64 x = 0;
            for (Int32 i = 0; i < str.Length; i++)
            {
                hash = (hash << 4) + str[i];
                if ((x = hash & 0xF0000000L) != 0)
                {
                    hash ^= (x >> 24);
                }
                hash &= ~x;
            }
            return hash;
        }

        static public Int64 HashD(String str)
        {
            const Int64 seed = 131; // 31 131 1313 13131 131313 etc..
            Int64 hash = 0;
            for (Int32 i = 0; i < str.Length; i++)
            {
                hash = (hash * seed) + str[i];
            }
            return hash;
        }

        static public Int64 HashE(String str)
        {
            Int64 hash = 0;
            for (Int32 i = 0; i < str.Length; i++)
            {
                hash = str[i] + (hash << 6) + (hash << 16) - hash;
            }
            return hash;
        }
    }
}

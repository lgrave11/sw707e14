using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject2
{
    class Pair<T>
    {
        public T left;
        public T right;

        public Pair(T Left, T Right)
        {
            left = Left;
            right = Right;
        }
    }
}

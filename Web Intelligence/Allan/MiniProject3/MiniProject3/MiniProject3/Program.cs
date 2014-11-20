using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject3
{
    class Program
    {
        static void Main(string[] args)
        {
            MathNet.Numerics.Control.UseNativeMKL();
            new Recommender();
            //new Loader();
        }
    }
}

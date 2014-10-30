using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System.Drawing;

namespace MiniProject2
{
    class Program
    {
        static void Main(string[] args)
        {
            Control.UseNativeMKL();
            //List<List<User>> lala = CommunityLib.FindCommunities(User.ReadUserFile());
            Console.WriteLine(Review.LoadAllReviews().Count);
            Console.WriteLine("lol");
            Console.Read();
        }

        
      
    }
}

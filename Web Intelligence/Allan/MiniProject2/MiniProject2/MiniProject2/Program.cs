using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using Accord.Math.Decompositions;

namespace MiniProject2
{
    class Program
    {
        static void Main(string[] args)
        {
            double[,] array = new double[9, 9] { { 2, -1, -1, 0, 0, 0, 0, 0, 0 }, { -1,2,-1,0,0,0,0,0,0 }, {-1,-1,4,-1,-1,0,0,0,0 }, {0,0,-1,4,-1,-1,-1,0,0 }, {0,0,-1,-1,4,-1,-1,0,0 },{0,0,0,-1,-1,4,-1,-1,0},{0,0,0,-1,-1,-1,4,-1,0},{0,0,0,0,0,-1,-1,3,-1},{0,0,0,0,0,0,0,-1,1} };

            
            var gevd = new EigenvalueDecomposition(array);
            
            for(int i = 0; i < 9; i++)
            {
                Console.WriteLine(gevd.DiagonalMatrix[i, i]);
            }

            for (int i = 0; i < 9; i++)
            {
                for(int j = 0; j < 9; j++)
                {
                    Console.Write(gevd.Eigenvectors[i, j] + " ");
                }
                Console.Write("\n");
            }           

            Console.Read();
        }
    }
}

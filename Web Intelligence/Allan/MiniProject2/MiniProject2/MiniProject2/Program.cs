using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace MiniProject2
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
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
             */

            /*foreach(User user in User.LoadAllUsers())
            {
                Console.WriteLine(user.Name);
            }*/
            
            Control.UseNativeMKL();
            FindCommunities(User.LoadAllUsers());
            Console.Read();
        }

        static void FindCommunities(List<User> users)
        {
            Console.WriteLine("Fik kaldt metoden");
            double[,] aMatrix = new double[users.Count, users.Count];
            
            for(int i = 0; i < users.Count; i++)
            {
                for(int j = 0; j < users.Count; j++)
                {
                    if (i == j)
                        aMatrix[i, j] = 0;
                    else if (users[i].Friends.Contains(users[j].Name))
                        aMatrix[i, j] = 1;
                    else
                        aMatrix[i, j] = 0;
                }
                
            }

            Console.WriteLine("evd konstruktion\n");

            Matrix<double> A = Matrix<double>.Build.DenseOfArray(aMatrix);
            Vector<double> dVector = A.RowAbsoluteSums();
            Matrix<double> D = Matrix<double>.Build.DenseOfDiagonalVector(dVector);
            Matrix<double> L = D - A;

            Evd<double> evd = L.Evd();

            //Console.WriteLine(evd.EigenVectors.Column(1));
            foreach(var x in evd.EigenVectors.Column(1))
            {
                Console.Write(x+ " ");
            }
            
          
        }

      
    }
}

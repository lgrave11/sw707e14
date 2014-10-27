using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System.Drawing;

namespace WIMiniProjekt2
{
    class Program
    {
        static void Main(string[] args)
        {
            Control.UseNativeMKL();
            List<User> userList = ReadUserFile();
            double[,] matrix = new double[userList.Count, userList.Count];
            for (int i = 0; i < userList.Count; i++)
            {
                for (int j = 0; j < userList.Count; j++)
                {
                    if (i == j)
                        matrix[i, j] = 0;
                    else if (userList[i].Friends.Contains(userList[j].Username))
                        matrix[i, j] = 1;
                    else
                        matrix[i, j] = 0;
                }
            }

            Matrix<double> A = Matrix<double>.Build.DenseOfArray(matrix);
            Matrix<double> aOriginal = Matrix<double>.Build.DenseOfArray(matrix);
            Vector<double> dVector = A.RowAbsoluteSums();
            Matrix<double> D = Matrix<double>.Build.DenseOfDiagonalVector(dVector);
            Matrix<double> L = D - A;
            
            
            Evd<double> evd = L.Evd();
            Vector<double> eigenVector = evd.EigenVectors.Column(1);
            Dictionary<int, double> evdDictionary = new Dictionary<int, double>();
            int p = 0;
            eigenVector.ToList().ForEach(x => { evdDictionary.Add(p, x); p++; });

            Dictionary<int, double> sortedEvd = evdDictionary.OrderBy(x => x.Value).Select(x => x).ToDictionary(x => x.Key, y => y.Value);

            int[] permutation = new int[A.ColumnCount];
            int k = 0;
            foreach (KeyValuePair<int, double> entry in sortedEvd)
            {
                permutation[entry.Key] = k++;
            }

            

            A.PermuteColumns(new Permutation(permutation));
            A.PermuteRows(new Permutation(permutation));
            
            //List<double> differences = sortedEvd.Zip(sortedEvd.Skip(1), (x, y) => x - y).ToList();
            Dictionary<int, double> differences = sortedEvd.Zip(sortedEvd.Skip(1), (x, y) => new KeyValuePair<int, double>(x.Key, Math.Abs(x.Value - y.Value))).ToDictionary(x => x.Key, y => y.Value);

            Dictionary<int, double> sortedDifferences = differences.OrderBy(x => x.Value).ToDictionary(x => x.Key, y => y.Value);
            
            WriteImage(A, permutation[sortedDifferences.Last().Key]);
            //Console.WriteLine(sortedEvd);

            /*
            for (int m = 0; m < userList.Count; m++)
            {
                for (int n = 0; n < userList.Count; n++)
                {
                    Console.Write(L[m, n]+ " ");
                }
                Console.WriteLine("");
            }
            */
        }

        public static List<User> ReadUserFile()
        {
            int i = 0;
            List<User> userList = new List<User>();
            string fileContent = File.ReadAllText("friendships.txt");
            List<string> userBlocks = new List<string>();
            userBlocks.AddRange(fileContent.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None));
            foreach (string block in userBlocks)
            {
                string[] splitBlock = block.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                User user = new User();
                user.Index = i++;
                user.Username = splitBlock[0].Substring(5).Trim();
                user.Friends = splitBlock[1].Substring(8).Trim().Split('\t').ToList();
                user.Summary = splitBlock[2].Substring(8).Trim();
                user.Review = splitBlock[3].Substring(7).Trim();
                userList.Add(user);
            }
            return userList;
        }

        public static void WriteImage(Matrix<double> matrix, int special=-1){
            Bitmap bm = new Bitmap(matrix.ColumnCount, matrix.ColumnCount);
            for(int m = 0; m < matrix.ColumnCount; m++) 
            {
                for(int n = 0; n < matrix.ColumnCount; n++) 
                {
                    if (m == special || n == special)
                    {
                        bm.SetPixel(m, n, Color.Red);
                    } else if(matrix[m,n] > 0) 
                    {
                        bm.SetPixel(m,n, Color.Black);
                    }
                    else 
                    {
                        bm.SetPixel(m,n, Color.White);
                    }
                }
            }
            bm.Save("matrix.png");
        }
    }
}

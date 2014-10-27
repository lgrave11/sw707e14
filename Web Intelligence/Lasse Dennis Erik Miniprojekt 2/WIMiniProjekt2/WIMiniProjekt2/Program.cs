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
            Console.WriteLine("Making A, D, L");
            List<User> userList = ReadUserFile();
            Matrix<double> A = MakeMatrix(userList);
            //Matrix<double> aOriginal = MakeMatrix(userList);
            Vector<double> dVector = A.RowAbsoluteSums();
            Matrix<double> D = Matrix<double>.Build.DenseOfDiagonalVector(dVector);
            Matrix<double> L = D - A;


            Console.WriteLine("Finding EVD.");
            Evd<double> evd = L.Evd();
            Vector<double> eigenVector = evd.EigenVectors.Column(1);
            Dictionary<int, double> evdDictionary = new Dictionary<int, double>();
            int p = 0;
            eigenVector.ToList().ForEach(x => { evdDictionary.Add(p, x); p++; });

            Dictionary<int, double> sortedEvd = evdDictionary.OrderBy(x => x.Value).Select(x => x)
                                                             .ToDictionary(x => x.Key, y => y.Value);
            #region stuff
            /*
            int[] permutation = new int[A.ColumnCount];
            int k = 0;
            foreach (KeyValuePair<int, double> entry in sortedEvd)
            {
                permutation[entry.Key] = k++;
            }
            A.PermuteColumns(new Permutation(permutation));
            A.PermuteRows(new Permutation(permutation));
            */

            #endregion
            Console.WriteLine("Finding differences");
            Dictionary<int, double> differences = sortedEvd.Zip(sortedEvd.Skip(1), (x, y) => new KeyValuePair<int, double>(x.Key, Math.Abs(x.Value - y.Value)))
                                                           .ToDictionary(x => x.Key, y => y.Value);

            Dictionary<int, double> sortedDifferences = differences.OrderBy(x => x.Value)
                                                                   .ToDictionary(x => x.Key, y => y.Value);

            Dictionary<int, double> before = new Dictionary<int, double>();
            Dictionary<int, double> after = new Dictionary<int, double>();
            int index = sortedDifferences.Last().Key;
            bool b = false;
            foreach (KeyValuePair<int, double> kv in evdDictionary)
            {
                if (kv.Key == index)
                {
                    b = true;
                }
                if (b)
                {
                    after.Add(kv.Key, kv.Value);
                }
                else
                {
                    before.Add(kv.Key, kv.Value);
                }
            }
            Console.WriteLine("Splitting graphs.");
            List<User> userListBefore = new List<User>();
            List<User> userListAfter = new List<User>();
            foreach (KeyValuePair<int, double> kv in evdDictionary)
            {
                userList[kv.Key].Eigen = kv.Value;
                if (before.ContainsKey(kv.Key)) 
                {
                    foreach (var v in after)
                    {
                        userList[kv.Key].Friends.Remove(userList[v.Key].Username);
                    }
                    userListBefore.Add(userList[kv.Key]);
                }
                else if (after.ContainsKey(kv.Key)) 
                {
                    foreach (var v in before)
                    {
                        userList[kv.Key].Friends.Remove(userList[v.Key].Username);
                    }
                    userListAfter.Add(userList[kv.Key]);
                }
            }

            userListBefore = userListBefore.OrderByDescending(x => x.Eigen).ToList();
            userListAfter = userListAfter.OrderByDescending(x => x.Eigen).ToList();
            var ABefore = MakeMatrix(userListBefore);
            var AAfter = MakeMatrix(userListAfter);

            WriteImage(ABefore, filename:"ABefore.png");
            WriteImage(AAfter, filename: "AAfter.png");
        }

        public static Matrix<double> MakeMatrix(List<User> listOfUsers) 
        {
            double[,] matrix1 = new double[listOfUsers.Count, listOfUsers.Count];
            for (int i = 0; i < listOfUsers.Count; i++)
            {
                for (int j = 0; j < listOfUsers.Count; j++)
                {
                    if (i == j)
                        matrix1[i, j] = 0;
                    else if (listOfUsers[i].Friends.Contains(listOfUsers[j].Username))
                        matrix1[i, j] = 1;
                    else
                        matrix1[i, j] = 0;
                }
            }

            return Matrix<double>.Build.DenseOfArray(matrix1);
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
                user.Eigen = 0.0;
                user.Username = splitBlock[0].Substring(5).Trim();
                user.Friends = splitBlock[1].Substring(8).Trim().Split('\t').ToList();
                user.Summary = splitBlock[2].Substring(8).Trim();
                user.Review = splitBlock[3].Substring(7).Trim();
                userList.Add(user);
            }
            return userList;
        }

        public static void WriteImage(Matrix<double> matrix, int special = -1, string filename = "matrix.png")
        {
            Bitmap bm = new Bitmap(matrix.ColumnCount, matrix.ColumnCount);
            for (int m = 0; m < matrix.ColumnCount; m++)
            {
                for (int n = 0; n < matrix.ColumnCount; n++)
                {
                    if (m == special || n == special)
                    {
                        bm.SetPixel(m, n, Color.Red);
                    }
                    else if (matrix[m, n] > 0)
                    {
                        bm.SetPixel(m, n, Color.Black);
                    }
                    else
                    {
                        bm.SetPixel(m, n, Color.White);
                    }
                }
            }
            bm.Save(filename);
        }
    }
}

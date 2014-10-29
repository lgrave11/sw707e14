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
            Console.WriteLine("Starting...");
            List<User> userList = ReadUserFile();
            var communities = FindCommunities(userList);
            int i = 0;
            foreach (var v in communities)
            {
                Matrix<double> vA = MakeMatrix(v);
                WriteImage(vA, filename: string.Format("community-{0}", i++));
            }
        }


        /// <summary>
        /// Find all the communities in a list of users.
        /// </summary>
        /// <param name="userList"></param>
        /// <returns></returns>
        public static List<List<User>> FindCommunities(List<User> userList) 
        {
            Console.WriteLine("Making matrixes");
            Matrix<double> A = MakeMatrix(userList);
            Vector<double> dVector = A.RowAbsoluteSums();
            Matrix<double> D = Matrix<double>.Build.DenseOfDiagonalVector(dVector);
            Matrix<double> L = D - A;

            Console.WriteLine("Finding EVD.");
            Evd<double> evd = L.Evd();
            Vector<double> eigenVector = evd.EigenVectors.Column(1);

            Console.WriteLine("Adding eigen vector value to users.");
            for (int ev = 0; ev < eigenVector.Count; ev++)
            {
                userList[ev].Eigen = eigenVector[ev];
            }
            var sortedUserList = userList.OrderBy(x => x.Eigen).ToList();

            Console.WriteLine("Cutting communities.");
            Tuple<List<User>, List<User>> cutCommunities = Cut(sortedUserList);
            if (cutCommunities == null)
            {
                return new List<List<User>>() { userList };
            }

            List<List<User>> allCommunities = new List<List<User>>();
            foreach (var x in FindCommunities(cutCommunities.Item1))
            {
                allCommunities.Add(x);
            }
            foreach (var x in FindCommunities(cutCommunities.Item2))
            {
                allCommunities.Add(x);
            }
            return allCommunities;
        }

        /// <summary>
        /// Cut a sorted user list two make two different 'graphs'.
        /// </summary>
        /// <param name="sortedUserList"></param>
        /// <returns></returns>
        public static Tuple<List<User>, List<User>> Cut(List<User> sortedUserList) 
        {
            double largestGap = 0.0;
            int index = 0;
            for (int i = 0; i < sortedUserList.Count - 1; i++)
            {
                var gap = Math.Abs(sortedUserList[i].Eigen - sortedUserList[i + 1].Eigen);
                if (gap > largestGap)
                {
                    index = i;
                    largestGap = gap;
                }
            }
            if (largestGap > 0.7)
            {
                return null;
            }

            Console.WriteLine("Making new lists.");
            List<User> ListLeft = sortedUserList.Take(index + 1).ToList();
            List<User> ListRight = sortedUserList.Skip(index + 1).ToList();

            Console.WriteLine("Cutting connections, should we be doing this?");
            for (int ll = 0; ll < ListLeft.Count; ll++)
            {
                foreach (User u in ListRight)
                {
                    ListLeft[ll].Friends.Remove(u.Username);
                }
            }
            for (int ll = 0; ll < ListRight.Count; ll++)
            {
                foreach (User u in ListLeft)
                {
                    ListRight[ll].Friends.Remove(u.Username);
                }
            }

            return new Tuple<List<User>, List<User>>(ListLeft, ListRight);
        }

        /// <summary>
        /// Create an adjacency matrix from a list of users.
        /// </summary>
        /// <param name="listOfUsers"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Read the user file by splitting it into blocks, and then splitting those blocks into individual lines
        /// to read the relevant information.
        /// </summary>
        /// <returns></returns>
        public static List<User> ReadUserFile()
        {
            int i = 0;
            List<User> userList = new List<User>();
            string fileContent = File.ReadAllText("friendships.txt");
            List<string> userBlocks = new List<string>();
            userBlocks.AddRange(fileContent.Split(new string[] { "\n\n" }, StringSplitOptions.None));
            foreach (string block in userBlocks)
            {
                string[] splitBlock = block.Split(new string[] { "\n" }, StringSplitOptions.None);
                User user = new User();
                // Eigen to be used to sort later.
                user.Eigen = 0.0;
                user.Username = splitBlock[0].Substring(5).Trim();
                user.Friends = splitBlock[1].Substring(8).Trim().Split('\t').ToList();
                user.Summary = splitBlock[2].Substring(8).Trim();
                user.Review = splitBlock[3].Substring(7).Trim();
                userList.Add(user);
            }
            return userList;
        }

        public static void WriteImage(Matrix<double> matrix, int special = -1, string filename = "matrix")
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
            bm.Save(filename + ".png");
        }
    }
}

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
    class CommunityLib
    {
        public static List<List<User>> FindCommunities(List<User> users)
        {
            if (users.Count == 1)
            {
                return new List<List<User>>() { users };
            }
            Evd<double> evd = ConstructEvd(users);
            List<SortClass> sortedEVD = SortEVD(users, evd);
            Pair<List<User>> cutUserResult = CutUsers(sortedEVD);

            if (cutUserResult == null)
            {
                return new List<List<User>>() { users };
            }

            List<List<User>> returnList = new List<List<User>>();
            foreach (var x in FindCommunities(cutUserResult.left))
            {
                returnList.Add(x);
            }
            foreach (var x in FindCommunities(cutUserResult.right))
            {
                returnList.Add(x);
            }

            return returnList;
        }

        private static Pair<List<User>> CutUsers(List<SortClass> sortedUsers)
        {
            List<User> returnValLeft = new List<User>(), returnValRight = new List<User>();

            double largestGap = 0.0;
            int index = 0;
            for (int i = 0; i < sortedUsers.Count - 1; i++)
            {
                if (Math.Abs(sortedUsers[i].EVDValue - sortedUsers[i + 1].EVDValue) > largestGap)
                {
                    index = i;
                    largestGap = Math.Abs(sortedUsers[i].EVDValue - sortedUsers[i + 1].EVDValue);
                }
            }
            if (largestGap > 0.7)
            {
                return null;
            }
            Console.WriteLine(largestGap);

            for (int i = 0; i <= index; i++)
            {
                for (int j = index + 1; j < sortedUsers.Count; j++)
                {
                    sortedUsers[i].User.Friends.Remove(sortedUsers[j].User.Name);
                    sortedUsers[j].User.Friends.Remove(sortedUsers[i].User.Name);
                }
            }

            for (int i = 0; i <= index; i++)
            {
                returnValLeft.Add(sortedUsers[i].User);
            }
            for (int i = index + 1; i < sortedUsers.Count; i++)
            {
                returnValRight.Add(sortedUsers[i].User);
            }
            Console.WriteLine("L: " + returnValLeft.Count + " R: " + returnValRight.Count);
            return new Pair<List<User>>(returnValLeft, returnValRight);
        }

        private static List<SortClass> SortEVD(List<User> users, Evd<double> evd)
        {
            List<SortClass> sortedEvd = new List<SortClass>();

            for (int i = 0; i < evd.EigenVectors.Column(1).Count; i++)
            {
                sortedEvd.Add(new SortClass(users[i], evd.EigenVectors.Column(1)[i]));
            }
            sortedEvd.Sort();
            return sortedEvd;
        }

        private static Evd<double> ConstructEvd(List<User> users)
        {
            double[,] aMatrix = new double[users.Count, users.Count];

            for (int i = 0; i < users.Count; i++)
            {
                for (int j = 0; j < users.Count; j++)
                {
                    if (i == j)
                        aMatrix[i, j] = 0;
                    else if (users[i].Friends.Contains(users[j].Name))
                        aMatrix[i, j] = 1.0;
                    else
                        aMatrix[i, j] = 0;
                }

            }

            Matrix<double> A = Matrix<double>.Build.DenseOfArray(aMatrix);
            Vector<double> dVector = A.RowAbsoluteSums();
            Matrix<double> D = Matrix<double>.Build.DenseOfDiagonalVector(dVector);
            Matrix<double> L = D - A;

            Evd<double> evd = L.Evd();
            return evd;
        }
    }
}

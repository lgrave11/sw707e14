using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject1
{
    class NearDuplicatesWithoutSketches
    {
        public static bool NearDuplicate(string s1, string s2, int shingleSize, float threshold)
        {
            int minShingle = Math.Min(s1.Split(' ', '\n').Count(), s2.Split(' ', '\n').Count());
            if (minShingle < shingleSize)
                shingleSize = minShingle;

            List<int> shingles1 = MakeShingleSet(s1, shingleSize);
            List<int> shingles2 = MakeShingleSet(s2, shingleSize);

            float similarity = (shingles1.Intersect(shingles2).Count()) / (shingles1.Union(shingles2).Count());
            return similarity >= threshold;
        }

        private static List<int> MakeShingleSet(string s, int shingleSize)
        {
            List<int> returnList = new List<int>();

            for (int i = 0; i <= s.Split(' ', '\n').Count() - shingleSize; i++)
            {
                string concatenate = "";
                for (int j = i; j < shingleSize + i; j++)
                {
                    concatenate += s.Split(' ', '\n')[j];
                }
                returnList.Add(concatenate.GetHashCode());
            }
            return returnList;
        }
    }
}

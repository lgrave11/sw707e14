using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject1
{
    class NearDuplicatesWithSketches
    {
        public static bool NearDuplicate(string s1, string s2, int shingleSize, float threshold)
        {

            int minShingle = Math.Min(s1.Split(' ', '\n').Count(), s2.Split(' ', '\n').Count());
            if (minShingle < shingleSize)
                shingleSize = minShingle;

            List<Func<string, Int64>> hashFunctions = GetHashFunctions();
            int alike = 0;
            int notalike = 0;

            foreach (Func<string, Int64> func in hashFunctions)
            {
                if (GetMinShingle(s1, shingleSize, func) == GetMinShingle(s2, shingleSize, func))
                {
                    alike++;
                }
                else
                {
                    notalike++;
                }
            }

            return ((float)alike) / ((float)alike + notalike) >= threshold;
        }


        private static List<Func<string, Int64>> GetHashFunctions()
        {
            var returnList = new List<Func<string, Int64>>();
            returnList.Add(StringHash.APHash);
            returnList.Add(StringHash.BKDRHash);
            returnList.Add(StringHash.BPHash);
            returnList.Add(StringHash.DEKHash);
            returnList.Add(StringHash.DJBHash);
            returnList.Add(StringHash.ELFHash);
            returnList.Add(StringHash.FNVHash);
            returnList.Add(StringHash.JSHash);
            returnList.Add(StringHash.RSHash);
            returnList.Add(StringHash.SDBMHash);
            return returnList;
        }
        


        private static Int64 GetMinShingle(string s, int shingleSize, Func<string, Int64> function)
        {
            List<Int64> returnList = new List<Int64>();
            
            for (int i = 0; i <= s.Split(' ', '\n').Count() - shingleSize; i++)
            {
                string concatenate = "";
                for (int j = i; j < shingleSize + i; j++)
                {
                    concatenate += s.Split(' ', '\n')[j];
                }
                returnList.Add(function(concatenate));
            }
            return returnList.Min();
        }

    }
}

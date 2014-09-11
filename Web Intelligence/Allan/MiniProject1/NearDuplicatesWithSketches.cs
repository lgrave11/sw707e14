using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject1
{
    class NearDuplicatesWithSketches
    {
        static Dictionary<Uri, Dictionary<Func<string, Int64>, Int64>> cachedMinShingle = new Dictionary<Uri, Dictionary<Func<string, Int64>, Int64>>();
        public static bool NearDuplicate(string s1, string s2, int shingleSize, float threshold, Uri uri1, Uri uri2)
        {
            string[] splittedS1 = s1.Split(' ', '\n');
            string[] splittedS2 = s2.Split(' ', '\n');
            
            if (Math.Min(splittedS1.Count(), splittedS2.Count()) < shingleSize)
                shingleSize = Math.Min(splittedS1.Count(), splittedS2.Count());

            int alike = 0;
            int notalike = 0;

            foreach (Func<string, Int64> func in GetHashFunctions())
            {
                
                if (GetMinShingle(splittedS1, shingleSize, func, uri1) == GetMinShingle(splittedS2, shingleSize, func, uri2))
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
            returnList.Add(StringHash.AP_BKDRHash);
            returnList.Add(StringHash.AP_BPHash);
            returnList.Add(StringHash.AP_DEKHash);
            returnList.Add(StringHash.AP_DJBHash);
            returnList.Add(StringHash.AP_ELFHash);
            returnList.Add(StringHash.AP_FNVHash);
            returnList.Add(StringHash.AP_JSHash);
            returnList.Add(StringHash.AP_RSHash);
            returnList.Add(StringHash.AP_SDBMHash);
            return returnList;
        }
        


        private static Int64 GetMinShingle(string[] s, int shingleSize, Func<string, Int64> function, Uri uri)
        {
            if (cachedMinShingle.ContainsKey(uri) && cachedMinShingle[uri].ContainsKey(function))
            {
                return cachedMinShingle[uri][function];
            }
            Int64 minValue = Int64.MaxValue;
            for (int i = 0; i <= s.Count() - shingleSize; i++)
            {
                string concatenate = "";
                
                for (int j = i; j < shingleSize + i; j++)
                {
                    concatenate += s[j];
                }
                Int64 value = function(concatenate);
                if (value < minValue)
                    minValue = value;
            }
            if (!cachedMinShingle.ContainsKey(uri))
            {
                cachedMinShingle[uri] = new Dictionary<Func<string, long>, long>();
            }
            cachedMinShingle[uri][function] = minValue;
            return minValue;
        }

    }
}

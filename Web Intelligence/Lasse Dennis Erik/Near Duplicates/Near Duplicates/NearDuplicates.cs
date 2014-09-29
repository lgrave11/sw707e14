using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace CrawlerNamespace
{
    public class NearDuplicates
    {
        static Dictionary<Uri, Dictionary<Func<string, Int64>, Int64>> cachedShingle = new Dictionary<Uri, Dictionary<Func<string, Int64>, Int64>>();

        static void Main(string[] args)
        {
            /*while (true) {
                string a = File.ReadAllText(@"C:\Users\Lasse\Desktop\www.kongregate.com.htm");
                string b = File.ReadAllText(@"C:\Users\Lasse\Desktop\www.kongregate.com.htm");

            List<long> aShingles = NShingle(8, a);
            List<long> bShingles = NShingle(8, b);
            Console.WriteLine(NearDuplicates.SketchIsNearDuplicate(aShingles, bShingles, 0.9));
            }*/
        }

        public static bool SketchIsNearDuplicate(List<long> a, List<long> b, double duplicateThreshold)
        {
            return Jaccard(a, b) > duplicateThreshold;

        }

        /* Get shingles of n size, hashed with GetHashCode*/
        public static List<long> NShingle(int sizeShingle, string input, Uri uri) {
            List<long> hashes = new List<long>();
            var splitInput = input.Split(' ');
            foreach (Func<string, Int64> v in HashFunctions()) 
            {
                hashes.Add(HashAndGetMin(splitInput, sizeShingle, v, uri));
            }         

            return hashes;
        }

        public static Int64 HashAndGetMin(string[] splitInput, int sizeShingle, Func<string, Int64> hashFunction, Uri uri) 
        {
            if (cachedShingle.ContainsKey(uri) && cachedShingle[uri].ContainsKey(hashFunction)) 
            {
                return cachedShingle[uri][hashFunction];
            }
            long minValue = Int64.MaxValue;
            for (int i = 0; i < splitInput.Count(); i++) {
                long value = hashFunction(string.Concat(splitInput.Skip(i).Take(sizeShingle)));
                if (value < minValue) 
                {
                    minValue = value;
                }
            }

            if (!cachedShingle.ContainsKey(uri)) 
            {
                cachedShingle[uri] = new Dictionary<Func<string, long>, long>();
            }
            cachedShingle[uri][hashFunction] = minValue;

            return minValue;
        }

        public static List<Func<string, Int64>> HashFunctions() 
        {
            var hashFunctions = new List<Func<string, Int64>> {
                Hash.APHash,
                Hash.BKDRHash,
                Hash.BPHash,
                Hash.DEKHash,
                Hash.DJBHash,
                Hash.ELFHash,
                Hash.FNVHash,
                Hash.JSHash,
                Hash.RSHash,
                Hash.SDBMHash
            };

            return hashFunctions;

        }

        public static double Jaccard(List<long> A, List<long> B) {
            return Convert.ToDouble(A.Intersect(B).Count()) / Convert.ToDouble(A.Union(B).Count());
        }
    }
}

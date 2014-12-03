using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject3
{
    class Program
    {
        static void Main(string[] args)
        {
            //MathNet.Numerics.Control.UseNativeMKL();
            new Loader();
            
            Console.WriteLine("done loading");
            List<KeyValuePair<int, double>> list = new List<KeyValuePair<int, double>>();
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("graphfile.txt"))
            {
                var recommender = new Recommender();

                for (int k = 1; k < 31; k++)
                {
                   
                    recommender.RMUHat = recommender.CalcRMUHat(k);
                    file.WriteLine(k + "\t" + calcRMSE(recommender));
                    file.Flush();
                }
            }

            Console.WriteLine("All is done");
            Console.Read();
           // 
        
        }

        static double calcRMSE(Recommender recommender)
        {
            Dictionary<int, Dictionary<int, int>> probeSet = new Dictionary<int, Dictionary<int, int>>(); // movieid -->  userid--> rating

            FillProbeSet(probeSet);

            double sum = 0.0;
            double count = 0.0;
            foreach(int movieKey in probeSet.Keys)
            {
                foreach(int userKey in probeSet[movieKey].Keys)
                {
                    int movierow = 0;
                    int usercolumn = 0;
                    try
                    {
                        movierow = recommender.movieMapper[movieKey];
                        usercolumn = recommender.userMapper[userKey];
                    }
                    catch(Exception e)
                    {
                        continue;
                    }
                    double adjustedRHat = recommender.RMUHat[movierow, usercolumn];
                    adjustedRHat += recommender.dicMovieMean[movieKey];
                    adjustedRHat += recommender.dicUserMean[userKey];
                    adjustedRHat -= recommender.overallMean;

                    sum += Math.Pow(adjustedRHat - (double)probeSet[movieKey][userKey], 2);
                    count++;
                }
            }


            return Math.Sqrt(sum/count);
        }

        private static void FillProbeSet(Dictionary<int, Dictionary<int, int>> probeSet)
        {
            string path = "C:\\Users\\Praetorian\\Documents\\p7\\download\\newprobe.txt";
            string[] lines = System.IO.File.ReadAllLines(path);
            int movieid = -1;
            foreach (string line in lines)
            {
                if (line.Contains(':'))
                {
                    movieid = Convert.ToInt32(line.Split(':')[0]);
                    probeSet[movieid] = new Dictionary<int, int>();
                }
                else
                {
                    string[] splittedStrings = line.Split(',');
                    int userid = Convert.ToInt32(splittedStrings[0]);
                    int rating = Convert.ToInt32(splittedStrings[1]);
                    probeSet[movieid][userid] = rating;
                }
            }
        }
    }
}

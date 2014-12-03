using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netflix
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, Dictionary<int, UserRating>> probeData;
            Dictionary<int, Dictionary<int, UserRating>> trainingData;
            DataLoader dataLoader = new DataLoader();
            Learning learner = new Learning();
            probeData = dataLoader.LoadProbeData("probe.txt");
            trainingData = dataLoader.LoadTrainingData("training_set", probeData);
            Console.WriteLine("Manipulating data");
            var data = dataLoader.ManipulateData(trainingData, probeData);
            trainingData = data.Item1;
            probeData = data.Item2;
            Result result = learner.SubtractMeans(trainingData);
            learner.CalcRMUHat(result.userMean, result.movieMean, trainingData, result.Sum, result.N);
            Console.WriteLine("###");
            double RMSE = 0.0;
            int n = 0;
            foreach (var v in trainingData) 
            {
                foreach (var l in v.Value) 
                {
                    if (l.Value.Rating != 0) 
                    {
                        RMSE += Math.Pow(Convert.ToDouble((trainingData[v.Key][l.Key].RMUHat - l.Value.Rating)), 2);
                        n++;
                    }
                    
                } 
            }
            Console.WriteLine(Math.Sqrt(RMSE / n));

            RMSE = 0.0;
            n = 0;
            foreach (var v in probeData)
            {
                foreach (var l in v.Value)
                {
                    if (trainingData[v.Key].ContainsKey(l.Key))
                    {
                        //Console.WriteLine(String.Format("Original: {0:0.00} - RMUHat: {1:0.00}", l.Value.Rating, trainingData[v.Key][l.Key].RMUHat));
                        RMSE += Math.Pow(Convert.ToDouble((trainingData[v.Key][l.Key].RMUHat - l.Value.Rating)), 2);
                        n++;
                    }
                }
            }
            Console.WriteLine(Math.Sqrt(RMSE / n));

            Console.ReadLine();
        }
    }
}

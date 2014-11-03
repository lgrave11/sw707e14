using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;

namespace SentimentClassifier
{
    class Program
    {
        static void Main(string[] args)
        {
            List<List<Review>> partitions = LoadPartitions(10);
            for (int i = 0; i < 10; i++) 
            {
                Console.WriteLine("######");
                List<int> range = Enumerable.Range(0, 10).ToList();
                List<Review> testData = partitions.ElementAt(i);
                List<Review> learnData = partitions.Where((value, index) => index != i).SelectMany(x => x).ToList();
                NaiveBayesClassifier nbc = new NaiveBayesClassifier(learnData);
                nbc.ScoreData(testData);
            }
        }


        static List<List<Review>> LoadPartitions(int dataSets, bool debug=false) 
        {
            List<List<Review>> partitions = new List<List<Review>>();
            Parser parser = new Parser("SentimentTrainingData.txt", debug: debug);
            partitions = parser.getDataSets(dataSets);

            return partitions;
        }
    }
}


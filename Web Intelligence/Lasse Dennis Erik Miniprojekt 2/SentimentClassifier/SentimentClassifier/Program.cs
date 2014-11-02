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
            List<Review> testData = partitions.First();
            List<Review> testDataPos = testData.Where(x => x.c == Classification.Positive).ToList();
            List<Review> testDataNeg = testData.Where(x => x.c == Classification.Negative).ToList();
            //testDataPos.AddRange(testDataNeg);
            List<Review> learnData = partitions.Skip(1).SelectMany(x => x).ToList();
            List<Review> learnDataPos = learnData.Where(x => x.c == Classification.Positive).Take(70000).ToList();
            List<Review> learnDataNeg = learnData.Where(x => x.c == Classification.Negative).Take(70000).ToList();
            learnDataPos.AddRange(learnDataNeg);
            NaiveBayesClassifier nbc = new NaiveBayesClassifier(learnDataPos);
            nbc.ScoreData(testDataNeg);
            Console.WriteLine("#######");
            nbc.ScoreData(testDataPos);
            Console.WriteLine("#######");
            nbc.ScoreData(testData);
            Console.WriteLine("#######");
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


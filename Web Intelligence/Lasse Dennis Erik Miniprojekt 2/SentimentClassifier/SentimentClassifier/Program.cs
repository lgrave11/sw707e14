using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace SentimentClassifier
{
    class Program
    {
        static void Main(string[] args)
        {
            List<List<Review>> partitions = LoadPartitions(10);
            SavePartitions(partitions);
            List<Review> testData = partitions.First();
            List<Review> testDataPos = testData.Where(x => x.c == Classification.Positive).Take(500).ToList();
            List<Review> testDataNeg = testData.Where(x => x.c == Classification.Negative).Take(500).ToList();
            testDataPos.AddRange(testDataNeg);
            List<Review> learnData = partitions.Skip(1).SelectMany(x => x).ToList();
            List<Review> learnDataPos = learnData.Where(x => x.c == Classification.Positive).Take(10000).ToList();
            List<Review> learnDataNeg = learnData.Where(x => x.c == Classification.Negative).Take(10000).ToList();
            learnDataPos.AddRange(learnDataNeg);
            NaiveBayesClassifier nbc = new NaiveBayesClassifier(learnDataPos);
            nbc.ScoreData(testDataPos);
        }


        static List<List<Review>> LoadPartitions(int dataSets) 
        {
            List<List<Review>> partitions = new List<List<Review>>();
            for (int i = 0; i < dataSets; i++) 
            {
                Console.WriteLine(string.Format("Loaded {0}", i));
                string partitionFile = string.Format("partitions-{0}.bin", i);
                if (File.Exists(partitionFile))
                {
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new FileStream(partitionFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    partitions.Add((List<Review>)formatter.Deserialize(stream));
                    stream.Close();
                }
                else
                {
                    Parser parser = new Parser("SentimentTrainingData.txt", debug: false);
                    partitions = parser.getDataSets(dataSets);
                    break;
                }
            }

            return partitions;
        }
        static void SavePartitions(List<List<Review>> partitions) 
        {
            foreach (List<Review> partition in partitions) 
            {
                string partitionFile = string.Format("partitions-{0}.bin", partitions.IndexOf(partition));
                if (!File.Exists(partitionFile))
                {
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new FileStream(partitionFile, FileMode.Create, FileAccess.Write, FileShare.None);
                    formatter.Serialize(stream, partition);
                    stream.Close();
                }
            }
            
        }
    }
}


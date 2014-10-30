using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SentimentClassifier
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser parser = new Parser("SentimentTrainingData.txt", debug:true);
            List<List<Review>> partitions = parser.getDataSets(10);
            List<Review> testData = partitions.First();
            List<Review> learnData = partitions.Skip(1).SelectMany(x => x).ToList();
            List<Review> learnDataPos = learnData.Where(x => x.c == Classification.Positive).Take(2500).ToList();
            List<Review> learnDataNeg = learnData.Where(x => x.c == Classification.Negative).Take(2500).ToList();
            learnDataPos.AddRange(learnDataNeg);
            NaiveBayesClassifier nbc = new NaiveBayesClassifier(learnDataPos);
            nbc.ScoreData(testData.Skip(1).Take(10).ToList());
        }
    }
}


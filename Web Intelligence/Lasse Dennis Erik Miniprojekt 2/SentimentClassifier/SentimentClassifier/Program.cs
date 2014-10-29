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
            Tokenizer tok = new Tokenizer();
            var tokens = tok.tokenize(partitions.First().First().Summary + " " + partitions.First().First().Text);
            Console.WriteLine("");
        }
    }
}

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
            //Parser parser = new Parser("SentimentTrainingData.txt");
            //List<List<Review>> partitions = parser.getDataSets(10);
            Console.WriteLine(HttpUtility.HtmlDecode("&amp;"));
            Console.ReadLine();
        }
    }
}

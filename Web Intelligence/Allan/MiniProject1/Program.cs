using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject1
{
    class Program
    {
        static void Main(string[] args)
        {
            CrawlerContext1 db = new CrawlerContext1();

            //Crawler crawler = new Crawler(new List<Uri>() { new Uri("http://aau.dk/"), new Uri("http://stackoverflow.com/") }, "Allan");
            //crawler.StartCrawling();

            Dictionary<int, string> translatorIdtoURL = new Dictionary<int, string>();
            int id = 0;
            foreach (var x in db.crawlerStorage)
            {
                translatorIdtoURL[id] = x.url;
                Indexer.Instance.AddDoc(x.content);
                id++;
            }
            Indexer.Instance.ConstructIndex();
            Console.WriteLine("ranker started");
            foreach (int i in Ranker.Instance.GetSites(new List<string>() {"Kom", "med", "ind", "i", "byens", "maskinrum.", "Hør,", "hvad", "Frank", "Jensen", "og", "førende", "fagfolk", "forestiller", "sig", "for", "byens", "fremtid", "med", "100.000", "nye", "københavnere,", "og", "kom", "med", "din", "vision", "for", "fremtidens", "København" }))
            {
                Console.WriteLine(i + " - " + translatorIdtoURL[i]);
            }
            
            //Testcase-we know it works
            /*
            List<string> contexts = new List<string>() { "fede fin fede fin fede fin", "fin og boys og fin boys fin boys", "fede fin og funny boys", "fede funny boys" };
            foreach (string s in contexts)
            {
                Indexer.Instance.AddDoc(s);
            }
            Indexer.Instance.ConstructIndex();
            Console.WriteLine("ranker started");
            foreach (int i in Ranker.Instance.GetSites(new List<string>() {"funny", "fin"}))
            {
                Console.WriteLine(i);
            }*/
            Console.Read();
        }
    }
}

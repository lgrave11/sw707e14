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
            foreach (var x in db.crawlerStorage)
            {
                Indexer.Instance.AddDoc(x.content);
            }
            Indexer.Instance.ConstructIndex();
            
            Console.Read();
        }
    }
}

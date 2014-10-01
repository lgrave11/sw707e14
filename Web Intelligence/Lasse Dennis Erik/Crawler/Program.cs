using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CrawlerNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Spider spider = new Spider(new List<string> { "http://en.wikipedia.org/wiki/Internet_of_Things" });
            spider.CrawlAll(amount: 100);
            Console.WriteLine(sw.Elapsed);

            /*CrawlerContext db = new CrawlerContext();
            var q = from item in db.Crawler
                    select item.Html;
            foreach (var i in q) 
            {
                Console.WriteLine(ZipString.UnZipStr(i));
            }*/
                        
            

            /*using(var db = new CrawlerContext())
            {
                var query = from b in db.Crawler
                            select b;
                foreach (var q in query) {
                    Console.WriteLine(q.Id);
                }
            }*/


        }
    }
}

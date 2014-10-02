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
            Spider spider = new Spider(new List<string> { "http://en.wikipedia.org/wiki/Internet_of_Things" });
            spider.CrawlAll(amount: 100);
        }
    }
}

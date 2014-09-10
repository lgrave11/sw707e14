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
            Crawler crawler = new Crawler(new List<string>() { "http://aau.dk" }, "Allan");
            crawler.StartCrawling();
            Console.Read();
        }
    }
}

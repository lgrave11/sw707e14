using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;

namespace Indexer
{
    public class CrawlerDoc 
    {
        public int DocId;
        public string s;
    }

    public class Program
    {
        static void Main(string[] args)
        {

            Indexer indexer = new Indexer();
            indexer.makeInvertedIndex();

            string query = "internet of things";
            Ranker ranker = new Ranker(indexer, indexer.items.Select(x => x.Id).ToList(), indexer.items.Count());
            foreach (var v in ranker.GetKSites(query, K: 25)) 
            {
                Console.WriteLine(indexer.items.Where(x => x.Id == v.Key).FirstOrDefault().Url);
            };

        }
    }
}

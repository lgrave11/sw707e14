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
            CrawlerContext db = new CrawlerContext();
            var items = (from b in db.Crawler
                        select b).ToList();

            Indexer indexer = new Indexer();
            foreach (Crawler item in items) 
            {
                //Console.WriteLine("On item {0} out of {1}", items.IndexOf(item), items.Count);
                List<string> tokens = Tokenizer.ExtractText(ZipString.UnZipStr(item.Html));
                List<string> features = FeatureConstructor.RemoveAndStem(tokens);
                foreach(string term in features) 
                {
                    indexer.addIncidence(term, item.Id);
                }
                
            }

            string query = "internet of things";

            Ranker ranker = new Ranker(indexer, items.Select(x => x.Id).ToList(), items.Count());
            foreach (var v in ranker.GetKSites(query, K: 10)) 
            {
                Console.WriteLine(items.Where(x => x.Id == v.Key).FirstOrDefault().Url);
            };

        }
    }
}

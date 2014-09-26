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
                Console.WriteLine("On item {0} out of {1}", items.IndexOf(item), items.Count);
                List<string> tokens = Tokenizer.ExtractText(ZipString.UnZipStr(item.Html));
                List<string> features = FeatureConstructor.RemoveAndStem(tokens);
                foreach(string term in features) 
                {
                    indexer.addIncidence(term, item.Id);
                }
                
            }

            CrawlerDoc doc1 = new CrawlerDoc { DocId = 1, s = "lars ko fisk hest kat lars fisk ko kat kat" };
            CrawlerDoc doc2 = new CrawlerDoc { DocId = 2, s = "fisk fisk fisk hest lars ko fisk lars lars" };
            CrawlerDoc doc3 = new CrawlerDoc { DocId = 3, s = "kat kat fisk fisk ko ko hest hest" };
            CrawlerDoc doc4 = new CrawlerDoc { DocId = 4, s = "hest hest fisk fisk lars ko lars" };
            List<CrawlerDoc> docs = new List<CrawlerDoc> { doc1, doc2, doc3, doc4 };
            string query = "lars kat fuck";
            foreach (CrawlerDoc doc in docs)
            {
                List<string> tokens = Tokenizer.ExtractText(doc.s);
                List<string> features = FeatureConstructor.RemoveAndStem(tokens);
                foreach (string term in features)
                {
                    indexer.addIncidence(term, doc.DocId);
                }

            }

            Ranker ranker = new Ranker(indexer, docs.Select(x => x.DocId).ToList(), docs.Count());
            foreach (var v in ranker.GetKSites(query, K: 10)) 
            {
                Console.WriteLine(v);
                Console.WriteLine(docs.Where(x => x.DocId == v.Key).FirstOrDefault());
            };

        }
    }
}

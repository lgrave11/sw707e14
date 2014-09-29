using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using RobotsParser;
using System.IO.Compression;

namespace CrawlerNamespace
{
    class Spider
    {
        public QueueHashSet frontier;
        public HashSet<Uri> visitedUrls = new HashSet<Uri>();
        public Dictionary<Uri, List<long>> otherHashes = new Dictionary<Uri, List<long>>();
        public Dictionary<String, int> otherSimpleHashes = new Dictionary<string, int>();
        public TimedWebClient client = new TimedWebClient { Timeout = 500 };
        public CrawlerContext db = new CrawlerContext();
        public Uri currentUri;
        public int crawlAmount;

        public Spider(List<string> seedUrls) {
            // Reset database first.
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE Crawler");
            db.SaveChanges();

            frontier = new QueueHashSet(seedUrls);
            
        }

        public void CrawlAll(int amount=1) {
            this.crawlAmount = amount;
            while (db.Crawler.Count() <= this.crawlAmount) 
            {
                currentUri = frontier.Dequeue();
                if (allowedRobotsTxt(currentUri))
                {
                    Console.WriteLine(String.Format("Frontier size: {0}, Urls crawled: #{1}, {2}", frontier.hashSet.Count(), db.Crawler.Count(), currentUri));
                    string content = "";

                    try
                    {
                        content = client.DownloadString(currentUri);
                    }
                    catch (Exception e)
                    { // Empty page, no need to stay on this url because nothing useful is here.
                        visitedUrls.Add(currentUri);
                        System.Threading.Thread.Sleep(1000);
                        continue;
                    }


                    if (HasEqual(content))
                    {
                        if (!visitedUrls.Contains(currentUri)) 
                        {
                            visitedUrls.Add(currentUri);
                        }
                        
                        System.Threading.Thread.Sleep(1000);
                        continue;
                    }
                    visitedUrls.Add(currentUri);
                    System.Threading.Thread.Sleep(1000);
                    ExtractAndFixUrls(content);

                }
                continue;
            
            }
            
        }

        // Shingling is a major bottle neck. Replaced it with simple GetHashCode() check instead.
        public bool HasEqual(string html) {
            List<long> hashes = NearDuplicates.NShingle(8, html, currentUri);
            
            foreach(var l in otherHashes) 
            {
                if(NearDuplicates.SketchIsNearDuplicate(hashes, l.Value, 0.9)) 
                {
                    return true;
                }
            }
            otherHashes.Add(currentUri, hashes);
            /*int newHash = html.GetHashCode();
            if (otherSimpleHashes.ContainsKey(currentUri.ToString()) || otherSimpleHashes.ContainsValue(newHash)) 
            {
                return true;
            }
            otherSimpleHashes.Add(currentUri.ToString(), newHash);*/
            db.Crawler.Add(new Crawler { Html = ZipString.ZipStr(html), Url = currentUri.ToString() });
            db.SaveChanges();
            return false;
        }

        public void ExtractAndFixUrls(string html) {
            List<string> links = GetAllLinks(html);
            List<Uri> fixedLinks = new List<Uri>();
            foreach (string s in links)
            {
                try
                {
                    fixedLinks.Add(new Uri(new Uri(currentUri, s).AbsoluteUri.Normalize()));
                }
                catch (Exception e) {
                    try
                    {
                        Console.WriteLine("Could not fix {0}", s);
                        fixedLinks.Add(new Uri(s.Normalize()));
                    }
                    catch (Exception) 
                    {
                        continue;
                    }
                }
            }

            if (frontier.hashSet.Count() < this.crawlAmount) {
                foreach (Uri u in fixedLinks)
                {
                    if (!frontier.hashSet.Contains(u) &&
                        !visitedUrls.Contains(u) &&
                        PassesURLFilter(u))
                    {
                            frontier.Enqueue(u);
                    }
                }
            
            }
            
        }

        public bool allowedRobotsTxt(Uri uri) 
        { 
            string robots = "http://" + uri.Host;

            try
            {
                UserAgent userAgent = Robots.ReadRobots(robots);
                if (userAgent.AllowFetching(uri.ToString()))
                {
                    return true;
                }
            }
            catch (Exception e) 
            {
                return false;
            }
            
            return false;
        
        }

        public static List<string> GetAllLinks(string message)
        {
            List<string> list = new List<string>();
            Regex urlRx = new Regex("href=\"(.*?)\"", RegexOptions.IgnoreCase);

            MatchCollection matches = urlRx.Matches(message);
            foreach (Match match in matches)
            {
                list.Add(match.Groups[1].Value);
            }
            return list;
        }

        public bool PassesURLFilter(Uri uri) {
            try
            {
                return IsText(uri);
            }
            catch (Exception e) 
            {
                return false;
            }
            
        }

        private bool IsText(Uri uri)
        {
            List<string> fileTypes = new List<string>() { ".jpeg", ".jpg", ".png", ".pdf", ".ogg" };

            foreach (string s in fileTypes)
            {
                if (uri.ToString().EndsWith(s))
                {
                    return false;
                }
            }
            return true;
        }


    }

   

}

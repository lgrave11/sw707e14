using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace MiniProject1
{
    class Crawler
    {
        private string _crawlerName;

        Queue<string> frontier = new Queue<string>();
        List<string> visitedURLs = new List<string>();
        Dictionary<string, string> texts = new Dictionary<string, string>();
        public Crawler(List<string> seedURLs, string crawlerName)
        {
            _crawlerName = crawlerName;
            seedURLs.ForEach(x => frontier.Enqueue(x));
        }

        public void StartCrawling()
        {
            WebClient client = new WebClient();
            
            while (frontier.Count > 0 && visitedURLs.Count < 1000)
            {
                try
                {
                    string currentURL = frontier.Dequeue().Normalize().ToLower();
                    if (IsAllowed(currentURL))
                    {
                        string extractedText = "";
                        try
                        {
                            extractedText = client.DownloadString(currentURL);
                        }
                        catch (WebException e)
                        {
                            continue;
                        }
                        bool seen = false;
                        /*foreach (string visited in visitedURLs)
                        {
                            if ( NearDuplicatesWithSketches.NearDuplicate(extractedText, texts[visited], 8, 0.9f))
                            {
                                seen = true;
                                break;
                            }
                        }*/
                        if (!seen)
                        {
                            visitedURLs.Add(currentURL);
                            texts.Add(currentURL, extractedText);

                            //step 4
                            List<string> extractedURLs = Extract(extractedText, currentURL);
                            extractedURLs.RemoveAll(x => !IsText(x));
                            foreach (string URL in extractedURLs)
                            {
                                //step 4 a
                                string normalizedURL = URL.Normalize();
                                //step 4 c
                                if (!visitedURLs.Contains(normalizedURL) && !frontier.Contains(normalizedURL))
                                {
                                    //step 4 d
                                    frontier.Enqueue(normalizedURL);
                                }
                            }
                            Console.WriteLine(currentURL + "\t" + visitedURLs.Count);
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                }
                catch (Exception e)
                {

                }
                
            }

            StreamWriter sw;
            string path = @"c:\crawler\";
            foreach (string s in visitedURLs)
            {
                sw = new StreamWriter(path + s.GetHashCode()+ ".txt");
                sw.Write(s + "\n\n" + texts[s]);
                sw.Flush();
            }
            Console.WriteLine("Done writing to files");
        }
        private bool IsText(string url)
        {
            List<string> fileTypes= new List<string>(){
                ".jpeg",
                ".jpg",
                ".png"
            };
            foreach (string s in fileTypes)
            {
                if (url.EndsWith(s))
                {
                    return false;
                }
            }
            return true;
        }

        //metode taget fra http://www.anotherchris.net/csharp/extracting-all-links-from-a-html-page/
        public static List<string> Extract(string html, string url)
        {
            List<string> list = new List<string>();

            Regex regex = new Regex("(?:href|src)=[\"|']?(.*?)[\"|'|>]+", RegexOptions.Singleline | RegexOptions.CultureInvariant);
            if (regex.IsMatch(html))
            {
                foreach (Match match in regex.Matches(html))
                {
                    try {
                    Uri uri = new Uri(match.Groups[1].Value);
                    }
                    catch {
                        try
                        {
                            if (!list.Contains(new Uri(new Uri(url), match.Groups[1].Value).AbsoluteUri))
                                list.Add(new Uri(new Uri(url), match.Groups[1].Value).AbsoluteUri);
                            continue;
                        }
                        catch(Exception e)
                        {
                            continue;
                        }
                    }
                    if(!list.Contains(match.Groups[1].Value))
                        list.Add(match.Groups[1].Value);
                }
            }

            return list;
        }

        private bool IsAllowed(string URL)
        {
            Restrictions restrictions = Parser.GetRestrictions(URL, _crawlerName);
            if (restrictions == null)
            {
                return true;
            }
            else if(restrictions.DisallowList.Contains(""))
            {
                return true;
            }

            Uri uri = new Uri(URL.Normalize());
            bool allowed = true;
            foreach (string s in restrictions.DisallowList)
            {
                if (Matches(uri.LocalPath, s))
                {
                    allowed =  false;
                }
            }
            if (!allowed)
            {
                return restrictions.AllowList.Contains(CutFile(uri).LocalPath);
            }
            return true;
        }

        private Uri CutFile(Uri url)
        {
            return new Uri(url.AbsoluteUri.Substring(0, url.AbsoluteUri.LastIndexOf('/') + 1));
        }

        private bool Matches(string ourString, string allowanceString){
            if (allowanceString.Contains("*"))
            {
                return true;
            }
            return ourString.StartsWith(allowanceString);
        }
    }
}

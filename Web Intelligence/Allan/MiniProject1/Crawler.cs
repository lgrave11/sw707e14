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

        Queue<Uri> frontier = new Queue<Uri>();
        List<Uri> visitedURLs = new List<Uri>();
        Dictionary<Uri, string> texts = new Dictionary<Uri, string>();
        int allowedCount = 0;

        public Crawler(List<Uri> seedURLs, string crawlerName)
        {
            _crawlerName = crawlerName;
            seedURLs.ForEach(x => frontier.Enqueue(x));
        }

        public void StartCrawling()
        {
            WebClient client = new WebClient();
            
            while (frontier.Count > 0 && visitedURLs.Count < 1000000)
            {
                
                Uri currentURL = frontier.Dequeue();
                currentURL = NormalizeUri(currentURL);

                if (IsAllowed(currentURL))
                {
                    allowedCount++;
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
                    foreach (Uri visited in visitedURLs)
                    {
                        
                        if ( NearDuplicatesWithSketches.NearDuplicate(extractedText, texts[visited], 8, 0.9f))
                        {
                            seen = true;
                            break;
                        }
                    }
                    if (!seen)
                    {
                        visitedURLs.Add(currentURL);
                        texts.Add(currentURL, extractedText);

                        //step 4
                        List<Uri> extractedURLs = Extract(extractedText, currentURL);
                        extractedURLs.RemoveAll(x => !IsText(x));

                        foreach (Uri URL in extractedURLs)
                        {
                            try
                            {
                                //step 4 a
                                Uri normalizedURL = NormalizeUri(URL);
                                //step 4 c
                                if (!visitedURLs.Contains(normalizedURL) && !frontier.Contains(normalizedURL))
                                {
                                    //step 4 d
                                    frontier.Enqueue(normalizedURL);
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        Console.WriteLine(currentURL + "\t" + visitedURLs.Count + ":" + allowedCount + "\t" + DateTime.Now.ToString("h:mm:ss tt"));
                        System.Threading.Thread.Sleep(1000);
                    }
                }

                
            }
        }

        // Found on http://stackoverflow.com/questions/1874632/normalizing-uri-to-make-it-work-correctly-with-makerelativeuri
        private Uri NormalizeUri(Uri url)
        {
            return new Uri(string.Format("{0}://{1}:{2}{3}{4}", url.Scheme, url.Host, url.Port, Regex.Replace(url.LocalPath, @"(?<!\:)/{2,}", "/"), url.Query));
        }

        private bool IsText(Uri url)
        {
            List<string> fileTypes= new List<string>(){
                ".jpeg",
                ".jpg",
                ".png"
            };
            foreach (string s in fileTypes)
            {
                if (url.AbsoluteUri.EndsWith(s))
                {
                    return false;
                }
            }
            return true;
        }

        //metode taget fra http://www.anotherchris.net/csharp/extracting-all-links-from-a-html-page/
        public static List<Uri> Extract(string html, Uri url)
        {
            List<Uri> list = new List<Uri>();

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
                            if (!list.Contains(new Uri(url, match.Groups[1].Value)))
                                list.Add(new Uri(url, match.Groups[1].Value));
                            continue;
                        }
                        catch(Exception e)
                        {
                            continue;
                        }
                    }
                    if(!list.Contains(new Uri(match.Groups[1].Value)))
                        list.Add(new Uri(match.Groups[1].Value));
                }
            }

            return list;
        }

        private bool IsAllowed(Uri URL)
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
            bool allowed = true;
            foreach (string s in restrictions.DisallowList)
            {
                if (Matches(URL.LocalPath, s))
                {
                    allowed =  false;
                }
            }
            if (!allowed)
            {
                return restrictions.AllowList.Contains(CutFile(URL).LocalPath);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace RobotsParser
{
    public class Robots
    {
        static void Main(string[] args)
        {
            /*
            UserAgent userAgent = ReadFile("robots.txt");
            userAgent._allow.ForEach(x => Console.WriteLine("Allow: " + x));
            userAgent._disallow.ForEach(x => Console.WriteLine("Disallow: " + x));
             */
        }

        public static UserAgent ReadRobots(string robotsUrl)
        {
            WebClient client = new WebClient();
            StreamReader clientRead = new StreamReader(client.OpenRead(robotsUrl + "/robots.txt"));
            string file = clientRead.ReadToEnd();
            string[] splitFile = file.Split('\n');
            bool readAllowsDisallows = false;
            UserAgent userAgent = new UserAgent();

            foreach (string line in splitFile)
            {
                if (line.Contains("#"))
                {
                    continue;
                }
                else if (line.ToLower().Contains("user-agent: *"))
                {
                    userAgent.UserAgentName = line.Split(new char[] {':'}, 2)[1].Trim();
                    readAllowsDisallows = true;
                }
                else if (readAllowsDisallows && line.ToLower().Contains("disallow:"))
                {
                    userAgent.SetDisallow(line.Split(new char[] { ':' }, 2)[1].Trim());
                }
                else if (readAllowsDisallows && line.ToLower().Contains("allow:"))
                {
                    userAgent.SetAllow(line.Split(new char[] { ':' }, 2)[1].Trim());
                }
                else if (line.ToLower().Contains("user-agent:"))
                {
                    readAllowsDisallows = false;
                }
            }
            return userAgent;
        }
    }

    public class UserAgent
    {
        private string _userAgent;
        private List<string> _allow = new List<string>();
        private List<string> _disallow = new List<string>();

        public string UserAgentName { 
            get { return _userAgent; }
            set { _userAgent = value; }
            }

        public void SetAllow(string allow)
        {
            _allow.Add(allow);
        }

        public void SetDisallow(string disallow)
        {
            _disallow.Add(disallow);
        }

        private bool Allows(string allow){
            if (_allow.Contains(allow))
                return true;
            else
                return false;
        }

        private bool Disallows(string disallow)
        {
            if (_disallow.Contains(disallow))
                return true;
            else
                return false;
        }
        
        public bool AllowFetching(string url)
        {
            Uri uri = new Uri(url);
            string absolutePath = uri.AbsolutePath;
            string[] segments = uri.Segments;
            string concatSegments = "";

            foreach (string segment in segments)
            {
                concatSegments += segment;
                if (Disallows(concatSegments))
                    return false;
            }
            return true;
        }
    }
}

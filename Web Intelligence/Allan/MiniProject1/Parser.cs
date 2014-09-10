using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;

namespace MiniProject1
{
    class Parser
    {
        public static Restrictions GetRestrictions(Uri URL, string crawlerName)
        {
            //Retrieve the robot text
            WebClient client = new WebClient();
            string robotText = "";
            try
            {
                robotText = client.DownloadString("http://" + URL.Host + "/robots.txt");
            }
            catch (WebException e)
            {
                return null;
            }
            catch (ArgumentException e)
            {
                return null;
            }


            //parse the text
            Restrictions restrictions = new Restrictions();
            List<string> userAgentParts = robotText.Split('\n').ToList();

            bool myPart = false;
            foreach(string s in userAgentParts)
            {
                if(s.ToLower().StartsWith("user-agent:")){
                    if (IsMyUserAgent(crawlerName, s))
                    {
                        myPart = true;
                        continue;
                    }
                    else if (myPart)
                    {
                        break;
                    }
                    else if (s.Split(new char[] { ':' }, 2)[1].Trim() == "*")
                    {
                        myPart = true;
                        continue;
                    }
                }
                else if (myPart)
                {
                    if (s.ToLower().Trim().StartsWith("allow:"))
                    {
                        restrictions.AddAllowance(s.Split(new char[] { ':' }, 2)[1].Trim());
                    }
                    else if (s.ToLower().Trim().StartsWith("disallow:"))
                    {
                        restrictions.AddDisallowance(s.Split(new char[] { ':' }, 2)[1].Trim());
                    }
                }
            }
            return restrictions;
        }

        private static bool IsMyUserAgent(string crawlername, string compareName)
        {
            return compareName.Split(new char[]{':'}, 2)[1].Trim().ToLower() == crawlername.ToLower();
       }

    }
}

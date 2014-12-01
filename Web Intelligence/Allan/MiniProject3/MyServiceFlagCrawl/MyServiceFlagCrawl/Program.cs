using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace MyServiceFlagCrawl
{
    class Program
    {
        static void Main(string[] args)
        {
            string url =  "http://10.0.0.15/mystatus/";

            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.AllowAutoRedirect = true;
            WebResponse response = webrequest.GetResponse();

            HttpWebResponse webResponse = ((HttpWebResponse)response);

            var stream = webResponse.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            string responseFromSErver = reader.ReadToEnd();
            Console.WriteLine(responseFromSErver);

            reader.Close();
            response.Close();

            
         
            Console.Read();

        }
    }
}

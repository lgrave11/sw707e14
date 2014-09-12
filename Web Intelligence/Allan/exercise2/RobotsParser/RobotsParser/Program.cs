using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotsParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Restrictions r = Parser.GetRestrictions("http://reddit.com", "allan");
            Console.WriteLine(r.AllowList.Count + " " + r.DisallowList.Count);
            Uri uri = new Uri("http://stackoverflow.com/questions/1214607/how-can-i-get-the-root-domain-uri-in-asp-net");
            Console.WriteLine();
            Console.Read();
        }
    }
}

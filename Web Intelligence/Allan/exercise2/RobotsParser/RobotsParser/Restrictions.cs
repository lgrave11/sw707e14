using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotsParser
{
    class Restrictions
    {
        public List<string> AllowList = new List<string>();
        public List<string> DisallowList = new List<string>();

        public void AddAllowance(string path)
        {
            DisallowList.RemoveAll(x => x == path);
            AllowList.Add(path);
        }

        public void AddDisallowance(string path)
        {
            AllowList.RemoveAll(x => x == path);
            DisallowList.Add(path);
        }
    }
}

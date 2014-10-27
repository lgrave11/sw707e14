using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIMiniProjekt2
{
    class User
    {
        public int Index;
        public double Eigen;
        public string Username;
        public List<string> Friends = new List<string>();
        public string Review;
        public string Summary;
    }
}

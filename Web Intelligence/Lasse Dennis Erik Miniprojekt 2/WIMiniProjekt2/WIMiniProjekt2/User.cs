using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIMiniProjekt2
{
    class User
    {
        public double Eigen;
        public string Username;
        public List<string> Friends = new List<string>();
        public string Review;
        public string Summary;
        public int Score;
        public bool WillBuy;

        public override string ToString()
        {
            return Username + "\t" + ((Score > 0) ? Score.ToString() : "*") + "\t" + ((Score > 0) ? "*" : (WillBuy) ? "yes" : "no");
        }
    }
}

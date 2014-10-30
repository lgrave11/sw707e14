using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject2
{
    class Review
    {
        public string productID { get; set; }
        public string USerID { get; set; }
        public string profilename { get; set; }
        public double helpfulness { get; set; }
        public double score { get; set; }
        public int time { get; set; }
        public string summary { get; set; }
        public string text { get; set; }

        public List<string> tokenStream { get; set; }

        public Review(string productid, string userid, string profilename, double helpfu, double score, int time, string summary, string txt)
        {
            this.productID = productid;
            this.profilename = profilename;
            USerID = userid;
            helpfulness = helpfu;
            this.score = score;
            this.time = time;
            this.summary = summary;
            this.text = txt;

            tokenStream = (new Tokenizer()).tokenize(summary + text);
        }

        public static List<Review> LoadAllReviews()
        {
            List<Review> returnList = new List<Review>();
            string productID = "";
            string USerID = "";
            string profilename= "";
            double helpfulness = 0.0;
            double score = 0.0;
            int time = 0;
            string summary = "";
            string text = "";

            int counter = 0;
            string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader("SentimentTrainingData.txt");
            while ((line = file.ReadLine()) != null)
            {
                counter++;

                if(counter == 1)
                {
                    productID = line.Substring(18).Trim();
                }
                else if(counter == 2)
                {
                    USerID = line.Substring(14).Trim();
                }
                else if (counter == 3)
                {
                    profilename = line.Substring(19).Trim();
                }
                else if (counter == 4)
                {
                    string[] parts = line.Substring(19).Trim().Split('/');
                    helpfulness = Convert.ToDouble(parts[0]) / Convert.ToDouble(parts[1]);
                }
                else if (counter == 5)
                {
                    score = Convert.ToDouble(line.Substring(13).Trim());
                }
                else if (counter == 6)
                {
                    time = Convert.ToInt32(line.Substring(12).Trim());
                }
                else if (counter == 7)
                {
                    summary = line.Substring(15).Trim();
                }
                else if (counter == 8)
                {
                    text = line.Substring(12).Trim();
                }
                else if (counter == 9)
                {
                    counter = 0;
                    returnList.Add(new Review(productID, USerID, profilename, helpfulness, score, time, summary, text));
                }

            }

            file.Close();

            // Suspend the screen.
            return returnList;
        }
    }
}

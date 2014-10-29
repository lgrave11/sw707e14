using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentimentClassifier
{
    class Review
    {
        public Review()
        {
        }

        public Review(string prodId, string userId, string profileName, string helpfulness, float score, int time, string summary, string text)
        {
            ProductId = prodId;
            UserId = userId;
            ProfileName = profileName;
            Helpfulness = helpfulness;
            Score = score;
            Time = time;
            Summary = summary;
            Text = text;
        }

        public string ProductId { get; set; }
        public string UserId { get; set; }
        public string ProfileName { get; set; }
        public string Helpfulness { get; set; }
        public float Score { get; set; }
        public int Time { get; set; }
        public string Summary { get; set; }
        public string Text { get; set; }

    }
}

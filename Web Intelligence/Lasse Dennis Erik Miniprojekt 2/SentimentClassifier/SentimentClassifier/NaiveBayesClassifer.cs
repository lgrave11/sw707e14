using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentimentClassifier
{
    enum Classification { Negative, Positive }
    class NaiveBayesClassifer
    {
        List<Review> LearningData { get; set; }
        List<string> Corpus;
        int TotalAmountOfReviews; // N
        Dictionary<Classification, int> NumberOfReviewsWithSentimentC; // N(c)

        public NaiveBayesClassifer(List<Review> learningData) 
        {
            LearningData = learningData;
            TotalAmountOfReviews = LearningData.Count;
            NumberOfReviewsWithSentimentC.Add(Classification.Negative, NumberOfReviews(Classification.Negative));
            NumberOfReviewsWithSentimentC.Add(Classification.Positive, NumberOfReviews(Classification.Positive));

        }

        public void LearnModel() 
        {
        }

        public int NumberOfReviews(Classification c) 
        {
            int count = 0;
            foreach (Review r in LearningData) 
            {
                if (c == Classification.Positive && r.Score >= 4) 
                {
                    count++;
                }
                else if (c == Classification.Negative && r.Score < 3) 
                {
                    count++;
                }
            }

            return count;
        }

        public float Probability(Classification c) 
        {
            //NumberOfReviewsWithSentimentC[c];
            return (NumberOfReviewsWithSentimentC[c] + 1) / TotalAmountOfReviews + Enum.GetNames(typeof(Classification)).Length;
        }

        public float ProbabilityOfWordInSentimentC(Classification c) 
        {
            
        }

        
    }
}

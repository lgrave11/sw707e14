using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SentimentClassifier
{
    
    class NaiveBayesClassifier
    {
        List<Review> LearningData { get; set; }
        int N; // N
        Dictionary<Classification, int> Nc = new Dictionary<Classification,int>(); // N(c)
        Dictionary<Classification, decimal> Pc = new Dictionary<Classification, decimal>(); // p(c)
        Dictionary<Classification, Dictionary<string, decimal>> PxiC = new Dictionary<Classification, Dictionary<string, decimal>>(); // p(xi|c)
        Dictionary<Classification, decimal> emptyScore = new Dictionary<Classification,decimal>();

        public NaiveBayesClassifier(List<Review> learningData) 
        {
            //Console.WriteLine("Learning the model...");
            LearningData = learningData;
            N = LearningData.Count;
            NumberOfReviews();
            Probability();
            //Console.WriteLine("Calculating the word probs...");
            PxiC.Add(Classification.Negative, new Dictionary<string, decimal>());
            PxiC.Add(Classification.Positive, new Dictionary<string, decimal>());
            ProbabilityOfWordInSentimentC();
            //Console.WriteLine("Calculating empty score...");
            calculateEmptyScore();
        }

        public void calculateEmptyScore() 
        {
            decimal sumPositive = 0;
            decimal sumNegative = 0;
            foreach (var w in PxiC[Classification.Positive])
            {
                sumPositive += (decimal)Math.Log(1 - (double)w.Value);
            }
            foreach (var w in PxiC[Classification.Negative]) 
            {
                sumNegative += (decimal)Math.Log(1 - (double)w.Value);
            }

            sumPositive += (decimal)Math.Log(Nc[Classification.Positive]);
            sumNegative += (decimal)Math.Log(Nc[Classification.Negative]);
            emptyScore[Classification.Positive] = sumPositive;
            emptyScore[Classification.Negative] = sumNegative;
        }

        public void ScoreData(List<Review> TestData) 
        {
            int truePositive = 0, falseNegative = 0, falsePositive = 0, trueNegative = 0;
            int whatever = 0;
            foreach (Review r in TestData)
            {
                decimal scorePositive = this.score(r.Summary, Classification.Positive);
                decimal scoreNegative = this.score(r.Summary, Classification.Negative);

                if (r.Score > 3.0)
                {
                    if (scorePositive >= scoreNegative)
                        truePositive++;
                    else if (scorePositive < scoreNegative) 
                    {
                        falseNegative++;
                    }
                    else
                        whatever++;
                }
                else
                {
                    if (scorePositive >= scoreNegative)
                        falsePositive++;
                    else if (scorePositive < scoreNegative)
                        trueNegative++;
                    else
                        whatever++;
                }
            }

            Console.WriteLine("TP: " + truePositive);
            Console.WriteLine("FN: " + falseNegative);
            Console.WriteLine("FP: " + falsePositive);
            Console.WriteLine("TN: " + trueNegative);
            var accuracy = ((double)truePositive + trueNegative) / ((double)truePositive + falseNegative + falsePositive + trueNegative);
            Console.WriteLine("Accuracy: " + accuracy);
            Console.WriteLine("Error rate: " + (1.0 - accuracy));       
        }

        public decimal score(string x, Classification c) 
        {
            decimal sum = 0;
            foreach (string w in Tokenizer.tokenize(x)) 
            {
                if (PxiC[c].ContainsKey(w)) 
                {
                    decimal val = PxiC[c][w] / (1 - PxiC[c][w]);
                    sum += (decimal)Math.Log((double)val);
                }
                
            }

            return emptyScore[Classification.Positive] + sum;
        }

        public void NumberOfReviews() 
        {
            int posCount = 0;
            int negCount = 0;
            for (int i = 0; i < LearningData.Count; i++)
            {
                if (LearningData[i].c == Classification.Positive)
                {
                    posCount++;
                }
                else if (LearningData[i].c == Classification.Negative)
                {
                    negCount++;
                }
            }
            Nc.Add(Classification.Positive, posCount);
            Nc.Add(Classification.Negative, negCount);
            
        }

        public void Probability() 
        {
            Pc.Add(Classification.Positive, ((decimal)Nc[Classification.Positive] + 1m) / ((decimal)N + (decimal)Enum.GetNames(typeof(Classification)).Length));
            Pc.Add(Classification.Negative, ((decimal)Nc[Classification.Negative] + 1m) / ((decimal)N + (decimal)Enum.GetNames(typeof(Classification)).Length));
        }

        public void ProbabilityOfWordInSentimentC() 
        {
            foreach (Classification c in Enum.GetValues(typeof(Classification))) 
            {
                foreach (Review r in LearningData)
                {
                    foreach (string s in r.Tokens)
                    {
                        PxiC[c][s] = 0m;
                    }
                }
            }            

            List<string> allWords = LearningData.SelectMany(x => x.Tokens).ToList();
            List<string> uniqueWords = allWords.Distinct().ToList(); // X
            foreach (Classification c in Enum.GetValues(typeof(Classification))) 
            {
                List<string> words = LearningData.Where(x => x.c == c).SelectMany(x => x.Tokens).ToList();
                var counts = words.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
                foreach (string w in uniqueWords)
                {
                    int count = 0;
                    if (counts.ContainsKey(w)) 
                    {
                        count = counts[w];
                    }
                    decimal tmp = Convert.ToDecimal(count + 1.0m);
                    decimal tmp2 = Convert.ToDecimal(Nc[c]) + uniqueWords.Count();
                    decimal pxic = tmp / tmp2;
                    PxiC[c][w] = pxic;
                    
                }
            }
            

        }

        
    }
}

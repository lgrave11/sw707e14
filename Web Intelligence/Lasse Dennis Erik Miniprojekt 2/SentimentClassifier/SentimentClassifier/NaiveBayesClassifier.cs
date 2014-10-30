﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            LearningData = learningData;
            N = LearningData.Count;
            NumberOfReviews();
            Probability();
            PxiC.Add(Classification.Negative, new Dictionary<string, decimal>());
            PxiC.Add(Classification.Positive, new Dictionary<string, decimal>());
            ProbabilityOfWordInSentimentC();
            /*List<string> words = LearningData.Where(x => x.c == Classification.Negative).SelectMany(x => x.Tokens).ToList();

            decimal prod = 1;
            foreach (string w in words) 
            {
                decimal val = 0;
                PxiC[Classification.Negative].TryGetValue(w, out val);
                if(val == 0) 
                {
                    val = 1;
                }
                prod *= 1m - val;
            }
            emptyScore.Add(Classification.Negative, prod * Pc[Classification.Negative]);

            words = LearningData.Where(x => x.c == Classification.Positive).SelectMany(x => x.Tokens).ToList();

            prod = 1;
            foreach (string w in words)
            {
                decimal val = 0;
                PxiC[Classification.Positive].TryGetValue(w, out val);
                if (val == 0)
                {
                    val = 1;
                }
                prod *= 1m - val;
            }
            emptyScore.Add(Classification.Positive, prod * Pc[Classification.Positive]);*/


        }

        public void ScoreData(List<Review> TestData) 
        {
            int totalRight = 0;
            int totalWrong = 0;
            int total = TestData.Count;
            foreach (Review r in TestData.ToList()) 
            {
                Console.WriteLine("###################");
                Console.WriteLine(string.Format("Score: {0}", r.c.ToString()));
                decimal scorePositive = this.score(r.Summary + " " + r.Text, Classification.Positive);
                decimal scoreNegative = this.score(r.Summary + " " + r.Text, Classification.Negative);
                Console.WriteLine(String.Format("Positive: {0}, Negative: {1}", scorePositive, scoreNegative));
                if(r.c == Classification.Negative && scoreNegative > scorePositive) 
                {
                    Console.WriteLine("True");
                    totalRight++;
                }
                else if (r.c == Classification.Positive && scoreNegative < scorePositive)
                {
                    Console.WriteLine("True");
                    totalRight++;
                }
                else if(r.c == Classification.Negative && scoreNegative < scorePositive)
                {
                    Console.WriteLine("False");
                    totalWrong++;
                }
                else if (r.c == Classification.Positive && scoreNegative > scorePositive)
                {
                    Console.WriteLine("False");
                    totalWrong++;
                }
            }
            Console.WriteLine("#######");
            Console.WriteLine(String.Format("Total Right: {0}", totalRight));
            Console.WriteLine(String.Format("Total Wrong: {0}", totalWrong));
            Console.WriteLine(String.Format("Total: {0}", total));
            
        }

        public decimal score(string x, Classification c) 
        {
            decimal sum = 0;
            foreach (string w in Tokenizer.tokenize(x)) 
            {
                decimal val = 0;
                PxiC[c].TryGetValue(w, out val);
                sum += val;
            }

            return (decimal)Math.Log10(Decimal.ToSingle(Pc[c] + sum));
            /*foreach (string w in tok.tokenize(x)) 
            {
                decimal val = 0;
                PxiC[c].TryGetValue(w, out val);
                if(val == 0) 
                {
                    val = 1;
                }
                prod *= (val / (1m - val));
            }
            return emptyScore[c] * prod;*/
            /*foreach(string w in tok.tokenize(x)) 
            {
                try
                {
                    sum += PxiC[c][w];
                }
                catch (Exception) 
                {
                    sum += 0;
                }
                
            }
            return (float)Math.Log(Pc[c] + sum);*/
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
            List<string> allWords = LearningData.SelectMany(x => x.Tokens).ToList(); // X
            List<string> uniqueWords = allWords.Distinct().ToList();
            foreach (Classification c in Enum.GetValues(typeof(Classification))) 
            {
                List<string> words = LearningData.Where(x => x.c == c).SelectMany(x => x.Tokens).ToList();
                var counts = words.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
                foreach (string w in uniqueWords)
                {
                    int count;
                    counts.TryGetValue(w, out count);
                    decimal tmp = Convert.ToDecimal(count + 1.0m);
                    decimal tmp2 = Convert.ToDecimal(Nc[c]) + uniqueWords.Count();
                    decimal pxic = tmp / tmp2;
                    PxiC[c].Add(w, pxic);
                    
                }
            }
            

        }

        
    }
}

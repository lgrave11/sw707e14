using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System.Drawing;
using System.Globalization;

namespace MiniProject2
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            Control.UseNativeMKL();
            //List<List<User>> lala = CommunityLib.FindCommunities(User.ReadUserFile());
            List<Review> allReviews = Review.LoadAllReviews();
            Console.WriteLine(allReviews.Count);
            Console.WriteLine("-------------------------------");
            allReviews.Reverse();
            List<Review> trainingSet = allReviews.Skip((int)(allReviews.Count * 0.1)).ToList();
            allReviews.Reverse();
            List<Review> testSet = allReviews.Skip((int)(allReviews.Count * 0.9)).Where(x=> x.score != 3.0).ToList();
            Console.WriteLine("Building Classifier");
            Classifier classifier = new Classifier(trainingSet);
            Console.WriteLine("Classifier built");

            int truePositive = 0, falseNegative = 0, falsePositive = 0, trueNegative = 0, turdcount = 0;

            foreach(Review r in testSet)
            {
                string result = classifier.WantsToBuy(r);

                if(r.score > 3.0 )
                {
                    if (result == "yes")
                        truePositive++;
                    else if (result == "no")
                        falseNegative++;
                    else
                        turdcount++;
                }
                else
                {
                    if (result == "yes")
                        falsePositive++;
                    else if (result == "no")
                        trueNegative++;
                    else
                        turdcount++;
                }
            }

            Console.WriteLine("TP: " + truePositive);
            Console.WriteLine("FN: " + falseNegative);
            Console.WriteLine("FP: " + falsePositive);
            Console.WriteLine("TN: " + trueNegative);
            Console.WriteLine("turdcount: " + turdcount);
            Console.Read();

        }

        
      
    }
}

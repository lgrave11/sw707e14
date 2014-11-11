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
            List<User> allUsers = User.ReadUserFile();
            Console.WriteLine("Building communities");
            List<List<User>> communities = CommunityLib.FindCommunities(allUsers.ToList());
            Console.WriteLine("Loading reviews");
            List<Review> allReviews = Review.LoadAllReviews();
            Console.WriteLine("-------------------------------");
            Console.WriteLine("Building Classifier");
            Classifier classifier = new Classifier(allReviews);
            Console.WriteLine("Classifier built");
            Console.WriteLine("-------------------------------");

            Console.WriteLine("Assigning user scores");
            foreach(User u in allUsers.Where(x=> !string.IsNullOrEmpty(x.Review)))
            {
                u.score = classifier.WantsToBuy(new Review(u.Summary)) == "yes" ? 5 : 1;
            }

            Console.WriteLine("Assigning purchase decisions");
            foreach(User u in allUsers.Where(x=> string.IsNullOrEmpty(x.Review)))
            {
                double sum = 0.0;
                double count = 0.0;
                List<User> myCommunity = new List<User>();
                foreach(var users in communities)
                {
                    if (users.Contains(u))
                    {
                        myCommunity = users;
                        break;
                    }
                }
                foreach(string s in u.Friends)
                {
                    var list = allUsers.Where(x => x.Name == s && x.score > 0);
                    if(list.Count() > 0)
                    {
                        User myFriend = list.First();
                        double multiplier = 1.0;
                        if (myFriend.Name == "kyle")
                            multiplier *= 10.0;

                        if (!myCommunity.Contains(myFriend))
                        {
                            multiplier *= 10.0;
                        }

                        count += multiplier;
                        sum += myFriend.score * multiplier;
                    }
                }

                double average = 0.0;
                if(count > 0)
                {
                    average = sum / count;
                }

                if (average <= 3)
                {
                    u.PurchaseDecision = "no";
                }
                else
                {
                    u.PurchaseDecision = "yes";
                }

            }
            Console.WriteLine("Writing to file");
            string finalOutputstring = "";
            foreach(User u in allUsers)
            {
                if (string.IsNullOrEmpty(u.Review))
                {
                    finalOutputstring += u.Name + "\t" + "*" + "\t" + u.PurchaseDecision + "\r\n";
                }
                else
                {
                    finalOutputstring += u.Name + "\t" + u.score + "\t" + "*\r\n";
                }
            }

            System.IO.File.WriteAllText("outputfile.txt", finalOutputstring);
            Console.WriteLine("We are done");
            Console.Read();
        }      
    }
}

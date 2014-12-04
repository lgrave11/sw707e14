using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netflix
{
    class Learning
    {
        Dictionary<int, Dictionary<int, UserRating>> trainingData;
        Dictionary<int, double?> movieMean = new Dictionary<int, double?>();
        Dictionary<int, double?> userMean = new Dictionary<int, double?>();
        public int Sum = 0;
        public int N = 0;


        public Learning(Dictionary<int, Dictionary<int, UserRating>> trainingdata) 
        {
            trainingData = trainingdata;
        }
        public void SubtractMeans()
        {

            #region testdata
            /*
             [5 3 4 5 x
              2 2 x 1 x
              3 x 3 3 4
              5 5 5 x 5]
             */
            /*trainingData = new Dictionary<int, Dictionary<int, UserRating>>();
            var d = new Dictionary<int, UserRating>();
            d.Add(1, new UserRating { MovieId = 1, Rating = 5, UserId = 1 });
            d.Add(2, new UserRating { MovieId = 1, Rating = 3, UserId = 2 });
            d.Add(3, new UserRating { MovieId = 1, Rating = 4, UserId = 3 });
            d.Add(4, new UserRating { MovieId = 1, Rating = 5, UserId = 4 });
            trainingData.Add(1, d);

            d = new Dictionary<int, UserRating>();
            d.Add(1, new UserRating { MovieId = 2, Rating = 2, UserId = 1 });
            d.Add(2, new UserRating { MovieId = 2, Rating = 2, UserId = 2 });
            d.Add(4, new UserRating { MovieId = 2, Rating = 1, UserId = 4 });
            trainingData.Add(2, d);

            d = new Dictionary<int, UserRating>();
            d.Add(1, new UserRating { MovieId = 3, Rating = 3, UserId = 1 });
            d.Add(3, new UserRating { MovieId = 3, Rating = 3, UserId = 3 });
            d.Add(4, new UserRating { MovieId = 3, Rating = 3, UserId = 4 });
            d.Add(5, new UserRating { MovieId = 3, Rating = 4, UserId = 5 });
            trainingData.Add(3, d);

            d = new Dictionary<int, UserRating>();
            d.Add(1, new UserRating { MovieId = 4, Rating = 5, UserId = 1 });
            d.Add(2, new UserRating { MovieId = 4, Rating = 5, UserId = 2 });
            d.Add(3, new UserRating { MovieId = 4, Rating = 5, UserId = 3 });
            d.Add(5, new UserRating { MovieId = 4, Rating = 5, UserId = 5 });
            trainingData.Add(4, d);*/
            #endregion


            Dictionary<int, List<int?>> tmp = new Dictionary<int, List<int?>>();

            // Calculate movie means
            foreach (KeyValuePair<int, Dictionary<int, UserRating>> item in trainingData)
            {
                var ratings = item.Value.Select(x => x.Value.Rating).ToList();
                movieMean.Add(item.Key, ratings.Average());

                Sum += (int)ratings.Sum();
                N += ratings.Count;
            }

            // Calculate user means
            foreach (KeyValuePair<int, Dictionary<int, UserRating>> item in trainingData)
            {
                
                foreach (KeyValuePair<int, UserRating> itemValue in item.Value)
                {
                    if (!tmp.ContainsKey(itemValue.Key)) 
                    {
                        tmp.Add(itemValue.Key, new List<int?>());
                        
                    }
                    tmp[itemValue.Key].Add(itemValue.Value.Rating);
                }
            }

            foreach (var v in tmp) 
            {
                userMean.Add(v.Key, v.Value.Average());
            }


            // Remove obvious structures
            foreach (KeyValuePair<int, Dictionary<int, UserRating>> item in trainingData)
            {
                foreach (KeyValuePair<int, UserRating> itemValue in item.Value)
                {
                    itemValue.Value.RatingFixed = itemValue.Value.Rating - movieMean[item.Key] - userMean[itemValue.Key] + (Convert.ToDouble(Sum) / N);
                    itemValue.Value.Structure = movieMean[item.Key] + userMean[itemValue.Key] - (Convert.ToDouble(Sum) / N);
                }
            }

            // Check if mean is 0
            double ratingSum = 0;
            int ratingCount = 0;
            foreach (KeyValuePair<int, Dictionary<int, UserRating>> item in trainingData)
            {
                foreach (KeyValuePair<int, UserRating> itemValue in item.Value)
                {
                    ratingSum += itemValue.Value.RatingFixed.Value;
                    ratingCount++;
                }
            }
            double ratingSumAvg = ratingSum / Convert.ToDouble(ratingCount);
            Console.WriteLine("ratingSumAvg: " + ratingSumAvg.ToString());
        }

        public void CalcRMUHat()
        {
            Random r = new Random();
            int k = 25;
            int epochs = 1000000;
            //double regularizer = 0.02;
            double learningRate = 0.001;
            Dictionary<int, Dictionary<int, double>> A = new Dictionary<int,Dictionary<int,double>>();
            Dictionary<int, Dictionary<int, double>> B = new Dictionary<int,Dictionary<int,double>>();
            double MAXIMUM = 0.5;
            double MINIMUM = -0.5;
            foreach (int i in movieMean.Keys)
            {
                for (int kVal = 0; kVal < k; kVal++)
                {
                    if (!A.ContainsKey(i)) 
                    {
                        A.Add(i, new Dictionary<int, double>());
                    }

                    A[i].Add(kVal, r.NextDouble() * (MAXIMUM - MINIMUM) + MINIMUM);//0.1;
                }
            }

            for (int kVal = 0; kVal < k; kVal++)
            {
                foreach(int j in userMean.Keys)
                {
                    if (!B.ContainsKey(kVal)) 
                    {
                        B.Add(kVal, new Dictionary<int, double>());
                    }
                    B[kVal].Add(j, r.NextDouble() * (MAXIMUM - MINIMUM) + MINIMUM);//;//0.1;
                }
            }

            int count = 0;
            int epoch = 0;
            List<int> movieKeys = trainingData.Keys.ToList();
            double prev_error = 0;
            while (epoch < epochs)
            {
                int movieId = movieKeys.ElementAt(r.Next(movieKeys.Count));
                int userId = trainingData[movieId].Keys.ElementAt(r.Next(trainingData[movieId].Keys.Count));
                for (int kVal = 0; kVal < k; kVal++)
                {
                    double rating = (double)trainingData[movieId][userId].RatingFixed;
                    double currRating = 0.0;
                    for (int i = 0; i < k; i++)
                    {
                        currRating += A[movieId][i] * B[i][userId];
                    }
                    A[movieId][kVal] += learningRate * (rating - currRating) * B[kVal][userId];// -(regularizer * A[movieId][kVal]);
                    B[kVal][userId] += learningRate * A[movieId][kVal] * (rating - currRating);// -(regularizer * B[kVal][userId]);
                }
                
                if (epoch % 100000 == 0)
                {
                    double error = 0.0;
                    foreach (var m in trainingData.Keys)
                    {
                        foreach (var u in trainingData[m].Keys)
                        {
                            double ABval = 0;
                            for (int i = 0; i < k; i++)
                            {
                                ABval += A[m][i] * B[i][u];
                            }
                            error += Math.Pow((double)trainingData[m][u].RatingFixed - ABval, 2);
                        }
                    }
                    if (epoch == 0) 
                    {
                        prev_error = error;
                        epoch++;
                        continue;
                    }
                    Console.WriteLine("{0:D8} - {1:0.00} - {2:0.00}", epoch, error, prev_error);
                    if (Math.Abs(error) > Math.Abs(prev_error))  break;
                    //if (count > 100) break;
                    prev_error = error;
                    //Console.WriteLine(epoch);
                }
                epoch++;
            }

            Dictionary<int, UserRating> userRatings = new Dictionary<int, UserRating>();

            foreach (KeyValuePair<int, Dictionary<int, UserRating>> item in trainingData)
            {
                foreach (KeyValuePair<int, UserRating> itemValue in item.Value)
                {
                    if (!userRatings.ContainsKey(itemValue.Key))
                        userRatings.Add(itemValue.Key, itemValue.Value);
                }
            }

            foreach (int mId in movieMean.Keys)
            {
                Console.WriteLine(mId);
                foreach (int uId in userMean.Keys)
                {
                    for (int j = 0; j < k; j++)
                    {

                        if (!trainingData[mId].ContainsKey(uId)) 
                        {
                            UserRating ur = new UserRating { MovieId = mId, UserId = uId, Rating = 0, RatingFixed = 0 };
                            trainingData[mId].Add(uId, ur);
                        }
                        trainingData[mId][uId].RMUHat += A[mId][j] * B[j][uId];

                        
                        
                    }
                    trainingData[mId][uId].RMUHat += movieMean[mId] + userMean[uId] - (Convert.ToDouble(Sum) / N);
                    trainingData[mId][uId].RMUHat = trainingData[mId][uId].RMUHat > 5 ? 5 : trainingData[mId][uId].RMUHat < 1 ? 1 : trainingData[mId][uId].RMUHat;
                }
            }
        }
    }
}

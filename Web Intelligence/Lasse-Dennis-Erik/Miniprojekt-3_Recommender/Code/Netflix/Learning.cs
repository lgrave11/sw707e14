using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netflix
{
    class Learning
    {
        public Dictionary<int, int> MovieMapper = new Dictionary<int,int>();
        public Dictionary<int, int> UserMapper = new Dictionary<int,int>();
        public Dictionary<int, Dictionary<int, UserRating>> trainingData;
        public Dictionary<int, double?> movieMean = new Dictionary<int, double?>();
        public Dictionary<int, double?> userMean = new Dictionary<int, double?>();
        public int Sum = 0;
        public int N;

        public Learning(Dictionary<int, Dictionary<int, UserRating>> trainingdata) 
        {
            trainingData = trainingdata;
        }

        

        public void SubtractMeans()
        {

            #region fuckoff
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

        public void AssignMappers()
        {
            int i = 0;
            foreach (int movieid in movieMean.Keys)
            {
                MovieMapper[movieid] = i;
                i++;
            }
            i = 0;
            foreach (int userid in userMean.Keys)
            {
                UserMapper[userid] = i;
                i++;
            }
        }

        public void CalcRMUHat()
        {
            Random r = new Random();
            int k = 25;
            int epochs = 10000000;
            double regularizer = 0.02;
            double learningRate = 0.001;
            double[,] Aarray = new double[movieMean.Keys.Count, k];
            double[,] Barray = new double[k, userMean.Keys.Count];
            double MAXIMUM = 0.5;
            double MINIMUM = -0.5;

            List<int> movieKeys = movieMean.Keys.ToList();
            for(int i = 0; i < movieMean.Keys.Count; i++)
            {
                for (int kVal = 0; kVal < k; kVal++)
                {
                    Aarray[i, kVal] = 0.1;//r.NextDouble() * (MAXIMUM - MINIMUM) + MINIMUM;
                }
            }

            List<int> userKeys = userMean.Keys.ToList();
            for (int kVal = 0; kVal < k; kVal++)
            {
                for (int j = 0; j < userMean.Keys.Count; j++)
                {
                    Barray[kVal, j] = 0.1;// r.NextDouble() * (MAXIMUM - MINIMUM) + MINIMUM;
                }
            }

            int count = 0;
            int epoch = 0;
            movieKeys = trainingData.Keys.ToList();
            double prev_error = 0;
            while (epoch < epochs)
            {
                int movieId = movieKeys.ElementAt(r.Next(movieKeys.Count));
                int mappedMovieId = MovieMapper[movieId];

                int userId = trainingData[movieId].Keys.ElementAt(r.Next(trainingData[movieId].Keys.Count));
                int mappedUserId = UserMapper[userId];
                for (int kVal = 0; kVal < k; kVal++)
                {
                    double rating = (double)trainingData[movieId][userId].RatingFixed;
                    double currRating = 0.0;
                    for (int i = 0; i < k; i++)
                    {
                        currRating += Aarray[mappedMovieId, i] * Barray[i, mappedUserId];
                    }
                    Aarray[mappedMovieId, kVal] += learningRate * (rating - currRating) * Barray[kVal, mappedUserId];// - (regularizer * Aarray[mappedMovieId, kVal]);
                    Barray[kVal, mappedUserId] += learningRate * Aarray[mappedMovieId, kVal] * (rating - currRating);// -(regularizer * Barray[kVal, mappedUserId]);
                }
                
                if (epoch % 100000 == 0)
                {
                    double error = 0.0;
                    foreach (var m in trainingData.Keys)
                    {
                        int mappedM = MovieMapper[m];
                        foreach (var u in trainingData[m].Keys)
                        {
                            int mappedU = UserMapper[u];
                            double ABval = 0;
                            for (int i = 0; i < k; i++)
                            {
                                ABval += Aarray[mappedM, i] * Barray[i, mappedU];
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
                    //if (Math.Abs(error) > Math.Abs(prev_error))  break;
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

            foreach (var mId in movieMean.Keys)
            {
                Console.WriteLine(mId);
                int mappedmId = MovieMapper[mId];
                foreach (var uId in userMean.Keys)
                {
                    int mappeduId = UserMapper[uId];
                    for (int j = 0; j < k; j++)
                    {

                        if (!trainingData[mId].ContainsKey(uId))
                        {
                            UserRating ur = new UserRating { MovieId = mId, UserId = uId, Rating = 0, RatingFixed = 0 };
                            trainingData[mId].Add(uId, ur);
                        }
                        trainingData[mId][uId].RMUHat += Aarray[mappedmId, j] * Barray[j, mappeduId];



                    }
                    trainingData[mId][uId].RMUHat += movieMean[mId] + userMean[uId] - (Convert.ToDouble(Sum) / N);
                    trainingData[mId][uId].RMUHat = trainingData[mId][uId].RMUHat > 5 ? 5 : trainingData[mId][uId].RMUHat < 1 ? 1 : trainingData[mId][uId].RMUHat;
                }
            }
        }
    }
}

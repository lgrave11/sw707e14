using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netflix
{
    class Learning
    {
        public void SubtractMeans(Dictionary<int, Dictionary<int, UserRating>> trainingData)
        {

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

            Dictionary<int, double?> movieMean = new Dictionary<int, double?>();
            Dictionary<int, double?> userMean = new Dictionary<int, double?>();
            Dictionary<int, List<int?>> tmp = new Dictionary<int, List<int?>>();
            int N = 0;
            int? sum = 0;

            // Calculate movie means
            foreach (KeyValuePair<int, Dictionary<int, UserRating>> item in trainingData)
            {
                var ratings = item.Value.Select(x => x.Value.Rating).ToList();
                movieMean.Add(item.Key, ratings.Average());

                sum += ratings.Sum();
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
                    itemValue.Value.RatingFixed = itemValue.Value.Rating - movieMean[item.Key] - userMean[itemValue.Key] + (Convert.ToDouble(sum.Value) / N);
                    itemValue.Value.Structure = movieMean[item.Key] + userMean[itemValue.Key] - (Convert.ToDouble(sum.Value) / N);
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

            CalcRMUHat(userMean, movieMean, trainingData, sum, N);
            double RMSE = 0.0;
            int n = 0;
            foreach (var v in trainingData.Values) 
            {
                foreach (var l in v) 
                {
                    if (l.Value.Rating != null && l.Value.Rating != 0) 
                    {
                        RMSE += Math.Pow(Convert.ToDouble((l.Value.RMUHat - l.Value.Rating)), 2);
                        n++;
                    }
                }
            }
            Console.WriteLine(Math.Sqrt(RMSE / n));
        }

        private Dictionary<int, int> CreateUserMapper(Dictionary<int, double?> userMean)
        {
            int i = 0;
            Dictionary<int, int> userMapping = new Dictionary<int, int>();
            foreach (KeyValuePair<int, double?> item in userMean)
            {
                userMapping.Add(item.Key, i);
                i++;
            }
            return userMapping;
        }

        private Dictionary<int, int> CreateMovieMapper(Dictionary<int, double?> movieMean)
        {
            int i = 0;
            Dictionary<int, int> movieMapping = new Dictionary<int, int>();
            foreach (KeyValuePair<int, double?> item in movieMean)
            {
                movieMapping.Add(item.Key, i);
                i++;
            }
            return movieMapping;
        }

        private void CalcRMUHat(Dictionary<int, double?> userMean, Dictionary<int, double?> movieMean, Dictionary<int, Dictionary<int, UserRating>> trainingData, int? sum, int N)
        {
            int k = 28;
            double n = 0.001;
            double[,] A = new double[movieMean.Count, k];
            for (int i = 0; i < movieMean.Count; i++)
            {
                for (int j = 0; j < k; j++)
                {
                    A[i, j] = 0.1;
                }
            }

            double[,] B = new double[k, userMean.Count];
            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < userMean.Count; j++)
                {
                    B[i, j] = 0.1;
                }
            }

            Dictionary<int, int> userMapper = CreateUserMapper(userMean);
            Dictionary<int, int> movieMapper = CreateMovieMapper(movieMean);

            /*double[,] test = new double[movieMean.Count,userMean.Count];
             foreach (var i in trainingData) 
            {
                foreach (var j in i) 
                {
                    test[i,j] = j
                }
            }*/

            int traversals = 0;
            while (traversals < 10000)
            {
                Random r = new Random();

                int posMovie = r.Next(0, movieMean.Keys.Count);
                int movieID = trainingData.Keys.ToArray()[posMovie];
                int posUser = r.Next(0, trainingData[movieID].Keys.Count);
                int userID = trainingData[movieID].Keys.ToArray()[posUser];

                double? innerparanthesis = 0.0;
                innerparanthesis = trainingData[movieID][userID].RatingFixed;
                for (int i = 0; i < k; i++)
                {
                    innerparanthesis -= A[movieMapper[movieID], i] * B[i, userMapper[userID]];
                }
                double[,] oldA = A, oldB = B;
                for (int j = 0; j < k; j++)
                {
                    A[movieMapper[movieID], j] = oldA[movieMapper[movieID], j] + n * innerparanthesis.Value * oldB[j, userMapper[userID]];
                    B[j, userMapper[userID]] = oldB[j, userMapper[userID]] + n * oldA[movieMapper[movieID], j] * innerparanthesis.Value;
                }

                if (traversals % 100 == 0)
                    Console.WriteLine(traversals);
                traversals++;
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

            foreach (int movieID in movieMean.Keys)
            {
                Console.WriteLine(movieID);
                foreach (int userID in userMean.Keys)
                {
                    for (int j = 0; j < k; j++)
                    {

                        if (!trainingData[movieID].ContainsKey(userID)) 
                        {
                            UserRating ur = new UserRating { MovieId = movieID, UserId = userID, Rating = 0, RatingFixed = 0 };
                            trainingData[movieID].Add(userID, ur);
                        }
                        trainingData[movieID][userID].RMUHat += A[movieMapper[movieID], j] * B[j, userMapper[userID]];

                        
                        
                    }
                    trainingData[movieID][userID].RMUHat += movieMean[movieID] + userMean[userID] - (Convert.ToDouble(sum.Value) / N);
                    trainingData[movieID][userID].RMUHat = trainingData[movieID][userID].RMUHat > 5 ? 5 : trainingData[movieID][userID].RMUHat < 1 ? 1 : (double?)Math.Round((decimal)trainingData[movieID][userID].RMUHat);
                }
            }
        }
    }
}

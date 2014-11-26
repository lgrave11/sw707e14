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
            Dictionary<int, double?> movieMean = new Dictionary<int, double?>();
            Dictionary<int, double?> userMean = new Dictionary<int, double?>();
            Dictionary<int, List<int?>> tmp = new Dictionary<int, List<int?>>();
            int N = 0;
            int? sum = 0;

            foreach (KeyValuePair<int, Dictionary<int, UserRating>> item in trainingData)
            {
                var ratings = item.Value.Select(x => x.Value.Rating).ToList();
                movieMean.Add(item.Key, ratings.Average());

                sum += ratings.Sum();
                N += ratings.Count;
            }
            foreach (KeyValuePair<int, Dictionary<int, UserRating>> item in trainingData)
            {
                
                foreach (KeyValuePair<int, UserRating> itemValue in item.Value)
                {
                    if (!tmp.ContainsKey(itemValue.Key)) 
                    {
                        tmp.Add(itemValue.Key, new List<int?>());
                        tmp[itemValue.Key].Add(itemValue.Value.Rating);

                    }
                }
            }


            foreach (var v in tmp) 
            {
                userMean.Add(v.Key, v.Value.Average());
            }

            foreach (KeyValuePair<int, Dictionary<int, UserRating>> item in trainingData)
            {
                foreach (KeyValuePair<int, UserRating> itemValue in item.Value)
                {
                    itemValue.Value.RatingFixed = itemValue.Value.Rating - movieMean[item.Key] - userMean[itemValue.Key] + (Convert.ToDouble(sum.Value) / N);
                    itemValue.Value.Structure = movieMean[item.Key] + userMean[itemValue.Key] - (Convert.ToDouble(sum.Value) / N);
                }
            }


        }
    }
}

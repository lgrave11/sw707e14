using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Netflix
{
    class DataLoader
    {
        public DataLoader()
        {

        }

        public Dictionary<int, Dictionary<int, UserRating>> LoadProbeData(string path)
        {
            Dictionary<int, Dictionary<int, UserRating>> probeData = new Dictionary<int, Dictionary<int, UserRating>>();
            
            StreamReader reader = new StreamReader(path);
            int movieId = 0;
            while (reader.Peek() > -1)
            {
                string line = reader.ReadLine();
                
                if (line.Contains(':'))
                {
                    movieId = Convert.ToInt32(line.Substring(0, line.Length - 1));
                    probeData.Add(movieId, new Dictionary<int, UserRating>());
                }
                else
                {
                    int userId = Convert.ToInt32(line);
                    probeData[movieId].Add(userId, new UserRating() { UserId = userId, MovieId = movieId, Rating = null });
                }
            }

            return probeData;
        }

        public Dictionary<int, Dictionary<int, UserRating>> LoadTrainingData(string path, Dictionary<int, Dictionary<int, UserRating>> probeData)
        {
            Dictionary<int, Dictionary<int, UserRating>> trainingData = new Dictionary<int, Dictionary<int, UserRating>>();
            string[] files = Directory.GetFiles(path);
            int i = 0;
            foreach(int item in probeData.Keys.Take(10000))
            {
                if (i % 250 == 0)
                {
                    Console.WriteLine(i.ToString());
                }
                i++;
                StreamReader reader = new StreamReader("training_set/mv_" + item.ToString("D7") + ".txt");
                //Console.WriteLine("Reading file mv_" + item.ToString("D7"));
                int movieId = 0;
                bool j = true;
                while (reader.Peek() > -1)
                {
                    string line = reader.ReadLine();
                    
                    if (j)
                    {
                        movieId = Convert.ToInt32(line.Substring(0, line.Length - 1));
                        trainingData.Add(movieId, new Dictionary<int, UserRating>());
                        j = false;
                    }
                    else
                    {
                        string[] splitLine = line.Split(',');
                        int userId = Convert.ToInt32(splitLine[0]);
                        int rating = Convert.ToInt32(splitLine[1]);
                        trainingData[movieId].Add(userId, new UserRating() { UserId = userId, Rating = rating, MovieId = movieId });
                    }
                }
            }
            Console.WriteLine("Manipulating data");
            ManipulateData(trainingData, probeData);

            return trainingData;
        }

        private void ManipulateData(Dictionary<int, Dictionary<int, UserRating>> trainingData, Dictionary<int, Dictionary<int, UserRating>> probeData)
        {
            foreach (KeyValuePair<int, Dictionary<int, UserRating>> item in trainingData)
            {
                foreach (KeyValuePair<int, UserRating> itemValue in item.Value)
                {
                    if (probeData[item.Key].ContainsKey(itemValue.Key))
                        probeData[item.Key][itemValue.Key].Rating = itemValue.Value.Rating;
                }
            }

            foreach (KeyValuePair<int, Dictionary<int, UserRating>> item in probeData)
            {
                foreach (KeyValuePair<int, UserRating> itemValue in item.Value)
                {
                    if (trainingData.ContainsKey(item.Key))
                    {
                        trainingData[item.Key].Remove(itemValue.Key);
                    }
                }
            }
        }
    }
}

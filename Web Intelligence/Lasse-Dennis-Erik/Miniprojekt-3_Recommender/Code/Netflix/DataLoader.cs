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
            DirectoryInfo di = new DirectoryInfo(path);
            List<FileInfo> fi = new List<FileInfo>();
            string[] files = new string[500];
            foreach (FileInfo fileInfo in di.GetFiles())
            {
                if (fileInfo.Length > 30000 && fileInfo.Length < 50000)
                    fi.Add(fileInfo);
            }
            
            int i = 0;

            foreach(FileInfo item in fi)
            {
                if (!probeData.ContainsKey(Convert.ToInt32(item.Name.Remove(item.Name.Length - 4).Replace("mv_", string.Empty))))
                {
                    continue;
                }
                if (i % 250 == 0)
                {
                    Console.WriteLine(i.ToString());
                }
                i++;
                if (i == 50)
                    break;
                
                StreamReader reader = new StreamReader("training_set/" + item.Name);
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
            
            return trainingData;
        }

        public Tuple<Dictionary<int, Dictionary<int, UserRating>>, Dictionary<int, Dictionary<int, UserRating>>> ManipulateData(Dictionary<int, Dictionary<int, UserRating>> trainingData, Dictionary<int, Dictionary<int, UserRating>> probeData)
        {
            Dictionary<int, Dictionary<int, UserRating>> newProbeData = new Dictionary<int, Dictionary<int, UserRating>>();
            foreach (KeyValuePair<int, Dictionary<int, UserRating>> item in trainingData)
            {
                foreach (KeyValuePair<int, UserRating> itemValue in item.Value)
                {
                    if (probeData[item.Key].ContainsKey(itemValue.Key))
                    {
                        probeData[item.Key][itemValue.Key].Rating = itemValue.Value.Rating;
                        if (!newProbeData.ContainsKey(item.Key)) 
                        {
                            newProbeData.Add(item.Key, new Dictionary<int, UserRating>());
                        }
                        newProbeData[item.Key].Add(itemValue.Key, probeData[item.Key][itemValue.Key]);
                    }
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

            return new Tuple<Dictionary<int, Dictionary<int, UserRating>>, Dictionary<int, Dictionary<int, UserRating>>>(trainingData, newProbeData);
        }
    }
}

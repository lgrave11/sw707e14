using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace MiniProject3
{
    class Recommender
    {
        Dictionary<int, Dictionary<int, int>> dicProbe = new Dictionary<int, Dictionary<int, int>>(); //movieid --> userid --> rating
        Dictionary<int, Dictionary<int, int>> dicTraining = new Dictionary<int, Dictionary<int, int>>(); //movieid --> userid --> rating

        public Dictionary<int, double> dicMovieMean = new Dictionary<int, double>();
        public Dictionary<int, double> dicUserMean = new Dictionary<int, double>();

        public Dictionary<int, int> userMapper = new Dictionary<int, int>();
        public Dictionary<int, int> movieMapper = new Dictionary<int, int>();
        public double overallMean = 0.0;

        Dictionary<int, Dictionary<int, double>> dicRMU = new Dictionary<int, Dictionary<int, double>>();

        public double[,] RMUHat;
        public Recommender()
        {
           // FillProbeDictionary(dicProbe);
            
            FillTrainingDictionary();
            Console.WriteLine("fill training done");
            CalcMean();
            Console.WriteLine("calc mean done");
            CalcRMU();
            Console.WriteLine("calc RMU done");
            AssignMappers();
            Console.WriteLine("mappers assigned");
        }

        public void updateRMUHat(int k){
            RMUHat = CalcRMUHat(k);
            Console.WriteLine("RmuHat done");
        }

        private void AssignMappers()
        {
            int i = 0;
            foreach(int movieid in dicMovieMean.Keys)
            {
                movieMapper[movieid] = i;
                i++;
            }

            i = 0;
            foreach(int userid in dicUserMean.Keys)
            {
                userMapper[userid] = i;
                i++;
            }

        }

        private double[,] CalcRMUHat(int k)
        {
            double n = 0.001;
            double[,] A = new double[dicMovieMean.Count, k];
            double[,] B = new double[k, dicUserMean.Count];

            InitAandB(k, A, B);

            Random r = new Random();
            int traversals = 0;
            while(traversals < 10000)
            {
                if(traversals % 1000 == 0)
                {
                    Console.WriteLine(traversals);
                }
                int posMovie = r.Next(0, dicMovieMean.Keys.Count);
                int movieID = dicRMU.Keys.ToArray()[posMovie];
                int posUser = r.Next(0, dicRMU[movieID].Keys.Count);
                int userID = dicRMU[movieID].Keys.ToArray()[posUser];

                double[,] oldA = (double[,])A.Clone(), oldB = (double[,])B.Clone();
                for (int j = 0; j < k; j++)
                {
                    userID = dicRMU[movieID].Keys.ToArray()[posUser];

                    double innerparanthesis = 0.0;
                    innerparanthesis = dicRMU[movieID][userID];
                    for (int i = 0; i < k; i++)
                    {
                        innerparanthesis -= A[movieMapper[movieID], i] * B[i, userMapper[userID]];
                    }

                    A[movieMapper[movieID], j] = oldA[movieMapper[movieID], j] + n * innerparanthesis * oldB[j, userMapper[userID]];
                    B[j, userMapper[userID]] = oldB[j, userMapper[userID]] + n * oldA[movieMapper[movieID], j] * innerparanthesis;
                }

                traversals++;
            }
            double[,] rmuhat = new double[dicMovieMean.Keys.Count, dicUserMean.Keys.Count];

            foreach(int movieid in dicMovieMean.Keys)
            {
                foreach(int userid in dicUserMean.Keys)
                {
                    for(int j = 0; j < k; j++)
                    {
                        rmuhat[movieMapper[movieid], userMapper[userid]] += A[movieMapper[movieid], j] * B[j, userMapper[userid]];
                    }
                    
                }
            }

            return rmuhat;

        }

        private void InitAandB(int k, double[,] A, double[,] B)
        {
            Random ABRandom = new Random();
            for (int j = 0; j < k; j++)
            {
                for (int i = 0; i < dicMovieMean.Count; i++)
                {
                    A[i, j] = ABRandom.NextDouble() - 0.5;
                }
                for (int i = 0; i < dicUserMean.Count; i++)
                {
                    B[j, i] = ABRandom.NextDouble() - 0.5;
                }
            }
        }

        private void CalcRMU()
        {
            foreach (int movieid in dicMovieMean.Keys)
            {
                if (!dicRMU.ContainsKey(movieid))
                    dicRMU[movieid] = new Dictionary<int, double>();
                foreach(int userid in dicUserMean.Keys)
                {
                    if (dicTraining[movieid].ContainsKey(userid))
                    {
                        double res = dicTraining[movieid][userid];
                        res -= dicMovieMean[movieid];
                        res -= dicUserMean[userid];
                        res += overallMean;
                        dicRMU[movieid][userid] = res;
                    }
                }
            }
        }

        private void CalcMean()
        {
            Dictionary<int, KeyValuePair<double, double>> tempUserMeanDic = new Dictionary<int, KeyValuePair<double, double>>();
            double count = 0;
            double ratingSum = 0;
            foreach(int movieid in dicTraining.Keys)
            {
                double countMovie = 0;
                double ratingSumMovie = 0;
                foreach(int userid in dicTraining[movieid].Keys)
                {
                    if(!tempUserMeanDic.ContainsKey(userid))
                    {
                        tempUserMeanDic[userid] = new KeyValuePair<double, double>(0.0, 0.0);
                    }
                    KeyValuePair<double, double> tempKeyValPair = new KeyValuePair<double, double>(tempUserMeanDic[userid].Key + 1, 
                                                                                                   tempUserMeanDic[userid].Value + dicTraining[movieid][userid]);

                    tempUserMeanDic[userid] = tempKeyValPair;
                    countMovie++;
                    ratingSumMovie += dicTraining[movieid][userid];
                    count++;
                    ratingSum += dicTraining[movieid][userid];
                }
                dicMovieMean[movieid] = ratingSumMovie / countMovie;
            }
            overallMean = ratingSum / count;

            foreach(int userid in tempUserMeanDic.Keys)
            {
                dicUserMean[userid] = tempUserMeanDic[userid].Value / tempUserMeanDic[userid].Key;
            }
        }
        private void FillTrainingDictionary()
        {
            string path = "C:\\Users\\Praetorian\\Documents\\p7\\download\\training_set_subset\\";
            foreach(string filePath in Directory.GetFiles(path))
            {
                string[] lines = File.ReadAllLines(filePath);
                int movieID = Convert.ToInt32(lines[0].Split(':')[0]);
                dicTraining[movieID] = new Dictionary<int, int>();

                for(int i = 1; i < lines.Count(); i++)
                {
                    var splitted = lines[i].Split(',');
                    int userid = Convert.ToInt32(splitted[0]);
                    int rating = Convert.ToInt32(splitted[1]);

                    dicTraining[movieID][userid] = rating;
                }

            }
        }
        private static void FillProbeDictionary(Dictionary<int, Dictionary<int, int>> dicProbe)
        {
            int currentMovieID = 0;

            StreamReader probeFile = new StreamReader("C:\\Users\\Praetorian\\Documents\\p7\\download\\newprobe.txt");
            string line = "";
            int count = 0;
            while ((line = probeFile.ReadLine()) != null)
            {
                if (line.Contains(':'))
                {
                    count++;
                    if (count > 1000)
                        break;
                    currentMovieID = Convert.ToInt32(line.Split(':')[0].Trim());
                    dicProbe[currentMovieID] = new Dictionary<int, int>();
                }
                else
                {
                    int itemToAdd = Convert.ToInt32(line.Trim().Split(',')[0]);
                    int rating = Convert.ToInt32(line.Trim().Split(',')[1]);
                    dicProbe[currentMovieID][itemToAdd] = rating;
                }
            }
        }

    }
}

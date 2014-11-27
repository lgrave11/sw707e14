using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MiniProject3
{
    class Loader
    {

        //public Dictionary<int, Dictionary<int, int>> dicMovieUserRating = new Dictionary<int, Dictionary<int, int>>();

        public Loader()
        {
            createFiles();
        }

        private void createFiles()
        {
            Dictionary<int, Dictionary<int, int>> dicProbe = new Dictionary<int, Dictionary<int, int>>(); //movie key --> dic user, rating pairs
            Dictionary<int, Dictionary<int, int>> dicProbeClone = new Dictionary<int, Dictionary<int, int>>(); //movie key --> list user, rating pairs
            FillProbeDictionary(dicProbe);
            FillProbeDictionary(dicProbeClone);

            CreateTrainingSubset(dicProbe, dicProbeClone);

            string newprobepath = "C:\\Users\\Praetorian\\Documents\\p7\\download\\newprobe.txt";

            StreamWriter sw = new StreamWriter(newprobepath);
            foreach(int movieId in dicProbe.Keys)
            {
                sw.WriteLine(movieId + ":");
                foreach(KeyValuePair<int,int> kvPair in dicProbe[movieId])
                {
                    sw.WriteLine(kvPair.Key + "," + dicProbe[movieId][kvPair.Key]);
                }
            }
        }

        private static void CreateTrainingSubset(Dictionary<int, Dictionary<int, int>> dicProbe, Dictionary<int, Dictionary<int, int>> dicProbeClone)
        {
            List<Task> tasks = new List<Task>();
            string outputPath = "C:\\Users\\Praetorian\\Documents\\p7\\download\\training_set_subset\\";
            if (Directory.Exists(outputPath))
                Directory.Delete(outputPath, true);
            Directory.CreateDirectory(outputPath);

            foreach (int movieId in dicProbeClone.Keys)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                    {
                        string trainingSetPath = "C:\\Users\\Praetorian\\Documents\\p7\\download\\training_set\\";
                        string fileNameTrainingSet = "mv_" + movieId.ToString("0000000") + ".txt";
                        string finalPath = trainingSetPath + fileNameTrainingSet;

                        List<string> lines = File.ReadAllLines(finalPath).ToList();
                        foreach (KeyValuePair<int, int> kvPair in dicProbeClone[movieId])
                        {
                            string desiredLine = lines.First(l => l.Trim().StartsWith(kvPair.Key + ","));
                            int rating = Convert.ToInt32(desiredLine.Trim().Split(',')[1]);
                            dicProbe[movieId][kvPair.Key] = rating;
                            lines.Remove(desiredLine);
                        }

                        File.WriteAllLines(outputPath + fileNameTrainingSet, lines);
                    }));

            }

            Console.WriteLine("Waiting for all");

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("All is done");
        }

        private static void FillProbeDictionary(Dictionary<int, Dictionary<int, int>> dicProbe)
        {
            int currentMovieID = 0;

            StreamReader probeFile = new StreamReader("C:\\Users\\Praetorian\\Documents\\p7\\download\\probe.txt");
            string line = "";
            int count = 0;
            while ((line = probeFile.ReadLine()) != null)
            {
                if (line.Contains(':'))
                {
                    count++;
                    if (count > 100)
                        break;
                    currentMovieID = Convert.ToInt32(line.Split(':')[0].Trim());
                    dicProbe[currentMovieID] = new Dictionary<int, int>();
                }
                else
                {
                    int itemToAdd = Convert.ToInt32(line.Split(',')[0].Trim());
                    dicProbe[currentMovieID][itemToAdd] = -1;
                }
            }
        }
    }
}

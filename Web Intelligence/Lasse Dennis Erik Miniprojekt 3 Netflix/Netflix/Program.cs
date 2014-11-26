using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netflix
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, Dictionary<int, UserRating>> probeData;
            Dictionary<int, Dictionary<int, UserRating>> trainingData;
            DataLoader dataLoader = new DataLoader();
            bool saveData = false;
            //if ((probeData = Binary.LoadProbeData()) == null)
           // {
                probeData = dataLoader.LoadProbeData("probe.txt");
            //    saveData = true;
           // }

            //if ((trainingData = Binary.LoadTrainingData()) == null)
            //{
                trainingData = dataLoader.LoadTrainingData("training_set", probeData);
            //    saveData = true;
            //}
                
            if (saveData)
                Binary.SaveData(probeData, trainingData);
            
            Console.ReadLine();
        }
    }
}

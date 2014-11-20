using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Netflix
{
    class Binary
    {
        // Loading
        public static Dictionary<int, Dictionary<int, UserRating>> LoadProbeData()
        {
            Dictionary<int, Dictionary<int, UserRating>> probeData = null;
            if (File.Exists("probedata.bin"))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream("probedata.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
                probeData = (Dictionary<int, Dictionary<int, UserRating>>)formatter.Deserialize(stream);
                stream.Close();
            }

            return probeData;
        }

        public static Dictionary<int, Dictionary<int, UserRating>> LoadTrainingData(){
            Dictionary<int, Dictionary<int, UserRating>> trainingData = null;
            if (File.Exists("trainingdata.bin"))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream("trainingdata.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
                trainingData = (Dictionary<int, Dictionary<int, UserRating>>)formatter.Deserialize(stream);
                stream.Close();
            }
            return trainingData;
        }

        // Saving

        public static void SaveData(Dictionary<int, Dictionary<int, UserRating>> probeData, Dictionary<int, Dictionary<int, UserRating>> trainingData) 
        {
            if (!File.Exists("probedata.bin")) 
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream("probedata.bin", FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, probeData);
                stream.Close();
            }
            if (!File.Exists("trainingdata.bin")) 
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream("trainingdata.bin", FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, trainingData);
                stream.Close();
            }
        }
    }
}

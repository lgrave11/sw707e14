using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace SentimentClassifier
{
    class Parser
    {
        private string text;
        StreamReader reader;
        public Parser(string path)
        {
            MemoryMappedFile file = MemoryMappedFile.CreateFromFile(path);
            MemoryMappedViewStream fileContent = file.CreateViewStream();
            reader = new StreamReader(fileContent);
        }

        private List<Review> parse()
        {
            List<Review> returnList = new List<Review>();
            List<string> reviewBlocks = new List<string>();
            reviewBlocks.AddRange(text.Split(new string[] { "\n\n" }, StringSplitOptions.None));

            foreach (string block in reviewBlocks)
            {
                string[] splitBlock = block.Split(new string[] { "\n" }, StringSplitOptions.None);
                Review review = new Review()
                {
                    ProductId = splitBlock[0].Substring(18).Trim(),
                    UserId = splitBlock[1].Substring(14).Trim(),
                    ProfileName = splitBlock[2].Substring(19).Trim(),
                    Helpfulness = splitBlock[3].Substring(19).Trim(),
                    Score = Convert.ToSingle(splitBlock[4].Substring(13).Trim()),
                    Time = Convert.ToInt32(splitBlock[5].Substring(12).Trim()),
                    Summary = splitBlock[6].Substring(15).Trim(),
                    Text = splitBlock[7].Substring(12).Trim()
                };
                returnList.Add(review);
            }
            return returnList;
        }

        public Review ReadReview()
        {
            Review review = new Review()
            {
                ProductId = reader.ReadLine().Substring(18).Trim(),
                UserId = reader.ReadLine().Substring(14).Trim(),
                ProfileName = reader.ReadLine().Substring(19).Trim(),
                Helpfulness = reader.ReadLine().Substring(19).Trim(),
                Score = Convert.ToSingle(reader.ReadLine().Substring(13).Trim()),
                Time = Convert.ToInt32(reader.ReadLine().Substring(12).Trim()),
                Summary = reader.ReadLine().Substring(15).Trim(),
                Text = reader.ReadLine().Substring(12).Trim()
            };
            if (reader.Peek() > -1)
                reader.ReadLine();
            return review;
        }

        public List<List<Review>> getDataSets(int numberOfPartitions)
        {
            List<Review> reviews = new List<Review>();
            while (reader.Peek() > -1)
                reviews.Add(ReadReview());

            List<List<Review>> returnList = new List<List<Review>>();
            int partitionSize;
            
            if (numberOfPartitions > 0)
            {
                partitionSize = (int)Math.Ceiling((decimal)reviews.Count / (decimal)numberOfPartitions);
            }
            else
            {
                partitionSize = reviews.Count;
            }

            for (int i = 0; i < numberOfPartitions; i++)
            {
                if (i == numberOfPartitions - 1)
                {
                    returnList.Add(reviews.Skip(partitionSize * i).ToList());
                }
                else 
                {
                    returnList.Add(reviews.Skip(partitionSize * i).Take(partitionSize).ToList());
                }
            }

            return returnList;
        }
    }
}

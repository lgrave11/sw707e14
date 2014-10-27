﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MiniProject2
{
    public class User
    {
        private string _name;
        public string Name { get { return _name; } }
        public List<string> Friends { get; set; }

        public string Summary = "";
        public string Review = "";
        public User(string name, string summary, string review, List<string> friends)
        {
            _name = name;
            Summary = summary;
            Review = review;
            Friends = friends;
        }

        public static List<User> ReadUserFile()
        {
            List<User> userList = new List<User>();
            string fileContent = System.IO.File.ReadAllText("friendships.txt");
            List<string> userBlocks = new List<string>();
            userBlocks.AddRange(fileContent.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None));
            foreach (string block in userBlocks)
            {
                string[] splitBlock = block.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                
                string name = splitBlock[0].Substring(5).Trim();
                List<string> friends = splitBlock[1].Substring(8).Trim().Split('\t').ToList();
                string Summary = splitBlock[2].Substring(8).Trim();
                string Review = splitBlock[3].Substring(7).Trim();
                userList.Add(new User(name, Summary, Review, friends));
            }
            return userList;
        }
    }
}

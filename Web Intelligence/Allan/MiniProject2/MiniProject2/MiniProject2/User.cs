using System;
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
        public string[] Friends { get; set; }

        public string Summary = "";
        public string Review = "";
        
        public User(string name, string summary, string review, string[] friends)
        {
            _name = name;
            Summary = summary;
            Review = review;
            Friends = friends;
        }

        public static List<User> LoadAllUsers()
        {
            string[] lines = System.IO.File.ReadAllLines("friendships.txt");

            List<User> users = new List<User>();

            /*Find users */
            for (int userindex = 0, friendsindex = 1, summaryindex = 2, reviewindex = 3; reviewindex < lines.Count(); userindex += 5, friendsindex+=5, summaryindex+= 5, reviewindex+=5 )
            {
                string username = lines[userindex].Split(' ')[1];
                string summary = lines[summaryindex].Split(' ')[1];
                string review = lines[reviewindex].Split(' ')[1];
                string[] friends = lines[friendsindex].Split(':')[1].Trim().Split('\t');
                users.Add(new User(username, summary, review, friends));
            }

            return users;
        }
    }
}

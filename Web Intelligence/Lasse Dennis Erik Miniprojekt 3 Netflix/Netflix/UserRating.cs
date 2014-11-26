using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netflix
{
    [Serializable]
    class UserRating
    {
        /*public UserRating(int userId, KeyValuePair<int, int> rating)
        {
            UserId = userId;
            Rating.Add(rating.Key, rating.Value);
        }*/

        public int UserId;
        public int MovieId;
        public int? Rating;
        public double? RatingFixed;
        public double? Structure;
    }
}

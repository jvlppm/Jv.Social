using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social.Twitter
{
    static class Extensions
    {
        public static T Create<T>(dynamic obj) where T : class
        {
            if (typeof(T) == typeof(User))
                return new User(obj) as T;
            if (typeof(T) == typeof(Tweet))
                return new Tweet(obj) as T;

            throw new NotImplementedException();
        }
    }
}

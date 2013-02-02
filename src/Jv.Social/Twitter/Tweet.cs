using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social.Twitter
{
    public class Tweet
    {
        dynamic _obj;
        internal Tweet(dynamic tweet)
        {
            _obj = tweet;
        }

        /// <summary>
        /// The string representation of the unique identifier for this Tweet.
        /// </summary>
        public string IdStr { get { return _obj.id_str; } }

        /// <summary>
        /// Nullable. If the represented Tweet is a reply, this field will contain the screen name of the original Tweet's author.
        /// </summary>
        public string InReplyToScreenName { get { return _obj.in_reply_to_screen_name; } }

        /// <summary>
        /// Nullable. If the represented Tweet is a reply, this field will contain the string representation of the original Tweet's ID.
        /// </summary>
        public string InReplyToStatusIdStr { get { return _obj.in_reply_to_status_id_str; } }

        /// <summary>
        /// Nullable. If the represented Tweet is a reply, this field will contain the string representation of the original Tweet's author ID.
        /// </summary>
        public string InReplyToUserIdStr { get { return _obj.in_reply_to_user_id_str; } }

        /// <summary>
        /// Number of times this Tweet has been retweeted.
        /// </summary>
        public int RetweetCount { get { return _obj.retweet_count; } }

        /// <summary>
        /// Perspectival. Indicates whether this Tweet has been retweeted by the authenticating user.
        /// </summary>
        public bool Retweeted { get { return _obj.retweeted; } }

        /// <summary>
        /// The actual UTF-8 text of the status update.
        /// </summary>
        public string Text { get { return _obj.text; } }

        /// <summary>
        /// The user who posted this Tweet. Perspectival attributes embedded within this object are unreliable.
        /// </summary>
        public User User { get { return _obj.user == null? null : new User(_obj.user); } }
    }
}

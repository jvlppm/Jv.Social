using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social.Twitter
{
    public class Tweet : Object
    {
        internal Tweet(dynamic tweet)
            : base((object)tweet)
        {
        }

        /// <summary>
        /// The string representation of the unique identifier for this Tweet.
        /// </summary>
        public string IdStr { get { return Object.id_str; } }

        /// <summary>
        /// Nullable. If the represented Tweet is a reply, this field will contain the screen name of the original Tweet's author.
        /// </summary>
        public string InReplyToScreenName { get { return Object.in_reply_to_screen_name; } }

        /// <summary>
        /// Nullable. If the represented Tweet is a reply, this field will contain the string representation of the original Tweet's ID.
        /// </summary>
        public string InReplyToStatusIdStr { get { return Object.in_reply_to_status_id_str; } }

        /// <summary>
        /// Nullable. If the represented Tweet is a reply, this field will contain the string representation of the original Tweet's author ID.
        /// </summary>
        public string InReplyToUserIdStr { get { return Object.in_reply_to_user_id_str; } }

        /// <summary>
        /// Number of times this Tweet has been retweeted.
        /// </summary>
        public int RetweetCount { get { return Object.retweet_count; } }

        /// <summary>
        /// Perspectival. Indicates whether this Tweet has been retweeted by the authenticating user.
        /// </summary>
        public bool Retweeted { get { return Object.retweeted; } }

        /// <summary>
        /// The actual UTF-8 text of the status update.
        /// </summary>
        public string Text { get { return Object.text; } }

        /// <summary>
        /// The user who posted this Tweet. Perspectival attributes embedded within this object are unreliable.
        /// </summary>
        public User User { get { return Object.user == null ? null : new User(Object.user); } }
    }
}

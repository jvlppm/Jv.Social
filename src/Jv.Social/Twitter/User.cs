using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jv.Social.Twitter
{
    public class User : Object
    {
        internal User(dynamic user)
            : base((object)user)
        {
        }

        /// <summary>
        /// The UTC datetime that the user account was created on Twitter.
        /// </summary>
        public string CreatedAt { get { return Object.created_at; } }

        /// <summary>
        /// Nullable. The user-defined UTF-8 string describing their account.
        /// </summary>
        public string Description { get { return Object.description; } }

        /// <summary>
        /// The number of tweets this user has favorited in the account's lifetime.
        /// </summary>
        public int FavoritesCount { get { return Object.favourites_count; } }

        /// <summary>
        /// The number of followers this account currently has. Under certain conditions of duress, this field will temporarily indicate "0." (null)
        /// </summary>
        public int? FollowersCount { get { return Object.followers_count as int?; } }

        /// <summary>
        /// The number of users this account is following (AKA their "followings"). Under certain conditions of duress, this field will temporarily indicate "0." (null)
        /// </summary>
        public int? FriendsCount { get { return Object.friends_count as int?; } }

        /// <summary>
        /// The string representation of the unique identifier for this Tweet. 
        /// </summary>
        public string IdStr { get { return Object.id_str; } }

        /// <summary>
        /// The name of the user, as they've defined it. Not necessarily a person's name. Typically capped at 20 characters, but subject to change.
        /// </summary>
        public string Name { get { return Object.name; } }

        /// <summary>
        /// When true, indicates that this user has chosen to protect their Tweets. 
        /// </summary>
        public bool Protected { get { return Object.@protected; } }

        /// <summary>
        /// The screen name, handle, or alias that this user identifies themselves with. screen_names are unique but subject to change.
        /// </summary>
        public string ScreenName { get { return Object.screen_name; } }

        /// <summary>
        /// Nullable. If possible, the user's most recent tweet or retweet. In some circumstances, this data cannot be provided and this field will be omitted, null, or empty. Perspectival attributes within tweets embedded within users cannot always be relied upon.
        /// </summary>
        public Tweet Status { get { return Object.status != null ? new Tweet(Object.status) : null; } }

        /// <summary>
        /// The number of tweets (including retweets) issued by the user.
        /// </summary>
        public int StatusesCount { get { return Object.statuses_count; } }

        /// <summary>
        /// Nullable. A string describing the Time Zone this user declares themselves within.
        /// </summary>
        public string TimeZone { get { return Object.time_zone; } }

        /// <summary>
        /// When true, indicates that the user has a verified account.
        /// </summary>
        public bool Verified { get { return Object.verified; } }
    }
}

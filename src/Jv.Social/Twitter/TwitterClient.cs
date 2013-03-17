using Jv.Web.OAuth;
using Jv.Web.OAuth.Extensions;
using Jv.Web.OAuth.v1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Metadata;

namespace Jv.Social.Twitter
{
    public sealed class TwitterClient
    {
        #region Properties
        public OAuthClient OAuthClient { get; private set; }
        #endregion

        #region Login
        public TwitterClient(OAuthClient oAuthClient)
        {
            if (oAuthClient == null)
                throw new System.ArgumentNullException();

            OAuthClient = oAuthClient;
        }

        public TwitterClient(KeyPair applicationInfo, KeyPair token)
            : this(new OAuthClient(applicationInfo, token))
        {
        }

        public static async Task<TwitterClient> Login(KeyPair applicationInfo)
        {
            try
            {
                var login = new TwitterLogin(applicationInfo);
                var oAuthClient = await login.Login();

                return new TwitterClient(oAuthClient);
            }
            catch (WebException ex)
            {
                throw new Exception(ex.Response.GetResponseString());
            }
        }
        #endregion

        #region Core
        public async Task<T> Get<T>(string resource, HttpParameters data = null) where T : Object
        {
            return Object.Create<T>(await Ajax(resource, "GET", data));
        }

        public async Task<T> Post<T>(string resource, HttpParameters data = null) where T : Object
        {
            return Object.Create<T>(await Ajax(resource, "POST", data));
        }

        public async Task<dynamic> Ajax(string resource, string type, HttpParameters data = null)
        {
            string url = string.Format("http://api.twitter.com/1.1/{0}.json", resource);
            return await OAuthClient.Ajax(url, type, data, DataType.Json, WebRequestFormat.MixedUrlMultipart);
        }
        #endregion

        #region Public API
        #region Resource

        #endregion

        #region Tweets
        /// <summary>
        /// Updates the authenticating user's current status, also known as tweeting.
        /// </summary>
        /// <param name="status">The text of your status update, typically up to 140 characters.</param>
        /// <returns>Tweet operation</returns>
        public Task<Tweet> Tweet(string status)
        {
            return Tweet(status, inReplyToStatusId: null, location: null);
        }

        /// <summary>
        /// Updates the authenticating user's current status, also known as tweeting.
        /// </summary>
        /// <param name="status">The text of your status update, typically up to 140 characters.</param>
        /// <param name="inReplyToStatusId">
        ///     The ID of an existing status that the update is in reply to.
        ///     Note: This parameter will be ignored unless the author of the tweet this parameter references is mentioned within the status text. Therefore, you must include @username, where username is the author of the referenced tweet, within the update.
        /// </param>
        /// <returns>Tweet operation</returns>
        public Task<Tweet> Tweet(string status, string inReplyToStatusId)
        {
            return Tweet(status, inReplyToStatusId, location: null);
        }

        /// <summary>
        /// Updates the authenticating user's current status, also known as tweeting.
        /// </summary>
        /// <param name="status">The text of your status update, typically up to 140 characters.</param>
        /// <param name="location">The location (lat/long) this tweet refers to.</param>
        /// <returns>Tweet operation</returns>
        public Task<Tweet> Tweet(string status, Geocoordinate location)
        {
            return Tweet(status, inReplyToStatusId: null, location: location);
        }

        /// <summary>
        /// Updates the authenticating user's current status, also known as tweeting.
        /// </summary>
        /// <param name="status">The text of your status update, typically up to 140 characters.</param>
        /// <param name="inReplyToStatusId">
        ///     The ID of an existing status that the update is in reply to.
        ///     Note: This parameter will be ignored unless the author of the tweet this parameter references is mentioned within the status text. Therefore, you must include @username, where username is the author of the referenced tweet, within the update.
        /// </param>
        /// <param name="location">The location (lat/long) this tweet refers to.</param>
        /// <returns>Tweet operation</returns>
        public Task<Tweet> Tweet(string status, string inReplyToStatusId, Geocoordinate location)
        {
            var parameters = new HttpParameters {
                    { "status", status },
                    { "in_reply_to_status_id", inReplyToStatusId },
                };

            if (location != null)
            {
                parameters.Add("lat", location.Latitude.ToString(CultureInfo.InvariantCulture));
                parameters.Add("long", location.Longitude.ToString(CultureInfo.InvariantCulture));
            }

            return Post<Tweet>(
                resource: "statuses/update",
                data: parameters
            );
        }


        /// <summary>
        /// Retweets a tweet.
        /// </summary>
        /// <param name="id">The ID of the desired status.</param>
        /// <returns>Returns the original tweet with retweet details embedded.</returns>
        public Task<Tweet> Retweet(string id)
        {
            return Post<Tweet>(
                   resource: "statuses/retweet/" + id
               );
        }
        #endregion

        public Task<User> CurrentUser()
        {
            return Get<User>(
                resource: "account/verify_credentials"
            );
        }
        #endregion
    }
}

using Jv.Web.OAuth;
using Jv.Web.OAuth.Authentication;
using Jv.Web.OAuth.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social.Twitter
{
    public class TwitterClient
    {
        #region Properties
        public OAuthClient OAuthClient { get; private set; }
        #endregion

        #region Login
        public TwitterClient(OAuthClient oAuthClient)
        {
            if (oAuthClient == null)
                throw new System.ArgumentNullException("oAuthClient");

            OAuthClient = oAuthClient;
        }

        public TwitterClient(KeyPair applicationInfo, KeyPair token)
            : this(new OAuthClient(applicationInfo, token))
        {
        }

        public static async Task<TwitterClient> Login(KeyPair applicationInfo, IWebAuthenticator authenticator)
        {
            var login = new TwitterLogin(applicationInfo);
            var oAuthClient = await login.Login(authenticator);

            return new TwitterClient(oAuthClient);
        }
        #endregion

        #region Core
        public async Task<T> Get<T>(string resource, HttpParameters data = null) where T : DynamicWrapper
        {
            return DynamicWrapper.Create<T>((SafeObject)await Ajax(resource, HttpMethod.Get, data));
        }

        public async Task<T> Post<T>(string resource, HttpParameters data = null) where T : DynamicWrapper
        {
            return DynamicWrapper.Create<T>((SafeObject)await Ajax(resource, HttpMethod.Post, data));
        }

        public async Task<dynamic> Ajax(string resource, HttpMethod method, HttpParameters parameters = null)
        {
            var url = new Uri(string.Format("https://api.twitter.com/1.1/{0}.json", resource));
            return await OAuthClient.Ajax(url, method, parameters, DataType.Json, WebRequestFormat.MixedUrlMultipart);
        }
        #endregion

        #region Public API
        public Task<User> GetCurrentUser()
        {
            return Get<User>(
                resource: "account/verify_credentials"
            );
        }

        #region Tweets
        /// <summary>
        /// Updates the authenticating user's current status, also known as tweeting.
        /// </summary>
        /// <param name="status">The text of your status update, typically up to 140 characters.</param>
        /// <param name="inReplyToStatusId">
        ///     The ID of an existing status that the update is in reply to.
        ///     Note: This parameter will be ignored unless the author of the tweet this parameter references is mentioned within the status text.
        ///     Therefore, you must include @username, where username is the author of the referenced tweet, within the update.
        /// </param>
        /// <!--param name="location">The location (lat/long) this tweet refers to.</param-->
        /// <returns>Tweet operation</returns>
        public Task<Tweet> Tweet(string status, string inReplyToStatusId = null/*, Geocoordinate location*/)
        {
            var parameters = new HttpParameters {
                    { "status", status },
                    { "in_reply_to_status_id", inReplyToStatusId },
                };

            /*if (location != null)
            {
                parameters.Add("lat", location.Latitude.ToString(CultureInfo.InvariantCulture));
                parameters.Add("long", location.Longitude.ToString(CultureInfo.InvariantCulture));
            }*/

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

        /// <summary>
        /// Destroys the status specified by the required ID parameter. The authenticating user must be the author of the specified status. Returns the destroyed status if successful.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Tweet> RemoveTweet(string id)
        {
            return Post<Tweet>(
                resource: "statuses/destroy/" + id
            );
        }
        #endregion
        #endregion
    }
}

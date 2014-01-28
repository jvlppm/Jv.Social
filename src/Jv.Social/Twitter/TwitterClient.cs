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
        #endregion
    }
}

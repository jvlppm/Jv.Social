using Jv.Social.Base;
using Jv.Web.OAuth;
using Jv.Web.OAuth.Extensions;
using Jv.Web.OAuth.v1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Jv.Social.Twitter
{
    public sealed class TwitterClient
    {
        #region Properties
        public KeyInfo Token
        {
            set { OAuthClient.Token = value.KeyPair; }
            get
            {
                if (_token == null || !_token.Equals(OAuthClient.Token))
                    _token = new KeyInfo(OAuthClient.Token);
                return _token;
            }
        }

        #region Internal
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal OAuthClient OAuthClient { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        KeyInfo _token;
        #endregion
        #endregion

        #region Login
        public TwitterClient(KeyInfo applicationInfo, KeyInfo token)
            : this(new OAuthClient(applicationInfo.KeyPair, token.KeyPair))
        {
        }

        public static IAsyncOperation<TwitterClient> Login(KeyInfo applicationInfo)
        {
            return Login(applicationInfo.KeyPair).AsAsyncOperation();
        }

        #region Internal
        internal TwitterClient(OAuthClient oAuthClient)
        {
            if (oAuthClient == null)
                throw new System.ArgumentNullException();

            OAuthClient = oAuthClient;
        }

        internal static async Task<TwitterClient> Login(KeyPair applicationInfo)
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
        #endregion

        #region Core
        internal async Task<T> Get<T>(string resource, HttpParameters data = null) where T : class
        {
            return Extensions.Create<T>(await Ajax(resource, "GET", data));
        }

        internal async Task<T> Post<T>(string resource, HttpParameters data = null) where T : class
        {
            return Extensions.Create<T>(await Ajax(resource, "POST", data));
        }

        internal async Task<dynamic> Ajax(string resource, string type, HttpParameters data = null)
        {
            string url = string.Format("http://api.twitter.com/1.1/{0}.json", resource);
            return await OAuthClient.Ajax(url, type, data, DataType.Json, WebRequestFormat.MixedUrlMultipart);
        }
        #endregion

        public IAsyncOperation<User> CurrentUser()
        {
            return Get<User>(
                resource: "account/verify_credentials"
            ).AsAsyncOperation();
        }

        public IAsyncOperation<Tweet> Tweet(string status)
        {
            return Post<Tweet>(
                resource: "statuses/update",
                data: new HttpParameters { { "status", status } }
            ).AsAsyncOperation();
        }
    }
}

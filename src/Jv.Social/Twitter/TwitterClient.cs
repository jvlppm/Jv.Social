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
        internal async Task<dynamic> Ajax(string resource, string type, HttpParameters data = null)
        {
            try
            {
                var req = await CreateHttpWebRequest(resource, type, data);
                return await req.Request(DataType.Json);
            }
            catch (WebException ex)
            {
                throw new Exception(ex.Response.GetResponseString());
            }
        }

        private async Task<WebRequest> CreateHttpWebRequest(string resource, string type, HttpParameters data = null)
        {
            data = data ?? new HttpParameters();
            string baseUrl = string.Format("http://api.twitter.com/1.1/{0}.json", resource);

            var oAuthParams = OAuthClient.GetOauthParameters(type, baseUrl, data.Fields);
            data.AddRange(oAuthParams);

            string url = HttpUtils.BuildUrl(baseUrl, data.Fields);
            return await HttpUtils.CreateHttpWebRequest(type, url, data.FileParameters);
        }
        #endregion

        /*public IAsyncOperation<dynamic> CurrentUser()
        {
            return Ajax(
                resource: "account/verify_credentials",
                type: "GET"
            ).AsAsyncOperation();
        }

        public IAsyncOperation<dynamic> Tweet(string status)
        {
            return Ajax(
                resource: "statuses/update",
                data: new HttpParameters { { "status", status } },
                type: "POST"
            ).AsAsyncOperation();
        }*/
    }
}

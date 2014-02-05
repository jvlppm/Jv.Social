using Jv.Web.OAuth;
using Jv.Web.OAuth.Authentication;
using Jv.Web.OAuth.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social.Google.Plus
{
    public class GooglePlusClient
    {
        #region Properties
        public OAuthClient OAuthClient { get; private set; }
        #endregion

        public GooglePlusClient(OAuthClient oAuthClient)
        {
            if (oAuthClient == null)
                throw new System.ArgumentNullException("oAuthClient");

            OAuthClient = oAuthClient;
        }

        public static async Task<GooglePlusClient> Login(KeyPair applicationInfo, IWebAuthenticator authenticator, HttpClient httpClient = null)
        {
            var login = new GoogleLoginV2(applicationInfo, new Uri("https://www.googleapis.com/auth/plus.login"), httpClient);
            var oAuthClient = await login.Login(authenticator);

            return new GooglePlusClient(oAuthClient);
        }

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
            var url = new Uri(string.Format("https://www.googleapis.com/plus/v1/{0}", resource));
            return await OAuthClient.Ajax(url, method, parameters, DataType.Json, WebRequestFormat.MixedUrlMultipart);
        }
        #endregion
    }
}

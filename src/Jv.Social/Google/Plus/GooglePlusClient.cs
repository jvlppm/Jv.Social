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
    }
}

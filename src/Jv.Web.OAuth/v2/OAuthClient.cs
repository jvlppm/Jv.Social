using Jv.Web.OAuth.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Web.OAuth.v2
{
    public abstract class OAuthClient : WebClient
    {
        public KeyPair ApplicationInfo { get; private set; }
        public OAuthAccessToken AccessToken { get; private set; }

        protected OAuthClient(KeyPair applicationInfo,
                                OAuthAccessToken accessToken,
                                HttpClient httpClient = null)
            : base(httpClient)
        {
            if (applicationInfo == null)
                throw new ArgumentNullException("applicationInfo");
            if (accessToken == null)
                throw new ArgumentNullException("accessToken");
            

            ApplicationInfo = applicationInfo;
            AccessToken = accessToken;
        }

        protected override HttpRequestMessage CreateRequest(Uri url, HttpMethod httpMethod, HttpParameters parameters, WebRequestFormat requestFormat)
        {
            var signedParams = AccessToken.Sign(ApplicationInfo, url, httpMethod, parameters);
            return base.CreateRequest(url, httpMethod, parameters, requestFormat);
        }

        #region Static
        public static OAuthClient Parse(string oauthClient)
        {
            var obj = oauthClient.AsJson();
            string tokenType = obj.token_type;

            string scope = serverResponse.scope;
            
            /*TimeSpan? expiresIn = null;
            int? expires = serverResponse.expires_in;
            if (expires != null)
                expiresIn = TimeSpan.FromSeconds((double)expires);*/

            switch (tokenType.ToLower())
            {
                case "bearer":
                    return new OAuthClientBearer(oauthClient);
            }

            throw new NotImplementedException("token_type of " + tokenType + " is not supported.");
        }
        #endregion
    }
}

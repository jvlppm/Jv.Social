using System;
using System.Net.Http;

namespace Jv.Web.OAuth.v2
{
    public class OAuthClientBearer : OAuthClientBase
    {
        public string AccessToken { get; private set; }

        public OAuthClientBearer(KeyPair applicationInfo, string accessToken, HttpClient httpClient = null)
            : base(applicationInfo, httpClient)
        {
            if (accessToken == null)
                throw new ArgumentNullException("accessToken");

            AccessToken = accessToken;
        }

        protected override HttpParameters Sign(Uri url, HttpMethod method, HttpParameters parameters)
        {
            return new HttpParameters(parameters)
            {
                {"access_token", AccessToken}
            };
        }
    }
}

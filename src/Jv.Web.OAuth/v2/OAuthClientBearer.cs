using System;
using System.Net.Http;

namespace Jv.Web.OAuth.v2
{
    public class OAuthClientBearer : OAuthClient
    {
        public OAuthClientBearer(KeyPair applicationInfo, string accessToken, TimeSpan? expiresIn, Uri scope, string refreshToken, HttpClient httpClient = null)
            : base(applicationInfo, accessToken, "bearer", expiresIn, scope, refreshToken, httpClient)
        {
        }

        protected override HttpParameters Sign(Uri url, HttpMethod method, HttpParameters parameters)
        {
            return new HttpParameters(parameters)
            {
                { "access_token", AccessToken }
            };
        }
    }
}

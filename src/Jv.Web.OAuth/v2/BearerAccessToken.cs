// http://tools.ietf.org/html/draft-ietf-oauth-v2-bearer-23
using System;
using System.Net.Http;

namespace Jv.Web.OAuth.v2
{
    public class BearerAccessToken : OAuthAccessToken
    {
        public BearerAccessToken(string accessToken, TimeSpan? expiresIn, string scope, string refreshToken)
            : base(accessToken, "bearer", expiresIn, scope, refreshToken)
        {
        }

        public override HttpParameters Sign(KeyPair applicationInfo, Uri url, HttpMethod method, HttpParameters parameters)
        {
            return new HttpParameters(parameters)
            {
                { "access_token", AccessToken }
            };
        }
    }
}

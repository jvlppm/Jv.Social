using Jv.Web.OAuth;
using Jv.Web.OAuth.Authentication;
using Jv.Web.OAuth.v2;
using System;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Jv.Social.Google
{
    public class GoogleLoginV2 : OAuthLogin
    {
        public GoogleLoginV2(KeyPair applicationInfo, Uri urlScope, HttpClient httpClient = null)
            : base(applicationInfo,
                urlGetAuthorizationCode: new Uri("https://accounts.google.com/o/oauth2/auth"),
                urlGetAccessToken: new Uri("https://accounts.google.com/o/oauth2/token"),
                scope: urlScope.ToString(),
                httpClient: httpClient)
        {
        }
    }
}

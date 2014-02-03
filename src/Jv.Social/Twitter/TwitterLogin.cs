using Jv.Web.OAuth;
using Jv.Web.OAuth.v1;
using System;
using System.Net.Http;

namespace Jv.Social.Twitter
{
    public class TwitterLogin : OAuthLogin
    {
        public TwitterLogin(KeyPair applicationInfo, HttpClient httpClient = null)
            : base(applicationInfo,
                urlGetRequestToken: new Uri("https://api.twitter.com/oauth/request_token"),
                urlGetAccessToken: new Uri("https://api.twitter.com/oauth/access_token"),
                urlAuthorizeToken: new Uri("https://api.twitter.com/oauth/authorize"),
                httpClient: httpClient)
        {
        }
    }
}

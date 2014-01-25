using Jv.Web.OAuth;
using Jv.Web.OAuth.v1;
using System;

namespace Jv.Social.Twitter
{
    public class TwitterLogin : OAuthLogin
    {
        public TwitterLogin(KeyPair applicationInfo)
            : base(applicationInfo,
                urlGetRequestToken: new Uri("https://api.twitter.com/oauth/request_token"),
                urlGetAccessToken: new Uri("https://api.twitter.com/oauth/access_token"),
                urlAuthorizeToken: new Uri("https://api.twitter.com/oauth/authorize"))
        {
        }
    }
}

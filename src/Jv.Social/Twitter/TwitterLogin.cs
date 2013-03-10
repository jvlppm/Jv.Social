using Jv.Web.OAuth;
using Jv.Web.OAuth.v1;

namespace Jv.Social.Twitter
{
    public class TwitterLogin : OAuthLogin
    {
        string UrlScope { get; set; }

        public TwitterLogin(KeyPair applicationInfo)
            : base(applicationInfo,
                urlGetRequestToken: "https://api.twitter.com/oauth/request_token",
                urlCallback: "http://localhost",
                urlAuthorize: "https://api.twitter.com/oauth/authorize",
                urlAccessToken: "https://api.twitter.com/oauth/access_token")
        {
        }
    }
}

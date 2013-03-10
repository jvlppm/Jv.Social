using Jv.Web.OAuth;
using Jv.Web.OAuth.v1;

namespace Jv.Social.Google
{
    public class GoogleLogin : OAuthLogin
    {
        string UrlScope { get; set; }

        public GoogleLogin(KeyPair applicationInfo, string urlScope)
            : base(applicationInfo,
                urlGetRequestToken: "https://www.google.com/accounts/OAuthGetRequestToken",
                urlCallback: "http://localhost",
                urlAuthorize: "https://www.google.com/accounts/OAuthAuthorizeToken",
                urlAccessToken: "https://www.google.com/accounts/OAuthGetAccessToken")
        {
            UrlScope = urlScope;
        }

        protected override async System.Threading.Tasks.Task<KeyPair> GetRequestToken()
        {
            var oauthClient = new OAuthClient(ApplicationInfo);

            var resp = await oauthClient.Ajax(UrlGetRequestToken,
                data: new HttpParameters {
                    {"oauth_callback", UrlCallback},
                    {"scope", UrlScope}
                },
                dataType: DataType.UrlEncoded
            );

            return new KeyPair(
                key: resp.oauth_token,
                secret: resp.oauth_token_secret
            );
        }
    }
}

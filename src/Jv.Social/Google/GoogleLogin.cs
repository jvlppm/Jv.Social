using Jv.Web.OAuth;
using Jv.Web.OAuth.v1;
using System;

namespace Jv.Social.Google
{
    public class GoogleLogin : OAuthLogin
    {
        Uri UrlScope { get; set; }

        public GoogleLogin(KeyPair applicationInfo, Uri urlScope)
            : base(applicationInfo,
                urlCallback: new Uri("http://localhost"),
                urlGetRequestToken: new Uri("https://www.google.com/accounts/OAuthGetRequestToken"),
                urlAuthorizeToken: new Uri("https://www.google.com/accounts/OAuthAuthorizeToken"),
                urlGetAccessToken: new Uri("https://www.google.com/accounts/OAuthGetAccessToken"))
        {
            UrlScope = urlScope;
        }

        protected override async System.Threading.Tasks.Task<KeyPair> GetRequestToken()
        {
            var oauthClient = new OAuthClient(ApplicationInfo);

            var resp = await oauthClient.Ajax(UrlGetRequestToken,
                parameters: new HttpParameters {
                    {"oauth_callback", UrlCallback.ToString()},
                    {"scope", UrlScope.ToString()}
                }//,
                //dataType: DataType.UrlEncoded
            );

            return new KeyPair(
                key: resp.oauth_token,
                secret: resp.oauth_token_secret
            );
        }
    }
}

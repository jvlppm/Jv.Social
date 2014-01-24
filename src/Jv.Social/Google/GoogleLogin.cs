using Jv.Web.OAuth;
using Jv.Web.OAuth.v1;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Jv.Social.Google
{
    public class GoogleLogin : OAuthLogin
    {
        Uri UrlScope { get; set; }

        public GoogleLogin(KeyPair applicationInfo, Uri urlScope)
            : base(applicationInfo,
                urlGetRequestToken: new Uri("https://www.google.com/accounts/OAuthGetRequestToken"),
                urlAuthorizeToken: new Uri("https://www.google.com/accounts/OAuthAuthorizeToken"),
                urlGetAccessToken: new Uri("https://www.google.com/accounts/OAuthGetAccessToken"))
        {
            UrlScope = urlScope;
        }

        protected override async Task<KeyPair> GetRequestToken(IUserAuthorizer authorizer)
        {
            var oauthClient = new OAuthClient(ApplicationInfo);

            var resp = await oauthClient.Ajax(UrlGetRequestToken,
                parameters: new HttpParameters {
                    {"oauth_callback", (await authorizer.GetCallback()).ToString() },
                    {"scope", UrlScope.ToString()}
                },
                dataType: DataType.UrlEncoded
            );

            if (resp.oauth_callback_confirmed != "true")
                throw new ProtocolException("Expected oauth_callback_confirmed to be true");

            return new KeyPair(
                key: resp.oauth_token,
                secret: resp.oauth_token_secret
            );
        }
    }
}

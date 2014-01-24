// http://oauth.net/core/1.0a/
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Jv.Web.OAuth.Extensions;

namespace Jv.Web.OAuth.v1
{
    public class OAuthLogin
    {
        #region Attributes
        public KeyPair ApplicationInfo { get; private set; }
        public Uri UrlGetRequestToken { get; private set; }
        public Uri UrlAuthorizeToken { get; private set; }
        public Uri UrlGetAccessToken { get; private set; }
        #endregion

        #region Constructors
        public OAuthLogin(KeyPair applicationInfo,
            Uri urlGetRequestToken,
            Uri urlAuthorizeToken,
            Uri urlGetAccessToken)
        {
            ApplicationInfo = applicationInfo;
            UrlGetRequestToken = urlGetRequestToken;
            UrlAuthorizeToken = urlAuthorizeToken;
            UrlGetAccessToken = urlGetAccessToken;
        }
        #endregion

        public virtual async Task<OAuthClient> Login()
        {
            var authorizer = IoC.Create<IUserAuthorizer>();

            var requestToken = await GetRequestToken(authorizer);
            var authResult = await GetUserAuthorization(requestToken, authorizer);
            if (requestToken.Key != authResult.OAuthToken)
                throw new Exception("Invalid token authorized by server");

            var accessToken = await GetAccessToken(authResult);
            return new OAuthClient(ApplicationInfo, accessToken);
        }

        protected virtual async Task<KeyPair> GetRequestToken(IUserAuthorizer authorizer)
        {
            var oauthClient = new OAuthClient(ApplicationInfo);

            var resp = await oauthClient.Ajax(UrlGetRequestToken,
                parameters: new HttpParameters { { "oauth_callback", (await authorizer.GetCallback()).ToString() } },
                dataType: DataType.UrlEncoded
            );

            if (resp.oauth_callback_confirmed != "true")
                throw new ProtocolException("Expected oauth_callback_confirmed to be true");

            return new KeyPair(
                key: resp.oauth_token,
                secret: resp.oauth_token_secret
            );
        }

        protected virtual Task<UserAuthorizationResult> GetUserAuthorization(KeyPair requestToken, IUserAuthorizer authorizer)
        {
            var authorizationUrlBuilder = new UriBuilder(UrlAuthorizeToken);
            authorizationUrlBuilder.AddToQuery("oauth_token", requestToken.Key);
            var uri = authorizationUrlBuilder.Uri;

            authorizer.AuthorizeUser(uri);

            throw new NotImplementedException();
        }

        protected virtual Task<KeyPair> GetAccessToken(UserAuthorizationResult authResult)
        {
            throw new NotImplementedException();
        }
    }
}

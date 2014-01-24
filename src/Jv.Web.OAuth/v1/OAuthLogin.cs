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
            UserAuthorizationResult userAuthResult;
            using (var authorizer = IoC.Create<IUserAuthorizer>())
            {
                var requestToken = await GetRequestToken(authorizer);
                userAuthResult = await GetUserAuthorization(requestToken, authorizer);

                if (requestToken.Key != userAuthResult.OAuthToken)
                    throw new ProtocolException("Invalid token authorized by server");

                var accessToken = await GetAccessToken(requestToken, userAuthResult);
                return new OAuthClient(ApplicationInfo, accessToken);
            }
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

            return authorizer.AuthorizeUser(authorizationUrlBuilder.Uri);
        }

        protected virtual async Task<KeyPair> GetAccessToken(KeyPair requestToken, UserAuthorizationResult authResult)
        {
            var oauthClient = new OAuthClient(ApplicationInfo, requestToken);

            var resp = await oauthClient.Ajax(UrlGetAccessToken,
                parameters: new HttpParameters { { "oauth_verifier", authResult.OAuthVerifier } },
                dataType: DataType.UrlEncoded);

            return new KeyPair(
                key: resp.oauth_token,
                secret: resp.oauth_token_secret
            );
        }
    }
}

// http://oauth.net/core/1.0a/
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Jv.Web.OAuth.Extensions;
using Jv.Web.OAuth.Authentication;

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

        public virtual async Task<OAuthClient> Login(IWebAuthenticator authenticator)
        {
            if (authenticator == null)
                throw new ArgumentNullException("authenticator");

            var requestToken = await GetRequestToken(authenticator);
            var userAuthResult = await GetUserAuthorization(requestToken, authenticator);

            string oAuthToken, oAuthVerifier;
            ReadUserAuthorizationResult(userAuthResult, out oAuthToken, out oAuthVerifier);

            if (requestToken.Key != oAuthToken)
                throw new ProtocolException("Invalid token authorized by server");

            var accessToken = await GetAccessToken(requestToken, oAuthVerifier);
            return new OAuthClient(ApplicationInfo, accessToken);
        }

        protected virtual void ReadUserAuthorizationResult(WebAuthenticationResult userAuthResult, out string oAuthToken, out string oAuthVerifier)
        {
            if (userAuthResult.ResponseStatus == WebAuthenticationStatus.UserCancel)
                throw new TaskCanceledException();
            if (userAuthResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                throw new WebException(userAuthResult.ResponseErrorDetail, userAuthResult.ResponseData);

            var responseData = userAuthResult.ResponseData.ParseUrlParameters();

            if (!responseData.TryGetValue("oauth_token", out oAuthToken))
                throw new ProtocolException("Server did not return oauth_token");

            if (!responseData.TryGetValue("oauth_verifier", out oAuthVerifier))
                throw new ProtocolException("Server did not return oauth_verifier");
        }

        protected virtual async Task<KeyPair> GetRequestToken(IWebAuthenticator authorizer)
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

        protected virtual Task<WebAuthenticationResult> GetUserAuthorization(KeyPair requestToken, IWebAuthenticator authorizer)
        {
            var authorizationUrlBuilder = new UriBuilder(UrlAuthorizeToken);
            authorizationUrlBuilder.AddToQuery("oauth_token", requestToken.Key);

            return authorizer.AuthorizeUser(authorizationUrlBuilder.Uri);
        }

        protected virtual async Task<KeyPair> GetAccessToken(KeyPair requestToken, string oAuthVerifier)
        {
            var oauthClient = new OAuthClient(ApplicationInfo, requestToken);

            var resp = await oauthClient.Ajax(UrlGetAccessToken,
                parameters: new HttpParameters { { "oauth_verifier", oAuthVerifier } },
                dataType: DataType.UrlEncoded);

            return new KeyPair(
                key: resp.oauth_token,
                secret: resp.oauth_token_secret
            );
        }
    }
}

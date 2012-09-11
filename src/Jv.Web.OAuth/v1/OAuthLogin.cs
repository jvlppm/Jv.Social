using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Jv.Web.OAuth.Extensions;

namespace Jv.Web.OAuth.v1
{
    public class OAuthLogin
    {
        public KeyPair ApplicationInfo { get; private set; }

        public string UrlGetRequestToken { get; private set; }
        public string UrlCallback { get; private set; }
        public string UrlAuthorize { get; private set; }
        public string UrlAccessToken { get; private set; }

        public OAuthLogin(KeyPair applicationInfo,
            string urlGetRequestToken,
            string urlCallback,
            string urlAuthorize,
            string urlAccessToken)
        {
            ApplicationInfo = applicationInfo;

            UrlGetRequestToken = urlGetRequestToken;
            UrlCallback = urlCallback;
            UrlAuthorize = urlAuthorize;
            UrlAccessToken = urlAccessToken;
        }

        public virtual async Task<OAuthClient> Login()
        {
            var requestToken = await GetRequestToken();
            var authResult = await AuthorizeToken(requestToken);

            if (authResult.ResponseStatus == WebAuthenticationStatus.UserCancel)
                throw new TaskCanceledException();
            if (authResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                throw new Exception("ErrorHttp: " + authResult.ResponseErrorDetail);

            var verifier = authResult.ResponseData.ParseUrlParameters()["oauth_verifier"];
            var accessToken = await GetAccessToken(requestToken, verifier);

            return new OAuthClient(ApplicationInfo, accessToken);
        }

        protected virtual async Task<KeyPair> GetRequestToken()
        {
            var oauthClient = new OAuthClient(ApplicationInfo);

            var resp = await oauthClient.Ajax(UrlGetRequestToken,
                data: new HttpParameters { { "oauth_callback", UrlCallback } },
                dataType: DataType.UrlEncoded
            );

            return new KeyPair(
                key: resp.oauth_token,
                secret: resp.oauth_token_secret
            );
        }

        protected virtual Task<WebAuthenticationResult> AuthorizeToken(KeyPair token)
        {
            string loginUrl = UrlAuthorize + "?oauth_token=" + token.Key;
            return WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(loginUrl), new Uri(UrlCallback)).AsTask();
        }

        protected virtual async Task<KeyPair> GetAccessToken(KeyPair requestToken, string oAuthVerifier)
        {
            var oauthClient = new OAuthClient(ApplicationInfo, requestToken);

            var resp = await oauthClient.Ajax(UrlAccessToken,
                data: new HttpParameters { { "oauth_verifier", oAuthVerifier } },
                dataType: DataType.UrlEncoded);

            return new KeyPair(
                key: resp.oauth_token,
                secret: resp.oauth_token_secret
            );
        }
    }
}

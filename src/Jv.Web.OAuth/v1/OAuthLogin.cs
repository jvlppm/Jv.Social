using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Web.OAuth.v1
{
    public class OAuthLogin
    {
        #region Attributes
        public KeyPair ApplicationInfo { get; private set; }
        public Uri UrlCallback { get; private set; }
        public Uri UrlGetRequestToken { get; private set; }
        public Uri UrlAuthorizeToken { get; private set; }
        public Uri UrlGetAccessToken { get; private set; }
        #endregion

        #region Constructors
        public OAuthLogin(KeyPair applicationInfo,
            Uri urlCallback,
            Uri urlGetRequestToken,
            Uri urlAuthorizeToken,
            Uri urlGetAccessToken)
        {
            ApplicationInfo = applicationInfo;
            UrlCallback = urlCallback;
            UrlGetRequestToken = urlGetRequestToken;
            UrlAuthorizeToken = urlAuthorizeToken;
            UrlGetAccessToken = urlGetAccessToken;
        }
        #endregion

        public virtual async Task<OAuthClient> Login()
        {
            var requestToken = await GetRequestToken();
            var authResult = await GetUserAuthorization(requestToken);
            if (requestToken.Key != authResult.OAuthToken)
                throw new Exception("Invalid token authorized by server");

            var accessToken = await GetAccessToken(authResult);
            return new OAuthClient(ApplicationInfo, accessToken);
        }

        protected virtual async Task<KeyPair> GetRequestToken()
        {
            var oauthClient = new OAuthClient(ApplicationInfo);

            var resp = await oauthClient.Ajax(UrlGetRequestToken,
                parameters: new HttpParameters { { "oauth_callback", UrlCallback.ToString() } }//,
                //dataType: DataType.UrlEncoded
            );

            return new KeyPair(
                key: resp.oauth_token,
                secret: resp.oauth_token_secret
            );
        }

        protected virtual Task<UserAuthorizationResult> GetUserAuthorization(KeyPair requestToken)
        {
            throw new NotImplementedException();
        }

        protected virtual Task<KeyPair> GetAccessToken(UserAuthorizationResult authResult)
        {
            throw new NotImplementedException();
        }
    }
}

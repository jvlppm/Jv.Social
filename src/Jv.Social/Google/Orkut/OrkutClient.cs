using Jv.Web.OAuth;
using Jv.Web.OAuth.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Jv.Social.Google.Orkut
{
    public sealed class OrkutClient
    {
        internal OAuthClient OAuthClient { get; private set; }

        TokenInfo _token;
        public TokenInfo Token
        {
            set { OAuthClient.Token = value.KeyPair; }
            get
            {
                if (_token == null || !_token.Equals(OAuthClient.Token))
                    _token = new TokenInfo(OAuthClient.Token);
                return _token;
            }
        }

        #region Login
        internal OrkutClient(OAuthClient oAuthClient)
        {
            if (oAuthClient == null)
                throw new System.ArgumentNullException();

            OAuthClient = oAuthClient;
        }

        public OrkutClient(ApplicationInfo applicationInfo, TokenInfo token)
            : this(new OAuthClient(applicationInfo.KeyPair, token.KeyPair))
        {
        }

        public static IAsyncOperation<OrkutClient> Login(ApplicationInfo applicationInfo)
        {
            return Login(applicationInfo.KeyPair).AsAsyncOperation();
        }

        internal static async Task<OrkutClient> Login(KeyPair applicationInfo)
        {
            var login = new GoogleLogin(applicationInfo, "http://orkut.gmodules.com/social");
            var oAuthClient = await login.Login();

            return new OrkutClient(oAuthClient);
        }
        #endregion
    }
}

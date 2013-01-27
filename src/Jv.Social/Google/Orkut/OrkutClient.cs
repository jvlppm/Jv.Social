using Jv.Social.Base;
using Jv.Web.OAuth;
using Jv.Web.OAuth.Extensions;
using Jv.Web.OAuth.v1;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Jv.Social.Google.Orkut
{
    public sealed class OrkutClient
    {
        #region Properties
        public KeyInfo Token
        {
            set { OAuthClient.Token = value.KeyPair; }
            get
            {
                if (_token == null || !_token.Equals(OAuthClient.Token))
                    _token = new KeyInfo(OAuthClient.Token);
                return _token;
            }
        }

        #region Internal
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal OAuthClient OAuthClient { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        KeyInfo _token;
        #endregion
        #endregion

        #region Login
        public OrkutClient(KeyInfo applicationInfo, KeyInfo token)
            : this(new OAuthClient(applicationInfo.KeyPair, token.KeyPair))
        {
        }

        public static IAsyncOperation<OrkutClient> Login(KeyInfo applicationInfo)
        {
            return Login(applicationInfo.KeyPair).AsAsyncOperation();
        }

        #region Internal
        internal OrkutClient(OAuthClient oAuthClient)
        {
            if (oAuthClient == null)
                throw new System.ArgumentNullException();

            OAuthClient = oAuthClient;
        }

        internal static async Task<OrkutClient> Login(KeyPair applicationInfo)
        {
            try
            {
                var login = new GoogleLogin(applicationInfo, "http://orkut.gmodules.com/social");
                var oAuthClient = await login.Login();

                return new OrkutClient(oAuthClient);
            }
            catch (WebException ex)
            {
                throw new Exception(ex.Response.GetResponseString());
            }
        }
        #endregion
        #endregion
    }
}

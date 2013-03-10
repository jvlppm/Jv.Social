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
        public OAuthClient OAuthClient { get; private set; }
        #endregion

        #region Login
        public OrkutClient(OAuthClient oAuthClient)
        {
            if (oAuthClient == null)
                throw new System.ArgumentNullException("oAuthClient");

            OAuthClient = oAuthClient;
        }

        public OrkutClient(KeyPair applicationInfo, KeyPair token)
            : this(new OAuthClient(applicationInfo, token))
        {
        }

        public static async Task<OrkutClient> Login(KeyPair applicationInfo)
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
    }
}

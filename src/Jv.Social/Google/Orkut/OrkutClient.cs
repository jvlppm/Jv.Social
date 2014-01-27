using Jv.Web.OAuth;
using Jv.Web.OAuth.Authentication;
using Jv.Web.OAuth.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social.Google.Orkut
{
    public class OrkutClient
    {
        #region Properties
        public OAuthClient OAuthClient { get; private set; }
        #endregion

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

        public static async Task<OrkutClient> Login(KeyPair applicationInfo, IWebAuthenticator authenticator)
        {
            var login = new GoogleLogin(applicationInfo, new Uri("http://orkut.gmodules.com/social"));
            var oAuthClient = await login.Login(authenticator);

            return new OrkutClient(oAuthClient);
        }
    }
}

using Jv.Web.OAuth;
using Jv.Web.OAuth.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social.Twitter
{
    public class TwitterClient
    {
        #region Properties
        public OAuthClient OAuthClient { get; private set; }
        #endregion

        public TwitterClient(OAuthClient oAuthClient)
        {
            if (oAuthClient == null)
                throw new System.ArgumentNullException("oAuthClient");

            OAuthClient = oAuthClient;
        }

        public TwitterClient(KeyPair applicationInfo, KeyPair token)
            : this(new OAuthClient(applicationInfo, token))
        {
        }

        public static async Task<TwitterClient> Login(KeyPair applicationInfo)
        {
            var login = new TwitterLogin(applicationInfo);
            var oAuthClient = await login.Login();

            return new TwitterClient(oAuthClient);
        }
    }
}

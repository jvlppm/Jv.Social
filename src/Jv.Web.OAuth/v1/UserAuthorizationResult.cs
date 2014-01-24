using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jv.Web.OAuth.v1
{
    public class UserAuthorizationResult
    {
        #region Attributes
        public string OAuthToken { get; private set; }
        public string OAuthVerifier { get; private set; }
        #endregion

        #region Constructors
        public UserAuthorizationResult(string token, string verifier)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be empty", "token");
            if (string.IsNullOrWhiteSpace(verifier))
                throw new ArgumentException("Verifier cannot be empty", "verifier");

            OAuthToken = token;
            OAuthVerifier = verifier;
        }
        #endregion
    }
}

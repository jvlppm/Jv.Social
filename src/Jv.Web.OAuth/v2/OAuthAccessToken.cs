using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Web.OAuth.v2
{
    public abstract class OAuthAccessToken
    {
        /// <summary>
        /// The access token issued by the authorization server.
        /// </summary>
        public string AccessToken { get; private set; }
        /// <summary>
        /// The type of the token issued as described in Section 7.1.
        /// Value is case insensitive.
        /// </summary>
        public string TokenType { get; private set; }
        /// <summary>
        /// The lifetime of the access token.
        /// Denotes when the access token will expire from the time the response was generated.
        /// </summary>
        public TimeSpan? ExpiresIn { get; private set; }
        public string Scope { get; private set; }
        /// <summary>
        /// The refresh token, which can be used to obtain new access tokens
        /// using the same authorization grant.
        /// </summary>
        public string RefreshToken { get; private set; }

        protected OAuthAccessToken(string accessToken,
                                   string tokenType,
                                   TimeSpan? expiresIn,
                                   string scope,
                                   string refreshToken = null)
        {
            if (accessToken == null)
                throw new ArgumentNullException("accessToken");
            if (tokenType == null)
                throw new ArgumentNullException("tokenType");
            if (scope == null)
                throw new ArgumentNullException("scope");

            AccessToken = accessToken;
            TokenType = tokenType;
            ExpiresIn = expiresIn;
            Scope = scope;
            RefreshToken = refreshToken;
        }

        public abstract HttpParameters Sign(KeyPair applicationInfo, Uri url, HttpMethod method, HttpParameters parameters);

        public abstract bool CanRefresh();
    }
}

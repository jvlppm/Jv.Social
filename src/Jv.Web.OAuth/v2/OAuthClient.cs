using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Web.OAuth.v2
{
    public abstract class OAuthClient : WebClient
    {
        public KeyPair ApplicationInfo { get; private set; }
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
        public Uri Scope { get; private set; }
        /// <summary>
        /// The refresh token, which can be used to obtain new access tokens
        /// using the same authorization grant.
        /// </summary>
        public string RefreshToken { get; private set; }

        protected OAuthClient(KeyPair applicationInfo,
                                string accessToken,
                                string tokenType,
                                TimeSpan? expiresIn,
                                Uri scope,
                                string refreshToken = null,
                                HttpClient httpClient = null)
            : base(httpClient)
        {
            if (applicationInfo == null)
                throw new ArgumentNullException("applicationInfo");
            if (accessToken == null)
                throw new ArgumentNullException("accessToken");
            if (tokenType == null)
                throw new ArgumentNullException("tokenType");
            if (scope == null)
                throw new ArgumentNullException("scope");

            ApplicationInfo = applicationInfo;
            AccessToken = accessToken;
            TokenType = tokenType;
            ExpiresIn = expiresIn;
            Scope = scope;
            RefreshToken = refreshToken;
        }

        protected override HttpRequestMessage CreateRequest(Uri url, HttpMethod httpMethod, HttpParameters parameters, WebRequestFormat requestFormat)
        {
            var signedParams = Sign(url, httpMethod, parameters);
            return base.CreateRequest(url, httpMethod, parameters, requestFormat);
        }

        protected abstract HttpParameters Sign(Uri url, HttpMethod method, HttpParameters parameters);
    }
}

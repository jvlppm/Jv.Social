using Jv.Web.OAuth.Extensions;
using Jv.Web.OAuth.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Jv.Web.OAuth.v2
{
    public class OAuthLogin
    {
        #region Attributes
        public KeyPair ApplicationInfo { get; private set; }
        public Uri UrlGetAuthorizationCode { get; private set; }
        public Uri UrlGetAccessToken { get; private set; }
        public Uri Scope { get; private set; }
        public HttpClient HttpClient { get; private set; }
        #endregion

        #region Constructors
        public OAuthLogin(KeyPair applicationInfo,
            Uri urlGetAuthorizationCode,
            Uri urlGetAccessToken,
            Uri scope,
            HttpClient httpClient = null)
        {
            ApplicationInfo = applicationInfo;
            UrlGetAuthorizationCode = urlGetAuthorizationCode;
            UrlGetAccessToken = urlGetAccessToken;
            Scope = scope;
            HttpClient = httpClient;
        }

        public virtual async Task<OAuthClientV2> Login(IWebAuthenticator authenticator)
        {
            if (authenticator == null)
                throw new ArgumentNullException("authenticator");

            var authorizationCode = await RequestAuthorizationCode(authenticator);
            var accessToken = await GetAccessToken(authorizationCode, authenticator);
            return DecodeAccessToken(accessToken);
        }

        protected virtual async Task<string> RequestAuthorizationCode(IWebAuthenticator authenticator)
        {
            var redirectUri = await authenticator.GetCallback();
            var loginUrl = new HttpParameters
            {
                { "response_type", "code" },
                { "client_id", ApplicationInfo.Key },
                { "redirect_uri", redirectUri.ToString() },
                { "scope", Scope.ToString() }
            }.AddToUrl(UrlGetAuthorizationCode);

            var userAuthResult = await authenticator.AuthorizeUser(loginUrl);

            return ReadUserAuthorizationCode(userAuthResult);
        }

        protected virtual async Task<dynamic> GetAccessToken(string authorizationCode, IWebAuthenticator authenticator)
        {
            var client = new WebClient();

            var redirectUri = await authenticator.GetCallback();
            return await client.Ajax(
                url: UrlGetAccessToken,
                method: HttpMethod.Post,
                parameters: new HttpParameters
                {
                    { "grant_type", "authorization_code" },
                    { "code", authorizationCode },
                    { "redirect_uri", redirectUri.ToString() },
                    { "client_id", ApplicationInfo.Key },
                    //{ "client_secret", ApplicationInfo.Secret },
                }, dataType: DataType.Json);
        }

        protected virtual string ReadUserAuthorizationCode(WebAuthenticationResult userAuthResult)
        {
            if (userAuthResult.ResponseStatus == WebAuthenticationStatus.UserCancel)
                throw new TaskCanceledException();
            if (userAuthResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                throw new WebException(userAuthResult.ResponseErrorDetail, userAuthResult.ResponseData);

            var responseData = userAuthResult.ResponseData.ParseUrlParameters();

            string authorizationCode;
            if (!responseData.TryGetValue("code", out authorizationCode))
                throw new ProtocolException("Server did not return authorization code");

            return authorizationCode;
        }

        protected OAuthClientV2 DecodeAccessToken(dynamic serverResponse)
        {
            string tokenType = serverResponse.token_type;

            Uri scope = serverResponse.scope == null? Scope : new Uri(serverResponse.scope);

            TimeSpan? expiresIn = null;
            int? expires = serverResponse.expires_in;
            if (expires != null)
                expiresIn = TimeSpan.FromSeconds((double)expires);

            switch (tokenType.ToLower())
            {
                case "bearer":
                    return new OAuthClientBearer(ApplicationInfo, serverResponse.access_token, expiresIn, scope, serverResponse.refresh_token, HttpClient);
            }

            throw new NotImplementedException("token_type of " + tokenType + " is not supported.");
        }
        #endregion
    }
}

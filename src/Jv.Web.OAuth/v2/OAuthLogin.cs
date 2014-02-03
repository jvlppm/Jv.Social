/*
 * 1) Solicita authorization grant do servidor (de uma das seguintes maneiras)
 * 2) Envia o authorization grant para buscar o AccessToken (e opcional refresh token)
 * 3) Utiliza o AccessToken para realizar os requests.
 * 
 * 4 Modos de Login (solicitar access token)
 *    - authorization code (4.1)
 *          abre o browser, para pegar o authorization code
 *          url de login:
 *              response_type: code
 *              client_id: <id string>
 *              redirect_uri?: <callback uri>
 *              scope?: <string>
 *              state? <campo opcional que deverá ser retornado pelo servidor>
 *          resposta:
 *              code: <string>
 *          repassa o authorization code e pega o access token.
 *              Envia:
 *                  grant_type: authorization_code
 *                  code: <authorization code>
 *                  redirect_uri: <callback uri>
 *                  client_id: <id string>
 *              Recebe:
 *                  <AccessToken>
 *    - implicit (4.2)
 *          modo web, necessário suporte à execução de JavaScript
 *    - resource owner password credentials (4.3)
 *          envia usuário e senha para o servidor e pega o access token.
 *    - client credentials (4.4)
 *          envia identificador interno do aplicativo, e pega o access token.
 *    - additional types
 *    
 *  Access Token Types:
 *      Definem como o pacote será assinado, contém a string access_token e opcionalmente parâmetros adicionais.
 *   - bearer (http://tools.ietf.org/html/draft-ietf-oauth-v2-bearer-23)
 *      adiciona o access_token no request
 *   - mac (http://tools.ietf.org/html/draft-ietf-oauth-v2-http-mac-04)
 *      envia uma chave mac junto com o access_token, que é usado para assinar partes do pacote.
 * */

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
        public string Scope { get; private set; }
        public HttpClient HttpClient { get; private set; }
        #endregion

        #region Constructors
        public OAuthLogin(KeyPair applicationInfo,
            Uri urlGetAuthorizationCode,
            Uri urlGetAccessToken,
            string scope,
            HttpClient httpClient = null)
        {
            ApplicationInfo = applicationInfo;
            UrlGetAuthorizationCode = urlGetAuthorizationCode;
            UrlGetAccessToken = urlGetAccessToken;
            Scope = scope;
            HttpClient = httpClient;
        }
        #endregion

        #region 4.1 - Authorization Code Grant
        public virtual async Task<OAuthClient> Login(IWebAuthenticator authenticator)
        {
            if (authenticator == null)
                throw new ArgumentNullException("authenticator");

            var authorizationCode = await RequestAuthorizationCode(authenticator);
            var accessToken = await GetAccessToken(authorizationCode, authenticator);
            return DecodeAccessToken(accessToken);
        }

        /// <summary>
        /// Requests an authorization grant from the server, which is a
        /// credential representing the resource owner's authorization,
        /// expressed using one of four grant types defined in this
        /// specification or using an extension grant type.
        /// </summary>
        /// <param name="authenticator">Platform specific UI to interact with the user.</param>
        /// <returns>Authorization code granted by the server.</returns>
        protected virtual async Task<string> RequestAuthorizationCode(IWebAuthenticator authenticator)
        {
            var redirectUri = await authenticator.GetCallback();
            var loginUrl = new HttpParameters
            {
                { "response_type", "code" },
                { "client_id", ApplicationInfo.Key },
                { "redirect_uri", redirectUri.ToString() },
                { "scope", Scope }
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
        #endregion

        protected OAuthClient DecodeAccessToken(dynamic serverResponse)
        {
            string tokenType = serverResponse.token_type;

            string scope = serverResponse.scope == null? Scope : serverResponse.scope;

            TimeSpan? expiresIn = null;
            int? expires = serverResponse.expires_in;
            if (expires != null)
                expiresIn = TimeSpan.FromSeconds((double)expires);

            switch (tokenType.ToLower())
            {
                case "bearer":
                    return new OAuthClient(ApplicationInfo, new BearerAccessToken(serverResponse.access_token, expiresIn, scope, serverResponse.refresh_token, HttpClient));
            }

            throw new NotImplementedException("token_type of " + tokenType + " is not supported.");
        }
    }
}

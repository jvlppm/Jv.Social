// http://oauth.net/core/1.0a/
using Jv.Web.OAuth.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;

namespace Jv.Web.OAuth.v1
{
    public class OAuthClient : WebClient
    {
        #region Constants
        protected static readonly Random Random = new Random(Environment.TickCount);
        #endregion

        #region Properties
        public KeyPair ApplicationInfo { get; private set; }
        public KeyPair AccessToken { get; private set; }
        #endregion

        #region Constructors
        public OAuthClient(KeyPair applicationInfo, KeyPair accessToken = null, HttpClient httpClient = null)
            : base(httpClient)
        {
            if (applicationInfo == null)
                throw new ArgumentNullException("applicationInfo");

            ApplicationInfo = applicationInfo;
            AccessToken = accessToken;
        }
        #endregion

        #region Protected Methods
        protected override HttpRequestMessage CreateRequest(Uri url, HttpMethod httpMethod, HttpParameters parameters, WebRequestFormat requestFormat)
        {
            var signedParams = Sign(url, httpMethod, parameters);
            return base.CreateRequest(url, httpMethod, signedParams, requestFormat);
        }

        protected HttpParameters Sign(Uri url, HttpMethod method, HttpParameters parameters)
        {
            var signed = new HttpParameters(parameters);
            var oAuthParams = GetOauthParameters(url, method, signed.Fields);
            signed.AddRange(oAuthParams);
            signed.Sort();
            return signed;
        }

        protected IDictionary<string, string> GetOauthParameters(Uri url, HttpMethod method, IEnumerable<KeyValuePair<string, string>> parameters = null)
        {
            parameters = parameters ?? Enumerable.Empty<KeyValuePair<string, string>>();

            var oauthParameters = new Dictionary<string, string>();

            oauthParameters["oauth_version"] = "1.0";
            oauthParameters["oauth_nonce"] = CreateNonce();
            oauthParameters["oauth_timestamp"] = CreateTimeStamp();

            oauthParameters["oauth_consumer_key"] = ApplicationInfo.Key;

            if (AccessToken != null)
                oauthParameters["oauth_token"] = AccessToken.Key;

            oauthParameters["oauth_signature_method"] = "HMAC-SHA1";
            oauthParameters["oauth_signature"] = BuildSignature(url, method, oauthParameters.Union(parameters));

            return oauthParameters;
        }

        protected static string CreateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        protected static string CreateNonce()
        {
            return Random.Next(123400, 9999999).ToString();
        }

        protected string BuildSignature(Uri url, HttpMethod method, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            parameters = parameters.OrderBy(p => p.Key);
            return GetSignature(string.Format("{0}&{1}&{2}", method, Uri.EscapeDataString(url.ToString()), parameters.AsUrlParameters().UriDataEscape()));
        }

        protected string GetSignature(string signatureBase)
        {
            string tokenSecretData = AccessToken != null ? AccessToken.Secret.UriDataEscape() : string.Empty;

            var key = (ApplicationInfo.Secret.UriDataEscape() + "&" + tokenSecretData).GetAsciiBytes();
            HMACSHA1 hmacsha1 = new HMACSHA1(key);

            byte[] dataBuffer = signatureBase.GetAsciiBytes();
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }
        #endregion
    }
}

using Jv.Web.OAuth.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace Jv.Web.OAuth.v1
{
    public class OAuthClient
    {
        #region Properties
        public KeyPair ApplicationInfo { get; private set; }
        public KeyPair Token { get; set; }
        #endregion

        #region Constructors
        public OAuthClient(KeyPair applicationInfo, KeyPair token = null)
        {
            if (applicationInfo == null)
                throw new ArgumentNullException("applicationInfo");

            ApplicationInfo = applicationInfo;
            Token = token;
        }
        #endregion

        #region Public
        /// <summary>
        /// Execute an asynchronous web request, and parse its response.
        /// </summary>
        /// <param name="url">The URI that identifies the Internet resource.</param>
        /// <param name="type">The protocol method to use in this request.</param>
        /// <param name="data">Data to be sent.</param>
        /// <param name="dataType">Requested data format.</param>
        /// <param name="requestFormat">Defines in which way data will be sent in the web request.</param>
        /// <param name="implicitNullValues">Do not send null values and assume that an undefined property, in the response object, have its value to null.</param>
        /// <param name="readResponseError">If a WebException is received, try to read its content and use it as an exception message.</param>
        /// <returns></returns>
        public async Task<dynamic> Ajax(string url,
            string type = "GET",
            HttpParameters data = null,
            DataType dataType = DataType.Automatic,
            WebRequestFormat requestFormat = WebRequestFormat.MultiPart,
            bool implicitNullValues = true,
            bool readResponseError = true)
        {
            try
            {
                var req = await CreateHttpWebRequest(type, url, data != null && implicitNullValues ? data.NotNullParameters : data, requestFormat);
                var resp = await req.Request(dataType);
                if (resp is IDictionary<string, object> && implicitNullValues)
                    return new SafeResponse(resp);
                return resp;
            }
            catch (WebException ex)
            {
                if (readResponseError)
                    throw new WebException(ex.Response.GetResponseString(), ex, ex.Status, ex.Response);
                throw ex;
            }
        }

        public HttpParameters Sign(string requestType, string url, HttpParameters parameters)
        {
            var signed = new HttpParameters(parameters);
            var oAuthParams = GetOauthParameters(requestType, url, signed.Fields);
            signed.AddRange(oAuthParams);
            return signed;
        }

        public IDictionary<string, string> GetOauthParameters(string requestType, string url, IEnumerable<KeyValuePair<string, string>> parameters = null)
        {
            parameters = parameters ?? Enumerable.Empty<KeyValuePair<string, string>>();

            var oauthParameters = new Dictionary<string, string>();

            oauthParameters["oauth_version"] = "1.0";
            oauthParameters["oauth_nonce"] = CreateNonce();
            oauthParameters["oauth_timestamp"] = CreateTimeStamp();

            oauthParameters["oauth_consumer_key"] = ApplicationInfo.Key;

            if (Token != null && !string.IsNullOrEmpty(Token.Key))
                oauthParameters["oauth_token"] = Token.Key;

            oauthParameters["oauth_signature_method"] = "HMAC-SHA1";
            oauthParameters["oauth_signature"] = BuildSignature(requestType, url, oauthParameters.Union(parameters));

            return oauthParameters;
        }
        #endregion

        #region Private
        private async Task<WebRequest> CreateHttpWebRequest(string type, string url, HttpParameters parameters, WebRequestFormat requestFormat)
        {
            parameters = Sign(type, url, parameters);

            switch (requestFormat)
            {
                case WebRequestFormat.MultiPart:
                    return await HttpUtils.CreateHttpWebRequest(type, url, parameters);

                case WebRequestFormat.MixedUrlMultipart:
                    string urlWithParams = HttpUtils.BuildUrl(url, parameters.Fields);
                    return await HttpUtils.CreateHttpWebRequest(type, urlWithParams, parameters.FileParameters);
            }

            throw new NotImplementedException();
        }

        static Random Random = new Random(Environment.TickCount);

        static string CreateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        static string CreateNonce()
        {
            return Random.Next(123400, 9999999).ToString();
        }

        string BuildSignature(string type, string url, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            return GetSignature(string.Format("{0}&{1}&{2}", type, Uri.EscapeDataString(url), parameters.AsUrlParameters().UriDataEscape()));
        }

        string GetSignature(string signatureBase)
        {
            string consumerSecret = Uri.EscapeDataString(ApplicationInfo.Secret);
            string tokenSecret = string.Empty;
            if (Token != null && Token.Secret != null)
                tokenSecret = Uri.EscapeDataString(Token.Secret);

            IBuffer keyMaterial = (consumerSecret + "&" + tokenSecret).AsBufferAscii();
            MacAlgorithmProvider hmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            CryptographicKey macKey = hmacSha1Provider.CreateKey(keyMaterial);
            IBuffer dataToBeSigned = signatureBase.AsBufferAscii();
            IBuffer signatureBuffer = CryptographicEngine.Sign(macKey, dataToBeSigned);
            return CryptographicBuffer.EncodeToBase64String(signatureBuffer);
        }
        #endregion
    }
}

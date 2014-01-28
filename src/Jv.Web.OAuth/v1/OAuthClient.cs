// http://oauth.net/core/1.0a/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Jv.Web.OAuth.Extensions;
using System.Security.Cryptography;

namespace Jv.Web.OAuth.v1
{
    public class OAuthClient
    {
        #region Constants
        static readonly Random Random = new Random(Environment.TickCount);
        #endregion

        #region Properties
        public KeyPair ApplicationInfo { get; private set; }
        public KeyPair AccessToken { get; private set; }

        public HttpClient HttpClient { get; private set; }
        #endregion

        #region Constructors
        public OAuthClient(KeyPair applicationInfo, KeyPair accessToken = null, HttpClient httpClient = null)
        {
            if (applicationInfo == null)
                throw new ArgumentNullException("applicationInfo");

            ApplicationInfo = applicationInfo;
            AccessToken = accessToken;
            HttpClient = httpClient ?? new HttpClient();
        }
        #endregion

        public Task<dynamic> Ajax(string url,
            string method = "GET",
            HttpParameters parameters = null,
            DataType dataType = DataType.Automatic,
            WebRequestFormat requestFormat = WebRequestFormat.MultiPart)
        {
            return Ajax(new Uri(url), new HttpMethod(method), parameters, dataType, requestFormat);
        }

        public Task<dynamic> Ajax(Uri url,
            HttpParameters parameters = null,
            DataType dataType = DataType.Automatic,
            WebRequestFormat requestFormat = WebRequestFormat.MultiPart)
        {
            return Ajax(url, HttpMethod.Get, parameters, dataType, requestFormat);
        }

        public async Task<dynamic> Ajax(Uri url,
            HttpMethod method,
            HttpParameters parameters = null,
            DataType dataType = DataType.Automatic,
            WebRequestFormat requestFormat = WebRequestFormat.MultiPart)
        {
            var req = CreateRequest(url, method, parameters, requestFormat);
            var resp = await HttpClient.SendAsync(req);

            if (resp.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var errorData = await resp.Content.ReadAsDynamicAsync();
                throw new WebException(resp.StatusCode, errorData);
            }

            var respData = await resp.Content.ReadAsDynamicAsync(dataType);
            return SafeObject.Create(respData);
        }

        protected virtual HttpRequestMessage CreateRequest(Uri url, HttpMethod httpMethod, HttpParameters parameters, WebRequestFormat requestFormat)
        {
            parameters = Sign(url, httpMethod, parameters);

            switch (requestFormat)
            {
                case WebRequestFormat.MultiPart:
                    return CreateHttpWebRequest(url, httpMethod, parameters);

                case WebRequestFormat.MixedUrlMultipart:
                    Uri urlWithParams = BuildUrl(url, parameters.Fields);
                    return CreateHttpWebRequest(urlWithParams, httpMethod, parameters.FileParameters);
            }

            throw new NotImplementedException();
        }

        private static HttpRequestMessage CreateHttpWebRequest(Uri url, HttpMethod httpMethod, HttpParameters parameters)
        {
            IList<Tuple<string, HttpContent, string>> requestContent = new List<Tuple<string, HttpContent, string>>();
            MultipartFormDataContent mpart = new MultipartFormDataContent();

            if (parameters != null && httpMethod != HttpMethod.Get)
            {
                foreach (var p in parameters.Fields)
                    requestContent.Add(new Tuple<string, HttpContent, string>(p.Key, new StringContent(p.Value), null));
            }

            foreach (var f in parameters.Files)
                requestContent.Add(new Tuple<string, HttpContent, string>(f.Key, new StreamContent(f.Value.Content), f.Value.Name));

            foreach (var content in requestContent.OrderBy(i => i.Item1))
            {
                if (content.Item3 == null)
                    mpart.Add(content.Item2, content.Item1);
                else
                    mpart.Add(content.Item2, content.Item1, content.Item3);
            }

            var requestUrl = httpMethod == HttpMethod.Get ? BuildUrl(url, parameters.Fields) : url;
            return new HttpRequestMessage(httpMethod, requestUrl)
            {
                Content = mpart.Any() ? mpart : null
            };
        }

        #region Protected Methods
        protected static Uri BuildUrl(Uri url, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (!parameters.Any())
                return url;

            var orderedParams = parameters.OrderBy(p => p.Key);
            return new Uri(url + "?" + orderedParams.AsUrlParameters());
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

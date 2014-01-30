// http://oauth.net/core/1.0a/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Jv.Web.OAuth.Extensions;
using System.Security.Cryptography;

namespace Jv.Web.OAuth
{
    public abstract class OAuthClientBase
    {
        #region Constants
        protected static readonly Random Random = new Random(Environment.TickCount);
        #endregion

        #region Properties
        public KeyPair ApplicationInfo { get; private set; }
        public HttpClient HttpClient { get; private set; }
        #endregion

        #region Constructors
        public OAuthClientBase(KeyPair applicationInfo, HttpClient httpClient = null)
        {
            if (applicationInfo == null)
                throw new ArgumentNullException("applicationInfo");

            ApplicationInfo = applicationInfo;
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

        protected static Uri BuildUrl(Uri url, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (!parameters.Any())
                return url;

            var orderedParams = parameters.OrderBy(p => p.Key);
            return new Uri(url + "?" + orderedParams.AsUrlParameters());
        }

        protected abstract HttpParameters Sign(Uri url, HttpMethod method, HttpParameters parameters);
    }
}

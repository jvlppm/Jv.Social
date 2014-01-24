using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Jv.Web.OAuth.Extensions;

namespace Jv.Web.OAuth.v1
{
    public class OAuthClient
    {
        #region Properties
        public KeyPair ApplicationInfo { get; private set; }
        private KeyPair AccessToken { get; set; }

        public HttpClient HttpClient { get; private set; }
        #endregion

        #region Constructors
        public OAuthClient(KeyPair applicationInfo, KeyPair accessToken = null, HttpClient httpClient = null)
        {
            ApplicationInfo = applicationInfo;
            AccessToken = accessToken;
            HttpClient = httpClient ?? new HttpClient();
        }
        #endregion

        public Task<dynamic> Ajax(Uri url,
            string method = "GET",
            HttpParameters parameters = null)
        {
            return Ajax(url.ToString(), method, parameters);
        }

        public async Task<dynamic> Ajax(string url,
            string method = "GET",
            HttpParameters parameters = null)
        {
            //try
            //{
                var req = CreateRequest(url, method, parameters);
                var resp = await HttpClient.SendAsync(req);
                return SafeObject.Create(resp);
            //}
            //catch (System.Net.Http.HttpRequestException ex)
            //{
            //    ex.
            //    throw new WebException(ex.Response.GetResponseString(), ex, ex.Status, ex.Response);
            //}
        }

        protected virtual HttpRequestMessage CreateRequest(string url, string method, HttpParameters parameters)
        {
            var httpMethod = new HttpMethod(method);

            MultipartFormDataContent mpart = new MultipartFormDataContent();

            if(parameters != null && httpMethod != HttpMethod.Get)
            {
                foreach (var p in parameters.Fields)
                    mpart.Add(new StringContent(p.Value), p.Key);
            }

            foreach (var f in parameters.Files)
                mpart.Add(new StreamContent(f.Value.Content), f.Key);

            var requestUrl = httpMethod == HttpMethod.Get? BuildUrl(url, parameters.Fields) : url;
            return new HttpRequestMessage(httpMethod, requestUrl)
            {
                Content = mpart.Any()? mpart : null
            };
        }

        #region Private Methods
        static string BuildUrl(string url, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (!parameters.Any())
                return url;

            var orderedParams = parameters.OrderBy(p => p.Key);
            return url + "?" + orderedParams.AsUrlParameters();
        }
        #endregion
    }
}

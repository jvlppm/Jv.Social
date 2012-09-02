using Jv.Web.OAuth.Extensions;
using Jv.Web.OAuth.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Jv.Web.OAuth
{
    public static class HttpUtils
    {
        public static async Task<WebRequest> CreateHttpWebRequest(string type, string url, HttpParameters parameters = null)
        {
            parameters = parameters ?? new HttpParameters();

            var mpart = new MultipartBuilder();

            if (type != "GET")
            {
                foreach (var p in parameters.Fields)
                    mpart.AddField(p.Key, p.Value);
            }

            foreach (var f in parameters.Files)
                await mpart.AddFileAsync(f.Key, f.Value);

            var req = HttpWebRequest.Create(type == "GET" ? BuildUrl(url, parameters.Fields) : url);
            req.Method = type.ToString();

            if (!mpart.IsEmpty)
            {
                req.ContentType = mpart.ContentType;
                using (var reqStream = await req.GetRequestStreamAsync())
                    await mpart.CopyToAsync(reqStream);
            }
            return req;
        }

        public static async Task<dynamic> Ajax(string url,
                    HttpParameters data = null,
                    string type = "GET",
                    DataType dataType = DataType.Automatic)
        {
            var ajaxRequest = await CreateHttpWebRequest(type.ToString(), url, data);
            return await ajaxRequest.Request(dataType);
        }

        public static string BuildUrl(string url, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (!parameters.Any())
                return url;

            return url + "?" + parameters.AsUrlParameters();
        }
    }
}

using Jv.Web.OAuth.Extensions;
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

        public static string BuildUrl(string url, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (!parameters.Any())
                return url;

            return url + "?" + parameters.AsUrlParameters();
        }
    }
}

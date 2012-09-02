using Jv.Json;
using Jv.Web.OAuth.v1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Web.OAuth.Extensions
{
    public static class WebExtensions
    {
        public static string UriDataEscape(this string str)
        {
            StringBuilder result = new StringBuilder();
            foreach (var chunk in str.Split(32766))
                result.Append(Uri.EscapeDataString(chunk));
            return result.ToString();
        }

        public static string AsUrlParameters(this IEnumerable<KeyValuePair<string, string>> parameters)
        {
            StringBuilder queryString = new StringBuilder();

            foreach (var param in parameters.Where(kv => kv.Value != null).OrderBy(kv => kv.Key))
            {
                if (queryString.Length != 0)
                    queryString.Append("&");

                queryString.Append(param.Key.UriDataEscape());
                queryString.Append("=");
                queryString.Append(param.Value.UriDataEscape());
            }

            return queryString.ToString();
        }

        public static IDictionary<string, string> ParseUrlParameters(this string query)
        {
            return (from parameter in query.Substring(query.IndexOf('?') + 1).Split('&')
                    let components = parameter.Split('=')
                    let name = Uri.UnescapeDataString(components[0])
                    let value = Uri.UnescapeDataString(components[1])
                    select new KeyValuePair<string, string>(name, value))
                   .ToDictionary(k => k.Key, k => k.Value);
        }

        public static async Task<dynamic> Request(this WebRequest req, DataType dataType = DataType.Automatic)
        {
            return (await req.GetResponseAsync()).ReadResponse(dataType);
        }

        public static string GetResponseString(this WebResponse response)
        {
            using (var reader = new StreamReader(response.GetResponseStream()))
                return reader.ReadToEnd();
        }

        public static dynamic ReadResponse(this WebResponse webResponse, DataType dataType = DataType.Automatic)
        {
            using (var response = webResponse)
            {
                if (dataType == DataType.Automatic)
                {
                    switch (response.ContentType)
                    {
                        case "application/json":
                            dataType = DataType.Json;
                            break;
                        default:
                            dataType = DataType.Text;
                            break;
                    }
                }

                switch (dataType)
                {
                    case DataType.Text:
                        return response.GetResponseString();

                    case DataType.Json:
                        return response.GetResponseString().AsJson();

                    case DataType.UrlEncoded:
                        return response.GetResponseString().ParseUrlParameters().ToExpandoObject();
                }
            }

            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Jv.Web.OAuth.Json;

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

        //TODO: Validar acesso com UTF8.
        public static byte[] GetAsciiBytes(this string str)
        {
            return str.ToCharArray().Select(c => (byte)c).ToArray();
        }

        public static string AsUrlParameters(this IEnumerable<KeyValuePair<string, string>> parameters)
        {
            StringBuilder queryString = new StringBuilder();

            foreach (var param in parameters.Where(kv => kv.Value != null))
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

        public static async Task<dynamic> ReadAsDynamicAsync(this HttpContent content, DataType dataType = DataType.Automatic)
        {
            if (dataType == DataType.Automatic)
            {
                switch (content.Headers.ContentType.MediaType)
                {
                    case "application/json":
                        dataType = DataType.Json;
                        break;

                    case "text/plain":
                    default:
                        dataType = DataType.Text;
                        break;
                }
            }

            var responseString = await content.ReadAsStringAsync();

            switch (dataType)
            {
                case DataType.Text:
                    return responseString;

                case DataType.Json:
                    return responseString.AsJson();

                case DataType.UrlEncoded:
                    return responseString.ParseUrlParameters().ToExpandoObject();

                default:
                    throw new NotImplementedException("Reading data as " + dataType + " is not supported.");
            }
        }
    }
}

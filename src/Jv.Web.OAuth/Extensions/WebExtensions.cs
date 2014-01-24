using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}

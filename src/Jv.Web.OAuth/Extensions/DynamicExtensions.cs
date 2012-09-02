using System.Collections.Generic;
using System.Dynamic;

namespace Jv.Web.OAuth.Extensions
{
    public static class DynamicExtensions
    {
        public static ExpandoObject ToExpandoObject<ValueType>(this IDictionary<string, ValueType> dictionary)
        {
            ExpandoObject exp = new ExpandoObject();

            foreach (var kv in dictionary)
                ((IDictionary<string, object>)exp).Add(kv.Key, kv.Value);

            return exp;
        }
    }
}

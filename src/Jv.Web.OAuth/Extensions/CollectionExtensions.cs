using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Web.OAuth.Extensions
{
    public static class CollectionExtensions
    {
        public static void Add<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>> collection, TKey key, TValue value)
        {
            collection.Add(new KeyValuePair<TKey,TValue>(key, value));
        }

        public static HttpParameters Union(this HttpParameters first, IEnumerable<KeyValuePair<string, string>> second)
        {
            var result = new HttpParameters(first);
            result.AddRange(second);
            return result;
        }
    }
}

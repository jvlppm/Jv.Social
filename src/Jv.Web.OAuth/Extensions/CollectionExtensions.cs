using System.Linq;
using System.Collections.Generic;

namespace Jv.Web.OAuth.Extensions
{
    public static class CollectionExtensions
    {
        public static void Add<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>> collection, TKey key, TValue value)
        {
            collection.Add(new KeyValuePair<TKey,TValue>(key, value));
        }

        public static void Add<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>?> collection, TKey key, TValue value)
        {
            collection.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        {
            return enumerable.ToDictionary(k => k.Key, k => k.Value);
        }

        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            foreach (var kv in collection)
                dictionary.Add(kv.Key, kv.Value);
        }

        public static HttpParameters Union(this HttpParameters first, IEnumerable<KeyValuePair<string, string>> second)
        {
            var result = new HttpParameters();
            result.AddRange(first);
            result.AddRange(second);
            return result;
        }
    }
}

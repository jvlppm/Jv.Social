using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Web.OAuth.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<string> Split(this string str, int chunkSize)
        {
            var lastPartSize = str.Length % chunkSize;

            foreach (var chunk in Enumerable.Range(0, str.Length / chunkSize).Select(i => str.Substring(i * chunkSize, chunkSize)))
                yield return chunk;

            yield return str.Substring(str.Length - lastPartSize);
        }
    }
}

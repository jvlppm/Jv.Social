using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Storage.Streams;

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

        public static IBuffer AsBufferUTF8(this string str)
        {
            using (var bw = new System.IO.BinaryWriter(new MemoryStream()))
            {
                bw.Write(Encoding.UTF8.GetBytes(str));
                using (var dr = new DataReader(bw.BaseStream.AsInputStream()))
                    return dr.DetachBuffer();
            }
        }
    }
}

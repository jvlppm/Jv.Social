using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social
{
    static class Extensions
    {
        public async static Task<T> Select<TSource, T>(this Task<TSource> task, Func<TSource, T> selector)
        {
            return selector(await task);
        }
    }
}

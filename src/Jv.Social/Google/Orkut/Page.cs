using Jv.Web.OAuth;
using System.Collections.Generic;
using System.Linq;

namespace Jv.Social.Google.Orkut
{
    public class Page<T> : DynamicWrapper where T : DynamicWrapper
    {
        public T[] List { get; private set; }

        public int TotalResults { get { return Object.totalResults; } }

        public Page(SafeObject d)
            : base(d)
        {
            var list = (IEnumerable<dynamic>)Object.list;
            List = list.Select(dl => DynamicWrapper.Create<T>((SafeObject)dl)).ToArray();
        }
    }
}

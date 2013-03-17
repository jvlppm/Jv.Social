using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jv.Social.Google.Orkut
{
    public class Page<T> : DynamicWrapper where T : DynamicWrapper
    {
        public T[] List { get; private set; }

        public int TotalResults { get { return Object.totalResults; } }

        public Page(dynamic d) : base((object)d)
        {
            var list = (IEnumerable<dynamic>)d.list;
            List = list.Select(DynamicWrapper.Create<T>).ToArray();
        }
    }
}

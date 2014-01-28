using Jv.Web.OAuth;
using System.Collections.Generic;
using System.Linq;

namespace Jv.Social.Google.Orkut
{
    public class Page<T> : DynamicWrapper where T : DynamicWrapper
    {
        public T[] List { get; private set; }

        public bool Filtered { get { return Object.filtered; } }
        public int StartIndex { get { return Object.startIndex; } }
        public int TotalResults { get { return Object.totalResults; } }
        //updatedSince: false

        public Page(SafeObject d)
            : base(d)
        {
            var list = (SafeList)Object.list;
            List = list.OfType<SafeObject>().Select(dl => DynamicWrapper.Create<T>((SafeObject)dl)).ToArray();
        }
    }
}

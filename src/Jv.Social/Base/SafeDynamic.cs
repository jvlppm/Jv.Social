using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Jv.Social
{
    internal class SafeDynamic : DynamicObject
    {
        IDictionary<string, object> _obj;
        public SafeDynamic(IDictionary<string, object> dict)
        {
            _obj = dict;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_obj.ContainsKey(binder.Name))
            {
                result = _obj[binder.Name];
                var resultAsDict = result as IDictionary<string, object>;
                if (resultAsDict != null)
                    result = new SafeDynamic(resultAsDict);
            }
            else
                result = null;

            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _obj.Keys;
        }
    }
}

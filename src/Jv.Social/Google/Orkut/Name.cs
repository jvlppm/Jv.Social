using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social.Google.Orkut
{
    [DebuggerDisplay("\"{GivenName,nq} {FamilyName,nq}\"")]
    public class Name : DynamicWrapper
    {
        public Name(dynamic obj)
            : base((object)obj)
        {
        }

        public string GivenName { get { return Object.givenName; } }
        public string FamilyName { get { return Object.familyName; } }
    }
}

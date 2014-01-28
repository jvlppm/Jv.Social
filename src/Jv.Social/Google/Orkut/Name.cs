using Jv.Web.OAuth;
using System.Diagnostics;

namespace Jv.Social.Google.Orkut
{
    [DebuggerDisplay("\"{GivenName,nq} {FamilyName,nq}\"")]
    public class Name : DynamicWrapper
    {
        public Name(SafeObject obj)
            : base(obj)
        {
        }

        public string GivenName { get { return Object.givenName; } }
        public string FamilyName { get { return Object.familyName; } }
    }
}

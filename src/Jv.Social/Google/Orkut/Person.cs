using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social.Google.Orkut
{
    [DebuggerDisplay("\"{Name.GivenName,nq} {Name.FamilyName,nq}\"")]
    public class Person : Object
    {
        internal Person(dynamic person)
            : base((object)person)
        {
            if (person.name != null)
                Name = new Name(person.name);
        }

        public Name Name { get; private set; }
        public string ThumbnailUrl { get { return Object.thumbnailUrl; } }
    }
}

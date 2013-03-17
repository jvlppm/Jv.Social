using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Jv.Social.Google.Orkut
{
    public class Object : DynamicWrapper
    {
        internal Object(dynamic obj) : base((object)obj)
        {
        }

        public string Id { get { return Object.id; } }
    }
}

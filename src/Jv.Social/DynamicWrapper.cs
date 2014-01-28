using Jv.Web.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social
{
    public abstract class DynamicWrapper
    {
        public dynamic Object { get; private set; }

        public DynamicWrapper(SafeObject obj)
        {
            Object = obj;
        }

        public static T Create<T>(SafeObject obj) where T : DynamicWrapper
        {
            Type type = typeof(T);
            ConstructorInfo ctor = (from ct in type.GetTypeInfo().DeclaredConstructors
                                    let prms = ct.GetParameters()
                                    where
                                         prms.Length == 1 &&
                                         prms[0].ParameterType == typeof(SafeObject)
                                    select ct).Single();

            return (T)ctor.Invoke(new object[] { obj });
        }
    }
}

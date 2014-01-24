using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Web.OAuth
{
    public static class IoC
    {
        static class Creator<T>
        {
            static Func<T> _func;

            public static void Register(Func<T> creator)
            {
                _func = creator;
            }

            public static T Create()
            {
                return _func();
            }
        }

        public static void Register<T>(Func<T> creator)
        {
            Creator<T>.Register(creator);
        }

        public static T Create<T>()
        {
            return Creator<T>.Create();
        }
    }
}

using Jv.Web.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social
{
    public sealed class ApplicationInfo
    {
        internal KeyPair KeyPair { get; private set; }

        public ApplicationInfo(string id, string secret)
        {
            KeyPair = new KeyPair(id, secret);
        }

        internal ApplicationInfo(KeyPair keyPair)
        {
            KeyPair = keyPair;
        }

        public string ClientId { get { return KeyPair.Key; } }
        public string ClientSecret { get { return KeyPair.Secret; } }
    }
}

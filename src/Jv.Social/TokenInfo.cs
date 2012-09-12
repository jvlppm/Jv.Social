using Jv.Web.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social
{
    public sealed class TokenInfo
    {
        internal KeyPair KeyPair { get; private set; }

        public TokenInfo(string token, string tokenSecret)
        {
            KeyPair = new KeyPair(token, tokenSecret);
        }

        internal TokenInfo(KeyPair keyPair)
        {
            KeyPair = keyPair;
        }

        public string Token { get { return KeyPair.Key; } }
        public string TokenSecret { get { return KeyPair.Secret; } }

        public override bool Equals(object obj)
        {
            if(obj is KeyPair)
                return KeyPair.Equals(obj as KeyPair);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return TokenSecret.GetHashCode();
        }
    }
}

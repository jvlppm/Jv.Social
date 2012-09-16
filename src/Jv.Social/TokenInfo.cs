using Jv.Web.OAuth;
using System.Diagnostics;

namespace Jv.Social
{
    public sealed class TokenInfo
    {
        #region Static
        public static TokenInfo Parse(string tokenInfo)
        {
            return new TokenInfo(KeyPair.Parse(tokenInfo));
        }
        #endregion

        #region Properties
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal KeyPair KeyPair { get; private set; }

        public string Token { get { return KeyPair.Key; } }
        public string TokenSecret { get { return KeyPair.Secret; } }
        #endregion

        #region Constructors
        public TokenInfo(string token, string tokenSecret)
        {
            KeyPair = new KeyPair(token, tokenSecret);
        }
        internal TokenInfo(KeyPair keyPair)
        {
            KeyPair = keyPair;
        }
        #endregion

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
        public override string ToString()
        {
            return KeyPair.ToString();
        }
    }
}

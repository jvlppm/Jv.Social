using Jv.Web.OAuth;
using System.Diagnostics;

namespace Jv.Social
{
    public sealed class KeyInfo
    {
        #region Static
        public static KeyInfo Parse(string tokenInfo)
        {
            return new KeyInfo(KeyPair.Parse(tokenInfo));
        }
        #endregion

        #region Properties
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal KeyPair KeyPair { get; private set; }

        public string Key { get { return KeyPair.Key; } }
        public string Secret { get { return KeyPair.Secret; } }
        #endregion

        #region Constructors
        public KeyInfo(string key, string secret)
        {
            KeyPair = new KeyPair(key, secret);
        }
        internal KeyInfo(KeyPair keyPair)
        {
            KeyPair = keyPair;
        }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj is KeyPair)
                return KeyPair.Equals(obj as KeyPair);
            if (obj is KeyInfo)
                return KeyPair.Equals(((KeyInfo)obj).KeyPair);

            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return Secret.GetHashCode();
        }
        public override string ToString()
        {
            return KeyPair.ToString();
        }
    }
}

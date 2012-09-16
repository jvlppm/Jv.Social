using Jv.Web.OAuth;
using System.Diagnostics;

namespace Jv.Social
{
    public sealed class ApplicationInfo
    {
        #region Static
        public static ApplicationInfo Parse(string tokenInfo)
        {
            return new ApplicationInfo(KeyPair.Parse(tokenInfo));
        }
        #endregion

        #region Properties
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal KeyPair KeyPair { get; private set; }
        public string ClientId { get { return KeyPair.Key; } }
        public string ClientSecret { get { return KeyPair.Secret; } }
        #endregion

        #region Constructors
        public ApplicationInfo(string id, string secret)
        {
            KeyPair = new KeyPair(id, secret);
        }

        internal ApplicationInfo(KeyPair keyPair)
        {
            KeyPair = keyPair;
        }
        #endregion
    }
}

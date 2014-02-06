using Jv.Web.OAuth.Authentication;
using System.Net.Http;
using System.Threading.Tasks;

namespace Jv.Web.OAuth.v2
{
    public interface IOAuthLogin
    {
        Task<OAuthClient> Login(IWebAuthenticator authenticator);
        Task<OAuthAccessToken> RefreshToken(string refreshToken);
    }

    public static class OAuthLoginExtensions
    {
        public static async Task<OAuthClient> Login(this IOAuthLogin login, KeyPair applicationInfo, string refreshToken, HttpClient httpClient = null)
        {
            var accessToken = await login.RefreshToken(refreshToken);
            return new OAuthClient(applicationInfo, accessToken, login, httpClient);
        }
    }
}

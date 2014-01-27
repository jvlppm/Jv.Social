using System;
using System.Threading.Tasks;

namespace Jv.Web.OAuth.Authentication
{
    public interface IWebAuthenticator : IDisposable
    {
        Task<Uri> GetCallback();
        Task<WebAuthenticationResult> AuthorizeUser(Uri requestUri);
    }
}

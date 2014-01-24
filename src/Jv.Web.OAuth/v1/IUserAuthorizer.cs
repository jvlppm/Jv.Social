using System;
using System.Threading.Tasks;

namespace Jv.Web.OAuth.v1
{
    public interface IUserAuthorizer : IDisposable
    {
        Task<Uri> GetCallback();
        Task<UserAuthorizationResult> AuthorizeUser(Uri requestUri);
    }
}

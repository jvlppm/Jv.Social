using Jv.Web.OAuth;
using System.Net;
using System.Threading.Tasks;

namespace Jv.Social.Google.Orkut
{
    public interface IRpc
    {
        string Method { get; }
        HttpParameters Parameters { get; }
        Task Task { get; }
        void SetResult(dynamic res);
        void SetError(HttpStatusCode code, dynamic res);
    }

    public interface IRpc<T> : IRpc
    {
        new Task<T> Task { get; }
    }
}

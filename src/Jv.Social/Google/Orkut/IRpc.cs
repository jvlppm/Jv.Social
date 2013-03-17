using Jv.Web.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social.Google.Orkut
{
    public interface IRpc
    {
        string Method { get; }
        HttpParameters Parameters { get; }

        Task Task { get; }
        void SetResult(dynamic res);
        void SetError(dynamic res);
    }

    public interface IRpc<T> : IRpc
    {
        new Task<T> Task { get; }
    }
}

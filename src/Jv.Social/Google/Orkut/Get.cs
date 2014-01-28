using Jv.Web.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social.Google.Orkut
{
    public abstract class Get<T> : IRpc<T> where T : DynamicWrapper
    {
        TaskCompletionSource<T> _taskCompletion { get; set; }

        public string Method { get; private set; }
        public HttpParameters Parameters { get; private set; }

        public Task<T> Task
        {
            get { return _taskCompletion.Task; }
        }

        Task IRpc.Task
        {
            get { return Task; }
        }

        public Get(string method)
        {
            Method = method;
            Parameters = new Web.OAuth.HttpParameters();
            _taskCompletion = new TaskCompletionSource<T>();
        }

        public void SetResult(dynamic res)
        {
            _taskCompletion.SetResult(DynamicWrapper.Create<T>((SafeObject)res));
        }

        public void SetError(HttpStatusCode code, dynamic res)
        {
            _taskCompletion.SetException(new Jv.Web.OAuth.WebException(code, res, res.error.message));
        }
    }
}

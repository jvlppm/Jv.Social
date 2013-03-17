using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social.Google.Orkut
{
    public abstract class Get<T> : IRpc<T> where T : DynamicWrapper
    {
        TaskCompletionSource<T> _taskCompletion { get; set; }

        public string Method { get; private set; }

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
            _taskCompletion.SetResult(DynamicWrapper.Create<T>(res));
        }

        public void SetError(dynamic res)
        {
            throw new NotImplementedException();
            //_taskCompletion.SetException();
        }

        public Web.OAuth.HttpParameters Parameters { get; private set; }
    }
}

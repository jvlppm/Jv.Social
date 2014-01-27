using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jv.Web.OAuth.Extensions
{
    public static class AsyncExtensions
    {
        public static Task<T> OrCancel<T>(this Task<T> task, CancellationToken cancellation, Action onCancel)
        {
            if (onCancel == null)
                throw new ArgumentNullException("onCancel");

            var tcs = new TaskCompletionSource<T>();

            cancellation.Register(() => tcs.TrySetCanceled());
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.TrySetException(t.Exception);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(t.Result);
            });

            tcs.Task.ContinueWith(t =>
            {
                onCancel();
            }, TaskContinuationOptions.OnlyOnCanceled);

            return tcs.Task;
        }
    }
}

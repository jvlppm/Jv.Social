using System;

namespace Jv.Web.OAuth.Helpers
{
    /// <summary>
    /// Provides a set of static methods for creating Disposables.
    /// </summary>
    public static class Disposable
    {
        #region Nested
        class ActionDisposable : IDisposable
        {
            Action _onDispose;

            public ActionDisposable(Action onDispose)
            {
                _onDispose = onDispose;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            ~ActionDisposable()
            {
                Dispose(false);
            }

            void Dispose(bool disposing)
            {
                Action onDispose;
                lock (this)
                {
                    onDispose = _onDispose;
                    _onDispose = null;
                }
                if (onDispose != null)
                    onDispose();
            }
        }
        #endregion

        static Disposable()
        {
            Empty = new ActionDisposable(null);
        }

        /// <summary>
        /// Gets a disposable that does nothing when disposed.
        /// </summary>
        public static IDisposable Empty { get; private set; }

        /// <summary>
        /// Creates a disposable that invokes the specified action when disposed.
        /// </summary>
        /// <param name="dispose">The action to run during IDisposable.Dispose.</param>
        /// <returns>The disposable object that runs the given action upon disposal.</returns>
        public static IDisposable Create(Action dispose)
        {
            return new ActionDisposable(dispose);
        }
    }
}

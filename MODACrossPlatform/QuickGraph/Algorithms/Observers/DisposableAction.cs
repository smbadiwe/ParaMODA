using System;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.Observers
{
    struct DisposableAction
        : IDisposable
    {
        public delegate void Action();

        Action action;

        public DisposableAction(Action action)
        {
            Contract.Requires(action != null);
            this.action = action;
        }

        public void Dispose()
        {
            var a = this.action;
            this.action = null;
            if (a != null)
                a();
        }
    }
}

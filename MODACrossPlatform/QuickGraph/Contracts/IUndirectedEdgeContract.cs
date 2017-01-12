using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Contracts
{
    [ContractClassFor(typeof(IUndirectedEdge<>))]
    abstract class IUndirectedEdgeContract<TVertex>
        : IUndirectedEdge<TVertex>
    {
        [ContractInvariantMethod]
        void IUndirectedEdgeInvariant()
        {
            IUndirectedEdge<TVertex> ithis = this;
            Contract.Invariant(Comparer<TVertex>.Default.Compare(ithis.Source, ithis.Target) <= 0);
        }

        #region IEdge<TVertex> Members

        public TVertex Source {
          get { throw new NotImplementedException(); }
        }

        public TVertex Target {
          get { throw new NotImplementedException(); }
        }

        #endregion
    }
}

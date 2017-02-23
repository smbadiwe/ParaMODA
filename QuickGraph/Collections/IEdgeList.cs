using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Collections
{
    /// <summary>
    /// A cloneable list of edges
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    [ContractClass(typeof(IEdgeListContract<>))]
    public interface IEdgeList<TVertex>
        : IList<Edge<TVertex>>
        #if !SILVERLIGHT
        , ICloneable
        #endif
    {
        /// <summary>
        /// Trims excess allocated space
        /// </summary>
        void TrimExcess();
        /// <summary>
        /// Gets a clone of this list
        /// </summary>
        /// <returns></returns>
#if !SILVERLIGHT
        new 
#endif
        IEdgeList<TVertex> Clone();
    }
}

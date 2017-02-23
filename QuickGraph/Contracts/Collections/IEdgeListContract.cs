using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph.Collections
{
    [ContractClassFor(typeof(IEdgeList<>))]
    abstract class IEdgeListContract<TVertex> 
        : IEdgeList<TVertex>
    {
        IEdgeList<TVertex> IEdgeList<TVertex>.Clone()
        {
            Contract.Ensures(Contract.Result<IEdgeList<TVertex>>() != null);
            throw new NotImplementedException();
        }

        void IEdgeList<TVertex>.TrimExcess()
        { }

        #region others
        int IList<Edge<TVertex>>.IndexOf(Edge<TVertex> item)
        {
            throw new NotImplementedException();
        }

        void IList<Edge<TVertex>>.Insert(int index, Edge<TVertex> item)
        {
            throw new NotImplementedException();
        }

        void IList<Edge<TVertex>>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        Edge<TVertex> IList<Edge<TVertex>>.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void ICollection<Edge<TVertex>>.Add(Edge<TVertex> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<Edge<TVertex>>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<Edge<TVertex>>.Contains(Edge<TVertex> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<Edge<TVertex>>.CopyTo(Edge<TVertex>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<Edge<TVertex>>.Count
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<Edge<TVertex>>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<Edge<TVertex>>.Remove(Edge<TVertex> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<Edge<TVertex>> IEnumerable<Edge<TVertex>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

#if!SILVERLIGHT
        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }
#endif
        #endregion
    }
}

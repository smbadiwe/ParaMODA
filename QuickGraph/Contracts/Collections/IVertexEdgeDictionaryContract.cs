using System;
using System.Diagnostics.Contracts;
namespace QuickGraph.Collections
{
    //[ContractClassFor(typeof(IVertexEdgeDictionary<,>))]
    abstract class IVertexEdgeDictionaryContract<TVertex> 
        : IVertexEdgeDictionary<TVertex>
    {
        IVertexEdgeDictionary<TVertex> IVertexEdgeDictionary<TVertex>.Clone()
        {
            Contract.Ensures(Contract.Result<IVertexEdgeDictionary<TVertex>>() != null);
            throw new NotImplementedException();
        }

        #region others
        void System.Collections.Generic.IDictionary<TVertex, IEdgeList<TVertex>>.Add(TVertex key, IEdgeList<TVertex> value)
        {
            throw new NotImplementedException();
        }

        bool System.Collections.Generic.IDictionary<TVertex, IEdgeList<TVertex>>.ContainsKey(TVertex key)
        {
            throw new NotImplementedException();
        }

        System.Collections.Generic.ICollection<TVertex> System.Collections.Generic.IDictionary<TVertex, IEdgeList<TVertex>>.Keys
        {
            get { throw new NotImplementedException(); }
        }

        bool System.Collections.Generic.IDictionary<TVertex, IEdgeList<TVertex>>.Remove(TVertex key)
        {
            throw new NotImplementedException();
        }

        bool System.Collections.Generic.IDictionary<TVertex, IEdgeList<TVertex>>.TryGetValue(TVertex key, out IEdgeList<TVertex> value)
        {
            throw new NotImplementedException();
        }

        System.Collections.Generic.ICollection<IEdgeList<TVertex>> System.Collections.Generic.IDictionary<TVertex, IEdgeList<TVertex>>.Values
        {
            get { throw new NotImplementedException(); }
        }

        IEdgeList<TVertex> System.Collections.Generic.IDictionary<TVertex, IEdgeList<TVertex>>.this[TVertex key]
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

        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TVertex, IEdgeList<TVertex>>>.Add(System.Collections.Generic.KeyValuePair<TVertex, IEdgeList<TVertex>> item)
        {
            throw new NotImplementedException();
        }

        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TVertex, IEdgeList<TVertex>>>.Clear()
        {
            throw new NotImplementedException();
        }

        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TVertex, IEdgeList<TVertex>>>.Contains(System.Collections.Generic.KeyValuePair<TVertex, IEdgeList<TVertex>> item)
        {
            throw new NotImplementedException();
        }

        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TVertex, IEdgeList<TVertex>>>.CopyTo(System.Collections.Generic.KeyValuePair<TVertex, IEdgeList<TVertex>>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TVertex, IEdgeList<TVertex>>>.Count
        {
            get { throw new NotImplementedException(); }
        }

        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TVertex, IEdgeList<TVertex>>>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TVertex, IEdgeList<TVertex>>>.Remove(System.Collections.Generic.KeyValuePair<TVertex, IEdgeList<TVertex>> item)
        {
            throw new NotImplementedException();
        }

        System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TVertex, IEdgeList<TVertex>>> System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TVertex, IEdgeList<TVertex>>>.GetEnumerator()
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

        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            throw new NotImplementedException();
        }
#endif
        #endregion
    }
}

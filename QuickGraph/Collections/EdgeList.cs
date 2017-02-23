using System;
using System.Collections.Generic;

namespace QuickGraph.Collections
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class EdgeList<TVertex>
        : List<Edge<TVertex>>
        , IEdgeList<TVertex>
#if !SILVERLIGHT
        , ICloneable
#endif
    {
        public EdgeList() 
        { }

        public EdgeList(int capacity)
            : base(capacity)
        { }

        public EdgeList(EdgeList<TVertex> list)
            : base(list)
        {}

        public EdgeList<TVertex> Clone()
        {
            return new EdgeList<TVertex>(this);
        }

        IEdgeList<TVertex> IEdgeList<TVertex>.Clone()
        {
            return this.Clone();
        }

#if !SILVERLIGHT
        object ICloneable.Clone()
        {
            return this.Clone();
        }
#endif
    }
}

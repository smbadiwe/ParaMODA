using System;
using System.Collections.Generic;
#if !SILVERLIGHT
using System.Runtime.Serialization;
#endif

namespace QuickGraph.Collections
{
    public sealed class VertexEdgeDictionary<TVertex>
        : Dictionary<TVertex, IEdgeList<TVertex>>
    {
        public VertexEdgeDictionary(IEqualityComparer<TVertex> vertexComparer)
            : base(vertexComparer)
        { }
        public VertexEdgeDictionary(int capacity, IEqualityComparer<TVertex> vertexComparer)
            : base(capacity, vertexComparer)
        { }
        
    }
}

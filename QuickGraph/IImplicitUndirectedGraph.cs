using QuickGraph.Contracts;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickGraph
{
    //[ContractClass(typeof(IImplicitUndirectedGraphContract<,>))]
    public interface IImplicitUndirectedGraph<TVertex, TEdge>
        : IImplicitVertexSet<TVertex>
        , IGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        
        //EdgeEqualityComparer<TVertex, TEdge> EdgeEqualityComparer { get; }

        
        IList<TEdge> AdjacentEdges(TVertex v);

        
        int AdjacentDegree(TVertex v);

        
        bool IsAdjacentEdgesEmpty(TVertex v);

        
        TEdge AdjacentEdge(TVertex v, int index);

        
        bool TryGetEdge(TVertex source, TVertex target, out TEdge edge);

        
        bool ContainsEdge(TVertex source, TVertex target);
    }
}

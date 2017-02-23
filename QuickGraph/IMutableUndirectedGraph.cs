using QuickGraph.Contracts;
using System.Diagnostics.Contracts;

namespace QuickGraph
{
    ///// <summary>
    ///// A mutable indirect graph
    ///// </summary>
    ///// <typeparam name="TVertex"></typeparam>
    ///// <typeparam name="TEdge"></typeparam>
    //[ContractClass(typeof(IMutableUndirectedGraphContract<,>))]
    //public interface IMutableUndirectedGraph<TVertex,TEdge> 
    //    : IMutableEdgeListGraph<TVertex,TEdge>
    //    , IMutableVertexSet<TVertex>
    //    , IUndirectedGraph<TVertex,TEdge>
    //    , IMutableVertexAndEdgeSet<TVertex, TEdge>
    //    where TEdge : IEdge<TVertex>
    //{
    //    int RemoveAdjacentEdgeIf(TVertex vertex, EdgePredicate<TVertex, TEdge> predicate);
    //    void ClearAdjacentEdges(TVertex vertex);
    //}
}

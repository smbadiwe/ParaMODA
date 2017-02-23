using System.Diagnostics.Contracts;

namespace QuickGraph
{
    
    public delegate bool EdgePredicate<TVertex, TEdge>(TEdge e)
        where TEdge : IEdge<TVertex>;

    public delegate bool EdgePredicate<TVertex>(Edge<TVertex> e);
}

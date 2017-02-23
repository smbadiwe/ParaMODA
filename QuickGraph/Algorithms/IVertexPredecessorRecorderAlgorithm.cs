namespace QuickGraph.Algorithms
{
    public interface IVertexPredecessorRecorderAlgorithm<TVertex,TEdge> 
        : ITreeBuilderAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        event VertexAction<TVertex> StartVertex;
        event VertexAction<TVertex> FinishVertex;
    }

    public interface IVertexPredecessorRecorderAlgorithm<TVertex> 
        : ITreeBuilderAlgorithm<TVertex>
    {
        event VertexAction<TVertex> StartVertex;
        event VertexAction<TVertex> FinishVertex;
    }
}

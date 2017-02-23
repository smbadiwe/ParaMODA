namespace QuickGraph.Algorithms
{
    public interface ITreeBuilderAlgorithm<TVertex,TEdge>
        where TEdge : IEdge<TVertex>
    {
        event EdgeAction<TVertex, TEdge> TreeEdge;
    }

    public interface ITreeBuilderAlgorithm<TVertex>
    {
        event EdgeAction<TVertex> TreeEdge;
    }
}

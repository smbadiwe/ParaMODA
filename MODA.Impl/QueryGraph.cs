using QuickGraph;

namespace MODA.Impl
{
    public class QueryGraph : UndirectedGraph<string, Edge<string>>
    {
        public QueryGraph(bool allowParralelEdges) : base(allowParralelEdges)
        {

        }

        public bool IsFrequentSubgraph { get; set; }
    }
}

using QuickGraph;

namespace MODA.Impl
{
    public class QueryGraph : UndirectedGraph<string, Edge<string>>
    {
        public QueryGraph() : base()
        {

        }

        public QueryGraph(bool allowParralelEdges) : base(allowParralelEdges)
        {
            
        }

        /// <summary>
        /// A name to identify / refer to this query graph
        /// </summary>
        public string Label { get; set; }

        public bool IsFrequentSubgraph { get; set; }
    }
}

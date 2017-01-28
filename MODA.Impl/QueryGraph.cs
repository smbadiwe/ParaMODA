using QuickGraph;

namespace MODA.Impl
{
    public sealed class QueryGraph : UndirectedGraph<string, Edge<string>>
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

        public override int GetHashCode()
        {
            return Label.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Label.Equals(((QueryGraph)obj).Label);
        }
    }
}

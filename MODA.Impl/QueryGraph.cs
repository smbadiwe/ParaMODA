using QuickGraph;
using System.Collections.Generic;

namespace MODA.Impl
{
    public sealed class QueryGraph : UndirectedGraph<int>
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
        public string Identifier { get; set; }

        public bool IsFrequentSubgraph { get; set; }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Identifier.Equals(((QueryGraph)obj).Identifier);
        }
    }
}

using QuickGraph;
using System.Collections.Generic;
using System.Text;

namespace MODA.Impl
{
    public class Mapping
    {
        public Mapping(Dictionary<string, string> function)
        {
            Function = function;
        }
        
        /// <summary>
        /// This represents the [f(h) = g] relation. Meaning key is h and value is g.
        /// </summary>
        public Dictionary<string, string> Function { get; private set; }
        
        /// <summary>
        /// The subgraph (with mapped edges) in the input graph G that fit the query graph (---Function.Keys)
        /// </summary>
        public UndirectedGraph<string, Edge<string>> MapOnInputSubGraph { get; set; } = new UndirectedGraph<string, Edge<string>>();

        public bool IsIsomorphicWith(Mapping otherMapping, UndirectedGraph<string, Edge<string>> inputSubgraph)
        {
            //NB: Node and edge count already guaranteed to be equal

            foreach (var node in this.MapOnInputSubGraph.Vertices)
            {
                //Test 1 - Vertices - sameness
                if (!otherMapping.MapOnInputSubGraph.ContainsVertex(node))
                {
                    return false;
                }

                //Test 2 - Node degrees.
                if (this.MapOnInputSubGraph.AdjacentDegree(node) != otherMapping.MapOnInputSubGraph.AdjacentDegree(node))
                {
                    //if input sub-graph is complete
                    if (inputSubgraph.EdgeCount == ((inputSubgraph.VertexCount * (inputSubgraph.VertexCount - 1)) / 2))
                    {
                        // Then the subgraphs is likely isomorphic, due to symmetry
                        return true;
                    }
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var functionSorted = new SortedDictionary<string, string>(Function);
            sb.Append("[");
            foreach (var item in functionSorted)
            {
                sb.AppendFormat("{0}-", item.Key);
            }
            //sb.AppendFormat("<{0}>] => [", this.MapOnInputSubGraph.AsString());
            sb.Append("] => [");
            foreach (var item in functionSorted)
            {
                sb.AppendFormat("{0}-", item.Value);
            }
            //sb.AppendFormat("] Exact map: <{0}>\n", this.MapOnInputSubGraph.AsString());
            sb.Append("]\n");
            //sb.AppendLine("]");
            return sb.ToString();
        }
    }
}

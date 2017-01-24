using QuickGraph;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MODA.Impl
{
    public sealed class Mapping
    {
        public Mapping(Dictionary<string, string> function)
        {
            Function = function;
            InputSubGraph = new UndirectedGraph<string, Edge<string>>();
            MapOnInputSubGraph = new UndirectedGraph<string, Edge<string>>();
        }

        /// <summary>
        /// This represents the [f(h) = g] relation. Meaning key is h and value is g.
        /// </summary>
        public Dictionary<string, string> Function { get; private set; }
        
        /// <summary>
        /// The subgraph (with mapped edges) in the input graph G that fit the query graph (---Function.Keys)
        /// </summary>
        public UndirectedGraph<string, Edge<string>> MapOnInputSubGraph { get; set; }

        /// <summary>
        /// The subgraph (with all edges) in the input graph G that fit the query graph (---Function.Keys)
        /// </summary>
        public UndirectedGraph<string, Edge<string>> InputSubGraph { get; set; }

        public bool IsIsomorphicWith(Mapping otherMapping)
        {
            //NB: Node and edge count already guaranteed to be equal
            foreach (var node in InputSubGraph.Vertices)
            {
                //Test 1 - Vertices - sameness
                if (!otherMapping.MapOnInputSubGraph.ContainsVertex(node)) //Remember, f(h) = g. So, key is h and value is g
                {
                    return false;
                }
            }
            int size = Function.Count;
            bool isComplete = InputSubGraph.EdgeCount == ((size * (size - 1)) / 2);
            foreach (var node in InputSubGraph.Vertices)
            {
                //Test 2 - Node degrees.
                if (MapOnInputSubGraph.AdjacentDegree(node) != otherMapping.MapOnInputSubGraph.AdjacentDegree(node))
                {
                    //if input sub-graph is complete
                    if (isComplete)
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

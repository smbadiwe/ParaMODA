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
        }
        
        /// <summary>
        /// This represents the [f(h) = g] relation. Meaning key is h and value is g.
        /// </summary>
        public Dictionary<string, string> Function { get; private set; }
        
        /// <summary>
        /// The subgraph (with mapped edges) in the input graph G that fit the query graph (---Function.Keys)
        /// </summary>
        public UndirectedGraph<string, Edge<string>> MapOnInputSubGraph { get; set; } = new UndirectedGraph<string, Edge<string>>();

        /// <summary>
        /// The subgraph (with all edges) in the input graph G that fit the query graph (---Function.Keys)
        /// </summary>
        public UndirectedGraph<string, Edge<string>> InputSubGraph { get; set; } = new UndirectedGraph<string, Edge<string>>();

        public bool IsIsomorphicWith(Mapping otherMapping)
        {
            //NB: Node and edge count already guaranteed to be equal
            var verts = this.MapOnInputSubGraph.Vertices.ToArray();
            for (var i = verts.Length - 1; i >= 0; i--)
            {
                //Test 1 - Vertices - sameness
                if (!otherMapping.MapOnInputSubGraph.ContainsVertex(verts[i]))
                {
                    return false;
                }
            }
            var isComplete = InputSubGraph.EdgeCount == ((InputSubGraph.VertexCount * (InputSubGraph.VertexCount - 1)) / 2);
            for (var i = this.MapOnInputSubGraph.VertexCount - 1; i >= 0; i--)
            {
                //Test 2 - Node degrees.
                if (this.MapOnInputSubGraph.AdjacentDegree(verts[i]) != otherMapping.MapOnInputSubGraph.AdjacentDegree(verts[i]))
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

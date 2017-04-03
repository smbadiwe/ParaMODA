using QuickGraph;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MODA.Impl
{
    public class Mapping
    {
        public Mapping(Mapping mapping)
        {
            Function = mapping.Function;
            Id = mapping.Id;
            SubGraphEdgeCount = mapping.SubGraphEdgeCount;
        }

        public Mapping(SortedList<int, int> function, int subGraphEdgeCount)
        {
            Function = function;
            Id = -1;
            SubGraphEdgeCount = subGraphEdgeCount;
        }

        /// <summary>
        /// Usage is temporary - to help organize stuffs in Algorithm 3
        /// </summary>
        public int Id;

        /// <summary>
        /// This represents the [f(h) = g] relation. Meaning key is h and value is g.
        /// </summary>
        public readonly SortedList<int, int> Function;

        /// <summary>
        /// Count of all the edges in the input subgraph G that fit the query graph (---Function.Keys).
        /// This count is for the induced subgraph
        /// </summary>
        public int SubGraphEdgeCount;

        /// <summary>
        /// Only for when (InducedSubGraphEdgesCount == currentQueryGraphEdgeCount)
        /// </summary>
        /// <param name="parentQueryGraphEdges"></param>
        /// <returns></returns>
        public Edge<int> GetImage(UndirectedGraph<int> inputGraph, IEnumerable<Edge<int>> parentQueryGraphEdges)
        {
            int subgraphSize = Function.Count;
            var g_nodes = Function.Values; // Remember, f(h) = g, so .Values is for g's
            Edge<int> edge_g; // = default(Edge<int>);
            var inducedSubGraphEdges = new List<Edge<int>>(SubGraphEdgeCount);
            for (int i = 0; i < subgraphSize - 1; i++)
            {
                for (int j = (i + 1); j < subgraphSize; j++)
                {
                    if (inputGraph.TryGetEdge(g_nodes[i], g_nodes[j], out edge_g))
                    {
                        inducedSubGraphEdges.Add(edge_g);
                    }
                }
            }

            var edgeImages = new HashSet<Edge<int>>();
            foreach (var x in parentQueryGraphEdges)
            {
                edgeImages.Add(new Edge<int>(Function[x.Source], Function[x.Target]));
            }
            foreach (var edgex in inducedSubGraphEdges)
            {
                if (!edgeImages.Contains(edgex))
                {
                    inducedSubGraphEdges.Clear();
                    edgeImages.Clear();
                    inducedSubGraphEdges = null;
                    edgeImages = null;
                    return edgex;
                }
            }
            inducedSubGraphEdges.Clear();
            edgeImages.Clear();
            inducedSubGraphEdges = null;
            edgeImages = null;
            return default(Edge<int>);
        }

        /// <summary>
        /// Only for when (InducedSubGraph.EdgeCount > currentQueryGraphEdgeCount)
        /// </summary>
        /// <param name="inputGraph"></param>
        /// <param name="newlyAddedEdge"></param>
        /// <returns></returns>
        public Edge<int> GetImage(UndirectedGraph<int> inputGraph, Edge<int> newlyAddedEdge)
        {
            Edge<int> image;
            if (inputGraph.TryGetEdge(Function[newlyAddedEdge.Source], Function[newlyAddedEdge.Target], out image))
            {
                return image;
            }
            return default(Edge<int>);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Mapping;
            if (other == null) return false;

            int i = 0;
            foreach (var func in Function)
            {
                if (other.Function.Keys[i] == func.Key && other.Function.Values[i] == func.Value)
                {
                    i++;
                    continue;
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Retruns the nodes in the input graph that the query graph mapped to.
        /// It's returned as a string where the nodes values are concatenated with '-'
        /// </summary>
        /// <returns></returns>
        public string GetMappedNodes()
        {
            var sb = new StringBuilder();
            foreach (var item in Function)
            {
                sb.AppendFormat("{0}-", item.Value);
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            foreach (var item in Function)
            {
                sb.AppendFormat("{0}-", item.Key);
            }
            sb.Append("] => [");
            foreach (var item in Function)
            {
                sb.AppendFormat("{0}-", item.Value);
            }
            sb.Append("]\n");
            return sb.ToString();
        }
    }
}

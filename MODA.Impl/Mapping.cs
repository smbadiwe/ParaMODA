using QuickGraph;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MODA.Impl
{
    public sealed class Mapping
    {
        public Mapping(SortedList<int, int> function)
        {
            Function = function;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// This represents the [f(h) = g] relation. Meaning key is h and value is g.
        /// </summary>
        public SortedList<int, int> Function { get; private set; }

        /// <summary>
        /// Count of all the edges in the input subgraph G that fit the query graph (---Function.Keys)
        /// </summary>
        public int InducedSubGraphEdgesCount { get; set; }

        /// <summary>
        /// Only for when (InducedSubGraphEdgesCount == currentQueryGraphEdgeCount)
        /// </summary>
        /// <param name="parentQueryGraphEdges"></param>
        /// <returns></returns>
        public Edge<int> GetImage(UndirectedGraph<int, Edge<int>> inputGraph, IEnumerable<Edge<int>> parentQueryGraphEdges)
        {
            int subgraphSize = Function.Count;
            var g_nodes = Function.Values; // Remember, f(h) = g, so .Values is for g's
            Edge<int> edge_g = null;
            var inducedSubGraphEdges = new List<Edge<int>>(InducedSubGraphEdgesCount);
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

            var edgeImages = new HashSet<Edge<int>>(parentQueryGraphEdges.Select(x => new Edge<int>(Function[x.Source], Function[x.Target])));
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
            return null;
        }

        /// <summary>
        /// Only for when (InducedSubGraph.EdgeCount > currentQueryGraphEdgeCount)
        /// </summary>
        /// <param name="inputGraph"></param>
        /// <param name="newlyAddedEdge"></param>
        /// <returns></returns>
        public Edge<int> GetImage(UndirectedGraph<int, Edge<int>> inputGraph, Edge<int> newlyAddedEdge)
        {
            Edge<int> image;
            if (inputGraph.TryGetEdge(Function[newlyAddedEdge.Source], Function[newlyAddedEdge.Target], out image))
            {
                return image;
            }
            return null;
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

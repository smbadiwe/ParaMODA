using QuickGraph;
using System.Collections.Generic;
using System.Linq;

namespace MODA.Impl
{
    public static class Extensions
    {
        public static IList<string> GetNeighbors(this UndirectedGraph<string, Edge<string>> graph, string vertex, bool isG)
        {
            if (string.IsNullOrWhiteSpace(vertex)) return new List<string>(1);
            IList<string> neighbors;
            if (isG)
            {
                if (!ModaAlgorithms.G_NodeNeighbours.TryGetValue(vertex, out neighbors))
                {
                    ModaAlgorithms.G_NodeNeighbours[vertex] = neighbors = graph.GetNeighbors(vertex);
                }
                return neighbors;
            }
            else
            {
                if (!ModaAlgorithms.H_NodeNeighbours.TryGetValue(vertex, out neighbors))
                {
                    ModaAlgorithms.H_NodeNeighbours[vertex] = neighbors = graph.GetNeighbors(vertex);
                }
                return neighbors;
            }
        }

        /// <summary>
        /// Converts a sequence of edges into a query graph
        /// </summary>
        /// <param name="edges"></param>
        /// <returns></returns>
        public static QueryGraph ToQueryGraph(this IEnumerable<Edge<string>> edges, string graphLabel = "")
        {
            var g = new QueryGraph
            {
                Label = graphLabel
            };
            g.AddVerticesAndEdgeRange(edges);
            return g;
        }
    }
}

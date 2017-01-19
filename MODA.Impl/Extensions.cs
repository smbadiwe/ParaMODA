using QuickGraph;
using System.Collections.Generic;
using System.Linq;

namespace MODA.Impl
{
    public static class Extensions
    {
        public static List<string> GetNeighbors(this UndirectedGraph<string, Edge<string>> graph, string vertex, bool isG)
        {
            if (string.IsNullOrWhiteSpace(vertex)) return new List<string>();
            List<string> neighbors;
            HashSet<string> set;
            if (isG)
            {
                if (ModaAlgorithms.G_NodeNeighbours.TryGetValue(vertex, out neighbors))
                {
                    return neighbors;
                }
                else
                {
                    var adjEdges = graph.AdjacentEdges(vertex);
                    set = new HashSet<string>();
                    foreach (var edge in adjEdges)
                    {
                        set.Add(edge.Source);
                        set.Add(edge.Target);
                    }
                    set.Remove(vertex);
                    neighbors = set.ToList();
                    ModaAlgorithms.G_NodeNeighbours[vertex] = neighbors;
                    //adjEdges = null;
                    return neighbors;
                }
            }
            else
            {
                if (ModaAlgorithms.H_NodeNeighbours.TryGetValue(vertex, out neighbors))
                {
                    return neighbors;
                }
                else
                {
                    var adjEdges = graph.AdjacentEdges(vertex);
                    set = new HashSet<string>();
                    foreach (var edge in adjEdges)
                    {
                        set.Add(edge.Source);
                        set.Add(edge.Target);
                    }
                    set.Remove(vertex);
                    neighbors = set.ToList();
                    ModaAlgorithms.H_NodeNeighbours[vertex] = neighbors;
                    //adjEdges = null;
                    return neighbors;
                }
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

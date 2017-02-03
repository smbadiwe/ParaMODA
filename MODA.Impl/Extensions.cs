using QuickGraph;
using System.Collections.Generic;
using System.Linq;

namespace MODA.Impl
{
    public static class Extensions
    {
        public static IList<string> GetNeighbors(this UndirectedGraph<string, Edge<string>> graph, string vertex, bool isG)
        {
            //if (string.IsNullOrWhiteSpace(vertex)) return new string[0];
            //return graph.GetNeighbors(vertex);
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

        // O(1) 
        public static void RemoveBySwap<T>(this List<T> list, int index)
        {
            list[index] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
        }


        // O(n)
        public static void RemoveBySwap<T>(this List<T> list, T item)
        {
            int index = list.IndexOf(item);
            list[index] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
        }
    }
}

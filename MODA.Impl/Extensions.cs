using QuickGraph;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MODA.Impl
{
    public static class Extensions
    {
        public static List<string> GetNonNeighbors(this UndirectedGraph<string, Edge<string>> graph, string vertex, bool isG = true, List<string> neighboursOfVertex = null)
        {
            if (string.IsNullOrWhiteSpace(vertex)) return new List<string>();
            if (neighboursOfVertex == null)
            {
                neighboursOfVertex = GetNeighbors(graph, vertex, isG);
            }
            var nonNeighbors = new List<string>();

            foreach (var node in graph.Vertices)
            {
                if (node.Equals(vertex)) continue;
                if (!neighboursOfVertex.Contains(node))
                {
                    nonNeighbors.Add(node);
                }
            }
            return nonNeighbors;
        }
        
        public static List<string> GetNeighbors(this UndirectedGraph<string, Edge<string>> graph, string vertex, bool isG = true)
        {
            if (string.IsNullOrWhiteSpace(vertex)) return new List<string>();
            List<string> neighbors;
            if (isG)
            {
                if (ModaAlgorithms.G_NodeNeighbours.TryGetValue(vertex, out neighbors))
                {
                    return neighbors;
                }
                else
                {
                    var adjEdges = graph.AdjacentEdges(vertex);
                    var set = new HashSet<string>(adjEdges.Select(x => x.Source).Union(adjEdges.Select(x => x.Target)));
                    set.Remove(vertex);
                    adjEdges = null;
                    ModaAlgorithms.G_NodeNeighbours[vertex] = neighbors = set.ToList();
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
                    var set = new HashSet<string>(adjEdges.Select(x => x.Source).Union(adjEdges.Select(x => x.Target)));
                    set.Remove(vertex);
                    adjEdges = null;
                    ModaAlgorithms.H_NodeNeighbours[vertex] = neighbors = set.ToList();
                    return neighbors;
                }
            }
            
        }
        
        public static string AsString(this UndirectedGraph<string, Edge<string>> graph)
        {
            if (graph.IsEdgesEmpty) return "";
            var sb = new StringBuilder("Graph-Edges_");
            foreach (var edge in graph.Edges)
            {
                sb.AppendFormat("[{0}],", edge);
            }
            //sb.AppendLine();
            return sb.ToString().TrimEnd();
        }

        public static UndirectedGraph<string, Edge<string>> Clone(this UndirectedGraph<string, Edge<string>> graph)
        {
            var inputGraphClone = new UndirectedGraph<string, Edge<string>>();
            inputGraphClone.AddVerticesAndEdgeRange(graph.Edges);
            Debug.Assert(inputGraphClone.EdgeCount == graph.EdgeCount && inputGraphClone.VertexCount == graph.VertexCount);
            return inputGraphClone;
        }

        /// <summary>
        /// NB: The degree sequence of an undirected graph is the non-increasing sequence of its vertex degrees;
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <param name="graph"></param>
        /// <param name="count">The expected number of items to return.</param>
        /// <returns></returns>
        public static string[] GetDegreeSequence(this UndirectedGraph<string, Edge<string>> graph, int count)
        {
            if (graph.IsVerticesEmpty) return new string[0];

            var vertices = graph.Vertices.Take(count).ToArray();
            var tempList = new Dictionary<string, int>(vertices.Length);

            foreach (var node in vertices)
            {
                tempList.Add(node, graph.AdjacentDegree(node));
            }

            var listToReturn = new List<string>(vertices.Length);
            foreach (var item in tempList.OrderByDescending(x => x.Value))
            {
                listToReturn.Add(item.Key);
            }

            vertices = null;
            tempList = null;
            return listToReturn.ToArray();
        }

        /// <summary>
        /// Converts a sequence of edges into a query graph
        /// </summary>
        /// <param name="edges"></param>
        /// <returns></returns>
        public static QueryGraph ToQueryGraph(this IEnumerable<Edge<string>> edges, string graphLabel = "")
        {
            return ToQueryGraph(edges, true, graphLabel);
        }

        /// <summary>
        /// Converts a sequence of edges into a query graph
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="allowParralelEdges"></param>
        /// <returns></returns>
        public static QueryGraph ToQueryGraph(this IEnumerable<Edge<string>> edges, bool allowParralelEdges, string graphLabel = "")
        {
            //if (edges == null || edges.Any(e => e == null)) throw new System.ArgumentNullException("edges", "Null value(s) not accepted.");

            var g = new QueryGraph(allowParralelEdges);
            g.AddVerticesAndEdgeRange(edges);
            return g;
        }

    }
}

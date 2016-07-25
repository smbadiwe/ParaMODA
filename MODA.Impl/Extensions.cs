using QuickGraph;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MODA.Impl
{
    public static class Extensions
    {
        public static List<TVertex> GetNonNeighbors<TVertex>(this UndirectedGraph<TVertex, Edge<TVertex>> graph, TVertex vertex, List<TVertex> neighboursOfVertex = null)
        {
            if (neighboursOfVertex == null)
            {
                neighboursOfVertex = GetNeighbors(graph, vertex);
            }
            var nonNeighbors = new List<TVertex>();

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

        public static List<TVertex> GetNeighbors<TVertex>(this UndirectedGraph<TVertex, Edge<TVertex>> graph, TVertex vertex)
        {
            var neighbors = new List<TVertex>();
            var adjEdges = graph.AdjacentEdges(vertex);
            foreach (var edge in adjEdges)
            {
                if (edge.Source.Equals(vertex))
                {
                    neighbors.Add(edge.Target);
                }
                else
                {
                    neighbors.Add(edge.Source);
                }
            }
            return neighbors;
        }
        
        public static string AsString<TVertex>(this UndirectedGraph<TVertex, Edge<TVertex>> graph)
        {
            if (graph.IsEdgesEmpty) return "";
            var sb = new StringBuilder("Graph: Edges - ");
            foreach (var edge in graph.Edges)
            {
                sb.AppendFormat("[{0}], ", edge);
            }
            //sb.AppendLine();
            return sb.ToString();
        }

        public static UndirectedGraph<TVertex, Edge<TVertex>> Clone<TVertex>(this UndirectedGraph<TVertex, Edge<TVertex>> graph)
        {
            var inputGraphClone = new UndirectedGraph<TVertex, Edge<TVertex>>();
            inputGraphClone.AddVerticesAndEdgeRange(graph.Edges);
            Debug.Assert(inputGraphClone.EdgeCount == graph.EdgeCount && inputGraphClone.VertexCount == graph.VertexCount);
            return inputGraphClone;
        }

        /// <summary>
        /// NB: The degree sequence of an undirected graph is the non-increasing sequence of its vertex degrees;
        /// </summary>
        /// <typeparam name="TVertex"></typeparam>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static List<TVertex> GetDegreeSequence<TVertex>(this UndirectedGraph<TVertex, Edge<TVertex>> graph)
        {
            var listToReturn = new List<TVertex>();
            if (graph.IsVerticesEmpty) return listToReturn;

            var tempList = new Dictionary<TVertex, int>();
            foreach (var node in graph.Vertices)
            {
                tempList.Add(node, graph.AdjacentDegree(node));
            }

            foreach (var item in tempList.OrderByDescending(x => x.Value))
            {
                listToReturn.Add(item.Key);
            }
            return listToReturn;
        }
        
        public static TValue get<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            TValue val;
            if (dict.TryGetValue(key, out val)) return val;
            return default(TValue);
        }
        
    }
}

using QuickGraph;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MODA.Impl
{
    public static class Extensions
    {
        public static List<string> GetNonNeighbors(this UndirectedGraph<string, Edge<string>> graph, string vertex, List<string> neighboursOfVertex = null)
        {
            if (neighboursOfVertex == null)
            {
                neighboursOfVertex = GetNeighbors(graph, vertex);
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

        public static List<string> GetNeighbors(this UndirectedGraph<string, Edge<string>> graph, string vertex)
        {
            var adjEdges = graph.AdjacentEdges(vertex);
            var set = new HashSet<string>(adjEdges.Select(x => x.Source).Union(adjEdges.Select(x => x.Target)));
            set.Remove(vertex);
            adjEdges = null;
            return set.ToList();
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
        /// <returns></returns>
        public static List<string> GetDegreeSequence(this UndirectedGraph<string, Edge<string>> graph)
        {
            if (graph.IsVerticesEmpty) return new List<string>(0);

            var vertices = graph.Vertices.ToList();
            var tempList = new Dictionary<string, int>(vertices.Count);
            //foreach (var node in vertices)
            vertices.ForEach(node =>
            {
                tempList.Add(node, graph.AdjacentDegree(node));
            }
            );
            var listToReturn = new List<string>(vertices.Count);
            //int index = 0;
            foreach (var item in tempList.OrderByDescending(x => x.Value))
            {
                //listToReturn[index] = item.Key;
                //index++;
                listToReturn.Add(item.Key);
            }
            vertices = null;
            tempList = null;
            return listToReturn;
        }
        
    }
}

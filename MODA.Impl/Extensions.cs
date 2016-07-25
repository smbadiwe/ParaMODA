using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;
using System.Diagnostics;

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

        public static double GetAverageDegree<TVertex>(this UndirectedGraph<TVertex, Edge<TVertex>> graph)
        {
            if (graph.IsVerticesEmpty) return 0;

            double avgDegree = 0, sumDegrees = 0;
            foreach (var node in graph.Vertices)
            {
                sumDegrees += graph.AdjacentDegree(node);
            }
            avgDegree = sumDegrees / graph.VertexCount;
            return avgDegree;
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

        /// <summary>
        /// Remove from <paramref name="list"/> all elements that is not contained in <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool retainAll<T>(this ICollection<T> list, IEnumerable<T> collection)
        {
            if (collection == null) return false;
            bool modified = false;
            foreach (var item in collection)
            {
                if (!list.Contains(item))
                {
                    list.Remove(item);
                    modified = true;
                }
            }
            return modified;
        }

        public static TValue get<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            TValue val;
            if (dict.TryGetValue(key, out val)) return val;
            return default(TValue);
        }

        public static void put<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue val)
        {
            dict.Add(key, val);
        }

        /// <summary>
        /// divides a given 'enumerable' (eg List) into 'chunkSize' slammer lists
        /// <para>USAGE</para>
        /// <para>var src = new[] { 1, 2, 3, 4, 5, 6 };</para>
        /// <para>var c3 = src.Chunks(3);      // {{1, 2, 3}, {4, 5, 6}}; </para>
        /// <para>var c4 = src.Chunks(4);      // {{1, 2, 3, 4}, {5, 6}}; </para>
        /// <para>var sum = c3.Select(c => c.Sum());    // {6, 15}</para>
        /// <para>var count = c3.Count();                 // 2</para>
        /// <para>var take2 = c3.Select(c => c.Take(2));  // {{1, 2}, {4, 5}}</para>
        /// <para>var batch1 = c3.Select(c => c.ToList()).First(); // list:{1, 2, 3}</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Chunks<T>(this IEnumerable<T> enumerable,
                                                    int chunkSize)
        {
            if (chunkSize < 1) throw new ArgumentException("chunkSize must be positive");

            using (var e = enumerable.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    var remaining = chunkSize;    // elements remaining in the current chunk
                    var innerMoveNext = new Func<bool>(() => --remaining > 0 && e.MoveNext());

                    yield return e.GetChunk(innerMoveNext);
                    while (innerMoveNext()) {/* discard elements skipped by inner iterator */}
                }
            }
        }

        private static IEnumerable<T> GetChunk<T>(this IEnumerator<T> e,
                                                  Func<bool> innerMoveNext)
        {
            do yield return e.Current;
            while (innerMoveNext());
        }
    }
}

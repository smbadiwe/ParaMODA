using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;

namespace MODA.Impl
{
    public static class Extensions
    {
        public static IList<TVertex> getNeighbors<TVertex>(this UndirectedGraph<TVertex, Edge<TVertex>> graph, TVertex vertex)
        {
           return graph.AdjacentEdges(vertex).Select(x => x.Target).ToList();
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

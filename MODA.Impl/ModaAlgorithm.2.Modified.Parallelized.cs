//This is the one that has gone bad
using QuickGraph;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl
{
    public class ModaAlgorithm2Parallelized
    {
        /// <summary>
        /// Mapping module (aka FindSubgraphInstances in Grochow & Kellis) modified
        /// The modification:
        ///     Instead of traversing all nodes in the query graph (H) for each node in the input graph (G),
        ///     we simply use just one node h in H to traverse G. This makes it much easier to parallelize 
        ///     unlike the original algorithm, and eliminate the need for removing visited g from G.
        ///     
        ///     Testing will show whether this improves, worsens or makes no difference in performance.
        /// </summary>
        /// <param name="queryGraph">H</param>
        /// <param name="inputGraph">G</param>
        public static List<Mapping> Algorithm2_Modified(UndirectedGraph<string, Edge<string>> queryGraph, UndirectedGraph<string, Edge<string>> inputGraph)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var numberOfSamples = inputGraph.VertexCount / 3;

            var h_ = queryGraph.Vertices.First();

            //Each chunk will go to a thread
            List<string>[] chunks = new List<string>[Environment.ProcessorCount - 1];
            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i] = new List<string>();
            }

            var nodesToWorkWith = inputGraph.GetDegreeSequence(numberOfSamples);
            for (int i = nodesToWorkWith.Length - 1; i >= 0; i--)
            {
                chunks[i % chunks.Length].Add(nodesToWorkWith[i]);
            }
            
            //Console.WriteLine("Calling Algo 2-Modified: Number of Iterations: {0}. Number of chunks to parallelize: {1}\n", numberOfSamples, chunks.Length);

            var comparer = new MappingNodesComparer();
            InputSubgraphs = new ConcurrentDictionary<string[], UndirectedGraph<string, Edge<string>>>(comparer);
            MostConstrainedNeighbours = new ConcurrentDictionary<string[], string>(comparer);
            NeighboursOfRange = new ConcurrentDictionary<string[], HashSet<string>>(comparer);
            comparer = null;

            G_NodeNeighbours = new ConcurrentDictionary<string, List<string>>();
            H_NodeNeighbours = new ConcurrentDictionary<string, List<string>>();
            //int counter = 0;
            var tasks = new List<Task>(chunks.Length);
            var theMappings = new ConcurrentDictionary<string, List<Mapping>>();
            foreach (var degSeqChunk in chunks)
            {
                //Console.WriteLine("\tChunk {0} loaded. Number of items in chunk: {1}\n", ++counter, degSeqChunk.Count);
                tasks.Add(Task.Factory.StartNew((objects) =>
                    {
                        var objectsSet = objects as object[];
                        var degSeqChunk_ = objectsSet[0] as string[];
                        var queryGraph_ = objectsSet[1] as UndirectedGraph<string, Edge<string>>;
                        var h = objectsSet[2] as string;
                        var inputGraph_ = objectsSet[3] as UndirectedGraph<string, Edge<string>>;
                        for (int i = 0; i < degSeqChunk_.Length; i++)
                        {
                            var g = degSeqChunk_[i];
                            if (CanSupport(queryGraph_, h, inputGraph_, g))
                            {
                                #region Can Support
                                var sw = System.Diagnostics.Stopwatch.StartNew();
                                //Remember: f(h) = g, so h is Domain and g is Range
                                //function, f = new Dictionary<string, string>(1) { { h, g } }
                                var mappings = IsomorphicExtension(new Dictionary<string, string>(1) { { h, g } }, queryGraph_, inputGraph_);
                                if (mappings.Count == 0) continue;

                                sw.Stop();
                                var logGist = new StringBuilder();
                                logGist.AppendFormat("Maps gotten from IsoExtension.\tTook:\t{0:N}s.\th = {1}. g = {2}\n", sw.Elapsed.ToString(), h, g);
                                sw.Restart();

                                foreach (Mapping mapping in mappings)
                                {
                                    List<Mapping> mappingsToSearch; //Recall: f(h) = g
                                    var g_key = mapping.Function.Last().Value;
                                    if (theMappings.TryGetValue(g_key, out mappingsToSearch) && mappingsToSearch != null)
                                    {
                                        var existing = mappingsToSearch.Find(x => x != null && x.Equals(mapping));

                                        if (existing == null)
                                        {
                                            theMappings[g_key].Add(mapping);
                                        }
                                    }
                                    else
                                    {
                                        theMappings[g_key] = new List<Mapping> { mapping };
                                    }
                                    mappingsToSearch = null;
                                }

                                sw.Stop();
                                logGist.AppendFormat("Map: {0}.\tTime to set:\t{1:N}s.\th = {2}. g = {3}\n", mappings.Count, sw.Elapsed.ToString(), h, g);
                                mappings = null;
                                sw = null;
                                logGist.AppendFormat("*****************************************\n");
                                Console.WriteLine(logGist);
                                logGist = null;
                                #endregion
                            }
                        }
                        objectsSet = null;
                        degSeqChunk_ = null;
                        queryGraph_ = null;
                        inputGraph_ = null;

                    }, new object[] { degSeqChunk.ToArray(), queryGraph, h_, inputGraph }
                ));
            }
            Task.WaitAll(tasks.ToArray());

            var toReturn = new List<Mapping>();
            foreach (var mapping in theMappings)
            {
                toReturn.AddRange(mapping.Value);
            }
            timer.Stop();
            theMappings = null;
            InputSubgraphs = null;
            MostConstrainedNeighbours = null;
            NeighboursOfRange = null;
            G_NodeNeighbours = null;
            H_NodeNeighbours = null;
            Console.WriteLine("Algorithm 2: All {2} tasks completed. Number of mappings found: {0}.\nTotal time taken: {1}", toReturn.Count, timer.Elapsed.ToString(), tasks.Count);
            tasks = null;
            timer = null;
            return toReturn;
        }

        /// <summary>
        /// Given a set of nodes(the Key), we find the subgraph in the input graph G that has those nodes.
        /// </summary>
        private static ConcurrentDictionary<string[], UndirectedGraph<string, Edge<string>>> InputSubgraphs;

        /// <summary>
        /// Algorithm taken from Grochow and Kellis. This is failing at the moment
        /// </summary>
        /// <param name="partialMap">f; Map is represented as a dictionary, with the Key as h and the Value as g</param>
        /// <param name="queryGraph">G</param>
        /// <param name="inputGraph">H</param>
        /// <returns>List of isomorphisms. Remember, Key is h, Value is g</returns>
        private static List<Mapping> IsomorphicExtension(Dictionary<string, string> partialMap, UndirectedGraph<string, Edge<string>> queryGraph
            , UndirectedGraph<string, Edge<string>> inputGraph)
        {
            if (partialMap.Count == queryGraph.VertexCount)
            {
                #region Return base case
                var map = new Mapping(partialMap);
                foreach (var qEdge in queryGraph.Edges)
                {
                    map.MapOnInputSubGraph.AddVerticesAndEdge(new Edge<string>(partialMap[qEdge.Source], partialMap[qEdge.Target]));
                }

                string[] inputSubgraphKey = partialMap.Values.ToArray();
                var keys = InputSubgraphs.Keys.ToArray();
                bool exists = keys.Any(x => new HashSet<string>(x).SetEquals(inputSubgraphKey));
                keys = null;
                if (!exists)
                {
                    var newInputSubgraph = new UndirectedGraph<string, Edge<string>>(false);
                    for (int i = 0; i < inputSubgraphKey.Length; i++)
                    {
                        for (int j = (i + 1); j < inputSubgraphKey.Length; j++)
                        {
                            Edge<string> edge;
                            if (inputGraph.TryGetEdge(inputSubgraphKey[i], inputSubgraphKey[j], out edge))
                            {
                                newInputSubgraph.AddVerticesAndEdge(edge);
                            }
                        }
                    }
                    InputSubgraphs[inputSubgraphKey] = newInputSubgraph;

                }
                map.InputSubGraph = InputSubgraphs[inputSubgraphKey];

                inputSubgraphKey = null;
                return new List<Mapping> { map };
                #endregion
            }

            //Remember: f(h) = g, so h is Domain and g is Range.
            //  In other words, Key is h and Value is g in the dictionary

            // get m, most constrained neighbor
            string m = GetMostConstrainedNeighbour(partialMap.Keys.ToArray(), queryGraph);
            if (string.IsNullOrWhiteSpace(m)) return new List<Mapping>();

            var listOfIsomorphisms = new List<Mapping>();

            var neighbourRange = ChooseNeighboursOfRange(partialMap.Values.ToArray(), inputGraph);
            foreach (var n in neighbourRange) //foreach neighbour n of f(D)
            {
                if (IsNeighbourIncompatible(inputGraph, queryGraph, n, m, partialMap))
                {
                    continue;
                }
                //It's not; so, let f' = f on D, and f'(m) = n.
                var newPartialMap = new Dictionary<string, string>(partialMap.Count + 1);
                foreach (var item in partialMap)
                {
                    newPartialMap.Add(item.Key, item.Value);
                }
                newPartialMap[m] = n;

                //Find all isomorphic extensions of f'.
                var subList = IsomorphicExtension(newPartialMap, queryGraph, inputGraph);
                if (listOfIsomorphisms.Count == 0)
                {
                    listOfIsomorphisms.AddRange(subList);
                }
                else
                {
                    subList.ForEach(item =>
                    {
                        if (new HashSet<string>(item.Function.Values).Count == item.Function.Count)
                        {
                            var existing = listOfIsomorphisms.Find(x => x.Equals(item));

                            if (existing == null)
                            {
                                listOfIsomorphisms.Add(item);
                            }
                            existing = null;
                        }
                    });
                }
                newPartialMap = null;
            }
            return listOfIsomorphisms;
        }

        internal static ConcurrentDictionary<string, List<string>> G_NodeNeighbours { get; set; }
        internal static ConcurrentDictionary<string, List<string>> H_NodeNeighbours { get; set; }

        /// <summary>
        /// If there is a neighbor d ∈ D of m such that n is NOT neighbors with f(d),
        /// or if there is a NON-neighbor d ∈ D of m such that n is neighbors with f(d) 
        /// [or if assigning f(m) = n would violate a symmetry-breaking condition in C(h)]
        /// then contiue with the next n
        /// </summary>
        /// <param name="queryGraph">H</param>
        /// <param name="inputGraph">G</param>
        /// <param name="n">g_node, pass in 'neighbour'; n in Grochow</param>
        /// <param name="m">h_node; the most constrained neighbor of any d ∈ D</param>
        /// <param name="domain">domain_in_H</param>
        /// <param name="partialMap">function</param>
        /// <returns></returns>
        private static bool IsNeighbourIncompatible(UndirectedGraph<string, Edge<string>> inputGraph, UndirectedGraph<string, Edge<string>> queryGraph,
            string n, string m, Dictionary<string, string> partialMap)
        {
            //  RECALL: m is for Domain, the Key => the query graph

            //A: If there is a neighbor d ∈ D of m such that n is NOT neighbors with f(d)...
            var neighboursOfN = GetNeighbors(inputGraph, n);
            var neighborsOfM = GetNeighbors(queryGraph, m, false);
            for (int i = 0; i < neighborsOfM.Count; i++)
            {
                if (!partialMap.ContainsKey(neighborsOfM[i]))
                {
                    neighboursOfN = null;
                    return false;
                }
                if (!neighboursOfN.Contains(partialMap[neighborsOfM[i]]))
                {
                    neighboursOfN = null;
                    return true;
                }
            }

            ////B: ...or if there is a NON-neighbor d ∈ D of m such that n is neighbors with f(d) 
            //var nonNeighborsOfM = queryGraph.GetNonNeighbors(m, neighborsOfM);
            //foreach (var d in nonNeighborsOfM)
            //{
            //    if (!partialMap.ContainsKey(d)) return false;

            //    if (neighboursOfN.Contains(partialMap[d]))
            //    {
            //        return false;
            //    }
            //}

            neighborsOfM = null;
            neighboursOfN = null;
            return false;
        }

        private static ConcurrentDictionary<string[], HashSet<string>> NeighboursOfRange;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static HashSet<string> ChooseNeighboursOfRange(string[] used_range, UndirectedGraph<string, Edge<string>> inputGraph)
        {
            var keys = NeighboursOfRange.Keys.ToArray();
            var exists = keys.Any(x => new HashSet<string>(x).SetEquals(used_range));
            keys = null;
            if (!exists)
            {
                var result = new HashSet<string>();
                for (int i = used_range.Length - 1; i >= 0; i--)
                {
                    var local = GetNeighbors(inputGraph, used_range[i]);
                    if (local.Count == 0)
                    {
                        local = null;
                        continue;
                    }
                    /* You'd wonder why I didn't just do this:
                     * for (int j = 0; j < local.Count; j++)
                        {
                            if (!used_range.Contains(local[j]))
                            {
                                result.Add(local[j]);
                            }
                        }
                     * instead of the creepy thing below.
                     * Well, it turns out that, for whatever reason only the compiler knows, the above code
                     * makes the program incredibly slow. Code that runs in 15secs suddenly started taking over 3hours. 
                     * */
                    int counter = 0;
                    for (int j = 0; j < local.Count + counter; j++)
                    {
                        if (used_range.Contains(local[j - counter]))
                        {
                            try
                            {
                                local.Remove(local[j - counter]);
                                counter++;
                            }
                            catch { }
                            if (local.Count == 0)
                            {
                                break;
                            }
                        }
                    }
                    foreach (var item in local)
                    {
                        result.Add(item);
                    }

                    local = null;
                }
                NeighboursOfRange[used_range] = result;

                result = null;
            }

            return NeighboursOfRange[used_range];
        }

        private static ConcurrentDictionary<string[], string> MostConstrainedNeighbours;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain">Domain, D, of fumction f</param>
        /// <param name="queryGraph">H</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetMostConstrainedNeighbour(string[] domain, UndirectedGraph<string, Edge<string>> queryGraph)
        {
            /*
             * As is standard in backtracking searches, the algorithm uses the most constrained neighbor
             * to eliminate maps that cannot be isomorphisms: that is, the neighbor of the already-mapped 
             * nodes which is likely to have the fewest possible nodes it can be mapped to. First we select 
             * the nodes with the most already-mapped neighbors, and amongst those we select the nodes with 
             * the highest degree and largest neighbor degree sequence.
             * */
            var keys = MostConstrainedNeighbours.Keys.ToArray();
            bool exists = keys.Any(x => new HashSet<string>(x).SetEquals(domain));
            keys = null;
            if (!exists)
            {
                var result = new List<string>();
                for (int i = domain.Length - 1; i >= 0; i--)
                {
                    var local = GetNeighbors(queryGraph, domain[i], false);
                    if (local.Count == 0)
                    {
                        local = null;
                        continue;
                    }
                    /* You'd wonder why I didn't just do this:
                     * for (int j = 0; j < local.Count; j++)
                        {
                            if (!domain.Contains(local[j]))
                            {
                                result.Add(local[j]);
                            }
                        }
                     * instead of the creepy thing below.
                     * Well, it turns out that, for whatever reason only the compiler knows, the above code
                     * makes the program incredibly slow. Code that runs in 15secs suddenly started taking over 3hours. 
                     * */
                    int counter = 0;
                    for (int j = 0; j < local.Count + counter; j++)
                    {
                        if (domain.Contains(local[j - counter]))
                        {
                            try
                            {
                                local.Remove(local[j - counter]);
                                counter++;
                            }
                            catch { }
                            if (local.Count == 0)
                            {
                                break;
                            }
                        }
                    }
                    foreach (var item in local)
                    {
                        result.Add(item);
                    }

                    local = null;
                }
                if (result.Count == 0)
                {
                    MostConstrainedNeighbours[domain] = "";
                }
                else
                {
                    MostConstrainedNeighbours[domain] = result[0];
                }
                result = null;
            }

            return MostConstrainedNeighbours[domain];
        }

        /// <summary>
        /// We say that <paramref name="node_G"/> (g) of <paramref name="inputGraph"/> (G) can support <paramref name="node_H"/> (h) of <paramref name="queryGraph"/> (H)
        /// if we cannot rule out a subgraph isomorphism from H into G which maps h to g based on the degrees of h and g, and the degree of their neighbours
        /// </summary>
        /// <param name="queryGraph">H</param>
        /// <param name="node_H">h</param>
        /// <param name="inputGraph">G</param>
        /// <param name="node_G">g</param>
        /// <returns></returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CanSupport(UndirectedGraph<string, Edge<string>> queryGraph, string node_H, UndirectedGraph<string, Edge<string>> inputGraph, string node_G)
        {
            // 1. Based on their degrees
            if (inputGraph.AdjacentDegree(node_G) < queryGraph.AdjacentDegree(node_H)) return false;

            //So, deg(g) >= deg(h).
            //2. Based on the degree of their neighbors
            var gNeighbors = GetNeighbors(inputGraph, node_G);
            var hNeighbors = GetNeighbors(queryGraph, node_H);
            for (int i = hNeighbors.Count - 1; i >= 0; i--)
            {
                for (int j = gNeighbors.Count - 1; j >= 0; j--)
                {
                    if (inputGraph.AdjacentDegree(gNeighbors[j]) >= queryGraph.AdjacentDegree(hNeighbors[i]))
                    {
                        gNeighbors = null;
                        hNeighbors = null;
                        return true;
                    }
                }
            }
            gNeighbors = null;
            hNeighbors = null;
            return false;
        }

        private static List<string> GetNeighbors(UndirectedGraph<string, Edge<string>> graph, string vertex, bool isG = true)
        {
            if (string.IsNullOrWhiteSpace(vertex)) return new List<string>();
            List<string> neighbors;
            if (isG)
            {
                if (G_NodeNeighbours.TryGetValue(vertex, out neighbors))
                {
                    return neighbors;
                }
                else
                {
                    var adjEdges = graph.AdjacentEdges(vertex);
                    var set = new HashSet<string>(adjEdges.Select(x => x.Source).Union(adjEdges.Select(x => x.Target)));
                    set.Remove(vertex);
                    adjEdges = null;
                    G_NodeNeighbours[vertex] = neighbors = set.ToList();
                    return neighbors;
                }
            }
            else
            {
                if (H_NodeNeighbours.TryGetValue(vertex, out neighbors))
                {
                    return neighbors;
                }
                else
                {
                    var adjEdges = graph.AdjacentEdges(vertex);
                    var set = new HashSet<string>(adjEdges.Select(x => x.Source).Union(adjEdges.Select(x => x.Target)));
                    set.Remove(vertex);
                    adjEdges = null;
                    H_NodeNeighbours[vertex] = neighbors = set.ToList();
                    return neighbors;
                }
            }

        }


    }
}

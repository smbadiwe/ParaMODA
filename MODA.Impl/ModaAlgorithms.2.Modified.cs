//This is the one that has gone bad
using QuickGraph;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        /// <summary>
        /// Mapping module (aka FindSubgraphInstances in Grochow & Kellis) modified
        /// </summary>
        /// <param name="queryGraph">H</param>
        /// <param name="inputGraph">G</param>
        /// <param name="numberOfSamples">To be decided. If not set, we use the <paramref name="inputGraph"/> size</param>
        private List<Mapping> Algorithm2_Modified(UndirectedGraph<string, Edge<string>> queryGraph, UndirectedGraph<string, Edge<string>> inputGraph, int numberOfSamples = -1)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            if (numberOfSamples <= 0) numberOfSamples = inputGraph.VertexCount / 3;
            //int counter = 0;
            // Do we need this clone? Can't we just remove the node directly from the graph?
            //var inputGraphClone = inputGraph.Clone();
            var theMappings = new Dictionary<string, List<Mapping>>();
            var h_ = queryGraph.Vertices.First(); // var queryGraphVertices = queryGraph.Vertices.ToList();
            var degSeq = inputGraph.GetDegreeSequence();
            var tasks = new List<Task>();
            List<string>[] chunks = new List<string>[numberOfSamples < 20 ? 1 : numberOfSamples / 20];
            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i] = new List<string>();
            }
            int iter = 0;
            foreach (var v in degSeq.Take(numberOfSamples))
            {
                chunks[iter % chunks.Length].Add(v);
                iter++;
            }
            iter = 0;
            var sBuilder = new StringBuilder();
            sBuilder.AppendFormat("Nomber of Iterations: {0}. Number of chunks to parallelize: {1}\n", numberOfSamples, chunks.Length);
            InputSubgraphs = new ConcurrentDictionary<string[], UndirectedGraph<string, Edge<string>>>();
            MostConstrainedNeighbours = new ConcurrentDictionary<string[], string>();
            NeighboursOfRange = new ConcurrentDictionary<string[], HashSet<string>>();
            G_NodeNeighbours = new ConcurrentDictionary<string, List<string>>();
            H_NodeNeighbours = new ConcurrentDictionary<string, List<string>>();
            foreach (var degSeqChunk in chunks)
            {
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
                            sw.Stop();
                            var logGist = new StringBuilder();
                            logGist.AppendFormat("Maps gotten from IsoExtension.\tTook:\t{0:N}s.\th = {1}. g = {2}\n", sw.Elapsed.ToString(), h, g);
                            sw.Restart();

                            lock (theMappings)
                            {
                                if (theMappings.Count == 0)
                                {
                                    theMappings.Add(g, mappings);
                                }
                                else
                                {
                                    List<Mapping> newMappings = new List<Mapping>();
                                    foreach (var map in mappings)
                                    {
                                        List<Mapping> mappingsToSearch;
                                        if (theMappings.TryGetValue(map.Function.Last().Value, out mappingsToSearch))
                                        {
                                            var existing = mappingsToSearch.Find(x => x.Equals(map));

                                            if (existing == null)
                                            {
                                                newMappings.Add(map);
                                            }
                                        }
                                        else
                                        {
                                            newMappings.Add(map);
                                        }
                                    }

                                    theMappings.Add(g, newMappings);
                                    newMappings = null;
                                }
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
                }, new object[] { degSeqChunk.ToArray(), queryGraph, h_, inputGraph })
                );
                sBuilder.AppendFormat("\tChunk {0} loaded. Number of items in chunk: {1}\n", ++iter, degSeqChunk.Count);

            }
            Console.WriteLine(sBuilder);
            Task.WaitAll(tasks.ToArray());

            var toReturn = theMappings.SelectMany(x => x.Value).ToList();
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
            degSeq = null;
            return toReturn;
        }

        /// <summary>
        /// Given a set of nodes(the Key), we find the subgraph in the input graph G that has those nodes.
        /// </summary>
        private ConcurrentDictionary<string[], UndirectedGraph<string, Edge<string>>> InputSubgraphs;

        /// <summary>
        /// Algorithm taken from Grochow and Kellis. This is failing at the moment
        /// </summary>
        /// <param name="partialMap">f; Map is represented as a dictionary, with the Key as h and the Value as g</param>
        /// <param name="queryGraph">G</param>
        /// <param name="inputGraph">H</param>
        /// <returns>List of isomorphisms. Remember, Key is h, Value is g</returns>
        private List<Mapping> IsomorphicExtension(Dictionary<string, string> partialMap, UndirectedGraph<string, Edge<string>> queryGraph
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

                var inputSubgraphKey = InputSubgraphs.Keys.ToList().Find(x => new HashSet<string>(x).SetEquals(partialMap.Values));
                if (inputSubgraphKey == null || inputSubgraphKey.Length == 0)
                {
                    var newInputSubgraph = new UndirectedGraph<string, Edge<string>>(false);
                    inputSubgraphKey = partialMap.Values.ToArray();
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
                    InputSubgraphs.TryAdd(inputSubgraphKey, newInputSubgraph);

                }
                map.InputSubGraph = InputSubgraphs[inputSubgraphKey];

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

        internal static ConcurrentDictionary<string, List<string>> G_NodeNeighbours;
        internal static ConcurrentDictionary<string, List<string>> H_NodeNeighbours;

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
        private bool IsNeighbourIncompatible(UndirectedGraph<string, Edge<string>> inputGraph, UndirectedGraph<string, Edge<string>> queryGraph,
            string n, string m, Dictionary<string, string> partialMap)
        {
            //  RECALL: m is for Domain, the Key => the query graph

            //A: If there is a neighbor d ∈ D of m such that n is NOT neighbors with f(d)...
            var neighboursOfN = inputGraph.GetNeighbors(n);
            var neighborsOfM = queryGraph.GetNeighbors(m, false);
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

        private ConcurrentDictionary<string[], HashSet<string>> NeighboursOfRange;

        private HashSet<string> ChooseNeighboursOfRange(string[] used_range, UndirectedGraph<string, Edge<string>> inputGraph)
        {
            var cachedData = NeighboursOfRange.FirstOrDefault(x => new HashSet<string>(x.Key).SetEquals(used_range)).Key;
            if (cachedData == null || cachedData.Length == 0)
            {
                var result = new HashSet<string>();
                for (int i = 0; i < used_range.Length; i++)
                {
                    var local = inputGraph.GetNeighbors(used_range[i]);
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
                                if (local.Count == 0)
                                {
                                    break;
                                }
                                counter++;
                            }
                            catch (Exception ex)
                            {
                                throw new ArgumentOutOfRangeException(string.Format("ERROR in 'ChooseNeighboursOfRange' trying to remove from local:\n\t in Index (j - counter) = {0}; j = {1}; counter = {2}; local.Count = {3}.", (j - counter), j, counter, local.Count), ex);
                            }
                        }
                    }
                    foreach (var item in local)
                    {
                        result.Add(item);
                    }

                    local = null;
                }
                cachedData = used_range;
                NeighboursOfRange[cachedData] = result;

                result = null;
            }

            return NeighboursOfRange[cachedData];
        }

        private ConcurrentDictionary<string[], string> MostConstrainedNeighbours;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain">Domain, D, of fumction f</param>
        /// <param name="queryGraph">H</param>
        private string GetMostConstrainedNeighbour(string[] domain, UndirectedGraph<string, Edge<string>> queryGraph)
        {
            /*
             * As is standard in backtracking searches, the algorithm uses the most constrained neighbor
             * to eliminate maps that cannot be isomorphisms: that is, the neighbor of the already-mapped 
             * nodes which is likely to have the fewest possible nodes it can be mapped to. First we select 
             * the nodes with the most already-mapped neighbors, and amongst those we select the nodes with 
             * the highest degree and largest neighbor degree sequence.
             * */
            var cachedData = MostConstrainedNeighbours.FirstOrDefault(x => new HashSet<string>(x.Key).SetEquals(domain)).Key;
            if (cachedData == null || cachedData.Length == 0)
            {
                HashSet<string> result = new HashSet<string>();
                for (int i = 0; i < domain.Length; i++)
                {
                    var local = queryGraph.GetNeighbors(domain[i], false);
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
                                if (local.Count == 0)
                                {
                                    break;
                                }
                                counter++;
                            }
                            catch (Exception ex)
                            {
                                throw new ArgumentOutOfRangeException(string.Format("ERROR in 'GetMostConstrainedNeighbour' trying to remove from local:\n\t in Index (j - counter) = {0}; j = {1}; counter = {2}; local.Count = {3}.", (j - counter), j, counter, local.Count), ex);
                            }
                        }
                    }
                    foreach (var item in local)
                    {
                        result.Add(item);
                    }

                    local = null;
                }
                cachedData = domain;
                if (result.Count == 0)
                {
                    MostConstrainedNeighbours[cachedData] = "";
                }
                else
                {
                    MostConstrainedNeighbours[cachedData] = result.ElementAt(0);
                }
                result = null;
            }

            return MostConstrainedNeighbours[cachedData];
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
        private bool CanSupport(UndirectedGraph<string, Edge<string>> queryGraph, string node_H, UndirectedGraph<string, Edge<string>> inputGraph, string node_G)
        {
            // 1. Based on their degrees
            if (inputGraph.AdjacentDegree(node_G) < queryGraph.AdjacentDegree(node_H)) return false;

            //So, deg(g) >= deg(h).
            //2. Based on the degree of their neighbors
            var gNeighbors = inputGraph.GetNeighbors(node_G); 
            var hNeighbors = queryGraph.GetNeighbors(node_H);
            for (int i = 0; i < hNeighbors.Count; i++)
            {
                for (int j = 0; j < gNeighbors.Count; j++)
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

    }
}

using QuickGraph;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        /// <summary>
        /// If true, it means we only care about how many mappings are found for each subgraph, not info about the mappings themselves.
        /// </summary>
        public static bool GetOnlyMappingCounts { get; set; }
        /// <summary>
        /// If true, the program will use my modified Grochow's algorithm (Algo 2)
        /// </summary>
        public static bool UseModifiedGrochow { get; set; }

        #region Useful mainly for the Algorithm 2 versions

        /// <summary>
        /// Given a set of nodes(the Key), we find the subgraph in the input graph G that has those nodes.
        /// </summary>
        private static Dictionary<string[], UndirectedGraph<string, Edge<string>>> InputSubgraphs;
        /// <summary>
        /// Used to cache 
        /// </summary>
        private static Dictionary<string[], List<string>> NeighboursOfRange;
        /// <summary>
        /// Used to cache 
        /// </summary>
        private static Dictionary<string[], string> MostConstrainedNeighbours;
        /// <summary>
        /// Used to cache 
        /// </summary>
        public static Dictionary<string, IList<string>> G_NodeNeighbours;
        /// <summary>
        /// Used to cache 
        /// </summary>
        public static Dictionary<string, IList<string>> H_NodeNeighbours;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputGraph">The original input graph G</param>
        /// <param name="g_nodes">Usually {Mapping Instance}.Function.Values.ToArray();</param>
        /// <param name="subgraphSize">The query graph H's size</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UndirectedGraph<string, Edge<string>> GetInputSubgraph(UndirectedGraph<string, Edge<string>> inputGraph, IEnumerable<string> g_nodes, int subgraphSize)
        {
            UndirectedGraph<string, Edge<string>> newInputSubgraph = new UndirectedGraph<string, Edge<string>>();
            int counter = 0;
            foreach (var node in g_nodes)
            {
                for (int j = (counter + 1); j < subgraphSize; j++)
                {
                    Edge<string> edge_;
                    if (inputGraph.TryGetEdge(node, g_nodes.ElementAt(j), out edge_))
                    {
                        newInputSubgraph.AddVerticesAndEdge(edge_);
                    }
                }
                counter++;
            }

            return newInputSubgraph;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="inputGraph"></param>
        ///// <param name="g_nodes">Usually {Mapping Instance}.Function.Values.ToArray();</param>
        ///// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static UndirectedGraph<string, Edge<string>> GetInputSubgraph(UndirectedGraph<string, Edge<string>> inputGraph, string[] g_nodes)
        //{
        //    UndirectedGraph<string, Edge<string>> newInputSubgraph;
        //    if (!InputSubgraphs.TryGetValue(g_nodes, out newInputSubgraph))
        //    {
        //        newInputSubgraph = new UndirectedGraph<string, Edge<string>>();
        //        for (int k = 0; k < g_nodes.Length; k++)
        //        {
        //            for (int j = (k + 1); j < g_nodes.Length; j++)
        //            {
        //                Edge<string> edge_;
        //                if (inputGraph.TryGetEdge(g_nodes[k], g_nodes[j], out edge_))
        //                {
        //                    newInputSubgraph.AddVerticesAndEdge(edge_);
        //                }
        //            }
        //        }
        //        InputSubgraphs[g_nodes] = newInputSubgraph;
        //    }
        //    return newInputSubgraph;
        //}

        /// <summary>
        /// Algorithm taken from Grochow and Kellis. This is failing at the moment
        /// </summary>
        /// <param name="partialMap">f; Map is represented as a dictionary, with the Key as h and the Value as g</param>
        /// <param name="queryGraph">G</param>
        /// <param name="inputGraph">H</param>
        /// <returns>List of isomorphisms. Remember, Key is h, Value is g</returns>
        private static IList<Mapping> IsomorphicExtension(Dictionary<string, string> partialMap, UndirectedGraph<string, Edge<string>> queryGraph
            , UndirectedGraph<string, Edge<string>> inputGraph)
        {
            if (partialMap.Count == queryGraph.VertexCount)
            {
                #region Return base case
                var map = new Mapping(partialMap);
                int counter = 0, subgraphSize = partialMap.Count;
                Edge<string> edge_;
                foreach (var node in partialMap) // Remember, f(h) = g, so .Values is for g's
                {
                    for (int j = (counter + 1); j < subgraphSize; j++)
                    {
                        if (inputGraph.TryGetEdge(node.Value, partialMap.ElementAt(j).Value, out edge_))
                        {
                            map.InducedSubGraph.AddVerticesAndEdge(edge_);
                        }
                    }
                    counter++;
                }
                return new List<Mapping>(1) { map };
                #endregion
            }

            //Remember: f(h) = g, so h is Domain and g is Range.
            //  In other words, Key is h and Value is g in the dictionary

            // get m, most constrained neighbor
            string m = GetMostConstrainedNeighbour(partialMap.Keys.ToArray(), queryGraph);
            if (string.IsNullOrWhiteSpace(m)) return new Mapping[0];

            var listOfIsomorphisms = new List<Mapping>();

            var neighbourRange = ChooseNeighboursOfRange(partialMap.Values.ToArray(), inputGraph);

            var neighborsOfM = queryGraph.GetNeighbors(m, false);
            var newPartialMapCount = partialMap.Count + 1;
            for (int i = 0; i < neighbourRange.Count; i++) //foreach neighbour n of f(D)
            {
                var n = neighbourRange[i];
                if (false == IsNeighbourIncompatible(inputGraph, n, partialMap, neighborsOfM))
                {
                    //It's not; so, let f' = f on D, and f'(m) = n.

                    //Find all isomorphic extensions of f'.
                    //newPartialMap[m] = neighbourRange[i];
                    var newPartialMap = new Dictionary<string, string>(newPartialMapCount);
                    foreach (var item in partialMap)
                    {
                        newPartialMap.Add(item.Key, item.Value);
                    }
                    newPartialMap[m] = n;
                    var subList = IsomorphicExtension(newPartialMap, queryGraph, inputGraph);
                    if (subList.Count > 0)
                    {
                        listOfIsomorphisms.AddRange(subList);
                    }
                }
            }
            return listOfIsomorphisms;
        }

        /// <summary>
        /// If there is a neighbor d ∈ D of m such that n is NOT neighbors with f(d),
        /// or if there is a NON-neighbor d ∈ D of m such that n is neighbors with f(d) 
        /// [or if assigning f(m) = n would violate a symmetry-breaking condition in C(h)]
        /// then neighbour is compatible. So contiue with the next n (as in, return false)
        /// </summary>
        /// <param name="inputGraph">G</param>
        /// <param name="n">g_node, pass in 'neighbour'; n in Grochow</param>
        /// <param name="domain">domain_in_H</param>
        /// <param name="partialMap">function</param>
        /// <param name="neighborsOfM">neighbors of h_node in the queryGraph/></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNeighbourIncompatible(UndirectedGraph<string, Edge<string>> inputGraph,
            string n, Dictionary<string, string> partialMap, IList<string> neighborsOfM)
        {
            //  RECALL: m is for Domain, the Key => the query graph

            //A: If there is a neighbor d ∈ D of m such that n is NOT neighbors with f(d)...
            //for (int i = 0; i < neighborsOfM.Count; i++)
            //{
            //    string val; // f(d)
            //    if (!partialMap.TryGetValue(neighborsOfM[i], out val))
            //    {
            //        return true; //=> it's compatible
            //    }
            //    var neighboursOfN = inputGraph.GetNeighbors(val, true);
            //    if (!neighboursOfN.Contains(n))
            //    {
            //        neighboursOfN = null;
            //        return true; //=> it's compatible
            //    }
            //}

            //return false;

            var neighboursOfN = inputGraph.GetNeighbors(n, true);
            for (int i = 0; i < neighborsOfM.Count; i++)
            {
                string val;
                if (!partialMap.TryGetValue(neighborsOfM[i], out val))
                {
                    neighboursOfN = null;
                    return false;
                }
                if (!neighboursOfN.Contains(val))
                {
                    neighboursOfN = null;
                    return true;
                }
            }

            neighboursOfN = null;
            return false;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="used_range"> Meaning that we're only interested in the Values. Remember: f(h) = g</param>
        ///// <param name="inputGraph">G</param>
        ///// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static List<string> ChooseNeighboursOfRange(Dictionary<string, string> used_range, UndirectedGraph<string, Edge<string>> inputGraph)
        //{
        //    List<string> toReturn = new List<string>();
        //    foreach (var rangeVal in used_range)
        //    {
        //        var local = inputGraph.GetNeighbors(rangeVal.Value, true);
        //        if (local.Count > 0)
        //        {
        //            List<string> batch = new List<string>(local.Count);
        //            for (int j = 0; j < local.Count; j++)
        //            {
        //                if (!used_range.ContainsValue(local[j]))
        //                {
        //                    batch.Add(local[j]);
        //                }
        //            }
        //            toReturn.AddRange(batch);
        //            batch = null;
        //            local = null;
        //        }
        //        else
        //        {
        //            return toReturn;
        //        }
        //    }

        //    return toReturn;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="used_range"> Meaning that we're only interested in the Values. Remember: f(h) = g</param>
        ///// <param name="inputGraph">G</param>
        ///// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static List<string> ChooseNeighboursOfRange(IEnumerable<string> used_range, UndirectedGraph<string, Edge<string>> inputGraph)
        //{
        //    List<string> toReturn = new List<string>();
        //    foreach (var rangeVal in used_range)
        //    {
        //        var local = inputGraph.GetNeighbors(rangeVal, true);
        //        if (local.Count > 0)
        //        {
        //            List<string> batch = new List<string>(local.Count);
        //            for (int j = 0; j < local.Count; j++)
        //            {
        //                if (!used_range.Contains(local[j]))
        //                {
        //                    batch.Add(local[j]);
        //                }
        //            }
        //            toReturn.AddRange(batch);
        //            batch = null;
        //            local = null;
        //        }
        //        else
        //        {
        //            return toReturn;
        //        }
        //    }

        //    return toReturn;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="used_range"> Meaning that we're only interested in the Values. Remember: f(h) = g</param>
        /// <param name="inputGraph">G</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<string> ChooseNeighboursOfRange(string[] used_range, UndirectedGraph<string, Edge<string>> inputGraph)
        {
            List<string> toReturn = new List<string>();
            for (int i = 0; i < used_range.Length; i++)
            {
                var local = inputGraph.GetNeighbors(used_range[i], true);
                if (local.Count > 0)
                {
                    List<string> batch = new List<string>(local.Count);
                    for (int j = 0; j < local.Count; j++)
                    {
                        if (!used_range.Contains(local[j]))
                        {
                            batch.Add(local[j]);
                        }
                    }
                    toReturn.AddRange(batch);
                    batch = null;
                    local = null;
                }
                else
                {
                    return toReturn;
                }
            }

            return toReturn;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="domain">Domain, D, of fumction f. Meaning that we're only interested in the Keys. Remember: f(h) = g</param>
        ///// <param name="queryGraph">H</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static string GetMostConstrainedNeighbour(Dictionary<string, string> domain, UndirectedGraph<string, Edge<string>> queryGraph)
        //{
        //    /*
        //     * As is standard in backtracking searches, the algorithm uses the most constrained neighbor
        //     * to eliminate maps that cannot be isomorphisms: that is, the neighbor of the already-mapped 
        //     * nodes which is likely to have the fewest possible nodes it can be mapped to. First we select 
        //     * the nodes with the most already-mapped neighbors, and amongst those we select the nodes with 
        //     * the highest degree and largest neighbor degree sequence.
        //     * */
        //    foreach (var item in domain)
        //    {
        //        var local = queryGraph.GetNeighbors(item.Key, false);
        //        if (local.Count > 0)
        //        {
        //            for (int j = 0; j < local.Count; j++)
        //            {
        //                if (!domain.ContainsKey(local[j]))
        //                {
        //                    return local[j];
        //                }
        //            }
        //        }
        //        local = null;
        //    }
        //    return "";
        //}
        
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="domain">Domain, D, of fumction f. Meaning that we're only interested in the Keys. Remember: f(h) = g</param>
        ///// <param name="queryGraph">H</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static string GetMostConstrainedNeighbour(IEnumerable<string> domain, UndirectedGraph<string, Edge<string>> queryGraph)
        //{
        //    /*
        //     * As is standard in backtracking searches, the algorithm uses the most constrained neighbor
        //     * to eliminate maps that cannot be isomorphisms: that is, the neighbor of the already-mapped 
        //     * nodes which is likely to have the fewest possible nodes it can be mapped to. First we select 
        //     * the nodes with the most already-mapped neighbors, and amongst those we select the nodes with 
        //     * the highest degree and largest neighbor degree sequence.
        //     * */
        //    foreach (var item in domain)
        //    {
        //        var local = queryGraph.GetNeighbors(item, false);
        //        if (local.Count > 0)
        //        {
        //            for (int j = 0; j < local.Count; j++)
        //            {
        //                if (!domain.Contains(local[j]))
        //                {
        //                    return local[j];
        //                }
        //            }
        //        }
        //        local = null;
        //    }
        //    return "";
        //}
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain">Domain, D, of fumction f. Meaning that we're only interested in the Keys. Remember: f(h) = g</param>
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
            for (int i = 0; i < domain.Length; i++)
            {
                var local = queryGraph.GetNeighbors(domain[i], false);
                if (local.Count > 0)
                {
                    for (int j = 0; j < local.Count; j++)
                    {
                        if (!domain.Contains(local[j]))
                        {
                            return local[j];
                        }
                    }
                }
                local = null;
            }
            return "";
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
        private static bool CanSupport(QueryGraph queryGraph, string node_H, UndirectedGraph<string, Edge<string>> inputGraph, string node_G)
        {
            // 1. Based on their degrees
            if (inputGraph.AdjacentDegree(node_G) >= queryGraph.AdjacentDegree(node_H))
            {
                // => we can map the querygraph unto the input graph, based on the nodes given.
                // That means we are not ruling out isomorphism. So...
                return true;
            }

            return false;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static bool CanSupport(QueryGraph queryGraph, string node_H, UndirectedGraph<string, Edge<string>> inputGraph, string node_G)
        //{
        //    // 1. Based on their degrees
        //    if (inputGraph.AdjacentDegree(node_G) < queryGraph.AdjacentDegree(node_H))
        //    {
        //        // => we cannot map the querygraph unto the input graph, based on the nodes given.
        //        // That means we are ruling out isomorphism. So...
        //        return false;
        //    }
        //    //So, deg(g) >= deg(h).

        //    //2. Based on the degree of their neighbors
        //    var gNeighbors = inputGraph.GetNeighbors(node_G, true);
        //    var hNeighbors = queryGraph.GetNeighbors(node_H, false);
        //    //TODO: either review or remove this test
        //    for (int i = 0; i < hNeighbors.Count; i++)
        //    {
        //        var hNode = hNeighbors[i];
        //        for (int j = 0; j < gNeighbors.Count; j++)
        //        {
        //            if (inputGraph.AdjacentDegree(gNeighbors[j]) >= queryGraph.AdjacentDegree(hNode))
        //            {
        //                gNeighbors = null;
        //                hNeighbors = null;
        //                return true;
        //            }
        //        }
        //    }
        //    gNeighbors = null;
        //    hNeighbors = null;
        //    return false;
        //}
        #endregion
    }
}

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
        public static bool UsingAlgo3 { get; set; }

        #region Useful mainly for the Algorithm 2 versions

        /// <summary>
        /// Used to cache 
        /// </summary>
        public static Dictionary<string, HashSet<string>> G_NodeNeighbours;
        /// <summary>
        /// Used to cache 
        /// </summary>
        public static Dictionary<string, HashSet<string>> H_NodeNeighbours;

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

        /// <summary>
        /// Algorithm taken from Grochow and Kellis. This is failing at the moment
        /// </summary>
        /// <param name="partialMap">f; Map is represented as a dictionary, with the Key as h and the Value as g</param>
        /// <param name="queryGraph">G</param>
        /// <param name="inputGraph">H</param>
        /// <returns>List of isomorphisms. Remember, Key is h, Value is g</returns>
        private static IList<Mapping> IsomorphicExtension(Dictionary<string, string> partialMap, QueryGraph queryGraph
            , UndirectedGraph<string, Edge<string>> inputGraph)
        {
            if (partialMap.Count == queryGraph.VertexCount)
            {
                #region Return base case
                var map = new Mapping(partialMap);
                int subgraphSize = partialMap.Count;
                var g_nodes = new List<string>(subgraphSize); // Remember, f(h) = g, so .Values is for g's
                var h_nodes = new List<string>(subgraphSize); // Remember, f(h) = g, so .Keys is for h's
                foreach (var item in partialMap)
                {
                    h_nodes.Add(item.Key);
                    g_nodes.Add(item.Value);
                }
                Edge<string> edge_g = null;
                var inducedSubGraphEdges = new List<Edge<string>>();
                for (int i = 0; i < subgraphSize - 1; i++)
                {
                    for (int j = (i + 1); j < subgraphSize; j++)
                    {
                        var edge_h = false;
                        if (queryGraph.ContainsEdge(h_nodes[i], h_nodes[j]))
                        {
                            edge_h = true;
                            if (!inputGraph.TryGetEdge(g_nodes[i], g_nodes[j], out edge_g))
                            {
                                g_nodes.Clear();
                                h_nodes.Clear();
                                inducedSubGraphEdges.Clear();
                                g_nodes = null;
                                h_nodes = null;
                                inducedSubGraphEdges = null;
                                return new Mapping[0];
                            }
                        }
                        if (edge_h == false) // => edge_g was never evaluated because the first part of the AND statement was false
                        {
                            if (inputGraph.TryGetEdge(g_nodes[i], g_nodes[j], out edge_g))
                            {
                                inducedSubGraphEdges.Add(edge_g);
                            }
                        }
                        else
                        {
                            if (edge_g != null)
                            {
                                inducedSubGraphEdges.Add(edge_g);
                            }
                        }
                    }
                }
                g_nodes.Clear();
                h_nodes.Clear();
                g_nodes = null;
                h_nodes = null;
                edge_g = null;
                if (queryGraph.EdgeCount > inducedSubGraphEdges.Count) // this shouuld never happen; but just in case
                {
                    inducedSubGraphEdges.Clear();
                    inducedSubGraphEdges = null;
                    return new Mapping[0];
                }

                map.InducedSubGraphEdgesCount = inducedSubGraphEdges.Count;

                inducedSubGraphEdges.Clear();
                inducedSubGraphEdges = null;
                return new List<Mapping>(1) { map };
                #endregion

            }

            //Remember: f(h) = g, so h is Domain and g is Range.
            //  In other words, Key is h and Value is g in the dictionary

            // get m, most constrained neighbor
            string m = GetMostConstrainedNeighbour(partialMap.Keys, queryGraph);
            if (string.IsNullOrWhiteSpace(m)) return new Mapping[0];

            var listOfIsomorphisms = new List<Mapping>();

            var neighbourRange = ChooseNeighboursOfRange(partialMap.Values, inputGraph);

            var neighborsOfM = queryGraph.GetNeighbors(m, false);
            var newPartialMapCount = partialMap.Count + 1;
            foreach (var n in neighbourRange) //foreach neighbour n of f(D)
            {
                if (false == IsNeighbourIncompatible(inputGraph, n, partialMap, queryGraph, neighborsOfM))
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
        /// then neighbour is incompatible. So contiue with the next n (as in, return true)
        /// </summary>
        /// <param name="inputGraph">G</param>
        /// <param name="n">g_node, pass in 'neighbour'; n in Grochow</param>
        /// <param name="domain">domain_in_H</param>
        /// <param name="partialMap">function</param>
        /// <param name="queryGraph"></param>
        /// <param name="neighborsOfM">neighbors of h_node in the <paramref name="queryGraph"/> /></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNeighbourIncompatible(UndirectedGraph<string, Edge<string>> inputGraph,
            string n, Dictionary<string, string> partialMap, QueryGraph queryGraph, IList<string> neighborsOfM)
        {
            //  RECALL: m is for Domain, the Key => the query graph
            if (partialMap.ContainsValue(n))
            {
                return true; // cos it's already done
            }

            //If there is a neighbor d ∈ D of m such that n is NOT neighbors with f(d),
            var neighboursOfN = inputGraph.GetNeighbors(n, true);
            bool doNext = false;
            string val; // f(d)
            foreach (var d in neighborsOfM)
            {
                if (!partialMap.TryGetValue(d, out val))
                {
                    //neighboursOfN = null;
                    //return false;
                    doNext = true;
                    break;
                }
                if (!neighboursOfN.Contains(val))
                {
                    neighboursOfN = null;
                    return true;
                }
            }

            // or if there is a NON - neighbor d ∈ D of m such that n IS neighbors with f(d)
            if (doNext && queryGraph.VertexCount > 4)
            {
                foreach (var d in queryGraph.Vertices.Except(neighborsOfM))
                {
                    if (!partialMap.TryGetValue(d, out val))
                    {
                        neighboursOfN = null;
                        return false;
                    }
                    if (neighboursOfN.Contains(val))
                    {
                        neighboursOfN = null;
                        return true;
                    }
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
        //private static HashSet<string> ChooseNeighboursOfRange(IEnumerable<string> used_range, UndirectedGraph<string, Edge<string>> inputGraph)
        //{
        //    var toReturn = new List<string>();
        //    foreach (var range in used_range)
        //    {
        //        var local = inputGraph.GetNeighbors(range, true);
        //        toReturn.AddRange(local);
        //        local = null;
        //    }

        //    return new HashSet<string>(toReturn); //.ToArray();
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="used_range"> Meaning that we're only interested in the Values. Remember: f(h) = g</param>
        /// <param name="inputGraph">G</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<string> ChooseNeighboursOfRange(IEnumerable<string> used_range, UndirectedGraph<string, Edge<string>> inputGraph)
        {
            List<string> toReturn = new List<string>();
            foreach (var range in used_range)
            {
                var local = inputGraph.GetNeighbors(range, true);
                if (local.Count > 0)
                {
                    List<string> batch = new List<string>(local.Count);
                    foreach (var loc in local)
                    {
                        if (!used_range.Contains(loc))
                        {
                            batch.Add(loc);
                        }
                    }
                    toReturn.AddRange(batch);
                    batch.Clear(); // = null;
                    local = null;
                }
                else
                {
                    return toReturn;
                }
            }

            return toReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain">Domain, D, of fumction f. Meaning that we're only interested in the Keys. Remember: f(h) = g</param>
        /// <param name="queryGraph">H</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetMostConstrainedNeighbour(IEnumerable<string> domain, UndirectedGraph<string, Edge<string>> queryGraph)
        {
            /*
             * As is standard in backtracking searches, the algorithm uses the most constrained neighbor
             * to eliminate maps that cannot be isomorphisms: that is, the neighbor of the already-mapped 
             * nodes which is likely to have the fewest possible nodes it can be mapped to. First we select 
             * the nodes with the most already-mapped neighbors, and amongst those we select the nodes with 
             * the highest degree and largest neighbor degree sequence.
             * */
            foreach (var dom in domain)
            {
                var local = queryGraph.GetNeighbors(dom, false);
                if (local.Count > 0)
                {
                    foreach (var loc in local)
                    {
                        if (!domain.Contains(loc))
                        {
                            return loc;
                        }
                    }
                }
                local = null;
            }
            return "";
        }

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
        //    var tempList = new Dictionary<string, int>();
        //    foreach (var node in queryGraph.Vertices.Except(domain))
        //    {
        //        tempList.Add(node, queryGraph.AdjacentDegree(node));
        //    }
        //    foreach (var item in tempList.OrderByDescending(x => x.Value))
        //    {
        //        return item.Key; // Only the first is needed
        //    }
        //    return "";
        //}

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

        #endregion
    }
}

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
                int counter = 0, subgraphSize = partialMap.Count;
                Edge<string> edge_;
                foreach (var node in partialMap) // Remember, f(h) = g, so .Values is for g's
                {
                    for (int j = (counter + 1); j < subgraphSize; j++)
                    {
                        if (inputGraph.TryGetEdge(node.Value, partialMap.ElementAt(j).Value, out edge_))
                        {
                            map.InducedSubGraphEdges.Add(edge_);
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
            string m = GetMostConstrainedNeighbour(partialMap.Keys, queryGraph);
            if (string.IsNullOrWhiteSpace(m)) return new Mapping[0];

            var listOfIsomorphisms = new List<Mapping>();

            var neighbourRange = ChooseNeighboursOfRange(partialMap.Values, inputGraph);

            var neighborsOfM = queryGraph.GetNeighbors(m, false);
            var newPartialMapCount = partialMap.Count + 1;
            foreach (var n in neighbourRange) //foreach neighbour n of f(D)
            {
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
            string n, Dictionary<string, string> partialMap, HashSet<string> neighborsOfM)
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

            string val;
            foreach (var d in neighborsOfM)
            {
                if (!partialMap.TryGetValue(d, out val))
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

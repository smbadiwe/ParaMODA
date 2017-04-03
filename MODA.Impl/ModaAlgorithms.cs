using QuickGraph;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        private static MappingNodesComparer comparer;
        /// <summary>
        /// If true, then the querygraph must match exactly to the input subgraph. In other words, only induced subgraphs will be returned
        /// </summary>
        private static bool getInducedMappingsOnly;
        static ModaAlgorithms()
        {
            comparer = new MappingNodesComparer();
            getInducedMappingsOnly = false;
        }
        
        /// <summary>
        /// If true, the program will use my modified Grochow's algorithm (Algo 2)
        /// </summary>
        public static bool UseModifiedGrochow { get; set; }

        #region Useful mainly for the Algorithm 2 versions

        /// <summary>
        /// Used to cache 
        /// </summary>
        public static Dictionary<int, HashSet<int>> G_NodeNeighbours;
        /// <summary>
        /// Used to cache 
        /// </summary>
        public static Dictionary<int, HashSet<int>> H_NodeNeighbours;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputGraph">The original input graph G</param>
        /// <param name="g_nodes">Usually {Mapping Instance}.Function.Values.ToArray();</param>
        /// <param name="subgraphSize">The query graph H's size</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UndirectedGraph<int> GetInputSubgraph(UndirectedGraph<int> inputGraph, IEnumerable<int> g_nodes, int subgraphSize)
        {
            UndirectedGraph<int> newInputSubgraph = new UndirectedGraph<int>();
            int counter = 0;
            foreach (var node in g_nodes)
            {
                for (int j = (counter + 1); j < subgraphSize; j++)
                {
                    Edge<int> edge_;
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
        private static Dictionary<IList<int>, Mapping> IsomorphicExtension(Dictionary<int, int> partialMap, QueryGraph queryGraph
            , UndirectedGraph<int> inputGraph)
        {
            if (partialMap.Count == queryGraph.VertexCount)
            {
                #region Return base case
                var function = new SortedList<int, int>(partialMap);
                int subgraphSize = partialMap.Count;
                Edge<int> edge_g = default(Edge<int>);
                var inducedSubGraphEdges = new List<Edge<int>>();
                for (int i = 0; i < subgraphSize - 1; i++)
                {
                    for (int j = (i + 1); j < subgraphSize; j++)
                    {
                        var edge_h = false;
                        if (queryGraph.ContainsEdge(function.Keys[i], function.Keys[j]))
                        {
                            edge_h = true;
                            if (!inputGraph.TryGetEdge(function.Values[i], function.Values[j], out edge_g))
                            {
                                // No correspondent in the input graph
                                //inducedSubGraphEdges.Clear();
                                inducedSubGraphEdges = null;
                                return null;
                            }
                        }
                        if (edge_h == false) // => edge_g was never evaluated because the first part of the AND statement was false
                        {
                            if (inputGraph.TryGetEdge(function.Values[i], function.Values[j], out edge_g))
                            {
                                inducedSubGraphEdges.Add(edge_g);
                            }
                        }
                        else
                        {
                            if (false == EqualityComparer<Edge<int>>.Default.Equals(edge_g, default(Edge<int>)))
                            {
                                inducedSubGraphEdges.Add(edge_g);
                            }
                        }
                    }
                }
                edge_g = default(Edge<int>);
                if (queryGraph.EdgeCount > inducedSubGraphEdges.Count) // this shouuld never happen; but just in case
                {
                    //inducedSubGraphEdges.Clear();
                    inducedSubGraphEdges = null;
                    return null;
                }

                // If we're to get induced mappings only, then the both query graph and the image must have the same number of edges
                if (getInducedMappingsOnly && (queryGraph.EdgeCount != inducedSubGraphEdges.Count))
                {
                    //inducedSubGraphEdges.Clear();
                    inducedSubGraphEdges = null;
                    return null;
                }
                
                return new Dictionary<IList<int>, Mapping>(1) { { function.Values, new Mapping(function, inducedSubGraphEdges.Count) } };
                #endregion

            }

            //Remember: f(h) = g, so h is Domain and g is Range.
            //  In other words, Key is h and Value is g in the dictionary

            // get m, most constrained neighbor
            int m = GetMostConstrainedNeighbour(partialMap.Keys, queryGraph);
            if (m < 0) return null;

            var listOfIsomorphisms = new Dictionary<IList<int>, Mapping>(comparer);

            var neighbourRange = ChooseNeighboursOfRange(partialMap.Values, inputGraph);

            var neighborsOfM = queryGraph.GetNeighbors(m, false);
            var newPartialMapCount = partialMap.Count + 1;
            //foreach (var n in neighbourRange) //foreach neighbour n of f(D)
            for (int i = 0; i < neighbourRange.Count; i++)
            {
                //int n = neighbourRange[i];
                if (false == IsNeighbourIncompatible(inputGraph, neighbourRange[i], partialMap, queryGraph, neighborsOfM))
                {
                    //It's not; so, let f' = f on D, and f'(m) = n.

                    //Find all isomorphic extensions of f'.
                    //newPartialMap[m] = neighbourRange[i];
                    var newPartialMap = new Dictionary<int, int>(newPartialMapCount);
                    foreach (var item in partialMap)
                    {
                        newPartialMap.Add(item.Key, item.Value);
                    }
                    newPartialMap[m] = neighbourRange[i];
                    var subList = IsomorphicExtension(newPartialMap, queryGraph, inputGraph);
                    if (subList != null && subList.Count > 0)
                    {
                        foreach (var item in subList)
                        {
                            listOfIsomorphisms[item.Key] = item.Value;
                        }
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
        private static bool IsNeighbourIncompatible(UndirectedGraph<int> inputGraph,
            int n, Dictionary<int, int> partialMap, QueryGraph queryGraph, IList<int> neighborsOfM)
        {
            //  RECALL: m is for Domain, the Key => the query graph
            if (partialMap.ContainsValue(n))
            {
                return true; // cos it's already done
            }

            //If there is a neighbor d ∈ D of m such that n is NOT neighbors with f(d),
            var neighboursOfN = inputGraph.GetNeighbors(n, true);
            bool doNext = false;
            int val; // f(d)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="used_range"> Meaning that we're only interested in the Values. Remember: f(h) = g</param>
        /// <param name="inputGraph">G</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<int> ChooseNeighboursOfRange(IEnumerable<int> used_range, UndirectedGraph<int> inputGraph)
        {
            List<int> toReturn = new List<int>();
            foreach (var range in used_range)
            {
                var local = inputGraph.GetNeighbors(range, true);
                if (local.Count > 0)
                {
                    List<int> batch = new List<int>(local.Count);
                    foreach (var loc in local)
                    {
                        if (!used_range.Contains(loc))
                        {
                            batch.Add(loc);
                        }
                    }
                    toReturn.AddRange(batch);
                    //batch.Clear(); // = null;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain">Domain, D, of fumction f. Meaning that we're only interested in the Keys. Remember: f(h) = g</param>
        /// <param name="queryGraph">H</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetMostConstrainedNeighbour(IEnumerable<int> domain, UndirectedGraph<int> queryGraph)
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
            return -1;
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
        private static bool CanSupport(QueryGraph queryGraph, int node_H, UndirectedGraph<int> inputGraph, int node_G)
        {
            //// 1. Based on their degrees
            //if (inputGraph.AdjacentDegree(node_G) >= queryGraph.AdjacentDegree(node_H))
            //{
            //    // => we can map the querygraph unto the input graph, based on the nodes given.
            //    // That means we are not ruling out isomorphism. So...
            //    return true;
            //}

            //return false;
            return inputGraph.GetDegree(node_G) >= queryGraph.GetDegree(node_H);
        }

        #endregion
    }
}

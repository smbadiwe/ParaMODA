using QuickGraph;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

namespace MODA.Impl
{
    public class Utils
    {
        public const int DefaultEdgeNodeVal = -999;

        /// <summary>
        /// Determines whether we have a correct mapping of the query graph to the input graph.
        /// </summary>
        /// <param name="function">The function f(h) = g.</param>
        /// <param name="queryGraph">The query graph.</param>
        /// <param name="inputGraph">The input graph.</param>
        /// <param name="checkInducedMappingOnly">If true, we ensure all edges in the query graph map to all edges in the subgraph</param>
        /// <param name="subGraphEdgeCount">The sub-graph edge count. This value can be safely ignored. We only use it to pre-size lists</param>
        /// <returns>
        ///   <c>true</c> if [is mapping correct] [the specified function]; otherwise, <c>false</c>.
        /// </returns>
        internal static MappingTestResult IsMappingCorrect(SortedList<int, int> function, QueryGraph queryGraph, UndirectedGraph<int> inputGraph, bool checkInducedMappingOnly, int subGraphEdgeCount = 0)
        {
            int subgraphSize = function.Count;
            var g_nodes = function.Values; // Remember, f(h) = g, so .Values is for g's

            // Try to get all the edges in the induced subgraph made up of these g_nodes
            var inducedSubGraphEdges = new List<Edge<int>>(subGraphEdgeCount);
            for (int i = 0; i < subgraphSize - 1; i++)
            {
                for (int j = (i + 1); j < subgraphSize; j++)
                {
                    Edge<int> edge_g;
                    if (inputGraph.TryGetEdge(g_nodes[i], g_nodes[j], out edge_g))
                    {
                        inducedSubGraphEdges.Add(edge_g);
                    }
                }
            }

            return IsMappingCorrect(function, queryGraph, inducedSubGraphEdges, checkInducedMappingOnly);
        }

        internal static MappingTestResult IsMappingCorrect(SortedList<int, int> function, QueryGraph queryGraph, List<Edge<int>> inducedSubGraphEdges, bool checkInducedMappingOnly)
        {
            // Gather the corresponding potential images of the parentQueryGraphEdges in the input graph
            var edgeImages = new HashSet<Edge<int>>();
            foreach (var x in queryGraph.Edges)
            {
                edgeImages.Add(new Edge<int>(function[x.Source], function[x.Target]));
            }

            var result = new MappingTestResult { SubgraphEdgeCount = inducedSubGraphEdges.Count };

            var compareEdgeCount = result.SubgraphEdgeCount.CompareTo(edgeImages.Count);

            // if mapping is possible (=> if compareEdgeCount >= 0)
            var baseG = new UndirectedGraph<int>();
            baseG.AddVerticesAndEdgeRange(inducedSubGraphEdges);
            var baseGdeg = baseG.GetDegreeSequence();
            var testG = new UndirectedGraph<int>();
            testG.AddVerticesAndEdgeRange(edgeImages);
            var testGdeg = testG.GetDegreeSequence();
            if (compareEdgeCount == 0)
            {
                // Same node count, same edge count
                //TODO: All we now need to do is check that the node degrees match
                for (int i = baseGdeg.Count - 1; i >= 0; i--)
                {
                    if (baseGdeg[i] != testGdeg[i])
                    {
                        result.IsCorrectMapping = false;
                        return result;
                    }
                }
                result.IsCorrectMapping = true;
                return result;
            }

            if (compareEdgeCount > 0) //=> result.SubgraphEdgeCount > edgeImages.Count
            {
                if (checkInducedMappingOnly)
                {
                    result.IsCorrectMapping = false;
                    return result;
                }

                for (int i = baseGdeg.Count - 1; i >= 0; i--)
                {
                    if (baseGdeg[i] < testGdeg[i]) // base should have at least the same value as test
                    {
                        result.IsCorrectMapping = false;
                        return result;
                    }
                }
                result.IsCorrectMapping = true;
                return result;
            }

            return result;
        }


        #region Useful mainly for the Algorithm 2 versions

        /// <summary>
        /// Algorithm taken from Grochow and Kellis. This is failing at the moment
        /// </summary>
        /// <param name="partialMap">f; Map is represented as a dictionary, with the Key as h and the Value as g</param>
        /// <param name="queryGraph">G</param>
        /// <param name="inputGraph">H</param>
        /// <param name="getInducedMappingsOnly">If true, then the querygraph must match exactly to the input subgraph. In other words, only induced subgraphs will be returned</param>
        /// <returns>List of isomorphisms. Remember, Key is h, Value is g</returns>
        internal static Dictionary<IList<int>, List<Mapping>> IsomorphicExtension(Dictionary<int, int> partialMap, QueryGraph queryGraph
            , UndirectedGraph<int> inputGraph, bool getInducedMappingsOnly)
        {
            if (partialMap.Count == queryGraph.VertexCount)
            {
                #region Return base case
                var function = new SortedList<int, int>(partialMap);

                var result = IsMappingCorrect(function, queryGraph, inputGraph, getInducedMappingsOnly);
                if (result.IsCorrectMapping)
                {
                    return new Dictionary<IList<int>, List<Mapping>>(1) { { function.Values, new List<Mapping> { new Mapping(function, result.SubgraphEdgeCount) } } };
                }
                return null;
                #endregion

            }

            //Remember: f(h) = g, so h is Domain and g is Range.
            //  In other words, Key is h and Value is g in the dictionary

            // get m, most constrained neighbor
            int m = GetMostConstrainedNeighbour(partialMap.Keys, queryGraph);
            if (m < 0) return null;

            var listOfIsomorphisms = new Dictionary<IList<int>, List<Mapping>>(ModaAlgorithms.MappingNodesComparer);

            var neighbourRange = ChooseNeighboursOfRange(partialMap.Values, inputGraph);

            var neighborsOfM = queryGraph.GetNeighbors(m, false);
            var newPartialMapCount = partialMap.Count + 1;
            //foreach neighbour n of f(D)
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
                    var subList = IsomorphicExtension(newPartialMap, queryGraph, inputGraph, getInducedMappingsOnly);
                    if (subList != null && subList.Count > 0)
                    {
                        foreach (var item in subList)
                        {
                            List<Mapping> maps;
                            if (listOfIsomorphisms.TryGetValue(item.Key, out maps))
                            {
                                maps.AddRange(item.Value);
                            }
                            else
                            {
                                listOfIsomorphisms[item.Key] = item.Value;
                            }
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
        internal static bool IsNeighbourIncompatible(UndirectedGraph<int> inputGraph,
            int n, Dictionary<int, int> partialMap, QueryGraph queryGraph, IList<int> neighborsOfM)
        {
            //  RECALL: m is for Domain, the Key => the query graph
            if (partialMap.ContainsValue(n))
            {
                return true; // cos it's already done
            }

            //If there is a neighbor d ∈ D of m such that n is NOT neighbors with f(d),
            var neighboursOfN = new HashSet<int>(inputGraph.GetNeighbors(n, true));

            bool doNext = false;
            int val; // f(d)
            foreach (var d in neighborsOfM)
            {
                if (!partialMap.TryGetValue(d, out val))
                {
                    doNext = true;
                    break;
                }
                if (!neighboursOfN.Contains(val))
                {
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
                        return false;
                    }
                    if (neighboursOfN.Contains(val))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usedRange"> Meaning that we're only interested in the Values. Remember: f(h) = g</param>
        /// <param name="inputGraph">G</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static List<int> ChooseNeighboursOfRange(IEnumerable<int> usedRange, UndirectedGraph<int> inputGraph)
        {
            List<int> toReturn = new List<int>();
            var usedRangeSet = new HashSet<int>(usedRange);
            foreach (var range in usedRangeSet)
            {
                var local = inputGraph.GetNeighbors(range, true);
                if (local.Count > 0)
                {
                    foreach (var loc in local)
                    {
                        if (!usedRangeSet.Contains(loc))
                        {
                            toReturn.Add(loc);
                        }
                    }
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
        internal static int GetMostConstrainedNeighbour(IEnumerable<int> domain, UndirectedGraph<int> queryGraph)
        {
            /*
             * As is standard in backtracking searches, the algorithm uses the most constrained neighbor
             * to eliminate maps that cannot be isomorphisms: that is, the neighbor of the already-mapped 
             * nodes which is likely to have the fewest possible nodes it can be mapped to. First we select 
             * the nodes with the most already-mapped neighbors, and amongst those we select the nodes with 
             * the highest degree and largest neighbor degree sequence.
             * */
            var domainDict = new HashSet<int>(domain);
            foreach (var dom in domainDict)
            {
                var local = queryGraph.GetNeighbors(dom, false);
                if (local.Count > 0)
                {
                    foreach (var loc in local)
                    {
                        if (!domainDict.Contains(loc))
                        {
                            return loc;
                        }
                    }
                }
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
        internal static bool CanSupport(QueryGraph queryGraph, int node_H, UndirectedGraph<int> inputGraph, int node_G)
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



        public class MappingTestResult
        {
            public bool IsCorrectMapping { get; set; }
            public int SubgraphEdgeCount { get; set; }
        }
    }
}

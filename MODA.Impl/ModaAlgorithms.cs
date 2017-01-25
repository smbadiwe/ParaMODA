using QuickGraph;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        /// <summary>
        /// Frequency value, above which we can comsider the subgraph a "frequent subgraph"
        /// </summary>
        public static int Threshold { get; set; }
        /// <summary>
        /// If true, it means we only care about how many mappings are found for each subgraph, not info about the mappings themselves.
        /// </summary>
        public static bool GetOnlyMappingCounts { get; set; }
        /// <summary>
        /// If true, the program will use my modified Grochow's algorithm (Algo 2)
        /// </summary>
        public static bool UseModifiedGrochow { get; set; }
        /// <summary>
        /// The query graph to be searched for. If not available, we use expansion trees (MODA). Otherwise, we use Grochow's (Algo 2)
        /// </summary>
        public static QueryGraph QueryGraph { get; set; }

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
        public static Dictionary<string, List<string>> G_NodeNeighbours;
        /// <summary>
        /// Used to cache 
        /// </summary>
        public static Dictionary<string, List<string>> H_NodeNeighbours;

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
                int counter = 0, count = partialMap.Count;
                foreach (var node in map.MapOnInputSubGraph.Vertices)
                {
                    for (int j = (counter + 1); j < count; j++)
                    {
                        Edge<string> edge_;
                        if (inputGraph.TryGetEdge(node, map.MapOnInputSubGraph.Vertices.ElementAt(j), out edge_))
                        {
                            map.InputSubGraph.AddVerticesAndEdge(edge_);
                        }
                    }
                    counter++;
                }
                //map.InputSubGraph = GetInputSubgraph(inputGraph, map.MapOnInputSubGraph.Vertices, partialMap.Count);
                //map.InputSubGraph = GetInputSubgraph(inputGraph, partialMap.Values.ToArray());
                return new List<Mapping>(1) { map };
                #endregion
            }

            //Remember: f(h) = g, so h is Domain and g is Range.
            //  In other words, Key is h and Value is g in the dictionary

            // get m, most constrained neighbor
            string m = GetMostConstrainedNeighbour(partialMap.Keys, queryGraph);
            if (string.IsNullOrWhiteSpace(m)) return null;

            var listOfIsomorphisms = new List<Mapping>();

            var neighbourRange = ChooseNeighboursOfRange(partialMap.Values, inputGraph);

            var neighborsOfM = queryGraph.GetNeighbors(m);
            for (int i = 0; i < neighbourRange.Count; i++) //foreach neighbour n of f(D)
            {
                var n = neighbourRange[i];
                if (IsNeighbourIncompatible(inputGraph, queryGraph, n, m, partialMap, neighborsOfM))
                {
                    continue;
                }
                //It's not; so, let f' = f on D, and f'(m) = n.

                //Find all isomorphic extensions of f'.
                var subList = IsomorphicExtension(new Dictionary<string, string>(partialMap) { { m, n } }, queryGraph, inputGraph);
                if (subList == null || subList.Count == 0) continue;

                if (listOfIsomorphisms.Count == 0)
                {
                    listOfIsomorphisms.AddRange(subList);
                }
                else
                {
                    var notIsoWithAnyExisting = new List<Mapping>();
                    for (int j = 0; j < subList.Count; j++)
                    {
                        if (!listOfIsomorphisms.Exists(x => x.IsIsomorphicWith(subList[j])))
                        {
                            notIsoWithAnyExisting.Add(subList[j]); // is NOT part of Iso
                        }
                    }
                    if (notIsoWithAnyExisting.Count > 0)
                    {
                        listOfIsomorphisms.AddRange(notIsoWithAnyExisting);
                    }
                }
            }
            return listOfIsomorphisms;
        }

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
        /// <param name="neighborsOfM">neighbors of <paramref name="m"/> iin the <paramref name="queryGraph"/></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNeighbourIncompatible(UndirectedGraph<string, Edge<string>> inputGraph, UndirectedGraph<string, Edge<string>> queryGraph,
            string n, string m, Dictionary<string, string> partialMap, IList<string> neighborsOfM)
        {
            //  RECALL: m is for Domain, the Key => the query graph

            //A: If there is a neighbor d ∈ D of m such that n is NOT neighbors with f(d)...
            var neighboursOfN = inputGraph.GetNeighbors(n);
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<string> ChooseNeighboursOfRange(IEnumerable<string> used_range, UndirectedGraph<string, Edge<string>> inputGraph)
        {
            List<string> toReturn = new List<string>();
            foreach (var rangeVal in used_range)
            {
                var local = inputGraph.GetNeighbors(rangeVal);
                if (local.Count == 0)
                {
                    local = null;
                    continue;
                }
                for (int j = 0; j < local.Count; j++)
                {
                    if (!used_range.Contains(local[j]))
                    {
                        toReturn.Add(local[j]);
                    }
                }
                local = null;
            }

            return toReturn;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static List<string> ChooseNeighboursOfRange(string[] used_range, UndirectedGraph<string, Edge<string>> inputGraph)
        //{
        //    List<string> toReturn;
        //    if (!NeighboursOfRange.TryGetValue(used_range, out toReturn))
        //    {
        //        toReturn = new List<string>();
        //        for (int i = used_range.Length - 1; i >= 0; i--)
        //        {
        //            var local = new List<string>(inputGraph.GetNeighbors(used_range[i]));
        //            if (local.Count == 0)
        //            {
        //                local = null;
        //                continue;
        //            }
        //            //for (int j = 0; j < local.Count; j++)
        //            foreach (var item in local)
        //            {
        //                if (!used_range.Contains(item))
        //                {
        //                    toReturn.Add(item);
        //                }
        //            }
        //            local = null;
        //        }
        //        NeighboursOfRange[used_range] = toReturn;

        //    }

        //    return toReturn; //.ToArray();
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain">Domain, D, of fumction f</param>
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
            foreach (var item in domain)
            {
                var local = queryGraph.GetNeighbors(item);
                if (local.Count == 0)
                {
                    local = null;
                    continue;
                }
                for (int j = 0; j < local.Count; j++)
                {
                    if (!domain.Contains(local[j]))
                    {
                        //toReturn = local[j];
                        return local[j]; // toReturn;
                    }
                }
                local = null;
            }
            return "";
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="domain">Domain, D, of fumction f</param>
        ///// <param name="queryGraph">H</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static string GetMostConstrainedNeighbour(string[] domain, UndirectedGraph<string, Edge<string>> queryGraph)
        //{
        //    /*
        //     * As is standard in backtracking searches, the algorithm uses the most constrained neighbor
        //     * to eliminate maps that cannot be isomorphisms: that is, the neighbor of the already-mapped 
        //     * nodes which is likely to have the fewest possible nodes it can be mapped to. First we select 
        //     * the nodes with the most already-mapped neighbors, and amongst those we select the nodes with 
        //     * the highest degree and largest neighbor degree sequence.
        //     * */
        //    string toReturn;
        //    if (!MostConstrainedNeighbours.TryGetValue(domain, out toReturn))
        //    {
        //        toReturn = "";
        //        for (int i = domain.Length - 1; i >= 0; i--)
        //        {
        //            var local = queryGraph.GetNeighbors(domain[i]);
        //            if (local.Count == 0)
        //            {
        //                local = null;
        //                continue;
        //            }
        //            for (int j = 0; j < local.Count; j++)
        //            {
        //                if (!domain.Contains(local[j]))
        //                {
        //                    //toReturn = local[j];
        //                    MostConstrainedNeighbours[domain] = local[j]; // toReturn;
        //                    return local[j]; // toReturn;
        //                }
        //            }
        //            ////This was slowr
        //            //foreach (var item in local)
        //            //{
        //            //    if (!domain.Contains(item))
        //            //    {
        //            //        MostConstrainedNeighbours[domain] = item;
        //            //        return item;
        //            //    }
        //            //}
        //            local = null;
        //        }

        //        MostConstrainedNeighbours[domain] = toReturn;
        //    }

        //    return toReturn;
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
            if (inputGraph.AdjacentDegree(node_G) < queryGraph.AdjacentDegree(node_H))
            {
                // => we cannot map the querygraph unto the input graph, based on the nodes given.
                // That means we are ruling out isomorphism. So...
                return false;
            }
            //So, deg(g) >= deg(h).

            //2. Based on the degree of their neighbors
            var gNeighbors = inputGraph.GetNeighbors(node_G);
            var hNeighbors = queryGraph.GetNeighbors(node_H);
            //TODO: either review or remove this test
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

        #endregion
    }
}

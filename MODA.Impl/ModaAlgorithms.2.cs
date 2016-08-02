using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        /// <summary>
        /// Mapping module; aka FindSubgraphInstances in Grochow & Kellis
        /// </summary>
        /// <param name="queryGraph">H</param>
        /// <param name="inputGraph">G</param>
        /// <param name="numberOfSamples">To be decided. If not set, we use the <paramref name="inputGraph"/> size</param>
        private List<Mapping> Algorithm2(UndirectedGraph<string, Edge<string>> queryGraph, UndirectedGraph<string, Edge<string>> inputGraph, int numberOfSamples = -1)
        {
            if (numberOfSamples <= 0) numberOfSamples = inputGraph.VertexCount / 3;
            int counter = 0;
            // Do we need this clone? Can't we just remove the node directly from the graph?
            var inputGraphClone = inputGraph.Clone();
            var theMappings = new List<Mapping>();
            var queryGraphVertices = queryGraph.Vertices.ToList();
            foreach (var g in inputGraph.GetDegreeSequence())
            {
                //foreach (var h in queryGraph.Vertices)
                queryGraphVertices.ForEach(h =>
                {
                    if (CanSupport(queryGraph, h, inputGraphClone, g))
                    {
                        #region Can Support
                        var sw = System.Diagnostics.Stopwatch.StartNew();
                        //Remember: f(h) = g, so h is Domain and g is Range
                        var function = new Dictionary<string, string>(1) { { h, g } }; //function, f
                        var mappings = IsomorphicExtension(function, queryGraph, inputGraphClone);
                        sw.Stop();
                        Console.WriteLine("Maps gotten from IsoExtension.\tTook:\t{0:N}s.\th = {1}. g = {2}", sw.Elapsed.ToString(), h, g);
                        sw.Restart();
                        //theMappings.AddRange(mappings);
                        int count = mappings.Count;
                        //foreach (var map in mappings)
                        if (theMappings.Count == 0)
                        {
                            theMappings.AddRange(mappings);
                        }
                        else
                        {
                            mappings.ForEach(map =>
                            {
                                var existing = theMappings.Find(x => x.Equals(map));

                                if (existing == null)
                                {
                                    theMappings.Add(map);
                                }
                            }
                            );
                        }
                        mappings = null;
                        function = null;
                        sw.Stop();
                        Console.WriteLine("Map: {0}.\tTime to set:\t{1:N}s.\th = {2}. g = {3}", count--, sw.Elapsed.ToString(), h, g);
                        sw = null;
                        Console.WriteLine("*****************************************\n");
                        #endregion
                    }
                }
                );

                //Remove g
                inputGraphClone.RemoveVertex(g);
                counter++;

                if (counter == numberOfSamples) break;

            }
            queryGraphVertices = null;
            inputGraphClone = null;
            return theMappings;
        }

        /// <summary>
        /// Given a set of nodes(the Key), we find the subgraph in the input graph G that has those nodes.
        /// </summary>
        private static readonly Dictionary<string[], UndirectedGraph<string, Edge<string>>> InputSubgraphs = new Dictionary<string[], UndirectedGraph<string, Edge<string>>>();

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
                var map = new Mapping(partialMap);
                foreach (var qEdge in queryGraph.Edges)
                {
                    map.MapOnInputSubGraph.AddVerticesAndEdge(new Edge<string>(partialMap[qEdge.Source], partialMap[qEdge.Target]));
                }

                var inputSubgraphKey = InputSubgraphs.Keys.FirstOrDefault(x => new HashSet<string>(x).SetEquals(partialMap.Values));
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
                    InputSubgraphs.Add(inputSubgraphKey, newInputSubgraph);
                }
                map.InputSubGraph = InputSubgraphs[inputSubgraphKey];

                return new List<Mapping> { map };
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
                        }
                    });
                }
                newPartialMap = null;
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
        /// <returns></returns>
        private bool IsNeighbourIncompatible(UndirectedGraph<string, Edge<string>> inputGraph, UndirectedGraph<string, Edge<string>> queryGraph,
            string n, string m, Dictionary<string, string> partialMap)
        {
            //  RECALL: m is for Domain, the Key => the query graph

            //A: If there is a neighbor d ∈ D of m such that n is NOT neighbors with f(d)...
            var neighboursOfN = inputGraph.GetNeighbors(n);
            //var neighborsOfM = queryGraph.GetNeighbors(m);
            foreach (var d in queryGraph.GetNeighbors(m)) // neighborsOfM)
            {
                if (!partialMap.ContainsKey(d))
                {
                    neighboursOfN = null;
                    return false; // continue;
                }
                if (!neighboursOfN.Contains(partialMap[d]))
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
            neighboursOfN = null;
            return false;
        }

        private HashSet<string> ChooseNeighboursOfRange(string[] used_range, UndirectedGraph<string, Edge<string>> inputGraph)
        {
            var result = new HashSet<string>();
            for (int i = 0; i < used_range.Length; i++)
            {
                var local = inputGraph.GetNeighbors(used_range[i]);
                if (local.Count == 0) continue; // return result;

                int counter = 0;
                for (int j = 0; j < local.Count + counter; j++)
                {
                    if (used_range.Contains(local[j - counter]))
                    {
                        local.Remove(local[j - counter]);
                        counter++;
                    }
                }
                foreach (var item in local)
                {
                    result.Add(item);
                }
                local = null;
            }
            return result;
        }

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
            HashSet<string> result = new HashSet<string>();
            for (int i = 0; i < domain.Length; i++)
            {
                var local = queryGraph.GetNeighbors(domain[i]);
                int local_counter = 0;
                for (int j = 0; j < local.Count + local_counter; j++)
                {
                    if (domain.Contains(local[j - local_counter]))
                    {
                        local.Remove(local[j - local_counter]);
                        local_counter++;
                    }
                }
                foreach (var item in local)
                {
                    result.Add(item);
                }
            }
            if (result.Count == 0) return "";

            return result.ElementAt(0);
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
            foreach (var hNeighbor in queryGraph.GetNeighbors(node_H))
            {
                foreach (var x in gNeighbors)
                {
                    if (inputGraph.AdjacentDegree(x) >= queryGraph.AdjacentDegree(hNeighbor))
                    {
                        return true;
                    }
                }
            }
            gNeighbors = null;
            return false;
        }

    }
}

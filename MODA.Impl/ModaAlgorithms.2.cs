using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
        private HashSet<Mapping<int>> Algorithm2(UndirectedGraph<int, Edge<int>> queryGraph, UndirectedGraph<int, Edge<int>> inputGraph, int numberOfSamples = -1)
        {
            if (numberOfSamples <= 0) numberOfSamples = inputGraph.VertexCount;
            int i = 0;
            // Do we need this clone? Can't we just remove the node directly from the graph?
            var inputGraphClone = inputGraph.Clone();
            var theMappings = new HashSet<Mapping<int>>();

            foreach (var g in inputGraph.GetDegreeSequence())
            {
                foreach (var h in queryGraph.Vertices)
                {
                    if (CanSupport(queryGraph, h, inputGraphClone, g))
                    {
                        //Remember: f(h) = g, so h is Domain and g is Range
                        var function = new Dictionary<int, int>(); //function, f
                        function.Add(h, g);
                        var mappings = IsomorphicExtension(function, queryGraph, inputGraphClone);
                        foreach (var map in mappings)
                        {
                            map.InputSubGraph = new UndirectedGraph<int, Edge<int>>(false);
                            var subgraphNodesInInputGraph = map.Function.Values.ToArray();
                            for (i = 0; i < subgraphNodesInInputGraph.Length; i++)
                            {
                                for (int j = (i + 1); j < subgraphNodesInInputGraph.Length; j++)
                                {
                                    Edge<int> edge;
                                    if (inputGraph.TryGetEdge(subgraphNodesInInputGraph[i], subgraphNodesInInputGraph[j], out edge))
                                    {
                                        map.InputSubGraph.AddVerticesAndEdge(edge);
                                    }
                                }
                            }

                            //Now we've captured the subgrph on the input as it is, with all of its edges, it's time to
                            //the exact edges mapped. 
                            //Put differently, we've gotten the nodes mapped, now is time to get the edges mapped.
                            map.MapOnInputSubGraph = new UndirectedGraph<int, Edge<int>>(false);
                            subgraphNodesInInputGraph = map.Function.Keys.ToArray();
                            for (i = 0; i < subgraphNodesInInputGraph.Length; i++)
                            {
                                for (int j = (i + 1); j < subgraphNodesInInputGraph.Length; j++)
                                {
                                    Edge<int> edge;
                                    if (queryGraph.TryGetEdge(subgraphNodesInInputGraph[i], subgraphNodesInInputGraph[j], out edge))
                                    {
                                        map.MapOnInputSubGraph.AddVerticesAndEdge(new Edge<int>(map.Function[edge.Source], map.Function[edge.Target]));
                                    }
                                }
                            }
                            if (theMappings.Any(x => x.Equals(map)))
                            {
                                continue;
                            }
                            theMappings.Add(map);
                        }
                    }
                }

                //Remove g
                inputGraphClone.RemoveVertex(g);

                i++;
                if (i == numberOfSamples) break;
            }
            var sb = new StringBuilder();
            sb.AppendFormat("{0}", queryGraph.AsString());
            sb.AppendLine("\n======================================");
            foreach (var map in theMappings)
            {
                sb.Append(map);
            }
            Console.WriteLine(sb);
            Console.WriteLine();
            return theMappings;
        }

        /// <summary>
        /// Algorithm taken from Grochow and Kellis. This is failing at the moment
        /// </summary>
        /// <param name="partialMap">f; Map is represented as a dictionary, with the Key as h and the Valuw as g</param>
        /// <param name="queryGraph">G</param>
        /// <param name="inputGraph">H</param>
        /// <returns>List of isomorphisms. Remember, Key is h, Value is g</returns>
        private HashSet<Mapping<int>> IsomorphicExtension(Dictionary<int, int> partialMap, UndirectedGraph<int, Edge<int>> queryGraph
            , UndirectedGraph<int, Edge<int>> inputGraph)
        {
            if (partialMap.Count == queryGraph.VertexCount)
            {
                return new HashSet<Mapping<int>> { new Mapping<int>(partialMap) };
            }

            var listOfIsomorphisms = new HashSet<Mapping<int>>();

            //Remember: f(h) = g, so h is Domain and g is Range.
            //  In other words, Key is h and Value is g in the dictionary

            // get m, most constrained neighbor
            var domainList = partialMap.Keys.ToList();
            int m = GetMostConstrainedNeighbour(domainList, queryGraph);
            if (m == -1) return listOfIsomorphisms;

            var neighbourRange = ChooseNeighboursOfRange(partialMap.Values.ToList(), inputGraph);
            foreach (var n in neighbourRange) //foreach neighbour n of f(D)
            {
                if (IsNeighbourIncompatible(inputGraph, queryGraph, n, m, partialMap))
                {
                    continue;
                }
                //It's not; so, let f' = f on D, and f'(m) = n.
                var newPartialMap = new Dictionary<int, int>(partialMap);
                newPartialMap[m] = n;

                //Find all isomorphic extensions of f'.
                var subList = IsomorphicExtension(newPartialMap, queryGraph, inputGraph);
                foreach (var item in subList)
                {
                    if (new HashSet<int>(item.Function.Values).Count != item.Function.Count)
                    {
                        continue;
                    }
                    listOfIsomorphisms.Add(item);
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
        /// <returns></returns>
        private bool IsNeighbourIncompatible(UndirectedGraph<int, Edge<int>> inputGraph, UndirectedGraph<int, Edge<int>> queryGraph,
            int n, int m, Dictionary<int, int> partialMap)
        {
            //  RECALL: m is for Domain, the Key => the query graph

            //A: If there is a neighbor d ∈ D of m such that n is NOT neighbors with f(d)...
            var neighboursOfN = inputGraph.GetNeighbors(n);
            var neighborsOfM = queryGraph.GetNeighbors(m);
            foreach (var d in neighborsOfM)
            {
                if (!partialMap.ContainsKey(d)) return false; // continue;

                if (!neighboursOfN.Contains(partialMap[d]))
                {
                    return true;
                }
            }

            //B: ...or if there is a NON-neighbor d ∈ D of m such that n is neighbors with f(d) 
            var nonNeighborsOfM = queryGraph.GetNonNeighbors(m, neighborsOfM);
            foreach (var d in nonNeighborsOfM)
            {
                if (!partialMap.ContainsKey(d)) return false;

                if (neighboursOfN.Contains(partialMap[d]))
                {
                    return false;
                }
            }

            return false;
        }

        private HashSet<int> ChooseNeighboursOfRange(List<int> used_range, UndirectedGraph<int, Edge<int>> inputGraph)
        {
            var local = new List<int>();
            var result = new HashSet<int>();
            for (int i = 0; i < used_range.Count; i++)
            {
                local = inputGraph.GetNeighbors(used_range[i]);
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
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain">Domain, D, of fumction f</param>
        /// <param name="queryGraph">H</param>
        private int GetMostConstrainedNeighbour(List<int> domain, UndirectedGraph<int, Edge<int>> queryGraph)
        {
            /*
             * As is standard in backtracking searches, the algorithm uses the most constrained neighbor
             * to eliminate maps that cannot be isomorphisms: that is, the neighbor of the already-mapped 
             * nodes which is likely to have the fewest possible nodes it can be mapped to. First we select 
             * the nodes with the most already-mapped neighbors, and amongst those we select the nodes with 
             * the highest degree and largest neighbor degree sequence.
             * */
            HashSet<int> result = new HashSet<int>();
            for (int i = 0; i < domain.Count; i++)
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
            if (result.Count == 0) return -1;

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
        private bool CanSupport(UndirectedGraph<int, Edge<int>> queryGraph, int node_H, UndirectedGraph<int, Edge<int>> inputGraph, int node_G)
        {
            // 1. Based on their degrees
            if (inputGraph.AdjacentDegree(node_G) < queryGraph.AdjacentDegree(node_H)) return false;

            //So, deg(g) >= deg(h).
            //2. Based on the degree of their neighbors
            bool canSupport = false;
            var gNeighbors = inputGraph.GetNeighbors(node_G);
            foreach (var hNeighbor in queryGraph.GetNeighbors(node_H))
            {
                if (gNeighbors.Any(x =>
                    inputGraph.AdjacentDegree(x) >= queryGraph.AdjacentDegree(hNeighbor)))
                {
                    canSupport = true;
                    break;
                }
            }

            return canSupport;
        }

    }
}

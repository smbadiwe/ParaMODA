using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<Dictionary<int, int>> Algorithm2(UndirectedGraph<int, Edge<int>> queryGraph, UndirectedGraph<int, Edge<int>> inputGraph, int numberOfSamples = -1)
        {
            if (numberOfSamples <= 0) numberOfSamples = inputGraph.VertexCount;
            int i = 0;
            // Do we need this clone? Can't we just remove the node directly from the graph?
            var inputGraphClone = inputGraph.Clone();
            var theMappings = new List<Dictionary<int, int>>();
            //var degreeSeq = new List<int>();
            foreach (int g in inputGraph.GetDegreeSequence())
            {
                List<int> domain_tags = new List<int>();
                List<int> range_tags = new List<int>();
                var domainAndRangeTags = new Dictionary<int, int>();
                foreach (int h in queryGraph.Vertices)
                {
                    if (CanSupport(queryGraph, h, inputGraphClone, g))
                    {
                        //Remember: f(h) = g, so h is Domain and g is Range
                        domainAndRangeTags.Add(h, g);
                        var partialMap = new Dictionary<int, int>();
                        partialMap.Add(h, g);

                        domain_tags.Add(h);
                        range_tags.Add(g);

                        var domain = new List<int> { h };
                        var used_range = new List<int> { g };
                        var mappings = IsomorphicExtension(partialMap, queryGraph, inputGraphClone, domain, used_range, domain_tags, range_tags);
                        theMappings.AddRange(mappings);
                    }
                }

                //Remove g
                inputGraphClone.RemoveVertex(g);

                i++;
                if (i == numberOfSamples) break;
            }
            var sb = new StringBuilder();
            foreach (var map in theMappings)
            {
                sb.Append("[");
                foreach (var item in map.Keys)
                {
                    sb.AppendFormat("{0}-", item);
                }
                sb.Append("] => [");
                foreach (var item in map.Values)
                {
                    sb.AppendFormat("{0}-", item);
                }
                sb.AppendLine("]");
            }
            Console.WriteLine(sb.ToString());
            Console.WriteLine();
            return theMappings;
        }

        /// <summary>
        /// Algorithm taken from Grochow and Kellis
        /// </summary>
        /// <param name="partialMap">f; Map is represented as a dictionary, with the Key as h and the Valuw as g</param>
        /// <param name="queryGraph">G</param>
        /// <param name="inputGraph">H</param>
        /// <returns>List of isomorphisms. Remember, Key is h, Value is g</returns>
        private List<Dictionary<int, int>> IsomorphicExtension(Dictionary<int, int> partialMap, UndirectedGraph<int, Edge<int>> queryGraph, 
            UndirectedGraph<int, Edge<int>> inputGraph)
        {
            if (partialMap.Count == queryGraph.VertexCount)
            {
                return new List<Dictionary<int, int>> { partialMap };
            }

            var listOfIsomorphisms = new List<Dictionary<int, int>>();

            //Remember: f(h) = g, so h is Domain and g is Range.
            //  In other words, Key is h and Value is g in the dictionary

            // get m, most constrained neighbor of the domain
            int m = GetMostConstrainedNeighbour(queryGraph);
            if (m == -1) return listOfIsomorphisms; // This should NEVER happen

            List<int> neighbourRange = ChooseNeighboursOfRange(used_range, inputGraph, range_tags);
            foreach (int neighbour in neighbourRange)
            {
                if (IsNeighbourCompatible(inputGraph, queryGraph, neighbour, m, domain, partialMap)) continue;

                //It's not; so, let f' = f on D, and f'(m) = n. Find all isomorphic extensions of f'.
                var newPartialMap = new Dictionary<int, int>(partialMap);
                newPartialMap[m] = neighbour;

                var usedRangeCopy = new List<int>(used_range);
                usedRangeCopy.Add(neighbour);

                var domainCopy = new List<int>(domain);
                domainCopy.Add(m);

                //var domainTagsCopy = new List<int>(domain_tags);
                //domainTagsCopy.Add(m);
                domain_tags.Add(m);

                //var rangeTagsCopy = new List<int>(range_tags);
                //rangeTagsCopy.Add(neighbour);
                range_tags.Add(neighbour);

                //var subList = IsomorphicExtension(newPartialMap, queryGraph, inputGraph, domainCopy, usedRangeCopy, domain_tags, range_tags);

                var subList = IsomorphicExtension(newPartialMap, queryGraph, inputGraph, domainCopy, range_tags, domain_tags, range_tags);
                listOfIsomorphisms.AddRange(subList);
            }
            return listOfIsomorphisms;
        }





        /// <summary>
        /// Algorithm taken from Grochow and Kellis
        /// </summary>
        /// <param name="partialMap">f; Map is represented as a dictionary, with the Key as h and the Valuw as g</param>
        /// <param name="queryGraph">G</param>
        /// <param name="inputGraph">H</param>
        /// <returns>List of isomorphisms. Remember, Key is h, Value is g</returns>
        private List<Dictionary<int, int>> IsomorphicExtension(Dictionary<int, int> partialMap, UndirectedGraph<int, Edge<int>> queryGraph, UndirectedGraph<int, Edge<int>> inputGraph
            , List<int> domain, List<int> used_range, List<int> domain_tags, List<int> range_tags)
        {
            if (partialMap.Count == queryGraph.VertexCount)
            {
                return new List<Dictionary<int, int>> { partialMap };
            }

            var listOfIsomorphisms = new List<Dictionary<int, int>>();

            //Remember: f(h) = g, so h is Domain and g is Range.
            //  In other words, Key is h and Value is g in the dictionary

            // get m, most constrained neighbor
            int m = GetMostConstrainedNeighbour(domain, queryGraph, domain_tags);
            if (m == -1) return listOfIsomorphisms; // This should NEVER happen

            List<int> neighbourRange = ChooseNeighboursOfRange(used_range, inputGraph, range_tags);
            foreach (int neighbour in neighbourRange)
            {
                if (IsNeighbourCompatible(inputGraph, queryGraph, neighbour, m, domain, partialMap)) continue;

                //It's not; so, let f' = f on D, and f'(m) = n. Find all isomorphic extensions of f'.
                var newPartialMap = new Dictionary<int, int>(partialMap);
                newPartialMap[m] = neighbour;

                var usedRangeCopy = new List<int>(used_range);
                usedRangeCopy.Add(neighbour);

                var domainCopy = new List<int>(domain);
                domainCopy.Add(m);

                //var domainTagsCopy = new List<int>(domain_tags);
                //domainTagsCopy.Add(m);
                domain_tags.Add(m);

                //var rangeTagsCopy = new List<int>(range_tags);
                //rangeTagsCopy.Add(neighbour);
                range_tags.Add(neighbour);

                //var subList = IsomorphicExtension(newPartialMap, queryGraph, inputGraph, domainCopy, usedRangeCopy, domain_tags, range_tags);

                var subList = IsomorphicExtension(newPartialMap, queryGraph, inputGraph, domainCopy, range_tags, domain_tags, range_tags);
                listOfIsomorphisms.AddRange(subList);
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
        private bool IsNeighbourCompatible(UndirectedGraph<int, Edge<int>> inputGraph, UndirectedGraph<int, Edge<int>> queryGraph,
            int n, int m, List<int> domain, Dictionary<int, int> partialMap)
        {
            if (partialMap.ContainsValue(n))
            {
                return false;
            }
            var neighbor = queryGraph.getNeighbors(m);

            //split into two sets
            int i = 0;
            int local_counter = 0;
            for (i = 0; i < neighbor.Count + local_counter; i++)
            {
                if (!domain.Contains(neighbor[i - local_counter]))
                {
                    neighbor.Remove(neighbor[i - local_counter]);
                    local_counter++;
                }
            }
            List<int> non_neighbor = new List<int>();
            for (i = 0; i < domain.Count; i++)
            {
                if (!neighbor.Contains(domain[i]))
                {
                    non_neighbor.Add(domain[i]);
                }
            }

            var local = inputGraph.GetNeighboursWithFlag(n, new List<int>()); //_flag
            if (local.Count == 0) return false;

            for (i = 0; i < neighbor.Count; i++)
            {
                if (!local.Contains(partialMap.Keys.ElementAt(neighbor[i])))
                {
                    return false;
                }
            }

            return true;
        }

        private List<int> ChooseNeighboursOfRange(List<int> used_range, UndirectedGraph<int, Edge<int>> inputGraph, List<int> range_tags)
        {
            List<int> local = new List<int>();
            List<int> result = new List<int>();
            int local_counter = 0;
            for (int i = 0; i < used_range.Count; i++)
            {
                local = inputGraph.getNeighbors(used_range[i]);
                if (local.Count == 0) return result;

                local_counter = 0;
                for (int j = 0; j < local.Count + local_counter; j++)
                {
                    if (range_tags.Contains(local[j - local_counter]) || false) // || _flag[local[j - local_counter]])
                    {
                        local.Remove(local[j - local_counter]);
                        local_counter++;
                    }
                }
                result.AddRange(local);
            }
            return result;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain">Domain, D, of fumction f</param>
        /// <param name="queryGraph">H</param>
        private int GetMostConstrainedNeighbour(List<int> domain, UndirectedGraph<int, Edge<int>> queryGraph, List<int> domain_tags)
        {
            /*
             * As is standard in backtracking searches, the algorithm uses the most constrained neighbor
             * to eliminate maps that cannot be isomorphisms: that is, the neighbor of the already-mapped 
             * nodes which is likely to have the fewest possible nodes it can be mapped to. First we select 
             * the nodes with the most already-mapped neighbors, and amongst those we select the nodes with 
             * the highest degree and largest neighbor degree sequence.
             * */
            List<int> result = new List<int>();
            for (int i = 0; i < domain.Count; i++)
            {
                var local = queryGraph.getNeighbors(domain[i]);
                int local_counter = 0;
                for (int j = 0; j < local.Count + local_counter; j++)
                {
                    if (domain_tags.Contains(local[j - local_counter]))
                    {
                        local.Remove(local[j - local_counter]);
                        local_counter++;
                    }
                }
                result.AddRange(local);
            }
            if (result.Count == 0) return -1;

            return result[0];
        }

        /// <summary>
        /// get m, most constrained neighbor of the domain
        /// </summary>
        /// <param name="domain">Domain, D, of fumction f</param>
        /// <param name="queryGraph">H</param>
        private int GetMostConstrainedNeighbour(UndirectedGraph<int, Edge<int>> queryGraph)
        {
            /*
             * As is standard in backtracking searches, the algorithm uses the most constrained neighbor
             * to eliminate maps that cannot be isomorphisms: that is, the neighbor of the already-mapped 
             * nodes which is likely to have the fewest possible nodes it can be mapped to. First we select 
             * the nodes with the most already-mapped neighbors, and amongst those we select the nodes with 
             * the highest degree and largest neighbor degree sequence.
             * */
            List<int> result = new List<int>();
            foreach (var node in queryGraph.Vertices)
            {
                var local = queryGraph.getNeighbors(node);
                int local_counter = 0; // Used to ensure the counter in the 'for' loop remains constant
                for (int j = 0; j < local.Count + local_counter; j++)
                {
                    if (domain_tags.Contains(local[j - local_counter]))
                    {
                        local.Remove(local[j - local_counter]);
                        local_counter++;
                    }
                }
                result.AddRange(local);
            }
            if (result.Count == 0) return -1;

            return result[0];
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
            var gNeighbors = inputGraph.getNeighbors(node_G);
            foreach (var hNeighbor in queryGraph.getNeighbors(node_H))
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

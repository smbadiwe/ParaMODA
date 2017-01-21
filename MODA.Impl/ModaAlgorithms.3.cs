using QuickGraph;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        /// <summary>
        /// Enumeration module
        /// </summary>
        /// <param name="queryGraph"></param>
        /// <param name="inputGraph"></param>
        /// <param name="expansionTree"></param>
        private static List<Mapping> Algorithm3(QueryGraph queryGraph, UndirectedGraph<string, Edge<string>> inputGraph,
            AdjacencyGraph<ExpansionTreeNode, Edge<ExpansionTreeNode>> expansionTree,
            Dictionary<QueryGraph, List<Mapping>> mappingsInMemory)
        {
            //var timer = System.Diagnostics.Stopwatch.StartNew();
            var parentQueryGraph = GetParent(queryGraph, expansionTree);

            List<Mapping> mappings;
            mappingsInMemory.TryGetValue(parentQueryGraph, out mappings);
            if (mappings?.Count == 0) return mappings;

            // Get the new edge in the queryGraph
            // New Edge = E(G') - E(H)
            var newEdgeNodes = new string[2];
            int index = 0;
            bool gotNewEdge = false;
            foreach (var node in queryGraph.Vertices)
            {
                //Trick: 
                // Given two graphs where one is a super-graph of the other and they only differ by one edge,
                // then we can be sure that there will be two nodes whose degrees changed.
                // These two nodes form the new edge.
                if (queryGraph.AdjacentDegree(node) == parentQueryGraph.AdjacentDegree(node)) continue;

                gotNewEdge = true;
                newEdgeNodes[index] = node;
                index++;
                if (index > 1) break;
            }

            if (!gotNewEdge) return null;

            var theNewEdge = new Edge<string>(newEdgeNodes[0], newEdgeNodes[1]);
            newEdgeNodes = null;

            var theMappings = new Dictionary<string, List<Mapping>>();

            for (int i = 0; i < mappings.Count; i++)
            {
                var map = mappings[i];

                var newInputSubgraph = GetInputSubgraph(inputGraph, map.MapOnInputSubGraph.Vertices.ToArray());
                // Reember, f(h) = g
                // Remember, newInputSubgraph is a subgraph of inputGraph
                Edge<string> edge;
                if (newInputSubgraph.TryGetEdge(map.Function[theNewEdge.Source], map.Function[theNewEdge.Target], out edge))
                {
                    var mapping = new Mapping(map.Function)
                    {
                        MapOnInputSubGraph = newInputSubgraph
                    };
                    List<Mapping> mappingsToSearch; //Recall: f(h) = g
                    var g_key = mapping.Function.ElementAt(map.Function.Count - 1).Value;
                    if (theMappings.TryGetValue(g_key, out mappingsToSearch) && mappingsToSearch != null)
                    {
                        var existing = mappingsToSearch.Find(x => x.IsIsomorphicWith(mapping));

                        if (existing == null)
                        {
                            theMappings[g_key].Add(mapping);
                        }
                    }
                    else
                    {
                        theMappings[g_key] = new List<Mapping> { mapping };
                    }
                    mapping = null;
                    mappingsToSearch = null;
                }
            }

            var toReturn = new List<Mapping>();
            foreach (var mapping in theMappings)
            {
                toReturn.AddRange(mapping.Value);
            }
            Console.WriteLine("Algorithm 3: All tasks completed. Number of mappings found: {0}.\n", theMappings.Count);

            //timer.Stop();
            //Console.WriteLine("Algorithm 3: All tasks completed. Number of mappings found: {0}.\nTotal time taken: {1}", theMappings.Count, timer.Elapsed);

            //timer = null;
            theMappings = null;
            return toReturn;
        }

        /// <summary>
        /// Helper method for algorithm 3
        /// </summary>
        /// <param name="queryGraph"></param>
        /// <param name="expansionTree"></param>
        /// <returns></returns>
        private static QueryGraph GetParent(QueryGraph queryGraph, AdjacencyGraph<ExpansionTreeNode, Edge<ExpansionTreeNode>> expansionTree)
        {
            var hasNode = expansionTree.ContainsVertex(new ExpansionTreeNode
            {
                QueryGraph = queryGraph,
            });
            if (hasNode)
            {
                return expansionTree.Vertices.First(x => !x.IsRootNode && x.NodeName == queryGraph.Label).ParentNode.QueryGraph;
            }
            return null;
        }

    }
}

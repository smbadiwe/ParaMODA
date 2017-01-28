using QuickGraph;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        /// <summary>
        /// Enumeration module
        /// </summary>
        /// <param name="queryGraph"></param>
        /// <param name="expansionTree"></param>
        /// <param name="parentQueryGraph"></param>
        /// <param name="parentGraphMappings"></param>
        private static List<Mapping> Algorithm3(QueryGraph queryGraph,
            AdjacencyGraph<ExpansionTreeNode, Edge<ExpansionTreeNode>> expansionTree,
            QueryGraph parentQueryGraph, List<Mapping> parentGraphMappings)
        {
            //var timer = System.Diagnostics.Stopwatch.StartNew();
            //var parentQueryGraph = GetParent(queryGraph, expansionTree);

            //List<Mapping> mappings;
            //mappingsInMemory.TryGetValue(parentQueryGraph, out mappings);
            //if (mappings?.Count == 0) return mappings;

            // Get the new edge in the queryGraph
            // New Edge = E(G') - E(H)
            var newEdgeNodes = new string[2];
            int index = 0;
            foreach (var node in queryGraph.Vertices)
            {
                //Trick: 
                // Given two graphs where one is a super-graph of the other and they only differ by one edge,
                // then we can be sure that there will be two nodes whose degrees changed.
                // These two nodes form the new edge.
                if (queryGraph.AdjacentDegree(node) == parentQueryGraph.AdjacentDegree(node)) continue;
                
                newEdgeNodes[index] = node;
                index++;
                if (index > 1) break;
            }

            // if no need to do new edge
            if (index == 0) return null;

            //var theNewEdge = new Edge<string>(newEdgeNodes[0], newEdgeNodes[1]);
            //newEdgeNodes = null;

            var theMappings = new Dictionary<string, List<Mapping>>();
            var subgraphSize = queryGraph.VertexCount;
            for (int i = 0; i < parentGraphMappings.Count; i++)
            {
                var map = parentGraphMappings[i];

                // Reember, f(h) = g
                // Remember, newInputSubgraph is a subgraph of inputGraph
                Edge<string> edge;
                if (map.InputSubGraph.TryGetEdge(map.Function[newEdgeNodes[0]], map.Function[newEdgeNodes[1]], out edge))
                {
                    var mapping = new Mapping(map.Function)
                    {
                        MapOnInputSubGraph = map.MapOnInputSubGraph,
                        InputSubGraph = map.InputSubGraph
                    };
                    mapping.MapOnInputSubGraph.AddEdge(edge);
                    List<Mapping> mappingsToSearch; //Recall: f(h) = g
                    var g_key = mapping.Function.ElementAt(subgraphSize - 1).Value;
                    if (theMappings.TryGetValue(g_key, out mappingsToSearch) && mappingsToSearch != null)
                    {
                        if (!mappingsToSearch.Exists(x => x.IsIsomorphicWith(mapping)))
                        //if (mappingsToSearch.Find(x => x.IsIsomorphicWith(mapping)) == null)
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
            Console.WriteLine("Algorithm 3: All tasks completed. Number of mappings found: {0}.\n", toReturn.Count);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

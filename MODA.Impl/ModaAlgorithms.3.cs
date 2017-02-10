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
        /// <param name="queryGraph">H</param>
        /// <param name="expansionTree">T_k</param>
        /// <param name="parentQueryGraph"></param>
        /// <param name="parentGraphMappings"></param>
        private static IList<Mapping> Algorithm3(QueryGraph queryGraph,
            AdjacencyGraph<ExpansionTreeNode, Edge<ExpansionTreeNode>> expansionTree,
            QueryGraph parentQueryGraph, IList<Mapping> parentGraphMappings)
        {
            if (parentGraphMappings.Count > 0)
            {
                //var timer = System.Diagnostics.Stopwatch.StartNew();
                var subgraphSize = queryGraph.VertexCount;
                var newEdge = GetEdgeDifference(queryGraph, parentQueryGraph);
                Edge<string> newEdgeImage;
                var list = new List<Mapping>();
                for (int i = 0; i < parentGraphMappings.Count; i++)
                {
                    var map = parentGraphMappings[i];
                    map.Id = i;
                    // Reember, f(h) = g

                    // if (f(u), f(v)) ϵ G and meets the conditions, add to list
                    if (map.InducedSubGraph.EdgeCount == queryGraph.EdgeCount)
                    {
                        newEdgeImage = map.GetImage(parentQueryGraph.Edges);
                    }
                    else if (map.InducedSubGraph.EdgeCount > queryGraph.EdgeCount)
                    {
                        newEdgeImage = map.GetImage(newEdge);
                    }
                    else
                    {
                        newEdgeImage = null;
                    }
                    if (newEdgeImage != null)
                    {
                        if (map.InducedSubGraph.ContainsEdge(newEdgeImage))
                        {
                            list.Add(map);
                        }
                    }
                }
                if (list.Count > 0)
                {
                    // Remove mappings from the parent qGraph that are found in this qGraph 
                    // This is because we're only interested in induced subgraphs
                    var dict = parentGraphMappings.ToDictionary(x => x.Id);
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (dict.ContainsKey(list[i].Id))
                        {
                            dict.Remove(list[i].Id);
                        }
                        //parentGraphMappings.RemoveBySwap(list[i]);
                    }
                    parentGraphMappings.Clear();
                    foreach (var item in dict)
                    {
                        parentGraphMappings.Add(item.Value);
                    }

                }
                Console.WriteLine("Algorithm 3: All tasks completed. Number of mappings found: {0}.\n", list.Count);

                //timer.Stop();
                //Console.WriteLine("Algorithm 3: All tasks completed. Number of mappings found: {0}.\nTotal time taken: {1}", theMappings.Count, timer.Elapsed);

                //timer = null;
                var toReturn = new List<Mapping>(list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    var map = list[i];
                    toReturn.Add(new Mapping(map.Function)
                    {
                        InducedSubGraph = map.InducedSubGraph
                    });
                }
                return toReturn;
            }
            return new Mapping[0];
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


        /// <summary>
        /// Returns the edge in <paramref="currentQueryGraph"/> that is not present in <paramref="parentQueryGraph"/>
        /// </summary>
        /// <param name="currentQueryGraph">The current subgraph being queried</param>
        /// <param name="parentQueryGraph">The parent to <paramref name="currentQueryGraph"/>. This parent is also a subset, meaning it has one edge less.</param>
        /// <returns></returns>
        private static Edge<string> GetEdgeDifference(QueryGraph currentQueryGraph, QueryGraph parentQueryGraph)
        {
            // Recall: currentQueryGraph is a super-graph of parentQueryGraph
            if (currentQueryGraph.EdgeCount - parentQueryGraph.EdgeCount != 1) throw new ArgumentException("Invalid arguments for the method: GetEdgeDifference");

            var edges = parentQueryGraph.Edges;
            foreach (var edge in currentQueryGraph.Edges)
            {
                if (!edges.Contains(edge))
                {
                    return edge;
                }
            }
            throw new InvalidOperationException();
        }


    }
}

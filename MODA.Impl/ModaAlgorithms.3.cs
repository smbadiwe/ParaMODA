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
        /// <param name="parentGraphMappings">NB: This param is still used even outside this method is call. So, be careful how you set/clear its values.</param>
        private static IList<Mapping> Algorithm3(QueryGraph queryGraph,
            AdjacencyGraph<ExpansionTreeNode, Edge<ExpansionTreeNode>> expansionTree,
            QueryGraph parentQueryGraph, IList<Mapping> parentGraphMappings)
        {
            if (parentGraphMappings.Count == 0) return new Mapping[0];

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
                if (map.InducedSubGraphEdges.Count == queryGraph.EdgeCount)
                {
                    newEdgeImage = map.GetImage(parentQueryGraph.Edges2);
                }
                else if (map.InducedSubGraphEdges.Count > queryGraph.EdgeCount)
                {
                    newEdgeImage = map.GetImage(newEdge);
                }
                else
                {
                    newEdgeImage = null;
                }
                if (newEdgeImage != null)
                {
                    if (map.InducedSubGraphEdges.Contains(newEdgeImage))
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
                parentGraphMappings.Clear();
                for (int i = 0; i < list.Count; i++)
                {
                    if (dict.ContainsKey(list[i].Id))
                    {
                        dict.Remove(list[i].Id);
                    }
                    //parentGraphMappings.RemoveBySwap(list[i]);
                }
                foreach (var item in dict)
                {
                    parentGraphMappings.Add(item.Value);
                }
                dict.Clear();
                var toReturn = new List<Mapping>(list.Count);
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    toReturn.Add(new Mapping(list[i].Function)
                    {
                        InducedSubGraphEdges = list[i].InducedSubGraphEdges
                    });
                    list.RemoveAt(i);
                }
                Console.WriteLine("Algorithm 3: All tasks completed. Number of mappings found: {0}.\n", toReturn.Count);
                return toReturn;
            }
            else
            {
                Console.WriteLine("Algorithm 3: All tasks completed. Number of mappings found: 0.\n");
                return new Mapping[0];
            }

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

            var edges = parentQueryGraph.Edges2;
            foreach (var edge in currentQueryGraph.Edges2)
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

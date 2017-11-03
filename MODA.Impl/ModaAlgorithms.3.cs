using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        /// <summary>
        /// Enumeration module. NB: If either of <paramref name="allMappings"/> or <paramref name="fileName"/> is null, the other will not be.
        /// </summary> 
        /// <param name="allMappings"></param>
        /// <param name="inputGraph">G</param>
        /// <param name="queryGraph">H</param>
        /// <param name="expansionTree">T_k</param>
        /// <param name="parentQueryGraph"></param>
        /// <param name="fileName"></param>
        /// <param name="parentGraphMappings">NB: This param is still used even outside this method is call. So, be careful how you set/clear its values.</param>
        private static IList<Mapping> Algorithm3(Dictionary<QueryGraph, IList<Mapping>> allMappings, UndirectedGraph<int> inputGraph, QueryGraph queryGraph,
            AdjacencyGraph<ExpansionTreeNode> expansionTree,
            QueryGraph parentQueryGraph, out string newFileName, string fileName = null)
        {
            newFileName = null;
            IList<Mapping> parentGraphMappings;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                if (!allMappings.TryGetValue(parentQueryGraph, out parentGraphMappings))
                {
                    return new Mapping[0];
                }
            }
            else
            {
                parentGraphMappings = parentQueryGraph.ReadMappingsFromFile(fileName);
            }
            if (parentGraphMappings.Count == 0) return new Mapping[0];

            var subgraphSize = queryGraph.VertexCount;
            var parentQueryGraphEdges = new Dictionary<Edge<int>, byte>(parentQueryGraph.EdgeCount);
            foreach (var edge in parentQueryGraph.Edges)
            {
                parentQueryGraphEdges.Add(edge, 1);
            }
            var newEdge = GetEdgeDifference(queryGraph, parentQueryGraph, parentQueryGraphEdges);

            // if it's NOT a valid edge
            if (newEdge.Source == Utils.DefaultEdgeNodeVal)
            {
                return new Mapping[0];
            }
            Edge<int> newEdgeImage;
            var list = new List<Mapping>();
            int oldCount = parentGraphMappings.Count, queryGraphEdgeCount = queryGraph.EdgeCount;
            for (int i = 0; i < oldCount; i++)
            {
                parentGraphMappings[i].Id = i;
                // Reember, f(h) = g

                // if (f(u), f(v)) ϵ G and meets the conditions, add to list
                if (parentGraphMappings[i].SubGraphEdgeCount == queryGraphEdgeCount)
                {
                    newEdgeImage = parentGraphMappings[i].GetImage(inputGraph, parentQueryGraphEdges);
                }
                else if (parentGraphMappings[i].SubGraphEdgeCount > queryGraphEdgeCount)
                {
                    newEdgeImage = parentGraphMappings[i].GetImage(inputGraph, newEdge);
                }
                else
                {
                    newEdgeImage = new Edge<int>(Utils.DefaultEdgeNodeVal, Utils.DefaultEdgeNodeVal);
                }

                // if it's a valid edge
                if (newEdgeImage.Source != Utils.DefaultEdgeNodeVal)
                {
                    if (inputGraph.ContainsEdge(newEdgeImage.Source, newEdgeImage.Target))
                    {
                        list.Add(parentGraphMappings[i]);
                    }
                }
            }
            var threadName = System.Threading.Thread.CurrentThread.ManagedThreadId;
            if (list.Count > 0)
            {
                // Remove mappings from the parent qGraph that are found in this qGraph 
                // This is because we're only interested in induced subgraphs
                var theRest = parentGraphMappings.Except(list).ToList();
                parentGraphMappings.Clear();
                foreach (var item in theRest)
                {
                    parentGraphMappings.Add(item);
                }
                if (!string.IsNullOrWhiteSpace(fileName) && oldCount > parentGraphMappings.Count)
                {
                    // This means that some of the mappings from parent fit the current query graph
                    newFileName = parentQueryGraph.WriteMappingsToFile(parentGraphMappings);
                    try
                    {
                        System.IO.File.Delete(fileName);
                    }
                    catch { } // we can afford to let this fail
                }

                Console.WriteLine("Thread {0}:\tAlgorithm 3: All tasks completed. Number of mappings found: {1}.\n", threadName, list.Count);
                return list;
            }
            else
            {
                Console.WriteLine("Thread {0}:\tAlgorithm 3: All tasks completed. Number of mappings found: 0.\n", threadName);
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
        private static QueryGraph GetParent(QueryGraph queryGraph, AdjacencyGraph<ExpansionTreeNode> expansionTree)
        {
            var hasNode = expansionTree.ContainsVertex(new ExpansionTreeNode
            {
                QueryGraph = queryGraph,
            });
            if (hasNode)
            {
                return expansionTree.Vertices.First(x => !x.IsRootNode && x.NodeName == queryGraph.Identifier).ParentNode.QueryGraph;
            }
            return null;
        }

        /// <summary>
        /// Returns the edge in <paramref="currentQueryGraph"/> that is not present in <paramref="parentQueryGraph"/>
        /// </summary>
        /// <param name="currentQueryGraph">The current subgraph being queried</param>
        /// <param name="parentQueryGraph">The parent to <paramref name="currentQueryGraph"/>. This parent is also a subset, meaning it has one edge less.</param>
        /// <param name="parentQueryGraphEdges">Edges of <see cref="parentQueryGraph"/>. We send it in to avoid re-computation since we already have it where this method is used</param>
        /// <returns></returns>
        private static Edge<int> GetEdgeDifference(QueryGraph currentQueryGraph, QueryGraph parentQueryGraph, Dictionary<Edge<int>, byte> parentQueryGraphEdges)
        {
            // Recall: currentQueryGraph is a super-graph of parentQueryGraph
            if (currentQueryGraph.EdgeCount - parentQueryGraph.EdgeCount != 1)
            {
                Console.WriteLine("Thread {0}:\tInvalid arguments for the method: GetEdgeDifference. [currentQueryGraph.EdgeCount - parentQueryGraph.EdgeCount] = {1}.\ncurrentQueryGraph.Label = '{2}'. parentQueryGraph.Label = '{3}'."
                    , System.Threading.Thread.CurrentThread.ManagedThreadId, (currentQueryGraph.EdgeCount - parentQueryGraph.EdgeCount), currentQueryGraph.Identifier, parentQueryGraph.Identifier);
                return new Edge<int>(Utils.DefaultEdgeNodeVal, Utils.DefaultEdgeNodeVal);
            }
            foreach (var edge in currentQueryGraph.Edges)
            {
                if (!parentQueryGraphEdges.ContainsKey(edge))
                {
                    return edge;
                }
            }
            return new Edge<int>(Utils.DefaultEdgeNodeVal, Utils.DefaultEdgeNodeVal);
        }
    }
}

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
                var theMappings = new Dictionary<string, List<Mapping>>();
                var subgraphSize = queryGraph.VertexCount;
                var list = new List<Mapping>();
                for (int i = 0; i < parentGraphMappings.Count; i++)
                {
                    var map = parentGraphMappings[i];
                    map.Id = i;
                    // Reember, f(h) = g
                    // Remember, newInputSubgraph is a subgraph of inputGraph
                    Edge<string> edge = map.GetEdgeDifference(queryGraph, parentQueryGraph);
                    if (edge != null)
                    {
                        var mapping = new Mapping(map.Function)
                        {
                            InducedSubGraph = map.InducedSubGraph
                        };
                        bool treated = false; string g_key_last = null;
                        if (theMappings.Count > 0)
                        {
                            foreach (var g_key in mapping.Function.Values)
                            {
                                g_key_last = g_key;
                                List<Mapping> mappingsToSearch; //Recall: f(h) = g
                                if (theMappings.TryGetValue(g_key, out mappingsToSearch))
                                {
                                    if (true == mappingsToSearch.Exists(x => x.IsIsomorphicWith(mapping, queryGraph)))
                                    {
                                        treated = true;
                                        break;
                                    }
                                    // else continue since it may exist in the other keys
                                }
                                mappingsToSearch = null;
                            }
                        }
                        else
                        {
                            g_key_last = mapping.Function.Last().Value;
                        }
                        if (!treated)
                        {
                            if (theMappings.ContainsKey(g_key_last))
                            {
                                theMappings[g_key_last].Add(mapping);
                            }
                            else
                            {
                                theMappings[g_key_last] = new List<Mapping> { mapping };
                            }
                            list.Add(map);
                        }
                        mapping = null;
                    }
                }
                if (list.Count > 0)
                {
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
                    list.Clear();
                }
                if (theMappings.Count > 0)
                {
                    foreach (var mapping in theMappings)
                    {
                        list.AddRange(mapping.Value);
                    }
                }
                Console.WriteLine("Algorithm 3: All tasks completed. Number of mappings found: {0}.\n", list.Count);

                //timer.Stop();
                //Console.WriteLine("Algorithm 3: All tasks completed. Number of mappings found: {0}.\nTotal time taken: {1}", theMappings.Count, timer.Elapsed);

                //timer = null;
                theMappings = null;
                return list;
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

    }
}

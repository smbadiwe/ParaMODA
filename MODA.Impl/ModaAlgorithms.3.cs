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
            QueryGraph parentQueryGraph, string fileName = null)
        {
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
                parentGraphMappings = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Mapping>>(Extensions.DecompressString(System.IO.File.ReadAllText(fileName)));
            }
            if (parentGraphMappings.Count == 0) return new Mapping[0];

            var subgraphSize = queryGraph.VertexCount;
            var newEdge = GetEdgeDifference(queryGraph, parentQueryGraph);

            if (true == EqualityComparer<Edge<int>>.Default.Equals(newEdge, default(Edge<int>)))
                return new Mapping[0];

            Edge<int> newEdgeImage;
            var list = new List<Mapping>();
            int oldCount = parentGraphMappings.Count;
            for (int i = 0; i < oldCount; i++)
            {
                parentGraphMappings[i].Id = i;
                //var map = parentGraphMappings[i];
                // Reember, f(h) = g

                // if (f(u), f(v)) ϵ G and meets the conditions, add to list
                if (parentGraphMappings[i].SubGraphEdgeCount == queryGraph.EdgeCount)
                {
                    newEdgeImage = parentGraphMappings[i].GetImage(inputGraph, parentQueryGraph.Edges2);
                }
                else if (parentGraphMappings[i].SubGraphEdgeCount > queryGraph.EdgeCount)
                {
                    newEdgeImage = parentGraphMappings[i].GetImage(inputGraph, newEdge);
                }
                else
                {
                    newEdgeImage = default(Edge<int>);
                }

                if (false == EqualityComparer<Edge<int>>.Default.Equals(newEdgeImage, default(Edge<int>)))
                {
                    if (inputGraph.ContainsEdge(newEdgeImage.Source, newEdgeImage.Target))
                    {
                        list.Add(parentGraphMappings[i]);
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
                }
                foreach (var item in dict)
                {
                    parentGraphMappings.Add(item.Value);
                }
                dict.Clear();
                if (!string.IsNullOrWhiteSpace(fileName) && oldCount > parentGraphMappings.Count)
                {
                    System.IO.File.WriteAllText(fileName, Extensions.CompressString(Newtonsoft.Json.JsonConvert.SerializeObject(parentGraphMappings)));
                }
                var toReturn = new List<Mapping>(list.Count);
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    toReturn.Add(new Mapping(list[i].Function, list[i].SubGraphEdgeCount, list[i].Id));
                    //list.RemoveAt(i);
                }
                list.Clear();
                list = null;
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
        private static QueryGraph GetParent(QueryGraph queryGraph, AdjacencyGraph<ExpansionTreeNode> expansionTree)
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
        private static Edge<int> GetEdgeDifference(QueryGraph currentQueryGraph, QueryGraph parentQueryGraph)
        {
            // Recall: currentQueryGraph is a super-graph of parentQueryGraph
            if (currentQueryGraph.EdgeCount - parentQueryGraph.EdgeCount != 1)
            {
                Console.WriteLine("Invalid arguments for the method: GetEdgeDifference. [currentQueryGraph.EdgeCount - parentQueryGraph.EdgeCount] = {0}.\ncurrentQueryGraph.Label = '{1}'. parentQueryGraph.Label = '{2}'."
                    , (currentQueryGraph.EdgeCount - parentQueryGraph.EdgeCount), currentQueryGraph.Label, parentQueryGraph.Label);
                return default(Edge<int>);
            }
            var edges = parentQueryGraph.Edges2;
            foreach (var edge in currentQueryGraph.Edges2)
            {
                if (!edges.Contains(edge))
                {
                    return edge;
                }
            }
            return default(Edge<int>); // throw new InvalidOperationException();
        }


    }
}

using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private HashSet<Mapping<int>> Algorithm3(UndirectedGraph<int, Edge<int>> queryGraph, UndirectedGraph<int, Edge<int>> inputGraph,
            AdjacencyGraph<ExpansionTreeNode<Edge<int>>, Edge<ExpansionTreeNode<Edge<int>>>> expansionTree,
            Dictionary<UndirectedGraph<int, Edge<int>>, HashSet<Mapping<int>>> foundMappings)
        {
            var parentQueryGraph = GetParent(queryGraph, expansionTree); // H
            var mappings = foundMappings[parentQueryGraph]; //It's guaranteed to be there. If it's not, there's a prolem
            if (mappings.Count == 0) return new HashSet<Mapping<int>>();

            // Get the new edge in the queryGraph
            // New Edge = E(G') - E(H)
            var newEdgeNodes = new List<int>(2);
            foreach (var node in queryGraph.Vertices)
            {
                //Trick: 
                // Given two graphs where one is a super-graph of the other and they only differ by one edge,
                // then we can be sure that there will be two nodes whose degrees changed.
                // These two nodes form the new edge.
                if (queryGraph.AdjacentDegree(node) == parentQueryGraph.AdjacentDegree(node)) continue;

                newEdgeNodes.Add(node);
            }
            var theNewEdge = new Edge<int>(newEdgeNodes[0], newEdgeNodes[1]);

            var theMappings = new HashSet<Mapping<int>>();
            foreach (var map in mappings)
            {
                if (map.Function.Count != map.InputSubGraph.VertexCount) continue;

                // Reember, f(h) = g
                // Remember, map.InputSubGraph is a subgraph of inputGraph
                Edge<int> edge;
                if (map.InputSubGraph.TryGetEdge(map.Function[theNewEdge.Source], map.Function[theNewEdge.Target], out edge))
                {
                    var newMap = new Mapping<int>(map.Function)
                    {
                        InputSubGraph = map.InputSubGraph,
                        MapOnInputSubGraph = map.InputSubGraph
                    };
                    if (theMappings.Any(x => x.Equals(newMap)))
                    {
                        continue;
                    }
                    theMappings.Add(newMap);
                }
            }

            var sb = new StringBuilder();
            sb.AppendFormat("{0}", queryGraph.AsString());
            sb.AppendLine("======================================");
            foreach (var map in theMappings)
            {
                sb.Append(map);
            }
            Console.WriteLine(sb);
            Console.WriteLine();
            return theMappings;
        }

        /// <summary>
        /// Helper method for algorithm 3
        /// </summary>
        /// <param name="queryGraph"></param>
        /// <param name="expansionTree"></param>
        /// <returns></returns>
        private UndirectedGraph<int, Edge<int>> GetParent(UndirectedGraph<int, Edge<int>> queryGraph, AdjacencyGraph<ExpansionTreeNode<Edge<int>>, Edge<ExpansionTreeNode<Edge<int>>>> expansionTree)
        {
            var vertex = new ExpansionTreeNode<Edge<int>>
            {
                QueryGraph = queryGraph,
            };
            var hasNode = expansionTree.ContainsVertex(vertex);
            if (hasNode)
            {
                return expansionTree.Vertices.First(x => !x.IsRootNode && x.QueryGraph == queryGraph).ParentNode.QueryGraph;
            }
            return null;
        }

    }
}

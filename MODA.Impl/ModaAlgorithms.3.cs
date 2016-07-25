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
        /// Enumeration module
        /// </summary>
        /// <param name="queryGraph"></param>
        /// <param name="inputGraph"></param>
        /// <param name="expansionTree"></param>
        public HashSet<Mapping<int>> Algorithm3(UndirectedGraph<int, Edge<int>> queryGraph, UndirectedGraph<int, Edge<int>> inputGraph,
            AdjacencyGraph<ExpansionTreeNode<Edge<int>>, Edge<ExpansionTreeNode<Edge<int>>>> expansionTree,
            Dictionary<UndirectedGraph<int, Edge<int>>, HashSet<Mapping<int>>> foundMappings)
        {
            var mappingsToReturn = new HashSet<Mapping<int>>();

            var parentQueryGraph = GetParent(queryGraph, expansionTree); // H
            var mappings = foundMappings[parentQueryGraph];

            // Get the new edge in the queryGraph
            Edge<int> theNewEdge; // E(G') - E(H)
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
            theNewEdge = new Edge<int>(newEdgeNodes[0], newEdgeNodes[1]);

            foreach (var map in mappings)
            {
                if (map.Function.Count != map.InputSubGraph.VertexCount) continue;
                // Remember, .MapGraph is a subgraph of inputGraph
                // Reember, f(h) = g
                Edge<int> edge;
                if (map.InputSubGraph.TryGetEdge(map.Function[theNewEdge.Source], map.Function[theNewEdge.Target], out edge))
                {
                    mappingsToReturn.Add(new Mapping<int>(map.Function) { InputSubGraph = queryGraph });
                }
            }
            return mappingsToReturn;
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

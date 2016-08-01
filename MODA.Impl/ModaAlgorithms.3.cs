using QuickGraph;
using System;
using System.Collections.Generic;
using System.IO;
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
        private HashSet<Mapping> Algorithm3(UndirectedGraph<string, Edge<string>> queryGraph, UndirectedGraph<string, Edge<string>> inputGraph,
            AdjacencyGraph<ExpansionTreeNode<Edge<string>>, Edge<ExpansionTreeNode<Edge<string>>>> expansionTree)
            //, Dictionary<UndirectedGraph<string, Edge<string>>, HashSet<Mapping>> foundMappings
        {
            var parentQueryGraph = GetParent(queryGraph, expansionTree); // H
            var file = Path.Combine(MapFolder, parentQueryGraph.AsString().Replace(">", "&lt;") + ".map");
            var mapObject = MyXmlSerializer.DeSerialize<Map>(File.ReadAllBytes(file));
            var mappings = mapObject.Mappings; // foundMappings[parentQueryGraph]; //It's guaranteed to be there. If it's not, there's a prolem
            if (mappings.Count == 0) return new HashSet<Mapping>();

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
            }
            var theNewEdge = new Edge<string>(newEdgeNodes[0], newEdgeNodes[1]);

            var theMappings = new HashSet<Mapping>();
            foreach (var map in mappings)
            {
                if (map.Function.Count != map.InputSubGraph.VertexCount) continue;

                // Reember, f(h) = g
                // Remember, map.InputSubGraph is a subgraph of inputGraph
                Edge<string> edge;
                if (map.InputSubGraph.TryGetEdge(map.Function[theNewEdge.Source], map.Function[theNewEdge.Target], out edge))
                {
                    var newMap = new Mapping(map.Function)
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

            return theMappings;
        }

        /// <summary>
        /// Helper method for algorithm 3
        /// </summary>
        /// <param name="queryGraph"></param>
        /// <param name="expansionTree"></param>
        /// <returns></returns>
        private UndirectedGraph<string, Edge<string>> GetParent(UndirectedGraph<string, Edge<string>> queryGraph, AdjacencyGraph<ExpansionTreeNode<Edge<string>>, Edge<ExpansionTreeNode<Edge<string>>>> expansionTree)
        {
            var vertex = new ExpansionTreeNode<Edge<string>>
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

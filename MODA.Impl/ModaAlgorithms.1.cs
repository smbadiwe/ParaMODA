using QuickGraph;
using System;
using System.Collections.Generic;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        /// <summary>
        /// Algo 1: Find subgraph frequency
        /// </summary>
        /// <param name="inputGraph"></param>
        /// <param name="subgraphSize"></param>
        /// <param name="thresholdValue"></param>
        /// <returns>Fg, frequent subgraph list</returns>
        public Dictionary<UndirectedGraph<int, Edge<int>>, HashSet<Mapping<int>>> Algorithm1(UndirectedGraph<int, Edge<int>> inputGraph, int subgraphSize, int thresholdValue = 0)
        {
            var builder = new ExpansionTreeBuilder<Edge<int>>(subgraphSize);
            builder.Build();

            var allMappings = new Dictionary<UndirectedGraph<int, Edge<int>>, HashSet<Mapping<int>>>();
            do
            {
                var qGraph = GetNextNode(builder.VerticesSorted).QueryGraph;
                HashSet<Mapping<int>> mappings;
                if (qGraph.EdgeCount == (subgraphSize - 1))
                {
                    //TODO: Mapping module - MODA and Grockow & Kellis
                    mappings = Algorithm2(qGraph, inputGraph);
                }
                else
                {
                    //TODO: Enumeration moodule - MODA
                    mappings = Algorithm3(qGraph, inputGraph, builder.ExpansionTree, allMappings);
                }
                if (mappings.Count == 0) continue;

                allMappings.Add(qGraph, mappings); //Save mappings. Do we need to save to disk?

                //Check for complete-ness; if complete, break
                //  A Complete graph of n nodes has n(n-1)/2 edges
                if (qGraph.EdgeCount == ((subgraphSize * (subgraphSize - 1)) / 2))
                {
                    break;
                }
            }
            while (true);

            var frequentSubgraphs = new Dictionary<UndirectedGraph<int, Edge<int>>, HashSet<Mapping<int>>>();
            if (allMappings.Count > 0)
            {
                foreach (var item in allMappings)
                {
                    if (item.Value.Count > thresholdValue)
                    {
                        frequentSubgraphs.Add(item.Key, item.Value);
                    }
                    Console.WriteLine("Subgraph: [Nodes: {0}; Edges: {1}]:\tNo. of mappings: {2}", item.Key.VertexCount, item.Key.EdgeCount, item.Value.Count);
                }
            }
            return frequentSubgraphs;
        }

        /// <summary>
        /// Helper method for algorithm 1
        /// </summary>
        /// <param name="extTreeNodesQueued"></param>
        /// <returns></returns>
        private ExpansionTreeNode<Edge<int>> GetNextNode(IDictionary<ExpansionTreeNode<Edge<int>>, GraphColor> extTreeNodesQueued)
        {
            foreach (var node in extTreeNodesQueued)
            {
                if (node.Value == GraphColor.White) continue;

                extTreeNodesQueued[node.Key] = GraphColor.White;
                return node.Key;
            }
            return null;
        }
    }
}

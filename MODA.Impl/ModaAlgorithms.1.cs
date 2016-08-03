using QuickGraph;
using System;
using System.Collections.Generic;
using System.IO;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        public static string MapFolder = @"C:\SOMA\Drive\MyUW\Research\Kim\Capstone\ExperimentalNetworks\MapFolder";
        /// <summary>
        /// Algo 1: Find subgraph frequency
        /// </summary>
        /// <param name="inputGraph"></param>
        /// <param name="subgraphSize"></param>
        /// <param name="thresholdValue"></param>
        /// <returns>Fg, frequent subgraph list</returns>
        public Dictionary<UndirectedGraph<string, Edge<string>>, int> Algorithm1(UndirectedGraph<string, Edge<string>> inputGraph, int subgraphSize, int thresholdValue = 0)
        {
            var builder = new ExpansionTreeBuilder<Edge<string>>(subgraphSize);
            builder.Build();

            var allMappings = new Dictionary<UndirectedGraph<string, Edge<string>>, int>();
            do
            {
                var qGraph = GetNextNode(builder.VerticesSorted).QueryGraph;
                List<Mapping> mappings;
                if (qGraph.EdgeCount == (subgraphSize - 1))
                {
                    //TODO: Mapping module - MODA and Grockow & Kellis
                    mappings = Algorithm2_Modified(qGraph, inputGraph); //Algorithm2_Original
                }
                else
                {
                    //TODO: Enumeration moodule - MODA
                    mappings = Algorithm3(qGraph, inputGraph, builder.ExpansionTree);
                }
                if (mappings.Count > 0)
                {
                    //Save mappings. Do we need to save to disk?
                    allMappings.Add(qGraph, mappings.Count);
                    File.WriteAllBytes(Path.Combine(MapFolder, qGraph.AsString().Replace(">", "&lt;") + ".map"), MySerializer.Serialize(new Map
                    {
                        QueryGraph = qGraph,
                        Mappings = mappings,
                    }));
                    mappings = null;

                    //Check for complete-ness; if complete, break
                    //  A Complete graph of n nodes has n(n-1)/2 edges
                    if (qGraph.EdgeCount == ((subgraphSize * (subgraphSize - 1)) / 2))
                    {
                        qGraph = null;
                        break;
                    }
                    qGraph = null;
                }
            }
            while (true);

            builder = null;
            return allMappings;
        }

        /// <summary>
        /// Helper method for algorithm 1
        /// </summary>
        /// <param name="extTreeNodesQueued"></param>
        /// <returns></returns>
        private ExpansionTreeNode<Edge<string>> GetNextNode(IDictionary<ExpansionTreeNode<Edge<string>>, GraphColor> extTreeNodesQueued)
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

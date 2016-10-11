using QuickGraph;
using System.Collections.Generic;
using System.IO;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        public static int VertexCountDividend { get; set; }
        
        public const string MapFolder = @"C:\SOMA\Drive\MyUW\Research\Kim\Capstone\ExperimentalNetworks\MapFolder";
        
        /// <summary>
        /// Algo 1: Find subgraph frequency (mappings help in memory)
        /// </summary>
        /// <param name="inputGraph"></param>
        /// <param name="subgraphSize"></param>
        /// <param name="thresholdValue"></param>
        /// <returns>Fg, frequent subgraph list</returns>
        public static Dictionary<UndirectedGraph<string, Edge<string>>, List<Mapping>> Algorithm1(UndirectedGraph<string, Edge<string>> inputGraph, int subgraphSize, int thresholdValue = 0)
        {
            var builder = new ExpansionTreeBuilder<Edge<string>>(subgraphSize);
            builder.Build();

            var allMappings = new Dictionary<UndirectedGraph<string, Edge<string>>, List<Mapping>>();
            do
            {
                var qGraph = GetNextNode(builder.VerticesSorted)?.QueryGraph;
                if (qGraph == null) break;
                List<Mapping> mappings;
                if (qGraph.EdgeCount == (subgraphSize - 1))
                {
                    // Modified Mapping module - MODA and Grockow & Kellis
#if MODIFIED 
                    //mappings = Algorithm2_Modified(qGraph, inputGraph);
                    mappings = ModaAlgorithm2Parallelized.Algorithm2_Modified(qGraph, inputGraph);
#else 
                    mappings = Algorithm2(qGraph, inputGraph);
#endif
                }
                else
                {
                    // Enumeration moodule - MODA
                    mappings = Algorithm3(qGraph, inputGraph, builder.ExpansionTree, allMappings);
                }

                // Save mappings. Do we need to save to disk? Yes!
                allMappings.Add(qGraph, mappings);

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
            while (true);

            builder = null;
            return allMappings;
        }

        /// <summary>
        /// Helper method for algorithm 1
        /// </summary>
        /// <param name="extTreeNodesQueued"></param>
        /// <returns></returns>
        public static ExpansionTreeNode<Edge<string>> GetNextNode(IDictionary<ExpansionTreeNode<Edge<string>>, GraphColor> extTreeNodesQueued)
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

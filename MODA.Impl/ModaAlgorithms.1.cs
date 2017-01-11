using QuickGraph;
using System.Collections.Generic;
using System.Linq;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        /// <summary>
        /// Frequency value, above which we can comsider the subgraph a "frequent subgraph"
        /// </summary>
        public static int Threshold { get; set; }
        /// <summary>
        /// If true, it means we only care about how many mappings are found for each subgraph, not info about the mappings themselves.
        /// </summary>
        public static bool GetOnlyMappingCounts { get; set; }
        
        public const string MapFolder = @"C:\SOMA\Drive\MyUW\Research\Kim\Capstone\ExperimentalNetworks\MapFolder";
        private static ExpansionTreeBuilder<Edge<string>> _builder;
        public static void BuildTree(UndirectedGraph<string, Edge<string>> queryGraph, int subgraphSize)
        {
            _builder = new ExpansionTreeBuilder<Edge<string>>(subgraphSize, queryGraph: queryGraph);
            _builder.Build();
        }

        /// <summary>
        /// Algo 1: Find subgraph frequency (mappings help in memory)
        /// </summary>
        /// <param name="inputGraph"></param>
        /// <param name="subgraphSize"></param>
        /// <param name="thresholdValue"></param>
        /// <returns>Fg, frequent subgraph list. NB: The dictionary .Value is an <see cref="object"/> which will be either a list of <see cref="Mapping"/> or a <see cref="long"/>
        /// depending on the value of <see cref="GetOnlyMappingCounts"/>.</returns>
        public static Dictionary<UndirectedGraph<string, Edge<string>>, object> Algorithm1(UndirectedGraph<string, Edge<string>> inputGraph, int subgraphSize, int thresholdValue = 0)
        {
            // The enumeration module (Algo 3) needs the mappings generated from the previous run(s)
            var allMappings = new Dictionary<UndirectedGraph<string, Edge<string>>, List<Mapping>>();
            do
            {
                var qGraph = GetNextNode()?.QueryGraph;
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
                    mappings = Algorithm3(qGraph, inputGraph, _builder.ExpansionTree, allMappings);
                }

                if (mappings.Count > Threshold)
                {
                    qGraph.IsFrequentSubgraph = true;
                }
                // Save mappings. Do we need to save to disk? Maybe not!
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

            _builder = null;
            var toReturn = new Dictionary<UndirectedGraph<string, Edge<string>>, object>();

            if (GetOnlyMappingCounts)
            {
                foreach (var item in allMappings)
                {
                    toReturn.Add(item.Key, item.Value.Count);
                } 
            }
            else
            {
                foreach (var item in allMappings)
                {
                    toReturn.Add(item.Key, item.Value);
                }
            }
            allMappings = null;
            return toReturn;
        }
        
        /// <summary>
        /// Helper method for algorithm 1
        /// </summary>
        /// <param name="extTreeNodesQueued"></param>
        /// <returns></returns>
        private static ExpansionTreeNode GetNextNode()
        {
            foreach (var node in _builder.VerticesSorted)
            {
                if (node.Value == GraphColor.White) continue;

                _builder.VerticesSorted[node.Key] = GraphColor.White;
                return node.Key;
            }
            return null;
        }
    }
}

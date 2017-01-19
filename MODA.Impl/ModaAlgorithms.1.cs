using QuickGraph;
using System.Collections.Generic;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        private static ExpansionTreeBuilder<Edge<string>> _builder;
        public static void BuildTree(int subgraphSize)
        {
            _builder = new ExpansionTreeBuilder<Edge<string>>(subgraphSize);
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
        public static Dictionary<QueryGraph, object> Algorithm1(UndirectedGraph<string, Edge<string>> inputGraph, int subgraphSize, int thresholdValue = 0)
        {
            // The enumeration module (Algo 3) needs the mappings generated from the previous run(s)
            var allMappings = new Dictionary<QueryGraph, List<Mapping>>();
            int numIterations = -1;
            if (inputGraph.VertexCount < 121) numIterations = inputGraph.VertexCount;
            if (QueryGraph != null)
            {
                List<Mapping> mappings;
                if (UseModifiedGrochow)
                {
                    // Modified Mapping module - MODA and Grockow & Kellis
                    mappings = Algorithm2_Modified(QueryGraph, inputGraph, numIterations);
                    //mappings = ModaAlgorithm2Parallelized.Algorithm2_Modified(qGraph, inputGraph, numIterations);
                }
                else
                {
                    mappings = Algorithm2(QueryGraph, inputGraph, numIterations);
                }
                allMappings.Add(QueryGraph, mappings);
            }
            else // Use MODA's expansion tree
            {
                do
                {
                    var qGraph = GetNextNode()?.QueryGraph;
                    if (qGraph == null) break;
                    List<Mapping> mappings;
                    if (qGraph.EdgeCount == (subgraphSize - 1))
                    {
                        if (UseModifiedGrochow)
                        {
                            // Modified Mapping module - MODA and Grockow & Kellis
                            mappings = Algorithm2_Modified(qGraph, inputGraph, numIterations);
                            //mappings = ModaAlgorithm2Parallelized.Algorithm2_Modified(qGraph, inputGraph);
                        }
                        else
                        {
                            mappings = Algorithm2(qGraph, inputGraph, numIterations);
                        }
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
                    if (qGraph.EdgeCount == ((qGraph.VertexCount * (qGraph.VertexCount - 1)) / 2))
                    {
                        qGraph = null;
                        break;
                    }
                    qGraph = null;
                }
                while (true);
            }
            var toReturn = new Dictionary<QueryGraph, object>();

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

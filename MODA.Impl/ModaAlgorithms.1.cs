using QuickGraph;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
        /// <param name="qGraph">The query graph to be searched for. If not available, we use expansion trees (MODA). Otherwise, we use Grochow's (Algo 2)</param>
        /// <param name="subgraphSize"></param>
        /// <param name="thresholdValue">Frequency value, above which we can comsider the subgraph a "frequent subgraph"</param>
        /// <returns>Fg, frequent subgraph list. NB: The dictionary .Value is an <see cref="object"/> which will be either a list of <see cref="Mapping"/> or a <see cref="long"/>
        /// depending on the value of <see cref="GetOnlyMappingCounts"/>.</returns>
        public static Dictionary<QueryGraph, IList<Mapping>> Algorithm1(UndirectedGraph<string, Edge<string>> inputGraph, QueryGraph qGraph, int subgraphSize, int thresholdValue)
        {
            // The enumeration module (Algo 3) needs the mappings generated from the previous run(s)
            Dictionary<QueryGraph, IList<Mapping>> allMappings;
            int numIterations = -1;
            if (inputGraph.VertexCount < 121) numIterations = inputGraph.VertexCount;

            if (qGraph == null) // Use MODA's expansion tree
            {
                allMappings = new Dictionary<QueryGraph, IList<Mapping>>(_builder.NumberOfQueryGraphs);
                do
                {
                    qGraph = GetNextNode()?.QueryGraph;
                    if (qGraph == null) break;
                    IList<Mapping> mappings;
                    if (qGraph.EdgeCount == (subgraphSize - 1))
                    {
                        var inputGraphClone = inputGraph.Clone();
                        if (UseModifiedGrochow)
                        {
                            // Modified Mapping module - MODA and Grockow & Kellis
                            mappings = Algorithm2_Modified(qGraph, inputGraphClone, numIterations);
                            //mappings = ModaAlgorithm2Parallelized.Algorithm2_Modified(qGraph, inputGraph);
                        }
                        else
                        {
                            mappings = Algorithm2(qGraph, inputGraphClone, numIterations);
                        }
                    }
                    else
                    {
                        // Enumeration moodule - MODA

                        //var timer = System.Diagnostics.Stopwatch.StartNew();

                        // This is part of Algo 3; but performance tweaks makes it more useful to get it here
                        var parentQueryGraph = GetParent(qGraph, _builder.ExpansionTree);
                        IList<Mapping> parentGraphMappings;
                        if (allMappings.TryGetValue(parentQueryGraph, out parentGraphMappings))
                        {
                            mappings = Algorithm3(qGraph, _builder.ExpansionTree, parentQueryGraph, parentGraphMappings);
                        }
                        else
                        {
                            mappings = null;
                        }
                    }
                    if (mappings != null && mappings.Count > thresholdValue)
                    {
                        qGraph.IsFrequentSubgraph = true;
                    }
                    // Save mappings. Do we need to save to disk? Maybe not!
                    allMappings.Add(qGraph, mappings);

                    mappings = null;

                    // Check for complete-ness; if complete, break
                    if (qGraph.EdgeCount == ((subgraphSize * (subgraphSize - 1)) / 2))
                    {
                        qGraph = null;
                        break;
                    }
                    qGraph = null;
                }
                while (true);
            }
            else
            {
                List<Mapping> mappings;
                if (UseModifiedGrochow)
                {
                    // Modified Mapping module - MODA and Grockow & Kellis
                    mappings = Algorithm2_Modified(qGraph, inputGraph, numIterations);
                    // mappings = ModaAlgorithm2Parallelized.Algorithm2_Modified(qGraph, inputGraph, numIterations);
                }
                else
                {
                    mappings = Algorithm2(qGraph, inputGraph, numIterations);
                }
                allMappings = new Dictionary<QueryGraph, IList<Mapping>>(1) { { qGraph, mappings } };
            }

            return allMappings;
        }

        /// <summary>
        /// Helper method for algorithm 1
        /// </summary>
        /// <param name="extTreeNodesQueued"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

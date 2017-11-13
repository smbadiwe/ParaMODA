using QuickGraph;
using System.Collections.Generic;

namespace ParaMODA.Impl
{
    public partial class ModaAlgorithms
    {

        /// <summary>
        /// Algo 1: Find subgraph frequency (mappings found are saved to disk to be retrieved later during Algo 3).
        /// The value of the dictionary returned is in the form: $"{mappings.Count}#{qGraph.Label}.ser"
        /// </summary>
        /// <param name="inputGraph"></param>
        /// <param name="qGraph">The query graph to be searched for. If not available, we use expansion trees (MODA). Otherwise, we use Grochow's (Algo 2)</param>
        /// <param name="subgraphSize"></param>
        /// <param name="thresholdValue">Frequency value, above which we can comsider the subgraph a "frequent subgraph"</param>
        /// <returns></returns>
        public static Dictionary<QueryGraph, string> Algorithm1_C(UndirectedGraph<int> inputGraph, QueryGraph qGraph, int subgraphSize, int thresholdValue)
        {
            // The enumeration module (Algo 3) needs the mappings generated from the previous run(s)
            Dictionary<QueryGraph, string> allMappings;
            int numIterations = -1;
            if (inputGraph.VertexCount < 121) numIterations = inputGraph.VertexCount;

            if (qGraph == null) // Use MODA's expansion tree
            {
                #region Use MODA's expansion tree
                var treatedNodes = new HashSet<QueryGraph>();
                allMappings = new Dictionary<QueryGraph, string>(_builder.NumberOfQueryGraphs);
                do
                {
                    qGraph = GetNextNode()?.QueryGraph;
                    if (qGraph == null) break;
                    ICollection<Mapping> mappings;
                    if (qGraph.EdgeCount == (subgraphSize - 1)) // i.e. if qGraph is a tree
                    {
                        if (UseModifiedGrochow)
                        {
                            // Modified Mapping module - MODA and Grockow & Kellis
                            mappings = Algorithm2_Modified(qGraph, inputGraph, numIterations, false);
                        }
                        else
                        {
                            var inputGraphClone = inputGraph.Clone();
                            mappings = Algorithm2(qGraph, inputGraphClone, numIterations, false);
                            inputGraphClone.Clear();
                            inputGraphClone = null;
                        }

                        // Because we're saving to file, we're better off doing this now
                        qGraph.RemoveNonApplicableMappings(mappings, inputGraph, false);
                        treatedNodes.Add(qGraph);
                    }
                    else
                    {
                        // Enumeration moodule - MODA
                        // This is part of Algo 3; but performance tweaks makes it more useful to get it here
                        var parentQueryGraph = GetParent(qGraph, _builder.ExpansionTree);
                        if (parentQueryGraph.EdgeCount == (subgraphSize - 1))
                        {
                            treatedNodes.Add(parentQueryGraph);
                        }
                        string _filename;
                        if (allMappings.TryGetValue(parentQueryGraph, out _filename))
                        {
                            string newFileName; // for parentQueryGraph
                            mappings = Algorithm3(null, inputGraph, qGraph, _builder.ExpansionTree, parentQueryGraph, out newFileName, _filename);
                            if (!string.IsNullOrWhiteSpace(newFileName))
                            {
                                // We change the _filename value in the dictionary since this means some of the mappings from parent fit the child
                                allMappings[parentQueryGraph] = newFileName;
                            }
                        }
                        else
                        {
                            mappings = new Mapping[0];
                        }
                    }

                    if (mappings.Count > thresholdValue)
                    {
                        qGraph.IsFrequentSubgraph = true;
                    }

                    // Save mappings. 
                    var fileName = qGraph.WriteMappingsToFile(mappings);
                    if (mappings.Count > 0) mappings.Clear();
                    allMappings.Add(qGraph, fileName);

                    // Check for complete-ness; if complete, break
                    if (qGraph.IsComplete(subgraphSize))
                    {
                        qGraph = null;
                        break;
                    }
                    qGraph = null;
                }
                while (true);
                #endregion
            }
            else
            {
                ICollection<Mapping> mappings;
                if (UseModifiedGrochow)
                {
                    // Modified Mapping module - MODA and Grockow & Kellis
                    mappings = Algorithm2_Modified(qGraph, inputGraph, numIterations, true);
                }
                else
                {
                    mappings = Algorithm2(qGraph, inputGraph, numIterations, true);
                }
                qGraph.RemoveNonApplicableMappings(mappings, inputGraph);
                var fileName = $"{mappings.Count}#{qGraph.Identifier}.ser";
                System.IO.File.WriteAllText(fileName, Extensions.CompressString(Newtonsoft.Json.JsonConvert.SerializeObject(mappings)));
                if (mappings.Count > 0) mappings.Clear();
                allMappings = new Dictionary<QueryGraph, string>(1) { { qGraph, fileName } };
            }

            return allMappings;
        }


        /// <summary>
        /// Algo 1: Find subgraph frequency (mappings help in memory)
        /// </summary>
        /// <param name="inputGraph"></param>
        /// <param name="qGraph">The query graph to be searched for. If not available, we use expansion trees (MODA). Otherwise, we use Grochow's (Algo 2)</param>
        /// <param name="subgraphSize"></param>
        /// <param name="thresholdValue">Frequency value, above which we can comsider the subgraph a "frequent subgraph"</param>
        /// <returns></returns>
        public static Dictionary<QueryGraph, ICollection<Mapping>> Algorithm1(UndirectedGraph<int> inputGraph, QueryGraph qGraph, int subgraphSize = -1, int thresholdValue = 0)
        {
            // The enumeration module (Algo 3) needs the mappings generated from the previous run(s)
            Dictionary<QueryGraph, ICollection<Mapping>> allMappings;
            int numIterations = -1;
            if (inputGraph.VertexCount < 121) numIterations = inputGraph.VertexCount;

            if (qGraph == null) // Use MODA's expansion tree
            {
                #region Use MODA's expansion tree
                var treatedNodes = new HashSet<QueryGraph>();
                allMappings = new Dictionary<QueryGraph, ICollection<Mapping>>(_builder.NumberOfQueryGraphs);
                do
                {
                    qGraph = GetNextNode()?.QueryGraph;
                    if (qGraph == null) break;
                    ICollection<Mapping> mappings;
                    if (qGraph.IsTree(subgraphSize))
                    {
                        if (UseModifiedGrochow)
                        {
                            // Modified Mapping module - MODA and Grockow & Kellis
                            mappings = Algorithm2_Modified(qGraph, inputGraph, numIterations, false);
                        }
                        else
                        {
                            // Mapping module - MODA and Grockow & Kellis.
                            var inputGraphClone = inputGraph.Clone();
                            mappings = Algorithm2(qGraph, inputGraphClone, numIterations, false);
                            inputGraphClone.Clear();
                            inputGraphClone = null;
                        }
                    }
                    else
                    {
                        // Enumeration moodule - MODA
                        // This is part of Algo 3; but performance tweaks makes it more useful to get it here
                        var parentQueryGraph = GetParent(qGraph, _builder.ExpansionTree);
                        if (parentQueryGraph.IsTree(subgraphSize))
                        {
                            treatedNodes.Add(parentQueryGraph);
                        }
                        string file;
                        mappings = Algorithm3(allMappings, inputGraph, qGraph, _builder.ExpansionTree, parentQueryGraph, out file);
                    }
                    if (mappings != null && mappings.Count > thresholdValue)
                    {
                        qGraph.IsFrequentSubgraph = true;
                    }
                    // Save mappings. Do we need to save to disk? Maybe not!

                    allMappings.Add(qGraph, mappings);
                    // Do not call mappings.Clear()
                    mappings = null;
                    // Check for complete-ness; if complete, break
                    if (qGraph.IsComplete(subgraphSize))
                    {
                        qGraph = null;
                        break;
                    }
                    qGraph = null;
                }
                while (true);

                if (treatedNodes.Count > 0)
                {
                    foreach (var mapping in allMappings)
                    {
                        if (mapping.Key.IsTree(subgraphSize) && !treatedNodes.Contains(mapping.Key))
                        {
                            mapping.Key.RemoveNonApplicableMappings(mapping.Value, inputGraph);
                        }
                    }
                    treatedNodes.Clear();
                }
                treatedNodes = null;
                #endregion
            }
            else
            {
                ICollection<Mapping> mappings;
                if (UseModifiedGrochow)
                {
                    // Modified Mapping module - MODA and Grockow & Kellis
                    mappings = Algorithm2_Modified(qGraph, inputGraph, numIterations, true);
                    // mappings = ModaAlgorithm2Parallelized.Algorithm2_Modified(qGraph, inputGraph, numIterations);
                }
                else
                {
                    mappings = Algorithm2(qGraph, inputGraph, numIterations, true);
                }

                qGraph.RemoveNonApplicableMappings(mappings, inputGraph);
                allMappings = new Dictionary<QueryGraph, ICollection<Mapping>>(1) { { qGraph, mappings } };

                // Do not call mappings.Clear()
                mappings = null;
            }

            return allMappings;
        }

    }
}

using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParaMODA.Impl
{
    public partial class ModaAlgorithms
    {
        /// <summary>
        /// Mapping module; aka FindSubgraphInstances in Grochow & Kellis
        /// </summary>
        /// <param name="queryGraph">H</param>
        /// <param name="inputGraphClone">G</param>
        /// <param name="numberOfSamples">To be decided. If not set, we use the <paramref name="inputGraphClone"/> size / 3</param>
        internal static ICollection<Mapping> Algorithm2(QueryGraph queryGraph, UndirectedGraph<int> inputGraphClone, int numberOfSamples, bool getInducedMappingsOnly)
        {
            if (numberOfSamples <= 0) numberOfSamples = inputGraphClone.VertexCount / 3;

            // Do we need this clone? Can't we just remove the node directly from the graph? 
            // We do need it.
            var theMappings = new Dictionary<int[], List<Mapping>>(MappingNodesComparer);
            var inputGraphDegSeq = inputGraphClone.GetNodesSortedByDegree(numberOfSamples);
            var queryGraphVertices = queryGraph.Vertices.ToArray();
            var queryGraphEdges = queryGraph.Edges.ToArray();
            var subgraphSize = queryGraphVertices.Length;
            var threadName = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Thread {0}:\tCallingu Algo 2:\n", threadName);
            for (int i = 0; i < inputGraphDegSeq.Count; i++)
            {
                var g = inputGraphDegSeq[i];
                for (int j = 0; j < subgraphSize; j++)
                {
                    var h = queryGraphVertices[j];
                    if (Utils.CanSupport(queryGraph, h, inputGraphClone, g))
                    {
                        #region Can Support
                        //Remember: f(h) = g, so h is Domain and g is Range
                        var f = new Dictionary<int, int>(1);
                        f[h] = g;
                        var mappings = Utils.IsomorphicExtension(f, queryGraph, queryGraphEdges, inputGraphClone, getInducedMappingsOnly);
                        f.Clear();
                        f = null;
                        if (mappings.Count > 0)
                        {
                            foreach (var item in mappings)
                            {
                                if (item.Value.Count > 1)
                                {
                                    queryGraph.RemoveNonApplicableMappings(item.Value, inputGraphClone, getInducedMappingsOnly);
                                }
                                //Recall: f(h) = g
                                List<Mapping> maps;
                                if (theMappings.TryGetValue(item.Key, out maps))
                                {
                                    maps.AddRange(item.Value);
                                }
                                else
                                {
                                    theMappings[item.Key] = item.Value;
                                }
                            }
                            mappings.Clear();
                        }
                        mappings = null;
                        #endregion
                    }
                }

                //Remove g
                inputGraphClone.RemoveVertex(g);
                if (inputGraphClone.EdgeCount == 0) break;
            }
            Array.Clear(queryGraphEdges, 0, queryGraphEdges.Length);
            queryGraphEdges = null;
            Array.Clear(queryGraphVertices, 0, subgraphSize);
            queryGraphVertices = null;
            inputGraphDegSeq.Clear();
            inputGraphDegSeq = null;

            var toReturn = GetSet(theMappings);
            theMappings = null;

            Console.WriteLine("Thread {0}:\tAlgorithm 2: All tasks completed. Number of mappings found: {1}.", threadName, toReturn.Count);
            return toReturn;
        }
    }
}

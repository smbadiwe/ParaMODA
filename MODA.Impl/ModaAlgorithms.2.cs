using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        /// <summary>
        /// Mapping module; aka FindSubgraphInstances in Grochow & Kellis
        /// </summary>
        /// <param name="queryGraph">H</param>
        /// <param name="inputGraphClone">G</param>
        /// <param name="numberOfSamples">To be decided. If not set, we use the <paramref name="inputGraphClone"/> size / 3</param>
        internal static List<Mapping> Algorithm2(QueryGraph queryGraph, UndirectedGraph<int> inputGraphClone, int numberOfSamples, bool getInducedMappingsOnly)
        {
            if (numberOfSamples <= 0) numberOfSamples = inputGraphClone.VertexCount / 3;

            // Do we need this clone? Can't we just remove the node directly from the graph? 
            // We do need it.
            var theMappings = new Dictionary<IList<int>, List<Mapping>>(MappingNodesComparer);
            var inputGraphDegSeq = inputGraphClone.GetNodesSortedByDegree(numberOfSamples);
            var queryGraphVertices = queryGraph.Vertices.ToArray();
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
                        var mappings = Utils.IsomorphicExtension(f, queryGraph, inputGraphClone, getInducedMappingsOnly);
                        
                        if (mappings.Count > 0)
                        {
                            foreach (var item in mappings)
                            {
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
                        }
                        #endregion
                    }
                }

                //Remove g
                inputGraphClone.RemoveVertex(g);
            }
            var toReturn = new HashSet<Mapping>(theMappings.Values.SelectMany(s => s));
            
            Console.WriteLine("Thread {0}:\tAlgorithm 2: All tasks completed. Number of mappings found: {1}.", threadName, toReturn.Count);
            return toReturn.ToList();
        }
    }
}

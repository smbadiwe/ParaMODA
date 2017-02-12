using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        internal static List<Mapping> Algorithm2(QueryGraph queryGraph, UndirectedGraph<string, Edge<string>> inputGraphClone, int numberOfSamples = -1)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            if (numberOfSamples <= 0) numberOfSamples = inputGraphClone.VertexCount / 3; // VertexCountDividend;

            // Do we need this clone? Can't we just remove the node directly from the graph? 
            // We do need it.

            H_NodeNeighbours = new Dictionary<string, IList<string>>();
            //var theMappings = new Dictionary<string[], List<Mapping>>(new MappingNodesComparer());
            var inputGraphDegSeq = inputGraphClone.GetDegreeSequence(numberOfSamples);
            var queryGraphVertices = queryGraph.Vertices.ToArray();
            var subgraphSize = queryGraphVertices.Length;

            Console.WriteLine("Calling Algo 2:\n");
            var toReturn = new List<Mapping>();
            for (int i = 0; i < inputGraphDegSeq.Count; i++)
            {
                var g = inputGraphDegSeq[i];
                G_NodeNeighbours = new Dictionary<string, IList<string>>();
                for (int j = 0; j < subgraphSize; j++)
                {
                    var h = queryGraphVertices[j];
                    if (CanSupport(queryGraph, h, inputGraphClone, g))
                    {
                        #region Can Support
                        //var sw = System.Diagnostics.Stopwatch.StartNew();
                        //Remember: f(h) = g, so h is Domain and g is Range
                        var f = new Dictionary<string, string>(1);
                        f[h] = g;
                        var mappings = IsomorphicExtension(f, queryGraph, inputGraphClone);
                        if (mappings.Count > 0)
                        {
                            toReturn.AddRange(mappings);
                        }
                        #endregion
                    }
                }

                //Remove g
                inputGraphClone.RemoveVertex(g);
                G_NodeNeighbours = null;
            }
            
            //Console.WriteLine("\nAlgorithm 2: All iteration tasks completed. Number of mappings found: {0}.\n", toReturn.Count);
            timer.Stop();
            Console.WriteLine("Algorithm 2: All tasks completed. Number of mappings found: {0}.\nTotal time taken: {1}", toReturn.Count, timer.Elapsed.ToString());
            timer = null;
            //theMappings = null;
            inputGraphDegSeq = null;
            queryGraphVertices = null;
            inputGraphClone = null;
            G_NodeNeighbours = null;
            H_NodeNeighbours = null;
            return toReturn;
        }
    }
}

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
            //var timer = System.Diagnostics.Stopwatch.StartNew();
            if (numberOfSamples <= 0) numberOfSamples = inputGraphClone.VertexCount / 3; // VertexCountDividend;

            // Do we need this clone? Can't we just remove the node directly from the graph? 
            // We do need it.

            H_NodeNeighbours = new Dictionary<string, HashSet<string>>();
            var theMappings = new Dictionary<string[], List<Mapping>>(new MappingNodesComparer());
            var inputGraphDegSeq = inputGraphClone.GetDegreeSequence(numberOfSamples);
            var queryGraphVertices = queryGraph.Vertices.ToArray();
            var subgraphSize = queryGraphVertices.Length;
            Console.WriteLine("Calling Algo 2:\n");
            for (int i = 0; i < inputGraphDegSeq.Count; i++)
            {
                var g = inputGraphDegSeq[i];
                G_NodeNeighbours = new Dictionary<string, HashSet<string>>();
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

                        //sw.Stop();
                        //Console.WriteLine("Time to do IsomorphicExtension: {0}\n", sw.Elapsed.ToString());
                        //Console.Write(".");
                        if (mappings.Count > 0)
                        {
                            //sw.Restart();

                            for (int k = mappings.Count - 1; k >= 0; k--)
                            {
                                Mapping mapping = mappings[k];
                                //Recall: f(h) = g
                                var key = mapping.Function.Values.ToArray();
                                List<Mapping> mapSet;
                                if (!theMappings.TryGetValue(key, out mapSet))
                                {
                                    theMappings[key] = new List<Mapping> { mapping };
                                }
                                else
                                {
                                    if (false == mapSet.Exists(x => x.IsIsomorphicWith(mapping, queryGraph)))
                                    {
                                        mapSet.Add(mapping);
                                    }
                                }
                                mappings.RemoveAt(k);
                            }
                            //sw.Stop();
                            //Console.WriteLine("Map: {0}.\tTime to set:\t{1:N}s.\th = {2}. g = {3}\n", mappings.Count, sw.Elapsed.ToString(), queryGraphVertices[j], inputGraphDegSeq[i]);
                            //sw = null;
                        }
                        #endregion
                    }
                }

                //Remove g
                inputGraphClone.RemoveVertex(g);
                G_NodeNeighbours.Clear();
            }

            var toReturn = new List<Mapping>(theMappings.Values.SelectMany(x => x));

            theMappings.Clear();
            inputGraphDegSeq.Clear();
            queryGraphVertices = null;
            inputGraphClone = null;
            H_NodeNeighbours.Clear();
            G_NodeNeighbours.Clear();
            Console.WriteLine("Algorithm 2: All tasks completed. Number of mappings found: {0}.", toReturn.Count);
            return toReturn;
        }
    }
}

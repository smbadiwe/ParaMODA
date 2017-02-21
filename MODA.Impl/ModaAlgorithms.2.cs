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
        internal static List<Mapping> Algorithm2(QueryGraph queryGraph, UndirectedGraph<int, Edge<int>> inputGraphClone, int numberOfSamples)
        {
            if (numberOfSamples <= 0) numberOfSamples = inputGraphClone.VertexCount / 3;

            // Do we need this clone? Can't we just remove the node directly from the graph? 
            // We do need it.

            H_NodeNeighbours = new Dictionary<int, HashSet<int>>();
            var theMappings = new Dictionary<IList<int>, Mapping>(comparer);
            var inputGraphDegSeq = inputGraphClone.GetNodesSortedByDegree(numberOfSamples);
            var queryGraphVertices = queryGraph.Vertices.ToArray();
            var subgraphSize = queryGraphVertices.Length;

            Console.WriteLine("Calling Algo 2:\n");
            for (int i = 0; i < inputGraphDegSeq.Count; i++)
            {
                var g = inputGraphDegSeq[i];
                G_NodeNeighbours = new Dictionary<int, HashSet<int>>();
                for (int j = 0; j < subgraphSize; j++)
                {
                    var h = queryGraphVertices[j];
                    if (CanSupport(queryGraph, h, inputGraphClone, g))
                    {
                        #region Can Support
                        //Remember: f(h) = g, so h is Domain and g is Range
                        var f = new Dictionary<int, int>(1);
                        f[h] = g;
                        var mappings = IsomorphicExtension(f, queryGraph, inputGraphClone);
                        
                        if (mappings.Count > 0)
                        {
                            foreach (var item in mappings)
                            {
                                //Recall: f(h) = g
                                theMappings[item.Key] = item.Value;
                            }
                            //mappings.Clear();
                            mappings = null;
                        }
                        #endregion
                    }
                }

                //Remove g
                inputGraphClone.RemoveVertex(g);
                //G_NodeNeighbours.Clear();
                G_NodeNeighbours = null;
            }

            var toReturn = new List<Mapping>(theMappings.Values);
            queryGraphVertices = null;
            inputGraphClone = null;

            //theMappings.Clear();
            theMappings = null;
            //inputGraphDegSeq.Clear();
            inputGraphDegSeq = null;
            //H_NodeNeighbours.Clear();
            H_NodeNeighbours = null;
            //G_NodeNeighbours.Clear();
            G_NodeNeighbours = null;
            Console.WriteLine("Algorithm 2: All tasks completed. Number of mappings found: {0}.", toReturn.Count);
            return toReturn;
        }
    }
}

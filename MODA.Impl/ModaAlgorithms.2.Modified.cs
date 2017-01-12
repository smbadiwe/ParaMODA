//This is the one that has gone bad
using QuickGraph;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        /// <summary>
        /// Mapping module (aka FindSubgraphInstances in Grochow & Kellis) modified
        /// The modification:
        ///     Instead of traversing all nodes in the query graph (H) for each node in the input graph (G),
        ///     we simply use just one node h in H to traverse G. This makes it much easier to parallelize 
        ///     unlike the original algorithm, and eliminate the need for removing visited g from G.
        ///     
        ///     Testing will show whether this improves, worsens or makes no difference in performance.
        /// </summary>
        /// <param name="queryGraph">H</param>
        /// <param name="inputGraph">G</param>
        /// <param name="numberOfSamples">To be decided. If not set, we use the <paramref name="inputGraph"/> size</param>
        private static List<Mapping> Algorithm2_Modified(QueryGraph queryGraph, UndirectedGraph<string, Edge<string>> inputGraph, int numberOfSamples = -1)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            if (numberOfSamples <= 0) numberOfSamples = inputGraph.VertexCount / 3; // VertexCountDividend;

            var comparer = new MappingNodesComparer();
            InputSubgraphs = new Dictionary<string[], UndirectedGraph<string, Edge<string>>>(comparer);
            MostConstrainedNeighbours = new Dictionary<string[], string>(comparer);
            NeighboursOfRange = new Dictionary<string[], HashSet<string>>(comparer);
            comparer = null;

            G_NodeNeighbours = new Dictionary<string, List<string>>();
            H_NodeNeighbours = new Dictionary<string, List<string>>();
            var theMappings = new Dictionary<string, List<Mapping>>();

            var logGist = new StringBuilder();
            logGist.AppendFormat("Calling Algo 2-Modified: Number of Iterations: {0}.\n", numberOfSamples);

            var h = queryGraph.Vertices.First();

            foreach (var g in inputGraph.GetDegreeSequence(numberOfSamples))
            {
                if (CanSupport(queryGraph, h, inputGraph, g))
                {
                    #region Can Support
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    //Remember: f(h) = g, so h is Domain and g is Range
                    //function, f = new Dictionary<string, string>(1) { { h, g } }
                    var mappings = IsomorphicExtension(new Dictionary<string, string>(1) { { h, g } }, queryGraph, inputGraph);
                    if (mappings.Count == 0) continue;

                    sw.Stop();

                    logGist.AppendFormat("Maps gotten from IsoExtension.\tTook:\t{0:N}s.\th = {1}. g = {2}\n", sw.Elapsed.ToString(), h, g);
                    sw.Restart();

                    foreach (Mapping mapping in mappings)
                    {
                        List<Mapping> mappingsToSearch; //Recall: f(h) = g
                        var g_key = mapping.Function.Last().Value;
                        if (theMappings.TryGetValue(g_key, out mappingsToSearch))
                        {
                            var existing = mappingsToSearch.Find(x => x.Equals(mapping));

                            if (existing == null)
                            {
                                theMappings[g_key].Add(mapping);
                            }
                        }
                        else
                        {
                            theMappings[g_key] = new List<Mapping> { mapping };
                        }
                        mappingsToSearch = null;
                    }

                    sw.Stop();
                    logGist.AppendFormat("Map: {0}.\tTime to set:\t{1:N}s.\th = {2}. g = {3}\n", mappings.Count, sw.Elapsed.ToString(), h, g);
                    mappings = null;
                    sw = null;
                    logGist.AppendFormat("*****************************************\n");
                    Console.WriteLine(logGist);
                    logGist.Clear();
                    #endregion
                }
            }

            var toReturn = new List<Mapping>();
            foreach (var mapping in theMappings)
            {
                toReturn.AddRange(mapping.Value);
            }
            timer.Stop();
            logGist = null;
            theMappings = null;
            InputSubgraphs = null;
            MostConstrainedNeighbours = null;
            NeighboursOfRange = null;
            G_NodeNeighbours = null;
            H_NodeNeighbours = null;
            Console.WriteLine("Algorithm 2: All tasks completed. Number of mappings found: {0}.\n", toReturn.Count, timer.Elapsed.ToString());
            timer = null;
            return toReturn;
        }

    }
}

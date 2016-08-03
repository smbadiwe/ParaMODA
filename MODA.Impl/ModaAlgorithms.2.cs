using QuickGraph;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        /// <summary>
        /// Mapping module; aka FindSubgraphInstances in Grochow & Kellis
        /// </summary>
        /// <param name="queryGraph">H</param>
        /// <param name="inputGraph">G</param>
        /// <param name="numberOfSamples">To be decided. If not set, we use the <paramref name="inputGraph"/> size</param>
        [Obsolete("Use Algorithm2_Modified instead.")]
        private List<Mapping> Algorithm2(UndirectedGraph<string, Edge<string>> queryGraph, UndirectedGraph<string, Edge<string>> inputGraph, int numberOfSamples = -1)
        {
            if (numberOfSamples <= 0) numberOfSamples = inputGraph.VertexCount / 3;
            int counter = 0;
            // Do we need this clone? Can't we just remove the node directly from the graph?
            var inputGraphClone = inputGraph.Clone();
            var theMappings = new List<Mapping>();
            var queryGraphVertices = queryGraph.Vertices.ToList();
            InputSubgraphs = new ConcurrentDictionary<string[], UndirectedGraph<string, Edge<string>>>();
            MostConstrainedNeighbours = new ConcurrentDictionary<string[], string>();
            NeighboursOfRange = new ConcurrentDictionary<string[], HashSet<string>>();
            G_NodeNeighbours = new ConcurrentDictionary<string, List<string>>();
            H_NodeNeighbours = new ConcurrentDictionary<string, List<string>>();
            foreach (var g in inputGraph.GetDegreeSequence())
            {
                //foreach (var h in queryGraph.Vertices)
                queryGraphVertices.ForEach(h =>
                {
                    if (CanSupport(queryGraph, h, inputGraphClone, g))
                    {
                        #region Can Support
                        var sw = System.Diagnostics.Stopwatch.StartNew();
                        //Remember: f(h) = g, so h is Domain and g is Range
                        var function = new Dictionary<string, string>(1) { { h, g } }; //function, f
                        var mappings = IsomorphicExtension(function, queryGraph, inputGraphClone);
                        sw.Stop();
                        Console.WriteLine("Maps gotten from IsoExtension.\tTook:\t{0:N}s.\th = {1}. g = {2}", sw.Elapsed.ToString(), h, g);
                        sw.Restart();

                        int count = mappings.Count;

                        if (theMappings.Count == 0)
                        {
                            theMappings.AddRange(mappings);
                        }
                        else
                        {
                            mappings.ForEach(map =>
                            {
                                var existing = theMappings.Find(x => x.Equals(map));

                                if (existing == null)
                                {
                                    theMappings.Add(map);
                                }
                            });
                        }
                        mappings = null;
                        function = null;
                        sw.Stop();
                        Console.WriteLine("Map: {0}.\tTime to set:\t{1:N}s.\th = {2}. g = {3}", count--, sw.Elapsed.ToString(), h, g);
                        sw = null;
                        Console.WriteLine("*****************************************\n");
                        #endregion
                    }
                }
                );

                //Remove g
                inputGraphClone.RemoveVertex(g);
                counter++;

                if (counter == numberOfSamples) break;

            }
            InputSubgraphs = null;
            MostConstrainedNeighbours = null;
            NeighboursOfRange = null;
            G_NodeNeighbours = null;
            H_NodeNeighbours = null;
            queryGraphVertices = null;
            inputGraphClone = null;
            return theMappings;
        }
        
    }
}

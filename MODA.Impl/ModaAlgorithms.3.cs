using QuickGraph;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        /// <summary>
        /// Enumeration module
        /// </summary>
        /// <param name="queryGraph"></param>
        /// <param name="inputGraph"></param>
        /// <param name="expansionTree"></param>
        private static List<Mapping> Algorithm3(UndirectedGraph<string, Edge<string>> queryGraph, UndirectedGraph<string, Edge<string>> inputGraph,
            AdjacencyGraph<ExpansionTreeNode<Edge<string>>, Edge<ExpansionTreeNode<Edge<string>>>> expansionTree)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var parentQueryGraph = GetParent(queryGraph, expansionTree); // H
            var file = Path.Combine(MapFolder, parentQueryGraph.AsString().Replace(">", "&lt;") + ".map");
            var mapObject = MySerializer.DeSerialize<Map>(File.ReadAllBytes(file));
            if (mapObject == null) return new List<Mapping>();
            var mappings = mapObject.Mappings; // foundMappings[parentQueryGraph]; //It's guaranteed to be there. If it's not, there's a prolem
            if (mappings.Count == 0) return new List<Mapping>();

            // Get the new edge in the queryGraph
            // New Edge = E(G') - E(H)
            var newEdgeNodes = new string[2];
            int index = 0;
            bool gotNewEdge = false;
            foreach (var node in queryGraph.Vertices)
            {
                //Trick: 
                // Given two graphs where one is a super-graph of the other and they only differ by one edge,
                // then we can be sure that there will be two nodes whose degrees changed.
                // These two nodes form the new edge.
                if (queryGraph.AdjacentDegree(node) == parentQueryGraph.AdjacentDegree(node)) continue;

                gotNewEdge = true;
                newEdgeNodes[index] = node;
                index++;
            }

            if (!gotNewEdge) return new List<Mapping>();

            var theNewEdge = new Edge<string>(newEdgeNodes[0], newEdgeNodes[1]);
            newEdgeNodes = null;
            
            var theMappings = new ConcurrentDictionary<string, List<Mapping>>();

            for (int i = 0; i < mappings.Count; i++)
            {
                var map = mappings[i];
                if (map.Function.Count == map.MapOnInputSubGraph.VertexCount)
                {
                    // Reember, f(h) = g
                    // Remember, map.InputSubGraph is a subgraph of inputGraph
                    Edge<string> edge;
                    if (map.InputSubGraph.TryGetEdge(map.Function[theNewEdge.Source], map.Function[theNewEdge.Target], out edge))
                    {
                        var mapping = new Mapping(map.Function)
                        {
                            InputSubGraph = map.InputSubGraph,
                            MapOnInputSubGraph = map.InputSubGraph
                        };
                        List<Mapping> mappingsToSearch; //Recall: f(h) = g
                        var g_key = mapping.Function.Last().Value;
                        if (theMappings.TryGetValue(g_key, out mappingsToSearch) && mappingsToSearch != null)
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
                        mapping = null;
                        mappingsToSearch = null;
                    }
                }
            }

            var toReturn = new List<Mapping>();
            foreach (var mapping in theMappings)
            {
                toReturn.AddRange(mapping.Value);
            }
            timer.Stop();
            Console.WriteLine("Algorithm 3: All tasks completed. Number of mappings found: {0}.\nTotal time taken: {1}", theMappings.Count, timer.Elapsed.ToString());
            
            timer = null;
            theMappings = null;
            return toReturn;
        }

        /// <summary>
        /// Enumeration module
        /// </summary>
        /// <param name="queryGraph"></param>
        /// <param name="inputGraph"></param>
        /// <param name="expansionTree"></param>
        private static List<Mapping> Algorithm3_Parallelized(UndirectedGraph<string, Edge<string>> queryGraph, UndirectedGraph<string, Edge<string>> inputGraph,
            AdjacencyGraph<ExpansionTreeNode<Edge<string>>, Edge<ExpansionTreeNode<Edge<string>>>> expansionTree)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var parentQueryGraph = GetParent(queryGraph, expansionTree); // H
            var file = Path.Combine(MapFolder, parentQueryGraph.AsString().Replace(">", "&lt;") + ".map");
            var mapObject = MySerializer.DeSerialize<Map>(File.ReadAllBytes(file));
            if (mapObject == null) return new List<Mapping>();
            var mappings = mapObject.Mappings; // foundMappings[parentQueryGraph]; //It's guaranteed to be there. If it's not, there's a prolem
            if (mappings.Count == 0) return new List<Mapping>();

            // Get the new edge in the queryGraph
            // New Edge = E(G') - E(H)
            var newEdgeNodes = new string[2];
            int index = 0;
            bool gotNewEdge = false;
            foreach (var node in queryGraph.Vertices)
            {
                //Trick: 
                // Given two graphs where one is a super-graph of the other and they only differ by one edge,
                // then we can be sure that there will be two nodes whose degrees changed.
                // These two nodes form the new edge.
                if (queryGraph.AdjacentDegree(node) == parentQueryGraph.AdjacentDegree(node)) continue;

                gotNewEdge = true;
                newEdgeNodes[index] = node;
                index++;
            }

            if (!gotNewEdge) return new List<Mapping>();

            var theNewEdge = new Edge<string>(newEdgeNodes[0], newEdgeNodes[1]);
            newEdgeNodes = null;
            var tasks = new List<Task>();
            List<Mapping>[] chunks = new List<Mapping>[mappings.Count < 40 ? 1 : Math.Min(mappings.Count / 40, 25)];
            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i] = new List<Mapping>();
            }
            int iter = 0;
            foreach (var v in mappings)
            {
                chunks[iter % chunks.Length].Add(v);
                iter++;
            }
            iter = 0;
            var theMappings = new ConcurrentDictionary<string, List<Mapping>>();

            foreach (var mappingsChunk in chunks)
            {
                tasks.Add(Task.Factory.StartNew((objects) =>
                {
                    var objectsSet = objects as object[];
                    var mappingsChunk_ = objectsSet[0] as Mapping[];
                    var theNewEdge_ = objectsSet[1] as Edge<string>;
                    var mappingsFound = new List<Mapping>();
                    for (int i = 0; i < mappingsChunk_.Length; i++)
                    {
                        var map = mappingsChunk_[i];
                        if (map.Function.Count == map.MapOnInputSubGraph.VertexCount)
                        {
                            // Reember, f(h) = g
                            // Remember, map.InputSubGraph is a subgraph of inputGraph
                            Edge<string> edge;
                            if (map.InputSubGraph.TryGetEdge(map.Function[theNewEdge_.Source], map.Function[theNewEdge_.Target], out edge))
                            {
                                mappingsFound.Add(new Mapping(map.Function)
                                {
                                    InputSubGraph = map.InputSubGraph,
                                    MapOnInputSubGraph = map.InputSubGraph
                                });
                            }
                        }
                    }

                    foreach (Mapping mapping in mappingsFound)
                    {
                        List<Mapping> mappingsToSearch; //Recall: f(h) = g
                        var g_key = mapping.Function.Last().Value;
                        if (theMappings.TryGetValue(g_key, out mappingsToSearch) && mappingsToSearch != null)
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

                }, new object[] { mappingsChunk.ToArray(), theNewEdge }));
            }
            Task.WaitAll(tasks.ToArray());
            var toReturn = new List<Mapping>();
            foreach (var mapping in theMappings)
            {
                toReturn.AddRange(mapping.Value);
            }
            timer.Stop();
            Console.WriteLine("Algorithm 3: All {2} tasks completed. Number of mappings found: {0}.\nTotal time taken: {1}", theMappings.Count, timer.Elapsed.ToString(), tasks.Count);

            tasks = null;
            timer = null;
            theMappings = null;
            return toReturn;
        }

        /// <summary>
        /// Helper method for algorithm 3
        /// </summary>
        /// <param name="queryGraph"></param>
        /// <param name="expansionTree"></param>
        /// <returns></returns>
        private static UndirectedGraph<string, Edge<string>> GetParent(UndirectedGraph<string, Edge<string>> queryGraph, AdjacencyGraph<ExpansionTreeNode<Edge<string>>, Edge<ExpansionTreeNode<Edge<string>>>> expansionTree)
        {
            var hasNode = expansionTree.ContainsVertex(new ExpansionTreeNode<Edge<string>>
            {
                QueryGraph = queryGraph,
            });
            if (hasNode)
            {
                return expansionTree.Vertices.First(x => !x.IsRootNode && x.QueryGraph == queryGraph).ParentNode.QueryGraph;
            }
            return null;
        }

    }
}

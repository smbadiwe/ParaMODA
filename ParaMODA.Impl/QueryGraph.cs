using QuickGraph;
using System.Collections.Generic;
using System.Linq;

namespace ParaMODA.Impl
{
    public sealed class QueryGraph : UndirectedGraph<int>
    {
        //NB: Don't bother overriding .Equals and .GetHashCode

        public QueryGraph(string label) : base()
        {
            Identifier = label;
        }

        public QueryGraph(string label, bool allowParralelEdges) : base(allowParralelEdges)
        {
            Identifier = label;
        }

        /// <summary>
        /// A name to identify / refer to this query graph
        /// </summary>
        public string Identifier { get; }

        public bool IsFrequentSubgraph { get; set; }

        public bool IsComplete(int subgraphSize = -1)
        {
            if (subgraphSize <= 1) subgraphSize = VertexCount;
            return EdgeCount == ((subgraphSize * (subgraphSize - 1)) / 2);
        }

        public bool IsTree(int subgraphSize = -1)
        {
            if (subgraphSize <= 1) subgraphSize = VertexCount;
            return EdgeCount == (subgraphSize - 1);
        }

        public IList<Mapping> ReadMappingsFromFile(string filename)
        {
            var mappings = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Mapping>>(Extensions.DecompressString(System.IO.File.ReadAllText(filename)));
            return mappings;
        }

        /// <summary>
        /// Write mappings to disk under this query graph. Returns the filename where it is written.
        /// The filename format used is this: $"{mappings.Count}#{Label}.ser"
        /// </summary>
        /// <param name="mappings"></param>
        /// <returns>filename where it is written</returns>
        public string WriteMappingsToFile(ICollection<Mapping> mappings)
        {
            var fileName = $"{mappings.Count}#{Identifier}.ser";
            System.IO.File.WriteAllText(fileName, Extensions.CompressString(Newtonsoft.Json.JsonConvert.SerializeObject(mappings)));
            return fileName;
        }

        public void RemoveNonApplicableMappings(ICollection<Mapping> mappings, UndirectedGraph<int> inputGraph, bool checkInducedMappingOnly = true)
        {
            if (mappings.Count < 2) return;

            int subgraphSize = VertexCount;
            var mapGroups = mappings.GroupBy(x => x.Function.Values, ModaAlgorithms.MappingNodesComparer); //.ToDictionary(x => x.Key, x => x.ToArray());
            
            var toAdd = new List<Mapping>();
            var queryGraphEdges = Edges.ToArray();
            foreach (var group in mapGroups)
            {
                var g_nodes = group.Key; // Remember, f(h) = g, so .Values is for g's
                // Try to get all the edges in the induced subgraph made up of these g_nodes
                var inducedSubGraphEdges = new List<Edge<int>>();
                for (int i = 0; i < subgraphSize - 1; i++)
                {
                    for (int j = (i + 1); j < subgraphSize; j++)
                    {
                        Edge<int> edge_g;
                        if (inputGraph.TryGetEdge(g_nodes[i], g_nodes[j], out edge_g))
                        {
                            inducedSubGraphEdges.Add(edge_g);
                        }
                    }
                }

                var subgraph = new UndirectedGraph<int>();
                subgraph.AddVerticesAndEdgeRange(inducedSubGraphEdges);
                foreach (var item in group)
                {
                    var result = Utils.IsMappingCorrect2(item.Function, subgraph, queryGraphEdges, checkInducedMappingOnly);
                    if (result.IsCorrectMapping)
                    {
                        toAdd.Add(item);
                        result = null;
                        break;
                    }
                    result = null;
                }
                subgraph = null;
                inducedSubGraphEdges.Clear();
                inducedSubGraphEdges = null;
            }
            mapGroups = null;
            mappings.Clear();
            if (toAdd.Count > 0)
            {
                foreach (var item in toAdd)
                {
                    mappings.Add(item);
                }
                toAdd.Clear();
            }
            toAdd = null;
        }
    }
}

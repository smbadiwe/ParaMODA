using QuickGraph;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MODA.Impl
{
    public sealed class Mapping
    {
        public Mapping(Dictionary<string, string> function)
        {
            Function = function;
            InducedSubGraph = new UndirectedGraph<string, Edge<string>>();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// This represents the [f(h) = g] relation. Meaning key is h and value is g.
        /// </summary>
        public Dictionary<string, string> Function { get; private set; }

        /// <summary>
        /// The subgraph (with all edges) in the input graph G that fit the query graph (---Function.Keys)
        /// </summary>
        public UndirectedGraph<string, Edge<string>> InducedSubGraph { get; set; }

        /// <summary>
        /// Returns the edge in <paramref="currentQueryGraph"/>'s image that is not present in <paramref="parentQueryGraph"/>'s image
        /// </summary>
        /// <param name="currentQueryGraph">The current subgraph being queried</param>
        /// <param name="parentQueryGraph">The parent to <paramref name="currentQueryGraph"/>. This parent is also a subset, meaning it has one edge less.</param>
        /// <returns></returns>
        public Edge<string> GetEdgeDifference(QueryGraph currentQueryGraph, QueryGraph parentQueryGraph)
        {
            if (InducedSubGraph.EdgeCount == currentQueryGraph.EdgeCount)
            {
                var edgeImages = parentQueryGraph.Edges.Select(x => new Edge<string>(Function[x.Source], Function[x.Target]));
                foreach (var edge in InducedSubGraph.Edges)
                {
                    if (!edgeImages.Contains(edge))
                    {
                        return edge;
                    }
                }
            }
            else if (InducedSubGraph.EdgeCount > currentQueryGraph.EdgeCount)
            {
                var edges = parentQueryGraph.Edges.Select(x => x);
                foreach (var edge in currentQueryGraph.Edges)
                {
                    if (!edges.Contains(edge))
                    {
                        Edge<string> edgeImage;
                        if (InducedSubGraph.TryGetEdge(Function[edge.Source], Function[edge.Target], out edgeImage))
                        {
                            return edgeImage;
                        }
                        return null;
                    }
                }
            }
            return null;
        }

        public bool IsIsomorphicWith(Mapping otherMapping, QueryGraph queryGraph)
        {
            //NB: Node count is guaranteed to be same for both this and other mapping
            // Test 0 - Edge count sameness
            if (InducedSubGraph.EdgeCount != otherMapping.InducedSubGraph.EdgeCount)
            {
                return false;
            }

            // Test 1 - Vertices sameness
            foreach (var node in InducedSubGraph.Vertices)
            {
                if (!otherMapping.InducedSubGraph.ContainsVertex(node)) //Remember, f(h) = g. So, key is h and value is g
                {
                    return false;
                }
            }

            // Since nodes are same for both mappings, the InducedSubgraphs must be same at this point
            if (InducedSubGraph.EdgeCount < queryGraph.EdgeCount)
            {
                // check if the two are same
                string[] mapSequence, otherMapSequence;
                var thisSequence = GetStringifiedMapSequence(out mapSequence);
                var otherSequence = otherMapping.GetStringifiedMapSequence(out otherMapSequence);
                if (thisSequence == otherSequence)
                {
                    return true;
                }

                // check if one is a reversed reading of the other
                bool isIso = true;
                int index = mapSequence.Length;
                for (int i = 0; i < index; i++)
                {
                    if (mapSequence[i] != otherMapSequence[index - i - 1])
                    {
                        isIso = false;
                        break;
                    }
                }
                if (isIso)
                {
                    return true;
                }

                // compare corresponding edges
                foreach (var edge in queryGraph.Edges)
                {
                    var edgeImage = new Edge<string>(Function[edge.Source], Function[edge.Target]);
                    var otherEdgeImage = new Edge<string>(otherMapping.Function[edge.Source], otherMapping.Function[edge.Target]);
                    if (edgeImage != otherEdgeImage)
                    {
                        return false;
                    }
                }
            }

            // let it go
            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var functionSorted = new SortedDictionary<string, string>(Function);
            sb.Append("[");
            foreach (var item in functionSorted)
            {
                sb.AppendFormat("{0}-", item.Key);
            }
            sb.Append("] => [");
            foreach (var item in functionSorted)
            {
                sb.AppendFormat("{0}-", item.Value);
            }
            sb.Append("]\n");
            return sb.ToString();
        }

        private string GetStringifiedMapSequence(out string[] mapSequence)
        {
            var sb = new StringBuilder();
            mapSequence = new string[Function.Count];
            var functionSorted = new SortedDictionary<string, string>(Function);
            int index = 0;
            foreach (var item in functionSorted)
            {
                mapSequence[index] = item.Value;
                sb.AppendFormat("{0}|", item.Value);
                index++;
            }
            return sb.ToString();
        }

    }
}

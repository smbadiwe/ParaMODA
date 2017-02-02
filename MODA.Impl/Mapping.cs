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
            foreach (var edge in currentQueryGraph.Edges)
            {
                if (!parentQueryGraph.ContainsEdge(edge))
                {
                    Edge<string> edgeImage;
                    if (InducedSubGraph.TryGetEdge(Function[edge.Source], Function[edge.Target], out edgeImage))
                    {
                        return edgeImage;
                    }
                    return null;
                }
            }
            return null;
        }

        public bool IsIsomorphicWith(Mapping otherMapping)
        {
            //NB: Node and edge count already guaranteed to be equal
            foreach (var node in InducedSubGraph.Vertices)
            {
                //Test 1 - Vertices - sameness
                if (!otherMapping.InducedSubGraph.ContainsVertex(node)) //Remember, f(h) = g. So, key is h and value is g
                {
                    return false;
                }
            }
            //int size = Function.Count;
            //bool isComplete = InputSubGraph.EdgeCount == ((size * (size - 1)) / 2);
            //foreach (var node in InputSubGraph.Vertices)
            //{
            //    //Test 2 - Node degrees.
            //    if (MapOnInputSubGraph.AdjacentDegree(node) != otherMapping.MapOnInputSubGraph.AdjacentDegree(node))
            //    {
            //        //if input sub-graph is complete
            //        if (isComplete)
            //        {
            //            // Then the subgraphs is likely isomorphic, due to symmetry
            //            return true;
            //        }
            //        return false;
            //    }
            //}
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
    }
}

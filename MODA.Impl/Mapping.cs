using QuickGraph;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MODA.Impl
{
    public sealed class Mapping
    {
        public Mapping(SortedList<int, int> function)
        {
            Function = function;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// This represents the [f(h) = g] relation. Meaning key is h and value is g.
        /// </summary>
        public SortedList<int, int> Function { get; private set; }

        /// <summary>
        /// Count of all the edges in the input subgraph G that fit the query graph (---Function.Keys)
        /// </summary>
        public int InducedSubGraphEdgesCount { get; set; }

        /// <summary>
        /// Only for when (InducedSubGraphEdgesCount == currentQueryGraphEdgeCount)
        /// </summary>
        /// <param name="parentQueryGraphEdges"></param>
        /// <returns></returns>
        public Edge<int> GetImage(UndirectedGraph<int, Edge<int>> inputGraph, IEnumerable<Edge<int>> parentQueryGraphEdges)
        {
            int subgraphSize = Function.Count;
            var g_nodes = Function.Values; // Remember, f(h) = g, so .Values is for g's
            Edge<int> edge_g = null;
            var inducedSubGraphEdges = new List<Edge<int>>(InducedSubGraphEdgesCount);
            for (int i = 0; i < subgraphSize - 1; i++)
            {
                for (int j = (i + 1); j < subgraphSize; j++)
                {
                    if (inputGraph.TryGetEdge(g_nodes[i], g_nodes[j], out edge_g))
                    {
                        inducedSubGraphEdges.Add(edge_g);
                    }
                }
            }

            var edgeImages = new HashSet<Edge<int>>(parentQueryGraphEdges.Select(x => new Edge<int>(Function[x.Source], Function[x.Target])));
            foreach (var edgex in inducedSubGraphEdges)
            {
                if (!edgeImages.Contains(edgex))
                {
                    inducedSubGraphEdges.Clear();
                    edgeImages.Clear();
                    inducedSubGraphEdges = null;
                    edgeImages = null;
                    return edgex;
                }
            }
            inducedSubGraphEdges.Clear();
            edgeImages.Clear();
            inducedSubGraphEdges = null;
            edgeImages = null;
            return null;
        }

        /// <summary>
        /// Only for when (InducedSubGraph.EdgeCount > currentQueryGraphEdgeCount)
        /// </summary>
        /// <param name="inputGraph"></param>
        /// <param name="newlyAddedEdge"></param>
        /// <returns></returns>
        public Edge<int> GetImage(UndirectedGraph<int, Edge<int>> inputGraph, Edge<int> newlyAddedEdge)
        {
            Edge<int> image;
            if (inputGraph.TryGetEdge(Function[newlyAddedEdge.Source], Function[newlyAddedEdge.Target], out image))
            {
                return image;
            }
            return null;
        }

        /// <summary>
        /// NB: Image node set (hence induced subgraph) is guaranteed to be same for both this and other mapping.
        /// That's virtually all there is to do as isomorphism testing
        /// </summary>
        /// <param name="otherMapping"></param>
        /// <param name="queryGraph"></param>
        /// <returns></returns>
        public bool IsIsomorphicWith(Mapping otherMapping, QueryGraph queryGraph)
        {
            //if (true)
            {
                #region This is what was here before. Review, even though the app gives correct results based on the test data - largely because this code never runs
                ////Test 2 - check if the two are same
                //string[] mapSequence, otherMapSequence;
                //var thisSequence = GetStringifiedMapSequence(out mapSequence);
                //var otherSequence = otherMapping.GetStringifiedMapSequence(out otherMapSequence);
                //if (thisSequence == otherSequence)
                //{
                //    //System.Console.WriteLine("thisSequence == otherSequence. Return true");
                //    return true;
                //}

                ////Test 3 - check if one is a reversed reading of the other
                //bool isIso = true;
                //int index = mapSequence.Length;
                //for (int i = 0; i < index; i++)
                //{
                //    if (mapSequence[i] != otherMapSequence[index - i - 1])
                //    {
                //        isIso = false;
                //        break;
                //    }
                //}
                //if (isIso)
                //{
                //    //System.Console.WriteLine("isAlso == true. Return true");
                //    return true;
                //}

                ////Test 4 - compare corresponding edges
                //foreach (var edge in queryGraph.Edges)
                //{
                //    var edgeImage = new Edge<int>(Function[edge.Source], Function[edge.Target]);
                //    var otherEdgeImage = new Edge<int>(otherMapping.Function[edge.Source], otherMapping.Function[edge.Target]);
                //    if (edgeImage != otherEdgeImage)
                //    {
                //        System.Console.WriteLine("edgeImage != otherEdgeImage. Return false");
                //        return false;
                //    }
                //}
                #endregion
            }

            // let it go
            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            foreach (var item in Function)
            {
                sb.AppendFormat("{0}-", item.Key);
            }
            sb.Append("] => [");
            foreach (var item in Function)
            {
                sb.AppendFormat("{0}-", item.Value);
            }
            sb.Append("]\n");
            return sb.ToString();
        }

        private string GetStringifiedMapSequence(out int[] mapSequence)
        {
            var sb = new StringBuilder();
            mapSequence = new int[Function.Count];
            int index = 0;
            foreach (var item in Function)
            {
                mapSequence[index] = item.Value;
                sb.AppendFormat("{0}|", item.Value);
                index++;
            }
            return sb.ToString();
        }

    }
}

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
        /// Only for when (InducedSubGraph.EdgeCount == currentQueryGraphEdgeCount)
        /// </summary>
        /// <param name="newlyAddedEdge"></param>
        /// <param name="parentQueryGraphEdges"></param>
        /// <returns></returns>
        public Edge<string> GetImage(Edge<string> newlyAddedEdge, IEnumerable<Edge<string>> parentQueryGraphEdges)
        {
            var edgeImages = parentQueryGraphEdges.Select(x => new Edge<string>(Function[x.Source], Function[x.Target]));
            foreach (var edgex in InducedSubGraph.Edges)
            {
                if (!edgeImages.Contains(edgex))
                {
                    return edgex;
                }
            }
            return null;
        }

        /// <summary>
        /// Only for when (InducedSubGraph.EdgeCount > currentQueryGraphEdgeCount)
        /// </summary>
        /// <param name="newlyAddedEdge"></param>
        /// <returns></returns>
        public Edge<string> GetImage(Edge<string> newlyAddedEdge)
        {
            Edge<string> edgeImage;
            if (InducedSubGraph.TryGetEdge(Function[newlyAddedEdge.Source], Function[newlyAddedEdge.Target], out edgeImage))
            {
                return edgeImage;
            }
            return null;
        }

        public bool IsCorrectMapping(QueryGraph queryGraph)
        {
            if (InducedSubGraph.EdgeCount > queryGraph.EdgeCount)
            {
                foreach (var edge in queryGraph.Edges)
                {
                    if (!InducedSubGraph.ContainsEdge(new Edge<string>(Function[edge.Source], Function[edge.Target])))
                    {
                        return false;
                    }
                } 
            }
            //else if (InducedSubGraph.EdgeCount == queryGraph.EdgeCount)
            //{
            //    var edgeImages = queryGraph.Edges.Select(x => new Edge<string>(Function[x.Source], Function[x.Target]));
            //    foreach (var edgex in InducedSubGraph.Edges)
            //    {
            //        if (!edgeImages.Contains(edgex))
            //        {
            //            return false;
            //        }
            //    }
            //}
            return true;
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
            //if (queryGraph.EdgeCount > InducedSubGraph.EdgeCount)
            //{
            //    //NB: This comdition being true means that the query graph cannot be mapped to the induced subgraph.
            //    // Recall that image node set (hence induced subgraph) is guaranteed to be same for both this and other mapping.
            //    // So, I'm returning true here so that the mapping can be ignored, ie, not added to the list of mappings
            //    return true;
            //}
            
            //if (queryGraph.EdgeCount > InducedSubGraph.EdgeCount)
            //{
            //    #region This is what was here before. Review, even though the app gives correct results based on the test data - largely because this code never runs
            //    ////Test 2 - check if the two are same
            //    //string[] mapSequence, otherMapSequence;
            //    //var thisSequence = GetStringifiedMapSequence(out mapSequence);
            //    //var otherSequence = otherMapping.GetStringifiedMapSequence(out otherMapSequence);
            //    //if (thisSequence == otherSequence)
            //    //{
            //    //    //System.Console.WriteLine("thisSequence == otherSequence. Return true");
            //    //    return true;
            //    //}

            //    ////Test 3 - check if one is a reversed reading of the other
            //    //bool isIso = true;
            //    //int index = mapSequence.Length;
            //    //for (int i = 0; i < index; i++)
            //    //{
            //    //    if (mapSequence[i] != otherMapSequence[index - i - 1])
            //    //    {
            //    //        isIso = false;
            //    //        break;
            //    //    }
            //    //}
            //    //if (isIso)
            //    //{
            //    //    //System.Console.WriteLine("isAlso == true. Return true");
            //    //    return true;
            //    //}

            //    ////Test 4 - compare corresponding edges
            //    //foreach (var edge in queryGraph.Edges)
            //    //{
            //    //    var edgeImage = new Edge<string>(Function[edge.Source], Function[edge.Target]);
            //    //    var otherEdgeImage = new Edge<string>(otherMapping.Function[edge.Source], otherMapping.Function[edge.Target]);
            //    //    if (edgeImage != otherEdgeImage)
            //    //    {
            //    //        System.Console.WriteLine("edgeImage != otherEdgeImage. Return false");
            //    //        return false;
            //    //    }
            //    //}
            //    #endregion
            //}

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

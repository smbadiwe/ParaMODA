using QuickGraph;
using System.Collections.Generic;
using System.Text;

namespace ParaMODA.Impl
{
    public class Mapping
    {
        public Mapping()
        {
            Id = -1;
        }

        public Mapping(Mapping mapping)
        {
            Function = mapping.Function;
            Id = mapping.Id;
            SubGraphEdgeCount = mapping.SubGraphEdgeCount;
        }

        //NB: SortedDictionary proved to be much worse for me
        public Mapping(SortedList<int, int> function, int subGraphEdgeCount)
        {
            Function = function;
            Id = -1;
            SubGraphEdgeCount = subGraphEdgeCount;
        }

        /// <summary>
        /// Usage is temporary - to help organize stuffs in Algorithm 3
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// This represents the [f(h) = g] relation. Meaning key is h and value is g.
        /// </summary>
        public SortedList<int, int> Function { get; set; }

        /// <summary>
        /// Count of all the edges in the input subgraph G that fit the query graph (---Function.Keys).
        /// This count is for the induced subgraph
        /// </summary>
        public int SubGraphEdgeCount { get; set; }

        /// <summary>
        /// Gets the corresponding image of the <see cref="newlyAddedEdge"/> in the <see cref="inputGraph"/>.
        /// Use only for when (InducedSubGraph.EdgeCount > currentQueryGraphEdgeCount)
        /// </summary>
        /// <param name="inputGraph"></param>
        /// <param name="newlyAddedEdge"></param>
        /// <returns></returns>
        public Edge<int> GetImage(UndirectedGraph<int> inputGraph, Edge<int> newlyAddedEdge)
        {
            Edge<int> image;
            if (inputGraph.TryGetEdge(Function[newlyAddedEdge.Source], Function[newlyAddedEdge.Target], out image))
            {
                return image;
            }
            return new Edge<int>(Utils.DefaultEdgeNodeVal, Utils.DefaultEdgeNodeVal);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Mapping;
            if (other == null) return false;

            if (Id >= 0 || other.Id >= 0) return Id == other.Id;

            int i = 0;
            foreach (var func in Function)
            {
                if (other.Function.Keys[i] == func.Key && other.Function.Values[i] == func.Value)
                {
                    i++;
                    continue;
                }
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            if (Id >= 0) return Id.GetHashCode();

            return base.GetHashCode();
        }

        /// <summary>
        /// Retruns the nodes in the input graph that the query graph mapped to.
        /// It's returned as a string where the nodes values are concatenated with '-'
        /// </summary>
        /// <returns></returns>
        public string GetMappedNodes()
        {
            return string.Join("-", Function.Values);
            //var sb = new StringBuilder();
            //foreach (var item in Function)
            //{
            //    sb.AppendFormat("{0}-", item.Value);
            //}
            //sb.Remove(sb.Length - 1, 1);
            //return sb.ToString();
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
    }
}

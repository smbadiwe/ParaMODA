using QuickGraph;
using System;
using System.Collections.Generic;
using System.Text;

namespace MODA.Impl
{
    [Serializable]
    public class Mapping
    {
        public Mapping(Dictionary<string, string> function)
        {
            Function = function;
        }

        public Dictionary<string, string> Function { get; private set; }

        /// <summary>
        /// The subgraph (with all edges) in the input graph G that fit the query graph (---Function.Values)
        /// </summary>
        public UndirectedGraph<string, Edge<string>> InputSubGraph { get; set; } = new UndirectedGraph<string, Edge<string>>(false);

        /// <summary>
        /// The subgraph (with mapped edges) in the input graph G that fit the query graph (---Function.Keys)
        /// </summary>
        public UndirectedGraph<string, Edge<string>> MapOnInputSubGraph { get; set; } = new UndirectedGraph<string, Edge<string>>(false);

        public override bool Equals(object obj)
        {
            //Test 0 - basic object test
            if (obj == null) return false;
            var other = obj as Mapping;
            if (other == null) return false;

            //Test 1 - Vertices
            var test = (new HashSet<string>(this.Function.Keys).SetEquals(other.Function.Keys)
                && new HashSet<string>(this.Function.Values).SetEquals(other.Function.Values));
            if (test == false)
            {
                return false;
            }

            //Test 2 - Edge count
            test = this.MapOnInputSubGraph.EdgeCount == other.MapOnInputSubGraph.EdgeCount;
            if (test == false)
            {
                return false;
            }

            //Test 3 - Node degrees. 
            //  This seems to be giving us isomorphic graphs; as in, same graph cast as diferent.
            //  So it may be nice to take it out.

            //var any = MapOnInputSubGraph.Vertices.ToList().Find(node =>
            //        this.MapOnInputSubGraph.AdjacentDegree(node) != other.MapOnInputSubGraph.AdjacentDegree(node));

            //if (any != null) return false;
            //foreach (var node in MapOnInputSubGraph.Vertices)
            //{
            //    if (this.MapOnInputSubGraph.AdjacentDegree(node) != other.MapOnInputSubGraph.AdjacentDegree(node))
            //    {
            //        return false;
            //    }
            //}

            return true;
        }

        /// <summary>
        /// With the way the code is now, you don't need this; but if you're
        /// going to do something with this object, see if you need to provide
        /// implementation.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            foreach (var item in this.Function)
            {
                sb.AppendFormat("{0}-", item.Key);
            }
            //sb.AppendFormat("<{0}>] => [", this.MapOnInputSubGraph.AsString());
            sb.Append("] => [");
            foreach (var item in this.Function)
            {
                sb.AppendFormat("{0}-", item.Value);
            }
            sb.AppendFormat("] Exact map: <{0}>\n", this.MapOnInputSubGraph.AsString());
            //sb.AppendLine("]");
            return sb.ToString();
        }
    }
}

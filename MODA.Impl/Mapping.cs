using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl
{
    public class Mapping<TVertex>
    {
        public Mapping(Dictionary<TVertex, TVertex> function)
        {
            Function = function;
        }

        public Dictionary<TVertex, TVertex> Function { get; }

        /// <summary>
        /// The subgraph (with all edges) in the input graph G that fit the query graph (---Function.Values)
        /// </summary>
        public UndirectedGraph<TVertex, Edge<TVertex>> InputSubGraph { get; set; }

        /// <summary>
        /// The subgraph (with mapped edges) in the input graph G that fit the query graph (---Function.Keys)
        /// </summary>
        public UndirectedGraph<TVertex, Edge<TVertex>> MapOnInputSubGraph { get; set; }
        
        public override bool Equals(object obj)
        {
            //Test 0 - basic object test
            if (obj == null) return false;
            var other = obj as Mapping<TVertex>;
            if (other == null) return false;

            //Test 1 - Vertices
            var test = (new HashSet<TVertex>(this.Function.Keys).SetEquals(other.Function.Keys)
                && new HashSet<TVertex>(this.Function.Values).SetEquals(other.Function.Values));
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
            foreach (var node in InputSubGraph.Vertices)
            {
                if (this.MapOnInputSubGraph.AdjacentDegree(node) != other.MapOnInputSubGraph.AdjacentDegree(node))
                {
                    return false;
                }
            }
            return true;
        }

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

using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;
using MODA.Impl;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl.Transformers
{
    public class NbrDegreeTransformer<TVertex> : Transformer<TVertex, List<int>>
    {
        private UndirectedGraph<TVertex, Edge<TVertex>> graph;
        public NbrDegreeTransformer(UndirectedGraph<TVertex, Edge<TVertex>> graph)
        {
            this.graph = graph;
        }

        public List<int> transform(TVertex input)
        {
            List<int> nbrDegs = new List<int>();
            foreach (TVertex n in graph.getNeighbors(input))
            {
                nbrDegs.Add(graph.AdjacentDegree(n));
            }
            return nbrDegs.OrderBy(x => x).ToList();
        }
    }
}

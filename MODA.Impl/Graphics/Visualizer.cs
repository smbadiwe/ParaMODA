using QuickGraph;
using QuickGraph.Graphviz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl.Graphics
{
    public static class Visualizer
    {
        //IVertexAndEdgeListGraph

        public static void Visualize<TVertex, TEdge>(this IEdgeListGraph<TVertex, TEdge> graph,
            string outputFullFileName)
            where TEdge : IEdge<TVertex>
        {
            Visualize(graph, outputFullFileName, NoOpEdgeFormatter);
        }

        public static void Visualize<TVertex, TEdge>(this IEdgeListGraph<TVertex, TEdge> graph,
            string outputFullFileName, FormatEdgeAction<TVertex, TEdge> edgeFormatter)
            where TEdge : IEdge<TVertex>
        {
            var viz = new GraphvizAlgorithm<TVertex, TEdge>(graph);

            viz.FormatVertex += VizFormatVertex;

            viz.FormatEdge += edgeFormatter;

            viz.Generate(new FileDotEngine(), outputFullFileName);
        }

        static void NoOpEdgeFormatter<TVertex, TEdge>(object sender, FormatEdgeEventArgs<TVertex, TEdge> e)
            where TEdge : IEdge<TVertex>
        {
            // noop
            //e.EdgeFormatter.Label.Value = e.Edge.Label;
        }

        public static string ToDotNotation<TVertex, TEdge>(this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var viz = new GraphvizAlgorithm<TVertex, TEdge>(graph);
            viz.FormatVertex += VizFormatVertex;
            return viz.Generate(new DotPrinter(), "");
        }

        static void VizFormatVertex<TVertex>(object sender, FormatVertexEventArgs<TVertex> e)
        {
            e.VertexFormatter.Label = e.Vertex.ToString();
            //e.VertexFormatter.Comment = e.Vertex.Label;
        }
    }
}

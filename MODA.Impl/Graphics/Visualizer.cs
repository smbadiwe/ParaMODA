using QuickGraph;
using QuickGraph.Graphviz;

namespace MODA.Impl.Graphics
{
    public static class Visualizer
    {
        //IVertexAndEdgeListGraph

        public static void Visualize<TEdge>(this IEdgeListGraph<string, TEdge> graph,
            string outputFullFileName)
            where TEdge : IEdge<string>
        {
            Visualize(graph, outputFullFileName, NoOpEdgeFormatter);
        }

        public static void Visualize<TEdge>(this IEdgeListGraph<string, TEdge> graph,
            string outputFullFileName, FormatEdgeAction<string, TEdge> edgeFormatter)
            where TEdge : IEdge<string>
        {
            var viz = new GraphvizAlgorithm<string, TEdge>(graph);

            viz.FormatVertex += VizFormastring;

            viz.FormatEdge += edgeFormatter;

            viz.Generate(new FileDotEngine(), outputFullFileName);
        }

        static void NoOpEdgeFormatter<TEdge>(object sender, FormatEdgeEventArgs<string, TEdge> e)
            where TEdge : IEdge<string>
        {
            // noop
            //e.EdgeFormatter.Label.Value = e.Edge.Label;
        }

        public static string ToDotNotation<TEdge>(this IVertexAndEdgeListGraph<string, TEdge> graph)
            where TEdge : IEdge<string>
        {
            var viz = new GraphvizAlgorithm<string, TEdge>(graph);
            viz.FormatVertex += VizFormastring;
            return viz.Generate(new DotPrinter(), "");
        }

        static void VizFormastring(object sender, FormatVertexEventArgs<string> e)
        {
            e.VertexFormatter.Label = e.Vertex.ToString();
            //e.VertexFormatter.Comment = e.Vertex.Label;
        }
    }
}

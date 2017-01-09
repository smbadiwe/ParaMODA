using QuickGraph;
using QuickGraph.Graphviz;

namespace MODA.Impl.Graphics
{
    public static class Visualizer
    {
        #region 'ExpansionTreeNode<Edge<string>> as TVertex
        public static void Visualize<TVertex, TEdge>(this IEdgeListGraph<TVertex, TEdge> graph,
            string dotProgramLocation, string outputFullFileName)
            where TEdge : IEdge<TVertex>
            where TVertex : ExpansionTreeNode<Edge<string>>
        {
            Visualize(graph, dotProgramLocation, outputFullFileName, NoOpEdgeFormatter);
        }

        public static void Visualize<TVertex, TEdge>(this IEdgeListGraph<TVertex, TEdge> graph,
            string dotProgramLocation, string outputFullFileName, FormatEdgeAction<TVertex, TEdge> edgeFormatter)
            where TEdge : IEdge<TVertex>
            where TVertex : ExpansionTreeNode<Edge<string>>
        {
            var viz = new GraphvizAlgorithm<TVertex, TEdge>(graph);

            viz.FormatVertex += VizFormastring;

            viz.FormatEdge += edgeFormatter;

            viz.Generate(new FileDotEngine(dotProgramLocation), outputFullFileName);
        }

        public static string ToDotNotation<TVertex, TEdge>(this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
            where TVertex : ExpansionTreeNode<Edge<string>>
        {
            var viz = new GraphvizAlgorithm<TVertex, TEdge>(graph);
            viz.FormatVertex += VizFormastring;
            return viz.Generate(new DotPrinter(), "");
        }

        #endregion
        
        public static void Visualize<TEdge>(this IEdgeListGraph<string, TEdge> graph,
            string dotProgramLocation, string outputFullFileName)
            where TEdge : IEdge<string>
        {
            Visualize(graph, dotProgramLocation, outputFullFileName, NoOpEdgeFormatter);
        }

        public static void Visualize<TEdge>(this IEdgeListGraph<string, TEdge> graph,
            string dotProgramLocation, string outputFullFileName, FormatEdgeAction<string, TEdge> edgeFormatter)
            where TEdge : IEdge<string>
        {
            var viz = new GraphvizAlgorithm<string, TEdge>(graph);

            viz.FormatVertex += VizFormastring;

            viz.FormatEdge += edgeFormatter;

            viz.Generate(new FileDotEngine(dotProgramLocation), outputFullFileName);
        }

        public static string ToDotNotation<TEdge>(this IVertexAndEdgeListGraph<string, TEdge> graph)
            where TEdge : IEdge<string>
        {
            var viz = new GraphvizAlgorithm<string, TEdge>(graph);
            viz.FormatVertex += VizFormastring;
            return viz.Generate(new DotPrinter(), "");
        }

        private static void VizFormastring(object sender, FormatVertexEventArgs<string> e)
        {
            e.VertexFormatter.Label = e.Vertex;
        } 

        private static void NoOpEdgeFormatter<TEdge>(object sender, FormatEdgeEventArgs<string, TEdge> e)
            where TEdge : IEdge<string>
        {
            // noop
            //e.EdgeFormatter.Label.Value = e.Edge.Label;
        }

        private static void VizFormastring<TVertex>(object sender, FormatVertexEventArgs<TVertex> e)
            where TVertex : ExpansionTreeNode<Edge<string>>
        {
            var graph = e.Vertex.QueryGraph;
            e.VertexFormatter.Shape = QuickGraph.Graphviz.Dot.GraphvizVertexShape.Box;
            e.VertexFormatter.Label = e.Vertex.NodeName;
            e.VertexFormatter.Label += graph == null ? "\n\n(Root)" : $"\n\n(Level {e.Vertex.Level}. #Edges: {graph.EdgeCount})";
        }

        private static void NoOpEdgeFormatter<TVertex, TEdge>(object sender, FormatEdgeEventArgs<TVertex, TEdge> e)
            where TEdge : IEdge<TVertex>
            where TVertex : ExpansionTreeNode<Edge<string>>
        {
            // noop
            //e.EdgeFormatter.Label.Value = e.Edge.Label;
        }
        
    }
}

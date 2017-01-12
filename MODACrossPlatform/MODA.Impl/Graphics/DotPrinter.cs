using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace MODA.Impl.Graphics
{
    public sealed class DotPrinter : IDotEngine
    {
        public string Run(GraphvizImageType imageType, string dot, string outputFileName)
        {
            return dot;
        }
    }
}

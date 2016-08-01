using QuickGraph;

namespace MODA.Impl
{
    public class ExpansionTreeNode<TEdge> where TEdge : IEdge<string>
    {
        public int Level { get; set; }
        public ExpansionTreeNode<TEdge> ParentNode { get; set; }
        public bool IsRootNode { get { return QueryGraph == null; } }

        public UndirectedGraph<string, TEdge> QueryGraph { get; set; }

        public override bool Equals(object obj)
        {
            var theObj = obj as ExpansionTreeNode<TEdge>;
            if (theObj.IsRootNode && this.IsRootNode) return true;

            return QueryGraph.Equals(theObj.QueryGraph);
        }

        public override int GetHashCode()
        {
            return IsRootNode ? base.GetHashCode() : QueryGraph.GetHashCode();
        }

        public override string ToString()
        {
            return $"Node:Level - {Level}; Is Root - {IsRootNode}; Number of Query Graph edges -  {QueryGraph?.EdgeCount}";
        }
    }
}

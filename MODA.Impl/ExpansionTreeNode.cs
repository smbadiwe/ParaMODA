using QuickGraph;

namespace MODA.Impl
{
    public class ExpansionTreeNode
    {
        public int Level { get; set; }
        /// <summary>
        /// A name to identify this node, especially useful
        /// </summary>
        public string NodeName { get; set; }
        public ExpansionTreeNode ParentNode { get; set; }
        public bool IsRootNode { get { return QueryGraph == null; } }

        public QueryGraph QueryGraph { get; set; }

        public override bool Equals(object obj)
        {
            var theObj = obj as ExpansionTreeNode;
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

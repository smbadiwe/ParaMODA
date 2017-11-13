using QuickGraph;

namespace ParaMODA.Impl
{
    public class ExpansionTreeNode
    {
        public int Level { get; set; }
        /// <summary>
        /// A name to identify this node, especially useful
        /// </summary>
        public string NodeName { get { return QueryGraph?.Identifier; } }
        public ExpansionTreeNode ParentNode { get; set; }
        public bool IsRootNode { get { return QueryGraph == null; } }

        public QueryGraph QueryGraph { get; set; }

        public override bool Equals(object obj)
        {
            var theObj = obj as ExpansionTreeNode;
            if (theObj.IsRootNode && this.IsRootNode) return true;

            return string.Equals(NodeName, theObj.NodeName, System.StringComparison.CurrentCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return IsRootNode ? base.GetHashCode() : QueryGraph.GetHashCode();
        }

        public override string ToString()
        {
            return $"Node: Name: {NodeName}; Level - {Level}; Is Root - {IsRootNode}; Number of Query Graph edges -  {QueryGraph?.EdgeCount}";
        }
    }
}

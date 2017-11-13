using QuickGraph;

namespace ParaMODA.Impl
{
    public static class ThreeNodes
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expansionTree"></param>
        /// <returns>The root node</returns>
        public static ExpansionTreeNode BuildThreeNodesTree(this AdjacencyGraph<ExpansionTreeNode> expansionTree)
        {
            //Level 0 - Root Node
            var rootNode = new ExpansionTreeNode();

            //Level 1
            var qGraphL1_1 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(2,3)
            }
            .ToQueryGraph("qGraphL1_1");
            var nodeL1_1 = new ExpansionTreeNode
            {
                Level = 1,
                QueryGraph = qGraphL1_1,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(rootNode, nodeL1_1));
            
            //Level 2
            var qGraphL2_1 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(2,3),
                new Edge<int>(1,3) //New Add;
            }
            .ToQueryGraph("qGraphL2_1");
            var nodeL2_1 = new ExpansionTreeNode
            {
                Level = 2,
                QueryGraph = qGraphL2_1,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL1_1, nodeL2_1));

            return rootNode;
        }
    }
}

using QuickGraph;

namespace MODA.Impl
{
    public static class ThreeNodes
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expansionTree"></param>
        /// <returns>The root node</returns>
        public static ExpansionTreeNode BuildThreeNodesTree(this AdjacencyGraph<ExpansionTreeNode, Edge<ExpansionTreeNode>> expansionTree)
        {
            //Level 0 - Root Node
            var rootNode = new ExpansionTreeNode();

            //Level 1
            var qGraphL1_1 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("B","C")
            }
            .ToQueryGraph(false);
            var nodeL1_1 = new ExpansionTreeNode
            {
                Level = 1,
                QueryGraph = qGraphL1_1,
                NodeName = "qGraphL1_1"
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(rootNode, nodeL1_1));
            
            //Level 2
            var qGraphL2_1 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("B","C"),
                new Edge<string>("A","C") //New Add;
            }
            .ToQueryGraph(false);
            var nodeL2_1 = new ExpansionTreeNode
            {
                Level = 2,
                QueryGraph = qGraphL2_1,
                NodeName = "qGraphL2_1"
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL1_1, nodeL2_1));

            return rootNode;
        }
    }
}

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
        public static ExpansionTreeNode<Edge<string>> BuildThreeNodesTree(this AdjacencyGraph<ExpansionTreeNode<Edge<string>>, Edge<ExpansionTreeNode<Edge<string>>>> expansionTree)
        {
            //Level 0 - Root Node
            var rootNode = new ExpansionTreeNode<Edge<string>>();

            //Level 1
            var qGraphL1_1 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("B","C")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL1_1 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 1,
                QueryGraph = qGraphL1_1,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(rootNode, nodeL1_1));
            
            //Level 2
            var qGraphL2_1 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("B","C"),
                new Edge<string>("A","C") //New Add;
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL2_1 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 2,
                QueryGraph = qGraphL2_1,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL1_1, nodeL2_1));

            return rootNode;
        }
    }
}

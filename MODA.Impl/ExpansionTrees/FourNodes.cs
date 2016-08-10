using QuickGraph;

namespace MODA.Impl
{
    public static class FourNodes
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expansionTree"></param>
        /// <returns>The root node</returns>
        public static ExpansionTreeNode<Edge<string>> BuildFourNodesTree(this AdjacencyGraph<ExpansionTreeNode<Edge<string>>, Edge<ExpansionTreeNode<Edge<string>>>> expansionTree)
        {
            //Level 0 - Root Node
            var rootNode = new ExpansionTreeNode<Edge<string>>();

            #region Level 1 - Two (2) Trees
            var qGraphL1_1 = new Edge<string>[]
                {
                new Edge<string>("A","B"),
                new Edge<string>("B","C"),
                new Edge<string>("C","D")
                }
                .ToUndirectedGraph<string, Edge<string>>(false);
            var qGraphL1_2 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("B","C"),
                new Edge<string>("B","D")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);

            var nodeL1_1 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 1,
                QueryGraph = qGraphL1_1,
            };
            var nodeL1_2 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 1,
                QueryGraph = qGraphL1_2,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(rootNode, nodeL1_1));
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(rootNode, nodeL1_2));
            #endregion

            #region Level 2 - Two (2) graphs
            var qGraphL2_1 = new Edge<string>[]
                {
                new Edge<string>("A","B"),
                new Edge<string>("B","C"),
                new Edge<string>("B","D"), //New Add; could have been 0-2
                new Edge<string>("C","D")
                }
                .ToUndirectedGraph<string, Edge<string>>(false);
            var qGraphL2_2 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("A","D"), //New Add
                new Edge<string>("B","C"),
                new Edge<string>("C","D")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);

            var nodeL2_1 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 2,
                QueryGraph = qGraphL2_1,
            };
            var nodeL2_2 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 2,
                QueryGraph = qGraphL2_2,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL1_1, nodeL2_1));
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL1_1, nodeL2_2));

            #endregion

            #region Level 3 - One (1) graph
            var qGraphL3_1 = new Edge<string>[]
                {
                new Edge<string>("A","B"),
                new Edge<string>("A","C"), //New Add
                new Edge<string>("B","C"),
                new Edge<string>("B","D"),
                new Edge<string>("C","D")
                }
                .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL3_1 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 3,
                QueryGraph = qGraphL3_1,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL2_1, nodeL3_1));

            #endregion

            #region Level 4 - One (1) Graph, which is the complete graph
            var qGraphL4_1 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("A","C"), //New Add
                new Edge<string>("A","D"),
                new Edge<string>("B","C"),
                new Edge<string>("B","D"),
                new Edge<string>("C","D")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);

            var nodeL4_1 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 4,
                QueryGraph = qGraphL4_1,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL3_1, nodeL4_1));
            #endregion

            #region clean up

            qGraphL1_1 = null;
            qGraphL1_2 = null;
            nodeL1_1 = null;
            nodeL1_2 = null;

            qGraphL2_1 = null;
            qGraphL2_2 = null;
            nodeL2_1 = null;
            nodeL2_2 = null;

            qGraphL3_1 = null;
            nodeL3_1 = null;

            qGraphL4_1 = null;
            nodeL4_1 = null;

            #endregion

            return rootNode;
        }
    }
}

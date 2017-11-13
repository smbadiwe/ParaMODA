using QuickGraph;

namespace ParaMODA.Impl
{
    public static class FourNodes
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expansionTree"></param>
        /// <returns>The root node</returns>
        public static ExpansionTreeNode BuildFourNodesTree(this AdjacencyGraph<ExpansionTreeNode> expansionTree)
        {
            //Level 0 - Root Node
            var rootNode = new ExpansionTreeNode();

            #region Level 1 - Two (2) Trees
            var qGraphL1_1 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(2,3),
                new Edge<int>(3,4)
            }
            .ToQueryGraph("qGraphL1_1");
            var qGraphL1_2 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(2,3),
                new Edge<int>(2,4)
            }
            .ToQueryGraph("qGraphL1_2");

            var nodeL1_1 = new ExpansionTreeNode
            {
                Level = 1,
                QueryGraph = qGraphL1_1,
            };
            var nodeL1_2 = new ExpansionTreeNode
            {
                Level = 1,
                QueryGraph = qGraphL1_2,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(rootNode, nodeL1_1));
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(rootNode, nodeL1_2));
            #endregion

            #region Level 2 - Two (2) graphs
            var qGraphL2_1 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(2,3),
                new Edge<int>(2,4), //New Add; could have been 0-2
                new Edge<int>(3,4)
            }
            .ToQueryGraph("qGraphL2_1");
            var qGraphL2_2 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(1,4), //New Add
                new Edge<int>(2,3),
                new Edge<int>(3,4)
            }
            .ToQueryGraph("qGraphL2_2");

            var nodeL2_1 = new ExpansionTreeNode
            {
                Level = 2,
                QueryGraph = qGraphL2_1,
            };
            var nodeL2_2 = new ExpansionTreeNode
            {
                Level = 2,
                QueryGraph = qGraphL2_2,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL1_1, nodeL2_1));
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL1_1, nodeL2_2));

            #endregion

            #region Level 3 - One (1) graph
            var qGraphL3_1 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(1,3), //New Add
                new Edge<int>(2,3),
                new Edge<int>(2,4),
                new Edge<int>(3,4)
            }
            .ToQueryGraph("qGraphL3_1");
            var nodeL3_1 = new ExpansionTreeNode
            {
                Level = 3,
                QueryGraph = qGraphL3_1,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL2_1, nodeL3_1));

            #endregion

            #region Level 4 - One (1) Graph, which is the complete graph
            var qGraphL4_1 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(1,3), //New Add
                new Edge<int>(1,4),
                new Edge<int>(2,3),
                new Edge<int>(2,4),
                new Edge<int>(3,4)
            }
            .ToQueryGraph("qGraphL4_1");

            var nodeL4_1 = new ExpansionTreeNode
            {
                Level = 4,
                QueryGraph = qGraphL4_1,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL3_1, nodeL4_1));
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

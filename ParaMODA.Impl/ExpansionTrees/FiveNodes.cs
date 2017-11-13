using QuickGraph;

namespace ParaMODA.Impl
{
    public static class FiveNodes
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expansionTree"></param>
        /// <returns>The root node</returns>
        public static ExpansionTreeNode BuildFiveNodesTree(this AdjacencyGraph<ExpansionTreeNode> expansionTree)
        {
            //Level 0 - Root Node
            var rootNode = new ExpansionTreeNode();

            #region Level 1 - Three (3) Trees
            var qGraphL1_1 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(2,3),
                new Edge<int>(3,4),
                new Edge<int>(4,5)
            }
            .ToQueryGraph("qGraphL1_1");
            var qGraphL1_2 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(2,3),
                new Edge<int>(2,4),
                new Edge<int>(2,5)
            }
            .ToQueryGraph("qGraphL1_2");
            var qGraphL1_3 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(2,3),
                new Edge<int>(3,4),
                new Edge<int>(3,5)
            }
            .ToQueryGraph("qGraphL1_3");

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
            var nodeL1_3 = new ExpansionTreeNode
            {
                Level = 1,
                QueryGraph = qGraphL1_3,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(rootNode, nodeL1_1));
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(rootNode, nodeL1_2));
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(rootNode, nodeL1_3));
            #endregion

            #region Level 2 - Five (5) graphs
            //From L1_3
            var qGraphL2_3 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(2,3),
                new Edge<int>(3,4),
                new Edge<int>(3,5),
                new Edge<int>(4,5) //New Add
            }
            .ToQueryGraph("qGraphL2_3");
            var nodeL2_3 = new ExpansionTreeNode
            {
                Level = 2,
                QueryGraph = qGraphL2_3,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL1_3, nodeL2_3));

            //From L1_2
            var qGraphL2_2 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(1,5), //New add
                new Edge<int>(2,3),
                new Edge<int>(2,4),
                new Edge<int>(2,5)
            }
            .ToQueryGraph("qGraphL2_2");
            var nodeL2_2 = new ExpansionTreeNode
            {
                Level = 2,
                QueryGraph = qGraphL2_2,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL1_2, nodeL2_2));
            
            //From L1_1
            var qGraphL2_1a = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(2,3),
                new Edge<int>(3,4),
                new Edge<int>(4,5),
                new Edge<int>(5,1) //new Add
            }
            .ToQueryGraph("qGraphL2_1a");
            var nodeL2_1a = new ExpansionTreeNode
            {
                Level = 2,
                QueryGraph = qGraphL2_1a,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL1_1, nodeL2_1a));

            var qGraphL2_1b = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(2,3),
                new Edge<int>(2,4), //New Add
                new Edge<int>(3,4),
                new Edge<int>(4,5),
            }
            .ToQueryGraph("qGraphL2_1b");
            var nodeL2_1b = new ExpansionTreeNode
            {
                Level = 2,
                QueryGraph = qGraphL2_1b,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL1_1, nodeL2_1b));

            var qGraphL2_1c = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(2,3),
                new Edge<int>(2,5), //New Add
                new Edge<int>(3,4),
                new Edge<int>(4,5),
            }
            .ToQueryGraph("qGraphL2_1c");
            var nodeL2_1c = new ExpansionTreeNode
            {
                Level = 2,
                QueryGraph = qGraphL2_1c,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL1_1, nodeL2_1c));
            #endregion

            #region Level 3 - Three (3) graphs
            //From L2_1a
            var qGraphL3_1 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(2,3),
                new Edge<int>(3,4),
                new Edge<int>(4,5),
                new Edge<int>(5,1),
                new Edge<int>(5,2) //new Add
            }
            .ToQueryGraph("qGraphL3_1");
            var nodeL3_1 = new ExpansionTreeNode
            {
                Level = 3,
                QueryGraph = qGraphL3_1,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL2_1a, nodeL3_1));

            //From L2_2
            var qGraphL3_2 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(1,5),
                new Edge<int>(2,3),
                new Edge<int>(2,4),
                new Edge<int>(2,5),
                new Edge<int>(3,4) //New add
            }
            .ToQueryGraph("qGraphL3_2");
            var nodeL3_2 = new ExpansionTreeNode
            {
                Level = 3,
                QueryGraph = qGraphL3_2,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL2_2, nodeL3_2));

            //From L2_3
            var qGraphL3_3 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(2,3),
                new Edge<int>(2,4), //New Add
                new Edge<int>(3,4),
                new Edge<int>(3,5),
                new Edge<int>(4,5)
            }
            .ToQueryGraph("qGraphL3_3");
            var nodeL3_3 = new ExpansionTreeNode
            {
                Level = 3,
                QueryGraph = qGraphL3_3,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL2_3, nodeL3_3));

            #endregion

            #region Level 4 - Three (3) graphs
            //From L3_1
            var qGraphL4_1 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(2,3),
                new Edge<int>(2,4), //new Add
                new Edge<int>(3,4),
                new Edge<int>(4,5),
                new Edge<int>(5,1),
                new Edge<int>(5,2)
            }
            .ToQueryGraph("qGraphL4_1");
            var nodeL4_1 = new ExpansionTreeNode
            {
                Level = 4,
                QueryGraph = qGraphL4_1,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL3_1, nodeL4_1));

            //From L3_2
            var qGraphL4_2 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(1,5),
                new Edge<int>(2,3),
                new Edge<int>(2,4),
                new Edge<int>(2,5),
                new Edge<int>(3,4),
                new Edge<int>(4,5) //New add
            }
            .ToQueryGraph("qGraphL4_2");
            var nodeL4_2 = new ExpansionTreeNode
            {
                Level = 4,
                QueryGraph = qGraphL4_2,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL3_2, nodeL4_2));
            
            //From L3_3
            var qGraphL4_3 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(1,5), //New Add
                new Edge<int>(2,3),
                new Edge<int>(2,4),
                new Edge<int>(3,4),
                new Edge<int>(3,5),
                new Edge<int>(4,5)
            }
            .ToQueryGraph("qGraphL4_3");
            var nodeL4_3 = new ExpansionTreeNode
            {
                Level = 4,
                QueryGraph = qGraphL4_3,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL3_3, nodeL4_3));

            #endregion

            #region Level 5 - Three (3) graphs
            //From L4_1
            var qGraphL5_1 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(2,3),
                new Edge<int>(2,4),
                new Edge<int>(3,4),
                new Edge<int>(3,5), //new Add
                new Edge<int>(4,5),
                new Edge<int>(5,1),
                new Edge<int>(5,2)
            }
            .ToQueryGraph("qGraphL5_1");
            var nodeL5_1 = new ExpansionTreeNode
            {
                Level = 5,
                QueryGraph = qGraphL5_1,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL4_1, nodeL5_1));

            //From L4_2
            var qGraphL5_2 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(1,3), //New add
                new Edge<int>(1,5),
                new Edge<int>(2,3),
                new Edge<int>(2,4),
                new Edge<int>(2,5),
                new Edge<int>(3,4),
                new Edge<int>(4,5) 
            }
            .ToQueryGraph("qGraphL5_2");
            var nodeL5_2 = new ExpansionTreeNode
            {
                Level = 5,
                QueryGraph = qGraphL5_2,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL4_2, nodeL5_2));

            //From L4_3
            var qGraphL5_3 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(1,5),
                new Edge<int>(2,3),
                new Edge<int>(2,4),
                new Edge<int>(2,5), //New Add
                new Edge<int>(3,4),
                new Edge<int>(3,5),
                new Edge<int>(4,5)
            }
            .ToQueryGraph("qGraphL5_3");
            var nodeL5_3 = new ExpansionTreeNode
            {
                Level = 5,
                QueryGraph = qGraphL5_3,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL4_3, nodeL5_3));

            #endregion

            #region Level 6 - Three (3) graphs
            //From L5_1
            var qGraphL6_1 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(1,3), //new Add
                new Edge<int>(2,3),
                new Edge<int>(2,4),
                new Edge<int>(3,4),
                new Edge<int>(3,5),
                new Edge<int>(4,5),
                new Edge<int>(5,1),
                new Edge<int>(5,2)
            }
            .ToQueryGraph("qGraphL6_1");
            var nodeL6_1 = new ExpansionTreeNode
            {
                Level = 6,
                QueryGraph = qGraphL6_1,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL5_1, nodeL6_1));

            //From L5_2
            var qGraphL6_2 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(1,3),
                new Edge<int>(1,4), //New add
                new Edge<int>(1,5),
                new Edge<int>(2,3),
                new Edge<int>(2,4),
                new Edge<int>(2,5),
                new Edge<int>(3,4),
                new Edge<int>(4,5)
            }
            .ToQueryGraph("qGraphL6_2");
            var nodeL6_2 = new ExpansionTreeNode
            {
                Level = 6,
                QueryGraph = qGraphL6_2,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL5_2, nodeL6_2));

            //From L5_3
            var qGraphL6_3 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(1,4), //New Add
                new Edge<int>(1,5),
                new Edge<int>(2,3),
                new Edge<int>(2,4),
                new Edge<int>(2,5),
                new Edge<int>(3,4),
                new Edge<int>(3,5),
                new Edge<int>(4,5)
            }
            .ToQueryGraph("qGraphL6_3");
            var nodeL6_3 = new ExpansionTreeNode
            {
                Level = 6,
                QueryGraph = qGraphL6_3,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL5_3, nodeL6_3));

            #endregion

            #region Level 7 - One (1) graph, the complete graph

            //From L6_2
            var qGraphL7_1 = new Edge<int>[]
            {
                new Edge<int>(1,2),
                new Edge<int>(1,3),
                new Edge<int>(1,4),
                new Edge<int>(1,5),
                new Edge<int>(2,3),
                new Edge<int>(2,4),
                new Edge<int>(2,5),
                new Edge<int>(3,4),
                new Edge<int>(3,5), //New add
                new Edge<int>(4,5)
            }
            .ToQueryGraph("qGraphL7_1");
            var nodeL7_1 = new ExpansionTreeNode
            {
                Level = 7,
                QueryGraph = qGraphL7_1,
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode>(nodeL6_2, nodeL7_1));

            #endregion

            #region Clean up
            //- GC doesn't come quickly and I need to keep memory down. I've learnt my lesson! T
            // This is optional of course, and can always be taken out

            qGraphL1_1 = null;
            qGraphL1_2 = null;
            qGraphL1_3 = null;
            nodeL1_1 = null;
            nodeL1_2 = null;
            nodeL1_3 = null;

            qGraphL2_1a = null;
            qGraphL2_1b = null;
            qGraphL2_1c = null;
            qGraphL2_2 = null;
            qGraphL2_3 = null;
            nodeL2_1a = null;
            nodeL2_1b = null;
            nodeL2_1c = null;
            nodeL2_2 = null;
            nodeL2_3 = null;

            qGraphL3_1 = null;
            qGraphL3_2 = null;
            qGraphL3_3 = null;
            nodeL3_1 = null;
            nodeL3_2 = null;
            nodeL3_3 = null;

            qGraphL4_1 = null;
            qGraphL4_2 = null;
            qGraphL4_3 = null;
            nodeL4_1 = null;
            nodeL4_2 = null;
            nodeL4_3 = null;

            qGraphL5_1 = null;
            qGraphL5_2 = null;
            qGraphL5_3 = null;
            nodeL5_1 = null;
            nodeL5_2 = null;
            nodeL5_3 = null;

            qGraphL6_1 = null;
            qGraphL6_2 = null;
            qGraphL6_3 = null;
            nodeL6_1 = null;
            nodeL6_2 = null;
            nodeL6_3 = null;

            qGraphL7_1 = null;
            nodeL7_1 = null;
            #endregion

            return rootNode;
        }
    }
}

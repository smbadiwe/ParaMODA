using QuickGraph;

namespace MODA.Impl
{
    public static class FiveNodes
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expansionTree"></param>
        /// <returns>The root node</returns>
        public static ExpansionTreeNode<Edge<string>> BuildFiveNodesTree(this AdjacencyGraph<ExpansionTreeNode<Edge<string>>, Edge<ExpansionTreeNode<Edge<string>>>> expansionTree)
        {
            //Level 0 - Root Node
            var rootNode = new ExpansionTreeNode<Edge<string>>();

            #region Level 1 - Three (3) Trees
            var qGraphL1_1 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("B","C"),
                new Edge<string>("C","D"),
                new Edge<string>("D","E")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var qGraphL1_2 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("B","C"),
                new Edge<string>("B","D"),
                new Edge<string>("B","E")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var qGraphL1_3 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("B","C"),
                new Edge<string>("C","D"),
                new Edge<string>("C","E")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);

            var nodeL1_1 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 1,
                QueryGraph = qGraphL1_1,
                NodeName = "qGraphL1_1"
            };
            var nodeL1_2 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 1,
                QueryGraph = qGraphL1_2,
                NodeName = "qGraphL1_2"
            };
            var nodeL1_3 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 1,
                QueryGraph = qGraphL1_3,
                NodeName = "qGraphL1_3"
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(rootNode, nodeL1_1));
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(rootNode, nodeL1_2));
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(rootNode, nodeL1_3));
            #endregion

            #region Level 2 - Five (5) graphs
            //From L1_3
            var qGraphL2_3 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("B","C"),
                new Edge<string>("C","D"),
                new Edge<string>("C","E"),
                new Edge<string>("D","E") //New Add
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL2_3 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 2,
                QueryGraph = qGraphL2_3,
                NodeName = "qGraphL2_3"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL1_3, nodeL2_3));

            //From L1_2
            var qGraphL2_2 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("A","E"), //New add
                new Edge<string>("B","C"),
                new Edge<string>("B","D"),
                new Edge<string>("B","E")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL2_2 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 2,
                QueryGraph = qGraphL2_2,
                NodeName = "qGraphL2_2"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL1_2, nodeL2_2));
            
            //From L1_1
            var qGraphL2_1a = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("B","C"),
                new Edge<string>("C","D"),
                new Edge<string>("D","E"),
                new Edge<string>("E","A") //new Add
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL2_1a = new ExpansionTreeNode<Edge<string>>
            {
                Level = 2,
                QueryGraph = qGraphL2_1a,
                NodeName = "qGraphL2_1a"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL1_1, nodeL2_1a));

            var qGraphL2_1b = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("B","C"),
                new Edge<string>("B","D"), //New Add
                new Edge<string>("C","D"),
                new Edge<string>("D","E"),
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL2_1b = new ExpansionTreeNode<Edge<string>>
            {
                Level = 2,
                QueryGraph = qGraphL2_1b,
                NodeName = "qGraphL2_1b"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL1_1, nodeL2_1b));

            var qGraphL2_1c = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("B","C"),
                new Edge<string>("B","E"), //New Add
                new Edge<string>("C","D"),
                new Edge<string>("D","E"),
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL2_1c = new ExpansionTreeNode<Edge<string>>
            {
                Level = 2,
                QueryGraph = qGraphL2_1c,
                NodeName = "qGraphL2_1c"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL1_1, nodeL2_1c));
            #endregion

            #region Level 3 - Three (3) graphs
            //From L2_1a
            var qGraphL3_1 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("B","C"),
                new Edge<string>("C","D"),
                new Edge<string>("D","E"),
                new Edge<string>("E","A"),
                new Edge<string>("E","B") //new Add
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL3_1 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 3,
                QueryGraph = qGraphL3_1,
                NodeName = "qGraphL3_1"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL2_1a, nodeL3_1));

            //From L2_2
            var qGraphL3_2 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("A","E"),
                new Edge<string>("B","C"),
                new Edge<string>("B","D"),
                new Edge<string>("B","E"),
                new Edge<string>("C","D") //New add
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL3_2 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 3,
                QueryGraph = qGraphL3_2,
                NodeName = "qGraphL3_2"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL2_2, nodeL3_2));

            //From L2_3
            var qGraphL3_3 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("B","C"),
                new Edge<string>("B","D"), //New Add
                new Edge<string>("C","D"),
                new Edge<string>("C","E"),
                new Edge<string>("D","E")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL3_3 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 3,
                QueryGraph = qGraphL3_3,
                NodeName = "qGraphL3_3"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL2_3, nodeL3_3));

            #endregion

            #region Level 4 - Three (3) graphs
            //From L3_1
            var qGraphL4_1 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("B","C"),
                new Edge<string>("B","D"), //new Add
                new Edge<string>("C","D"),
                new Edge<string>("D","E"),
                new Edge<string>("E","A"),
                new Edge<string>("E","B")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL4_1 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 4,
                QueryGraph = qGraphL4_1,
                NodeName = "qGraphL4_1"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL3_1, nodeL4_1));

            //From L3_2
            var qGraphL4_2 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("A","E"),
                new Edge<string>("B","C"),
                new Edge<string>("B","D"),
                new Edge<string>("B","E"),
                new Edge<string>("C","D"),
                new Edge<string>("D","E") //New add
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL4_2 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 4,
                QueryGraph = qGraphL4_2,
                NodeName = "qGraphL4_2"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL3_2, nodeL4_2));
            
            //From L3_3
            var qGraphL4_3 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("A","E"), //New Add
                new Edge<string>("B","C"),
                new Edge<string>("B","D"),
                new Edge<string>("C","D"),
                new Edge<string>("C","E"),
                new Edge<string>("D","E")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL4_3 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 4,
                QueryGraph = qGraphL4_3,
                NodeName = "qGraphL4_3"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL3_3, nodeL4_3));

            #endregion

            #region Level 5 - Three (3) graphs
            //From L4_1
            var qGraphL5_1 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("B","C"),
                new Edge<string>("B","D"),
                new Edge<string>("C","D"),
                new Edge<string>("C","E"), //new Add
                new Edge<string>("D","E"),
                new Edge<string>("E","A"),
                new Edge<string>("E","B")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL5_1 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 5,
                QueryGraph = qGraphL5_1,
                NodeName = "qGraphL5_1"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL4_1, nodeL5_1));

            //From L4_2
            var qGraphL5_2 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("A","C"), //New add
                new Edge<string>("A","E"),
                new Edge<string>("B","C"),
                new Edge<string>("B","D"),
                new Edge<string>("B","E"),
                new Edge<string>("C","D"),
                new Edge<string>("D","E") 
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL5_2 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 5,
                QueryGraph = qGraphL5_2,
                NodeName = "qGraphL5_2"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL4_2, nodeL5_2));

            //From L4_3
            var qGraphL5_3 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("A","E"),
                new Edge<string>("B","C"),
                new Edge<string>("B","D"),
                new Edge<string>("B","E"), //New Add
                new Edge<string>("C","D"),
                new Edge<string>("C","E"),
                new Edge<string>("D","E")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL5_3 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 5,
                QueryGraph = qGraphL5_3,
                NodeName = "qGraphL5_3"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL4_3, nodeL5_3));

            #endregion

            #region Level 6 - Three (3) graphs
            //From L5_1
            var qGraphL6_1 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("A","C"), //new Add
                new Edge<string>("B","C"),
                new Edge<string>("B","D"),
                new Edge<string>("C","D"),
                new Edge<string>("C","E"),
                new Edge<string>("D","E"),
                new Edge<string>("E","A"),
                new Edge<string>("E","B")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL6_1 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 6,
                QueryGraph = qGraphL6_1,
                NodeName = "qGraphL6_1"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL5_1, nodeL6_1));

            //From L5_2
            var qGraphL6_2 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("A","C"),
                new Edge<string>("A","D"), //New add
                new Edge<string>("A","E"),
                new Edge<string>("B","C"),
                new Edge<string>("B","D"),
                new Edge<string>("B","E"),
                new Edge<string>("C","D"),
                new Edge<string>("D","E")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL6_2 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 6,
                QueryGraph = qGraphL6_2,
                NodeName = "qGraphL6_2"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL5_2, nodeL6_2));

            //TODO: From L5_3
            var qGraphL6_3 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("A","E"),
                new Edge<string>("B","C"),
                new Edge<string>("B","D"),
                new Edge<string>("B","E"), //New Add
                new Edge<string>("C","D"),
                new Edge<string>("C","E"),
                new Edge<string>("D","E")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL6_3 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 6,
                QueryGraph = qGraphL6_3,
                NodeName = "qGraphL6_3"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL5_3, nodeL6_3));

            #endregion

            #region Level 7 - One (1) graph, the complete graph

            //From L6_2
            var qGraphL7_1 = new Edge<string>[]
            {
                new Edge<string>("A","B"),
                new Edge<string>("A","C"),
                new Edge<string>("A","D"),
                new Edge<string>("A","E"),
                new Edge<string>("B","C"),
                new Edge<string>("B","D"),
                new Edge<string>("B","E"),
                new Edge<string>("C","D"),
                new Edge<string>("C","E"), //New add
                new Edge<string>("D","E")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var nodeL7_1 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 7,
                QueryGraph = qGraphL7_1,
                NodeName = "qGraphL7_1"
            };
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL6_2, nodeL7_1));

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

using QuickGraph;
using System.Collections.Generic;

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

            //Level 1
            var qGraphL1_1 = new Edge<string>[]
            {
                new Edge<string>("0","1"),
                new Edge<string>("1","2"),
                new Edge<string>("2","3")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var qGraphL1_2 = new Edge<string>[]
            {
                new Edge<string>("0","1"),
                new Edge<string>("1","2"),
                new Edge<string>("1","3")
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

            //Level 2
            var qGraphL2_1 = new Edge<string>[]
            {
                new Edge<string>("0","1"),
                new Edge<string>("1","2"),
                new Edge<string>("1","3"), //New Add; could have been 0-2
                new Edge<string>("2","3")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var qGraphL2_2 = new Edge<string>[]
            {
                new Edge<string>("0","1"),
                new Edge<string>("0","3"), //New Add
                new Edge<string>("1","2"),
                new Edge<string>("2","3")
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

            //Level 3
            var qGraphL3_1 = new Edge<string>[]
            {
                new Edge<string>("0","1"),
                new Edge<string>("0","2"), //New Add
                new Edge<string>("1","2"),
                new Edge<string>("1","3"),
                new Edge<string>("2","3")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);
            var qGraphL3_2 = new Edge<string>[]
            {
                new Edge<string>("0","1"),
                new Edge<string>("0","3"),
                new Edge<string>("1","2"),
                new Edge<string>("1","3"), //New Add; could have been 0-2
                new Edge<string>("2","3")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);

            var nodeL3_1 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 3,
                QueryGraph = qGraphL3_1,
            };
            var nodeL3_2 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 3,
                QueryGraph = qGraphL3_2,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL2_1, nodeL3_1));
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL2_2, nodeL3_2));

            //Level 4
            var qGraphL4_1 = new Edge<string>[]
            {
                new Edge<string>("0","1"),
                new Edge<string>("0","2"), //New Add
                new Edge<string>("0","3"),
                new Edge<string>("1","2"),
                new Edge<string>("1","3"),
                new Edge<string>("2","3")
            }
            .ToUndirectedGraph<string, Edge<string>>(false);

            var nodeL4_1 = new ExpansionTreeNode<Edge<string>>
            {
                Level = 4,
                QueryGraph = qGraphL4_1,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<string>>>(nodeL3_1, nodeL4_1));
            
            return rootNode;
        }
    }
}

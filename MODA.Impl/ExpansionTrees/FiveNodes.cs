using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl
{
    public static class FiveNodes
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expansionTree"></param>
        /// <returns>The root node</returns>
        public static ExpansionTreeNode<Edge<int>> BuildFiveNodesTree(this AdjacencyGraph<ExpansionTreeNode<Edge<int>>, Edge<ExpansionTreeNode<Edge<int>>>> expansionTree)
        {
            //Level 0 - Root Node
            var rootNode = new ExpansionTreeNode<Edge<int>>();

            //Level 1
            var qGraphL1_1 = new List<Edge<int>>()
            {
                new Edge<int>(0,1),
                new Edge<int>(1,2),
                new Edge<int>(2,3),
                new Edge<int>(3,4),
            }
            .ToUndirectedGraph<int, Edge<int>>(false);
            var qGraphL1_2 = new List<Edge<int>>()
            {
                new Edge<int>(0,1),
                new Edge<int>(1,2),
                new Edge<int>(2,3),
                new Edge<int>(2,4),
            }
            .ToUndirectedGraph<int, Edge<int>>(false);
            var qGraphL1_3 = new List<Edge<int>>()
            {
                new Edge<int>(0,1),
                new Edge<int>(1,2),
                new Edge<int>(1,3),
                new Edge<int>(1,4),
            }
            .ToUndirectedGraph<int, Edge<int>>(false);

            var nodeL1_1 = new ExpansionTreeNode<Edge<int>>
            {
                Level = 1,
                QueryGraph = qGraphL1_1,
            };
            var nodeL1_2 = new ExpansionTreeNode<Edge<int>>
            {
                Level = 1,
                QueryGraph = qGraphL1_2,
            };
            var nodeL1_3 = new ExpansionTreeNode<Edge<int>>
            {
                Level = 1,
                QueryGraph = qGraphL1_3,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<int>>>(rootNode, nodeL1_1));
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<int>>>(rootNode, nodeL1_2));
            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<int>>>(rootNode, nodeL1_3));

            //Level 2
            

            //Level 3
            
            //Level 4
            

            return rootNode;
        }
    }
}

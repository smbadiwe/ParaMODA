using QuickGraph;
using System.Collections.Generic;

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

            //Level 1

            //Level 2


            //Level 3

            //Level 4


            return rootNode;
        }
    }
}

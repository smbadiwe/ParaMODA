using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {

        /// <summary>
        /// Enumeration module
        /// </summary>
        /// <param name="queryGraph"></param>
        /// <param name="inputGraph"></param>
        /// <param name="expansionTree"></param>
        public List<Dictionary<int, int>> Algorithm3(UndirectedGraph<int, Edge<int>> queryGraph, UndirectedGraph<int, Edge<int>> inputGraph,
            AdjacencyGraph<ExpansionTreeNode<Edge<int>>, Edge<ExpansionTreeNode<Edge<int>>>> expansionTree)
        {
            var parentNode = GetParent(queryGraph, expansionTree); queryGraph.GetAverageDegree();
            return new List<Dictionary<int, int>>();
        }

        /// <summary>
        /// Helper method for algorithm 3
        /// </summary>
        /// <param name="queryGraph"></param>
        /// <param name="expansionTree"></param>
        /// <returns></returns>
        private UndirectedGraph<int, Edge<int>> GetParent(UndirectedGraph<int, Edge<int>> queryGraph, AdjacencyGraph<ExpansionTreeNode<Edge<int>>, Edge<ExpansionTreeNode<Edge<int>>>> expansionTree)
        {
            var vertex = new ExpansionTreeNode<Edge<int>>
            {
                QueryGraph = queryGraph,
            };
            var hasNode = expansionTree.ContainsVertex(vertex);
            if (hasNode)
            {
                return expansionTree.Vertices.First(x => !x.IsRootNode && x.QueryGraph == queryGraph).ParentNode.QueryGraph;
            }
            return null;
        }

    }
}

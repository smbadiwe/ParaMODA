using QuickGraph;
using QuickGraph.Algorithms.Search;
using System.Collections.Generic;

namespace MODA.Impl
{
    public class ExpansionTreeBuilder<TEdge> where TEdge : IEdge<string>
    {
        public enum TreeTraversalType
        {
            BFS,
            DFS
        }

        private int _numberOfNodes;
        private TreeTraversalType _traversalType;
        private IVertexListGraph<ExpansionTreeNode<Edge<string>>, Edge<ExpansionTreeNode<Edge<string>>>> _graph;

        public IDictionary<ExpansionTreeNode<Edge<string>>, GraphColor> VerticesSorted { get; set; }
        public AdjacencyGraph<ExpansionTreeNode<Edge<string>>, Edge<ExpansionTreeNode<Edge<string>>>> ExpansionTree { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfNodes"></param>
        /// <param name="traversalType"></param>
        /// <param name="queryGraph">The query graph we're looking for. Leave null if you're not looking for any in particular</param>
        public ExpansionTreeBuilder(int numberOfNodes, TreeTraversalType traversalType = TreeTraversalType.BFS, UndirectedGraph<string, Edge<string>> queryGraph = null)
        {
            if (queryGraph != null)
            {
                numberOfNodes = queryGraph.VertexCount;
            }
            _numberOfNodes = numberOfNodes;
            _traversalType = traversalType;
            ExpansionTree = new AdjacencyGraph<ExpansionTreeNode<Edge<string>>, Edge<ExpansionTreeNode<Edge<string>>>>(false);

            ExpansionTree.EdgeAdded += e => e.Target.ParentNode = e.Source;
        }

        public void Build()
        {
            ExpansionTreeNode<Edge<string>> rootNode;
            switch (_numberOfNodes)
            {
                case 3:
                    rootNode = ExpansionTree.BuildThreeNodesTree();
                    break;
                case 4:
                    rootNode = ExpansionTree.BuildFourNodesTree();
                    break;
                case 5:
                    rootNode = ExpansionTree.BuildFiveNodesTree();
                    break;
                default: //Do for 4
                    rootNode = ExpansionTree.BuildFourNodesTree();
                    break;
            }
            //TODO: Construct the tree.
            // It turns out there's yet no formula to determine the number of isomorphic trees that can be formed
            // from n nodes; hence no way(?) of writing a general code

            if (_traversalType == TreeTraversalType.BFS)
            {
                var bfs = new BreadthFirstSearchAlgorithm<ExpansionTreeNode<Edge<string>>, Edge<ExpansionTreeNode<Edge<string>>>>(ExpansionTree);
                bfs.SetRootVertex(rootNode);
                bfs.Compute();

                VerticesSorted = bfs.VertexColors;
                _graph = bfs.VisitedGraph;
                bfs = null;
            }
            else
            {
                var dfs = new DepthFirstSearchAlgorithm<ExpansionTreeNode<Edge<string>>, Edge<ExpansionTreeNode<Edge<string>>>>(ExpansionTree);
                dfs.SetRootVertex(rootNode);
                dfs.Compute();

                VerticesSorted = dfs.VertexColors;
                _graph = dfs.VisitedGraph;
                dfs = null;
            }
            VerticesSorted[rootNode] = GraphColor.White;
        }

    }
}

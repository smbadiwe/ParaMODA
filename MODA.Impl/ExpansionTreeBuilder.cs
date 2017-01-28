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

        public int NumberOfQueryGraphs { get; private set; }
        private int _numberOfNodes;
        private TreeTraversalType _traversalType;
        private IVertexListGraph<ExpansionTreeNode, Edge<ExpansionTreeNode>> _graph;

        public IDictionary<ExpansionTreeNode, GraphColor> VerticesSorted { get; set; }
        public AdjacencyGraph<ExpansionTreeNode, Edge<ExpansionTreeNode>> ExpansionTree { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfNodes"></param>
        /// <param name="traversalType"></param>
        public ExpansionTreeBuilder(int numberOfNodes, TreeTraversalType traversalType = TreeTraversalType.BFS)
        {
            _numberOfNodes = numberOfNodes;
            _traversalType = traversalType;
            NumberOfQueryGraphs = 1;
            ExpansionTree = new AdjacencyGraph<ExpansionTreeNode, Edge<ExpansionTreeNode>>();

            ExpansionTree.EdgeAdded += e => e.Target.ParentNode = e.Source;
        }

        public void Build()
        {
            ExpansionTreeNode rootNode;
            switch (_numberOfNodes)
            {
                case 3:
                    rootNode = ExpansionTree.BuildThreeNodesTree();
                    NumberOfQueryGraphs = 2;
                    break;
                case 4:
                    rootNode = ExpansionTree.BuildFourNodesTree();
                    NumberOfQueryGraphs = 6;
                    break;
                case 5:
                    rootNode = ExpansionTree.BuildFiveNodesTree();
                    NumberOfQueryGraphs = 21;
                    break;
                default: 
                    throw new System.NotSupportedException("Subgraph sizes below 3 and above 5 are not supported, unless you supply a query graph.");
            }
            //TODO: Construct the tree.
            // It turns out there's yet no formula to determine the number of isomorphic trees that can be formed
            // from n nodes; hence no way(?) of writing a general code

            if (_traversalType == TreeTraversalType.BFS)
            {
                var bfs = new BreadthFirstSearchAlgorithm<ExpansionTreeNode, Edge<ExpansionTreeNode>>(ExpansionTree);
                bfs.SetRootVertex(rootNode);
                bfs.Compute();

                VerticesSorted = bfs.VertexColors;
                _graph = bfs.VisitedGraph;
                bfs = null;
            }
            else
            {
                var dfs = new DepthFirstSearchAlgorithm<ExpansionTreeNode, Edge<ExpansionTreeNode>>(ExpansionTree);
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

using QuickGraph;
using QuickGraph.Algorithms.Search;
using System.Collections.Generic;

namespace MODA.Impl
{
    public class ExpansionTreeBuilder<TEdge> where TEdge : IEdge<int>
    {
        public enum TreeTraversalType
        {
            BFS,
            DFS
        }

        public IDictionary<ExpansionTreeNode<Edge<int>>, GraphColor> VerticesSorted { get; set; }
        public IVertexListGraph<ExpansionTreeNode<Edge<int>>, Edge<ExpansionTreeNode<Edge<int>>>> Graph { get; private set; }
        public AdjacencyGraph<ExpansionTreeNode<Edge<int>>, Edge<ExpansionTreeNode<Edge<int>>>> ExpansionTree { get; private set; }

        public int NumberOfNodes { get; private set; }
        public TreeTraversalType TraversalType { get; private set; }

        public ExpansionTreeBuilder(int numberOfNodes, TreeTraversalType traversalType = TreeTraversalType.BFS)
        {
            NumberOfNodes = numberOfNodes;
            TraversalType = traversalType;
            ExpansionTree = new AdjacencyGraph<ExpansionTreeNode<Edge<int>>, Edge<ExpansionTreeNode<Edge<int>>>>(false);

            ExpansionTree.EdgeAdded += e => e.Target.ParentNode = e.Source;
        }

        public void Build()
        {
            ExpansionTreeNode<Edge<int>> rootNode;
            switch (NumberOfNodes)
            {
                case 3:
                    rootNode = ExpansionTree.BuildThreeNodesTree();
                    break;
                case 4:
                default:
                    rootNode = ExpansionTree.BuildFourNodesTree();
                    break;
            }
            //TODO: Construct the tree.
            // It turns out there's yet no formula to determine the number of isomorphic trees that can be formed
            // from n nodes; hence no way(?) of writing a general code

            if (TraversalType == TreeTraversalType.BFS)
            {
                var bfs = new BreadthFirstSearchAlgorithm<ExpansionTreeNode<Edge<int>>, Edge<ExpansionTreeNode<Edge<int>>>>(ExpansionTree);
                bfs.SetRootVertex(rootNode);
                bfs.Compute();

                VerticesSorted = bfs.VertexColors;
                Graph = bfs.VisitedGraph;
            }
            else
            {
                var dfs = new DepthFirstSearchAlgorithm<ExpansionTreeNode<Edge<int>>, Edge<ExpansionTreeNode<Edge<int>>>>(ExpansionTree);
                dfs.SetRootVertex(rootNode);
                dfs.Compute();

                VerticesSorted = dfs.VertexColors;
                Graph = dfs.VisitedGraph;
            }
            VerticesSorted[rootNode] = GraphColor.White;
        }

    }
}

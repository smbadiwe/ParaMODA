using QuickGraph;
using QuickGraph.Algorithms.Search;
using System.Collections.Generic;

namespace ParaMODA.Impl
{
    public class ExpansionTreeBuilder<TVertex>
    {
        private int _numberOfNodes;
        public int NumberOfQueryGraphs { get; private set; }
        public Queue<ExpansionTreeNode> VerticesSorted { get; private set; }
        public AdjacencyGraph<ExpansionTreeNode> ExpansionTree { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfNodes"></param>
        public ExpansionTreeBuilder(int numberOfNodes)
        {
            _numberOfNodes = numberOfNodes;
            NumberOfQueryGraphs = 1;
            ExpansionTree = new AdjacencyGraph<ExpansionTreeNode>();

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
            var bfs = new BreadthFirstSearchAlgorithm<ExpansionTreeNode>(ExpansionTree);
            bfs.SetRootVertex(rootNode);
            bfs.Compute();
            
            VerticesSorted = new Queue<ExpansionTreeNode>(bfs.VertexColors.Count);
            foreach (var item in bfs.VertexColors)
            {
                VerticesSorted.Enqueue(item.Key);
            }
            bfs.VertexColors.Clear();
            bfs = null;
            VerticesSorted.Dequeue(); // remove the root
        }
    }
}

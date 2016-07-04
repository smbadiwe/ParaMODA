using QuickGraph;
using QuickGraph.Algorithms.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var rootNode = new ExpansionTreeNode<Edge<int>>
            {
                Level = 0, //the root
            };

            //TODO: Construct the tree.
            // It turns out there's yet no formula to determine the number of isomorphic trees that can be formed
            // from n nodes; hence no way(?) of writing a general code

            #region Manual tree; n = 4

            //Level 1
            var qGraphL1_1 = new List<Edge<int>>()
            {
                new Edge<int>(0,1),
                new Edge<int>(1,2),
                new Edge<int>(2,3),
            }
            .ToUndirectedGraph<int, Edge<int>>(false);
            var qGraphL1_2 = new List<Edge<int>>()
            {
                new Edge<int>(0,1),
                new Edge<int>(1,2),
                new Edge<int>(1,3),
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

            ExpansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<int>>>(rootNode, nodeL1_1));
            ExpansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<int>>>(rootNode, nodeL1_2));

            //Level 2
            var qGraphL2_1 = new List<Edge<int>>()
            {
                new Edge<int>(0,1),
                new Edge<int>(1,2),
                new Edge<int>(1,3), //New Add; could have been 0-2
                new Edge<int>(2,3),
            }
            .ToUndirectedGraph<int, Edge<int>>(false);
            var qGraphL2_2 = new List<Edge<int>>()
            {
                new Edge<int>(0,1),
                new Edge<int>(0,3), //New Add
                new Edge<int>(1,2),
                new Edge<int>(2,3),
            }
            .ToUndirectedGraph<int, Edge<int>>(false);

            var nodeL2_1 = new ExpansionTreeNode<Edge<int>>
            {
                Level = 2,
                QueryGraph = qGraphL2_1,
            };
            var nodeL2_2 = new ExpansionTreeNode<Edge<int>>
            {
                Level = 2,
                QueryGraph = qGraphL2_2,
            };

            ExpansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<int>>>(nodeL1_1, nodeL2_1));
            ExpansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<int>>>(nodeL1_1, nodeL2_2));

            //Level 3
            var qGraphL3_1 = new List<Edge<int>>()
            {
                new Edge<int>(0,1),
                new Edge<int>(0,2), //New Add
                new Edge<int>(1,2),
                new Edge<int>(1,3),
                new Edge<int>(2,3),
            }
            .ToUndirectedGraph<int, Edge<int>>(false);
            var qGraphL3_2 = new List<Edge<int>>()
            {
                new Edge<int>(0,1),
                new Edge<int>(0,3),
                new Edge<int>(1,2),
                new Edge<int>(1,3), //New Add; could have been 0-2
                new Edge<int>(2,3),
            }
            .ToUndirectedGraph<int, Edge<int>>(false);

            var nodeL3_1 = new ExpansionTreeNode<Edge<int>>
            {
                Level = 3,
                QueryGraph = qGraphL3_1,
            };
            var nodeL3_2 = new ExpansionTreeNode<Edge<int>>
            {
                Level = 3,
                QueryGraph = qGraphL3_2,
            };

            ExpansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<int>>>(nodeL2_1, nodeL3_1));
            ExpansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<int>>>(nodeL2_2, nodeL3_2));

            //Level 4
            var qGraphL4_1 = new List<Edge<int>>()
            {
                new Edge<int>(0,1),
                new Edge<int>(0,2), //New Add
                new Edge<int>(0,3),
                new Edge<int>(1,2),
                new Edge<int>(1,3),
                new Edge<int>(2,3),
            }
            .ToUndirectedGraph<int, Edge<int>>(false);

            var nodeL4_1 = new ExpansionTreeNode<Edge<int>>
            {
                Level = 4,
                QueryGraph = qGraphL4_1,
            };

            ExpansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<int>>>(nodeL3_1, nodeL4_1));

            #endregion

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

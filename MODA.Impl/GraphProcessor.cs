using QuickGraph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl
{
    public class GraphProcessor
    {
        /// <summary>
        /// Reads the content of the file whose name is supplied and constructs the nodes and edges for
        /// the given <paramref name="newGRaphInstance"/>. It is assumed the file uploaded contains edges
        /// (pairs of nodes connecting with each other).
        /// </summary>
        /// <param name="filename">The uploaded filename</param>
        /// <param name="newGraphInstance"></param>
        /// <returns>A string containing feedback of the processing</returns>
        public static string LoadGraph<T>(string filename, UndirectedGraph<T, Edge<T>> newGraphInstance)
        {
            var lines = File.ReadAllLines(filename);
            Stopwatch sw = Stopwatch.StartNew();

            //Parallelizing this actually made it take longer to process my datasets
            string[] tmp;
            T temp_node;
            T temp_node2;
            foreach (var line in lines)
            {
                if (line.StartsWith("#")) continue;
                tmp = line.Split(new string[] { " ", "\t" }, StringSplitOptions.None);
                temp_node = (T)Convert.ChangeType(tmp[0], typeof(T));
                temp_node2 = (T)Convert.ChangeType(tmp[1], typeof(T));

                newGraphInstance.AddVerticesAndEdge(new Edge<T>(temp_node, temp_node2));
            }
            sw.Stop();
            filename = new FileInfo(filename).Name;
            var nodeCount = newGraphInstance.VertexCount;
            var edgeCount = newGraphInstance.EdgeCount;
            var inv = (nodeCount * nodeCount) / edgeCount;
            var sparse = inv > 64;
            var invDeg = string.Format("{0} ({1} Graph)", inv, inv > 64 ? "Sparse" : "Dense");
            return $"\tFile loaded: {filename}\n\nNumber of lines in file:\t{edgeCount}\nNumber of nodes:\t\t{nodeCount}\nInv Degree:\t{invDeg}\nTime Taken to Process:\t{sw.ElapsedMilliseconds:N2} ms";
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subgraphSize">k</param>
        public static void BuildExpansionTree(int subgraphSize = 4)
        {
            int[] nodeLabels = new int[subgraphSize];
            for (int i = 0; i < subgraphSize; i++)
            {
                nodeLabels[i] = i;
            }

            int maxDepthOfTree = ((subgraphSize * subgraphSize) - (3 * subgraphSize) + 4) / 2;
        }

        static int GetNumberOfEdgesInSubgraphGivenLevel(int subgraphSize, int levelNo)
        {
            return subgraphSize - 2 + levelNo;

        }
        
        /// <summary>
        /// Algo 1: Find subgraph frequency
        /// </summary>
        /// <param name="inputGraph"></param>
        /// <param name="subgraphSize"></param>
        /// <param name="thresholdValue"></param>
        /// <returns>Fg, frequent subgraph list</returns>
        public List<UndirectedGraph<int, Edge<int>>> Algorithm1(UndirectedGraph<int, Edge<int>> inputGraph, int subgraphSize, int thresholdValue = 0)
        {
            var builder = new ExpansionTreeBuilder<Edge<int>>(subgraphSize);
            builder.Build();

            var frequentSubgraphs = new List<UndirectedGraph<int, Edge<int>>>();
            var allMappings = new Dictionary<UndirectedGraph<int, Edge<int>>, List<object>>();
            do
            {
                var qGraph = GetNextNode(builder.VerticesSorted).QueryGraph;
                List<object> mappings;
                if (qGraph.EdgeCount == (subgraphSize - 1))
                {
                    //TODO: Mapping module
                    mappings = Algorithm2(qGraph, inputGraph);
                }
                else
                {
                    //TODO: Enumeration moodule
                    mappings = Algorithm3(qGraph, inputGraph, builder.ExpansionTree);
                }

                //TODO: Save mappings
                allMappings.Add(qGraph, mappings);
                if (mappings.Count > thresholdValue)
                {
                    frequentSubgraphs.Add(qGraph);
                }

                //Check for complete-ness; if complete, break
                //  A Complete graph of n nodes has n(n-1)/2 edges
                if (qGraph.EdgeCount == ((subgraphSize * (subgraphSize - 1)) / 2))
                {
                    break;
                }
            }
            while (true);

            return frequentSubgraphs;
        }

        /// <summary>
        /// Mapping module
        /// </summary>
        /// <param name="queryGraph"></param>
        /// <param name="inputGraph"></param>
        public List<object> Algorithm2(UndirectedGraph<int, Edge<int>> queryGraph, UndirectedGraph<int, Edge<int>> inputGraph)
        {
            return new List<object>();
        }

        /// <summary>
        /// Enumeration module
        /// </summary>
        /// <param name="queryGraph"></param>
        /// <param name="inputGraph"></param>
        /// <param name="expansionTree"></param>
        public List<object> Algorithm3(UndirectedGraph<int, Edge<int>> queryGraph, UndirectedGraph<int, Edge<int>> inputGraph,
            AdjacencyGraph<ExpansionTreeNode<Edge<int>>, Edge<ExpansionTreeNode<Edge<int>>>> expansionTree)
        {
            var parentNode = GetParent(queryGraph, expansionTree);
            return new List<object>();
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
        
        /// <summary>
        /// Helper method for algorithm 1
        /// </summary>
        /// <param name="extTreeNodesQueued"></param>
        /// <returns></returns>
        private ExpansionTreeNode<Edge<int>> GetNextNode(IDictionary<ExpansionTreeNode<Edge<int>>, GraphColor> extTreeNodesQueued)
        {
            foreach (var node in extTreeNodesQueued)
            {
                if (node.Value == GraphColor.White) continue;

                extTreeNodesQueued[node.Key] = GraphColor.White;
                return node.Key;
            }
            return null;
        }
    }
}

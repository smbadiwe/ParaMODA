using QuickGraph;
using System;
using System.Diagnostics;
using System.IO;

namespace MODA.Impl
{
    public partial class GraphProcessor
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
        /// Reads the content of the file whose name is supplied and constructs the nodes and edges for
        /// the given <paramref name="newGRaphInstance"/>. It is assumed the file uploaded contains edges
        /// (pairs of nodes connecting with each other).
        /// </summary>
        /// <param name="filename">The uploaded filename</param>
        /// <param name="newGraphInstance"></param>
        /// <returns>A string containing feedback of the processing</returns>
        public static UndirectedGraph<int, Edge<int>> LoadGraph(string filename)
        {
            var lines = File.ReadAllLines(filename);

            var newGraphInstance = new UndirectedGraph<int, Edge<int>>();
            //Parallelizing this actually made it take longer to process my datasets
            string[] tmp;
            int sourceNode;
            int targetNode;
            foreach (var line in lines)
            {
                if (line.StartsWith("#")) continue;
                tmp = line.Split(new string[] { " ", "\t" }, StringSplitOptions.None);
                sourceNode = Convert.ToInt32(tmp[0]);
                targetNode = Convert.ToInt32(tmp[1]);

                newGraphInstance.AddVerticesAndEdge(new Edge<int>(sourceNode, targetNode));
            }
            return newGraphInstance;
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

    }
}

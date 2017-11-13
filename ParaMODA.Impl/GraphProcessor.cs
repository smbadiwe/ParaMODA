using QuickGraph;
using System;
using System.Diagnostics;
using System.IO;

namespace ParaMODA.Impl
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
        private static string LoadGraph<T>(string filename, UndirectedGraph<T> newGraphInstance)
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
        /// <returns>A string containing feedback of the processing</returns>
        public static UndirectedGraph<int> LoadGraph(string filename, bool isQueryGraph = false)
        {
            var lines = File.ReadAllLines(filename);

            UndirectedGraph<int> newGraphInstance;
            if (isQueryGraph)
            {
                newGraphInstance = new QueryGraph(Path.GetFileNameWithoutExtension(filename))
                {
                };
            }
            else
            {
                newGraphInstance = new UndirectedGraph<int>();
            }
            //Parallelizing this actually made it take longer to process my datasets
            string[] tmp;
            foreach (var line in lines)
            {
                if (line.StartsWith("#")) continue;
                tmp = line.Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length < 2) continue;
                
                //newGraphInstance.AddVerticesAndEdge(new Edge<int>(tmp[0], tmp[1]));
                newGraphInstance.AddVerticesAndEdge(int.Parse(tmp[0]), int.Parse(tmp[1]));
            }
            return newGraphInstance;
        }

    }
}

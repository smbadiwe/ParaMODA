using QuickGraph;
using System;
using System.IO;

namespace MODA.Impl
{
    public class GraphLoader
    {
        /// <summary>
        /// Reads the content of the file whose name is supplied and constructs the nodes and edges for
        /// the given <paramref name="newGRaphInstance"/>. It is assumed the file uploaded contains edges
        /// (pairs of nodes connecting with each other).
        /// </summary>
        /// <param name="filename">The uploaded filename</param>
        /// <returns></returns>
        public static UndirectedGraph<T, Edge<T>> LoadGraph<T>(string filename)
        {
            var lines = File.ReadAllLines(filename);

            //Parallelizing this actually made it take longer to process my datasets
            string[] tmp;
            T temp_node;
            T temp_node2;
            var newGraphInstance = new UndirectedGraph<T, Edge<T>>();
            foreach (var line in lines)
            {
                if (line.StartsWith("#")) continue;
                tmp = line.Split(new string[] { " ", "\t" }, StringSplitOptions.None);
                temp_node = (T)Convert.ChangeType(tmp[0], typeof(T));
                temp_node2 = (T)Convert.ChangeType(tmp[1], typeof(T));

                newGraphInstance.AddVerticesAndEdge(new Edge<T>(temp_node, temp_node2));
            }
            filename = new FileInfo(filename).Name;

            return newGraphInstance;
        }

    }
}

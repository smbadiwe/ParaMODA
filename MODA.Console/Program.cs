using MODA.Impl;
using MODA.Impl.Graphics;
using System.IO;

namespace MODA.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = "QueryGraph.txt"; // "Ecoli20141001CR_idx.txt";
            string graphFolder = @"C:\SOMA\Drive\MyUW\Research\Kim\remodaalgorithmimplementation";
            var dotFileFulllName = Path.Combine(graphFolder, filename);
            var newGraphInstance = GraphProcessor.LoadGraph(dotFileFulllName);

            Visualizer.Visualize(newGraphInstance, dotFileFulllName + ".dot");

            var nodeCount = newGraphInstance.VertexCount;
            var edgeCount = newGraphInstance.EdgeCount;
            var inv = (nodeCount * nodeCount) / edgeCount;
            var sparse = inv > 64;
            var invDeg = string.Format("{0} ({1} Graph)", inv, inv > 64 ? "Sparse" : "Dense");
            System.Console.WriteLine($"\tFile loaded: {Path.GetFileName(filename)}\n\nNumber of Edges:\t{edgeCount}\nNumber of Nodes:\t\t{nodeCount}\nInv Degree:\t{invDeg}\n\n");

            var frequentSubgraphs = new ModaAlgorithms().Algorithm1(newGraphInstance, 3);
            System.Console.ReadKey();
        }
    }
}

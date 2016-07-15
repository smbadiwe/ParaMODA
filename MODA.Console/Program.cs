using MODA.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = "Ecoli20141001CR_idx.txt";
            string graphFolder = @"C:\SOMA\Drive\MyUW\Research\Kim\remodaalgorithmimplementation";
            var newGraphInstance = GraphProcessor.LoadGraph(Path.Combine(graphFolder, filename));

            var nodeCount = newGraphInstance.VertexCount;
            var edgeCount = newGraphInstance.EdgeCount;
            var inv = (nodeCount * nodeCount) / edgeCount;
            var sparse = inv > 64;
            var invDeg = string.Format("{0} ({1} Graph)", inv, inv > 64 ? "Sparse" : "Dense");
            System.Console.WriteLine($"\tFile loaded: {Path.GetFileName(filename)}\n\nNumber of lines in file:\t{edgeCount}\nNumber of nodes:\t\t{nodeCount}\nInv Degree:\t{invDeg}");

            var frequentSubgraphs = new ModaAlgorithms().Algorithm1(newGraphInstance, 4);
            System.Console.ReadKey();
        }
    }
}

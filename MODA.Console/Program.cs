using MODA.Impl;
using MODA.Impl.Graphics;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MODA.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string graphFolder = args[0]; // @"C:\SOMA\Drive\MyUW\Research\Kim\remodaalgorithmimplementation";
                string filename = args[1]; // "QueryGraph.txt"; // "Ecoli20141001CR_idx.txt";
                var dotFileFulllName = Path.Combine(graphFolder, filename);
                int subGraphSize = int.Parse(args[2]);
                int numIterations = int.Parse(args[3]);

                System.Console.WriteLine("Network File: {0}\nSub-graph Size: {1}\nNumber of Iterations: {2}\n", dotFileFulllName, subGraphSize, numIterations);

                var sb = new StringBuilder("Processing Graph...");
                sb.AppendFormat("Network File: {0}\nSub-graph Size: {1}\nNumber of Iterations: {2}\n", dotFileFulllName, subGraphSize, numIterations);
                sb.AppendLine("==========================================================================\n");
                for (int i = 0; i < numIterations; i++)
                {
                    var sw = Stopwatch.StartNew();
                    var newGraphInstance = GraphProcessor.LoadGraph(dotFileFulllName);

                    //Visualizer.Visualize(newGraphInstance, dotFileFulllName + ".dot");

                    var frequentSubgraphs = new ModaAlgorithms().Algorithm1(newGraphInstance, subGraphSize);
                    sw.Stop();
                    sb.AppendFormat("Iteration {0}: Time Taken: {1}ms\t Network: Nodes - {2}; Edges: {3}\n", (i + 1), sw.ElapsedMilliseconds.ToString("N"), newGraphInstance.VertexCount, newGraphInstance.EdgeCount);
                    System.Console.WriteLine("Iteration {0}: Time Taken: {1}ms\n", (i + 1), sw.ElapsedMilliseconds.ToString("N"));
                    sb.AppendLine("-------------------------------------------");
                    foreach (var queryGraph in frequentSubgraphs)
                    {
                        sb.AppendFormat("\tSub-graph: {0}\t Mappings: {1}\n", queryGraph.Key.AsString(), queryGraph.Value);
                    }
                    newGraphInstance = null;
                    frequentSubgraphs = null;
                    sb.AppendLine();
                }
                File.WriteAllText(dotFileFulllName + ".puo", sb.ToString());
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }
            System.Console.WriteLine("Done!");
            System.Console.ReadKey();
        }
    }
}

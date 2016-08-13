using MODA.Impl;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MODA.Console
{
    public class MODATest
    {
        internal static void Run(string[] args)
        {
            try
            {
                string graphFolder = args[0]; // @"C:\SOMA\Drive\MyUW\Research\Kim\remodaalgorithmimplementation";
                string filename = args[1]; // "QueryGraph.txt"; // "Ecoli20141001CR_idx.txt";
                var dotFileFulllName = Path.Combine(graphFolder, filename);
                int subGraphSize = int.Parse(args[2]);
                int vertexCountDividend = int.Parse(args[3]);

                var sb = new StringBuilder("Processing Graph...");
                sb.AppendFormat("Network File: {0}\nSub-graph Size: {1}\n", dotFileFulllName, subGraphSize);
                sb.AppendLine("==============================================================\n");
                System.Console.WriteLine(sb);
                sb.Clear();

                var sw = Stopwatch.StartNew();
                var newGraphInstance = GraphProcessor.LoadGraph(dotFileFulllName);
                System.Console.WriteLine("Iteration {0}:\t Network: Nodes - {1}; Edges: {2}\n", 1, newGraphInstance.VertexCount, newGraphInstance.EdgeCount);

                //Visualizer.Visualize(newGraphInstance, dotFileFulllName + ".dot");
                ModaAlgorithms.VertexCountDividend = vertexCountDividend;
                var frequentSubgraphs = ModaAlgorithms.Algorithm1(newGraphInstance, subGraphSize);
                sw.Stop();
                int totalMappings = 0;
                foreach (var queryGraph in frequentSubgraphs)
                {
                    sb.AppendFormat("\tSub-graph: {0}\t Mappings: {1}\n", queryGraph.Key.AsString(), queryGraph.Value);
                    totalMappings += queryGraph.Value;
                }
                sb.AppendFormat("\nTime Taken: {0} ({1}ms)\t Network: Nodes - {2}; Edges: {3}; Total Mappings found: {4}\n", sw.Elapsed.ToString(), sw.ElapsedMilliseconds.ToString("N"), newGraphInstance.VertexCount, newGraphInstance.EdgeCount, totalMappings);
                sb.AppendLine("-------------------------------------------\n");
                newGraphInstance = null;
                frequentSubgraphs = null;

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

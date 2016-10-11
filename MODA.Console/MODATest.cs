using MODA.Impl;
using MODA.Impl.Graphics;
using QuickGraph;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MODA.Console
{
    public class MODATest
    {
        //MODA.Console "C:\SOMA\Drive\MyUW\Research\Kim\remodaalgorithmimplementation" "Ecoli20141001CR_idx.txt" 3 1
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
                //ModaAlgorithms.VertexCountDividend = vertexCountDividend;
                var frequentSubgraphs = ModaAlgorithms.Algorithm1(newGraphInstance, subGraphSize);
                sw.Stop();
                int totalMappings = 0;
                foreach (var queryGraph in frequentSubgraphs)
                {
                    sb.AppendFormat("\tSub-graph: {0}\t Mappings: {1}\n", queryGraph.Key.AsString(), queryGraph.Value.Count);
                    totalMappings += queryGraph.Value.Count;
                }
                sb.AppendFormat("\nTime Taken: {0} ({1}ms)\t Network: Nodes - {2}; Edges: {3}; Total Mappings found: {4}\n", sw.Elapsed.ToString(), sw.ElapsedMilliseconds.ToString("N"), newGraphInstance.VertexCount, newGraphInstance.EdgeCount, totalMappings);
                sb.AppendLine("-------------------------------------------\n");
                newGraphInstance = null;
                frequentSubgraphs = null;
                System.Console.WriteLine(sb);

                File.WriteAllText(dotFileFulllName + ".puo", sb.ToString());
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }
            System.Console.WriteLine("Done!");
            System.Console.ReadKey();
        }

        internal static void GenerateExpansionTreeNodes()
        {
            int subgraphSize = 5;
            var builder = new ExpansionTreeBuilder<Edge<string>>(subgraphSize);
            builder.Build();

            var outputFolder = Path.Combine(System.Environment.CurrentDirectory, "ExpTree" + subgraphSize);
            if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);
            System.Console.WriteLine("Output Folder: {0} ", outputFolder);

            Visualizer.Visualize(builder.ExpansionTree, Path.Combine(outputFolder, "ExpansionTree_5.dot"));

            //do
            //{
            //    var node = ModaAlgorithms.GetNextNode(builder.VerticesSorted);
            //    var qGraph = node?.QueryGraph;
            //    if (qGraph == null) break;

            //    Visualizer.Visualize(qGraph, Path.Combine(outputFolder, node.NodeName + ".dot"));

            //    System.Console.WriteLine("\tDrawn node {0} ", node.NodeName);
            //    //Check for complete-ness; if complete, break
            //    //  A Complete graph of n nodes has n(n-1)/2 edges
            //    if (qGraph.EdgeCount == ((subgraphSize * (subgraphSize - 1)) / 2))
            //    {
            //        node = null;
            //        qGraph = null;
            //        break;
            //    }
            //    qGraph = null;
            //}
            //while (true);

            System.Console.WriteLine("Done!");
            System.Console.ReadKey();
        }
    }
}

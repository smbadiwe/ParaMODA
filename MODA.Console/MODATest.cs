using MODA.Impl;
using MODA.Impl.Graphics;
using QuickGraph;
using System.Diagnostics;
using System.IO;
using System;
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
                if (args == null || args.Length != 3)
                {
                    System.Console.ForegroundColor = System.ConsoleColor.Red;
                    //System.Console.WriteLine("Error. Use the command:\nMODA.Console  <graphFolder> <filename> <subGraphSize>\nSee ReadMe.txt file for more details.");
                    try
                    {
                        System.Console.WriteLine(File.ReadAllText("ReadMe.txt"));
                    }
                    catch
                    {
                        System.Console.WriteLine("Error. Use the command:\nMODA.Console  <graphFolder> <filename> <subGraphSize>\nSee ReadMe.txt file for more details.");
                    }
                    System.Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
                string graphFolder = args[0]; // @"C:\SOMA\Drive\MyUW\Research\Kim\remodaalgorithmimplementation";
                string filename = args[1]; // "QueryGraph.txt"; // "Ecoli20141001CR_idx.txt";
                var inputGraphFile = Path.Combine(graphFolder, filename);
                int subGraphSize;
                if (!int.TryParse(args[2], out subGraphSize))
                {
                    throw new ArgumentException("Invalid input for <subGraphSize> argument (arg[2])");
                }
                //int vertexCountDividend = int.Parse(args[3]);

                var sb = new StringBuilder("Processing Graph...");
                sb.AppendFormat("Network File: {0}\nSub-graph Size: {1}\n", inputGraphFile, subGraphSize);
                sb.AppendLine("==============================================================\n");
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine(sb);
                sb.Clear();

                var inputGraph = GraphProcessor.LoadGraph(inputGraphFile);
                UndirectedGraph<string, Edge<string>> queryGraph = null;
                System.Console.WriteLine("Do you have a particular size {0} query graph in mind? Y/N", subGraphSize);
                string resp = System.Console.ReadLine();
                if (resp == "y" || resp == "Y")
                {
                    while (true)
                    {
                        System.Console.WriteLine("Enter the path to the query graph file");
                        string queryGraphFile = System.Console.ReadLine();
                        queryGraph = GraphProcessor.LoadGraph(queryGraphFile);
                        if (queryGraph.VertexCount != subGraphSize)
                        {
                            System.Console.WriteLine("The specified subgraph size does not match with the query graph size. \nDo you want to use the size of the specified query graph instead? Y/N");
                            resp = System.Console.ReadLine();
                            if (resp == "y" || resp == "Y")
                            {
                                subGraphSize = queryGraph.VertexCount;
                                break;
                            }
                            // else contiue;
                        }
                        else // we're good. So,
                        {
                            break;
                        }
                    }
                }
                if (subGraphSize >= inputGraph.VertexCount)
                {
                    throw new NotSupportedException("The specified subgraaph size is too large.");
                }
                System.Console.WriteLine("Input Graph (G): Nodes - {0}; Edges: {1}\n", inputGraph.VertexCount, inputGraph.EdgeCount);
                if (queryGraph != null)
                {
                    System.Console.WriteLine("Query Graph (H): Nodes - {0}; Edges: {1}\n", queryGraph.VertexCount, queryGraph.EdgeCount);
                }

                ModaAlgorithms.BuildTree(queryGraph, subGraphSize);

                var sw = Stopwatch.StartNew();
                //Visualizer.Visualize(newGraphInstance, dotFileFulllName + ".dot");
                //ModaAlgorithms.VertexCountDividend = vertexCountDividend;
                var frequentSubgraphs = ModaAlgorithms.Algorithm1(inputGraph, subGraphSize);
                sw.Stop();
                int totalMappings = 0;
                sb.Append("\nCompleted. Result Summary\n");
                sb.AppendLine("-------------------------------------------\n");
                foreach (var qGraph in frequentSubgraphs)
                {
                    sb.AppendFormat("\tSub-graph: {0}\t Mappings: {1}\n", qGraph.Key.AsString(), qGraph.Value.Count);
                    totalMappings += qGraph.Value.Count;
                }
                sb.AppendFormat("\nTime Taken: {0} ({1}ms)\nNetwork: Nodes - {2}; Edges: {3};\nTotal Mappings found: {4}\nSubgraph Size: {5}\n", sw.Elapsed, sw.ElapsedMilliseconds.ToString("N"), inputGraph.VertexCount, inputGraph.EdgeCount, totalMappings, subGraphSize);
                sb.AppendLine("-------------------------------------------\n");
                inputGraph = null;
                frequentSubgraphs = null;
                System.Console.ForegroundColor = ConsoleColor.Blue;
                System.Console.WriteLine(sb);

                try
                {
                    File.WriteAllText(inputGraphFile + ".OUT", sb.ToString());
                }
                catch { }

                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.WriteLine("Done! Press any key to exit.");
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine(ex);
                System.Console.ForegroundColor = ConsoleColor.White;
            }
            System.Console.ReadKey();
        }

        //internal static void GenerateExpansionTreeNodes()
        //{
        //    int subgraphSize = 5;
        //    var builder = new ExpansionTreeBuilder<Edge<string>>(subgraphSize);
        //    builder.Build();

        //    var outputFolder = Path.Combine(System.Environment.CurrentDirectory, "ExpTree" + subgraphSize);
        //    if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);
        //    System.Console.WriteLine("Output Folder: {0} ", outputFolder);

        //    Visualizer.Visualize(builder.ExpansionTree, Path.Combine(outputFolder, "ExpansionTree_5.dot"));

        //    //do
        //    //{
        //    //    var node = ModaAlgorithms.GetNextNode(builder.VerticesSorted);
        //    //    var qGraph = node?.QueryGraph;
        //    //    if (qGraph == null) break;

        //    //    Visualizer.Visualize(qGraph, Path.Combine(outputFolder, node.NodeName + ".dot"));

        //    //    System.Console.WriteLine("\tDrawn node {0} ", node.NodeName);
        //    //    //Check for complete-ness; if complete, break
        //    //    //  A Complete graph of n nodes has n(n-1)/2 edges
        //    //    if (qGraph.EdgeCount == ((subgraphSize * (subgraphSize - 1)) / 2))
        //    //    {
        //    //        node = null;
        //    //        qGraph = null;
        //    //        break;
        //    //    }
        //    //    qGraph = null;
        //    //}
        //    //while (true);

        //    System.Console.WriteLine("Done!");
        //    System.Console.ReadKey();
        //}
    }
}

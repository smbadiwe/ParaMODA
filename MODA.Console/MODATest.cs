using MODA.Impl;
using QuickGraph;
using System.Diagnostics;
using System.IO;
using System;
using System.Text;
using StdConsole = System.Console;
using MODA.Impl.Graphics;

namespace MODA.Console
{
    public class MODATest
    {
        //MODA.Console "C:\SOMA\Drive\MyUW\Research\Kim\remodaalgorithmimplementation" "Ecoli20141001CR_idx.txt" 3 1
        internal static void Run(string[] args)
        {
            try
            {
                if (args == null || args.Length != 5)
                {
                    StdConsole.ForegroundColor = ConsoleColor.Red;
                    //StdConsole.WriteLine("Error. Use the command:\nMODA.Console  <graphFolder> <filename> <subGraphSize>\nSee ReadMe.txt file for more details.");
                    try
                    {
                        StdConsole.WriteLine(File.ReadAllText("ReadMe.txt"));
                    }
                    catch
                    {
                        StdConsole.WriteLine("Error. Use the command:\nMODA.Console  <graphFolder> <filename> <subGraphSize> <threshold> <getOnlyMappingCounts>\nSee ReadMe.txt file for more details.");
                    }
                    StdConsole.ForegroundColor = ConsoleColor.White;
                    return;
                }
                string graphFolder = args[0]; // @"C:\SOMA\Drive\MyUW\Research\Kim\remodaalgorithmimplementation";
                string filename = args[1]; // "QueryGraph.txt"; // "Ecoli20141001CR_idx.txt";
                var inputGraphFile = Path.Combine(graphFolder, filename);
                string queryGraphFile = null;
                int subGraphSize;
                if (!int.TryParse(args[2], out subGraphSize))
                {
                    throw new ArgumentException("Invalid input for <subGraphSize> argument (arg[2])");
                }
                int threshold;
                if (!int.TryParse(args[3], out threshold))
                {
                    throw new ArgumentException("Invalid input for <threshold> argument (arg[3])");
                }
                string getOnlyMappingCounts = args[4];
                if (getOnlyMappingCounts == "y" || getOnlyMappingCounts == "Y")
                {
                    ModaAlgorithms.GetOnlyMappingCounts = true;
                }
                var sb = new StringBuilder("Processing Graph...");
                sb.AppendFormat("Network File: {0}\nSub-graph Size: {1}\n", inputGraphFile, subGraphSize);
                sb.AppendLine("==============================================================\n");
                StdConsole.WriteLine(sb);
                sb.Clear();

                var inputGraph = GraphProcessor.LoadGraph(inputGraphFile);
                UndirectedGraph<string, Edge<string>> queryGraph = null;
                StdConsole.WriteLine("Do you have a particular size {0} query graph in mind? Y/N", subGraphSize);
                string resp = StdConsole.ReadLine();
                if (resp == "y" || resp == "Y")
                {
                    while (true)
                    {
                        StdConsole.WriteLine("Enter the (relative or absolute) path to the query graph file");
                        queryGraphFile = StdConsole.ReadLine();
                        queryGraph = GraphProcessor.LoadGraph(queryGraphFile);
                        if (queryGraph.VertexCount != subGraphSize)
                        {
                            StdConsole.WriteLine("The specified subgraph size does not match with the query graph size. \nDo you want to use the size of the specified query graph instead? Y/N");
                            resp = StdConsole.ReadLine();
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
                StdConsole.WriteLine("Input Graph (G): Nodes - {0}; Edges: {1}\n", inputGraph.VertexCount, inputGraph.EdgeCount);
                if (queryGraph != null)
                {
                    StdConsole.WriteLine("Query Graph (H): Nodes - {0}; Edges: {1}\n", queryGraph.VertexCount, queryGraph.EdgeCount);
                }

                StdConsole.WriteLine("Do you want to generate an image of the input (and query) graph(s)? Y/N\nIf Y, you'll need to provide the path to dod.exe program on your machine");
                resp = StdConsole.ReadLine();
                if (resp == "y" || resp == "Y")
                {
                    StdConsole.WriteLine("Enter the path of the dot.exe program on your machine:");
                    resp = StdConsole.ReadLine(); //the dot program's filename, including the path
                    Visualizer.Visualize(inputGraph, resp, inputGraphFile + ".dot");
                    if (queryGraph != null) // => queryGraphFile has a value
                    {
                        Visualizer.Visualize(queryGraph, resp, queryGraphFile + ".dot");
                    }
                }
                StdConsole.ForegroundColor = ConsoleColor.Green;

                ModaAlgorithms.BuildTree(queryGraph, subGraphSize);
                ModaAlgorithms.Threshold = threshold;

                var sw = Stopwatch.StartNew();

                var frequentSubgraphs = ModaAlgorithms.Algorithm1(inputGraph, subGraphSize);

                sw.Stop();
                long totalMappings = 0;
                sb.Append("\nCompleted. Result Summary\n");
                sb.AppendLine("-------------------------------------------\n");
                if (ModaAlgorithms.GetOnlyMappingCounts)
                {
                    foreach (var qGraph in frequentSubgraphs)
                    {
                        int count = (int)qGraph.Value;
                        sb.AppendFormat("\tSub-graph: {0}\t Mappings: {1}\n", qGraph.Key.AsString(), count);
                        totalMappings += count;
                    }
                }
                else
                {
                    foreach (var qGraph in frequentSubgraphs)
                    {
                        int count = ((System.Collections.Generic.List<Mapping>)qGraph.Value).Count;
                        sb.AppendFormat("\tSub-graph: {0}\t Mappings: {1}\n", qGraph.Key.AsString(), count);
                        totalMappings += count;
                    }
                }
                sb.AppendFormat("\nTime Taken: {0} ({1}ms)\nNetwork: Nodes - {2}; Edges: {3};\nTotal Mappings found: {4}\nSubgraph Size: {5}\n", sw.Elapsed, sw.ElapsedMilliseconds.ToString("N"), inputGraph.VertexCount, inputGraph.EdgeCount, totalMappings, subGraphSize);
                sb.AppendLine("-------------------------------------------\n");
                inputGraph = null;
                frequentSubgraphs = null;
                StdConsole.ForegroundColor = ConsoleColor.Blue;
                StdConsole.WriteLine(sb);

                try
                {
                    File.WriteAllText(inputGraphFile + ".OUT", sb.ToString());
                }
                catch { }

                StdConsole.ForegroundColor = ConsoleColor.White;
                StdConsole.WriteLine("Done! Press any key to exit.");
            }
            catch (Exception ex)
            {
                StdConsole.ForegroundColor = ConsoleColor.Red;
                StdConsole.WriteLine(ex);
                StdConsole.ForegroundColor = ConsoleColor.White;
            }
            StdConsole.ReadKey();
        }

        //internal static void GenerateExpansionTreeNodes()
        //{
        //    int subgraphSize = 5;
        //    var builder = new ExpansionTreeBuilder<Edge<string>>(subgraphSize);
        //    builder.Build();

        //    var outputFolder = Path.Combine(System.Environment.CurrentDirectory, "ExpTree" + subgraphSize);
        //    if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);
        //    StdConsole.WriteLine("Output Folder: {0} ", outputFolder);

        //    Visualizer.Visualize(builder.ExpansionTree, Path.Combine(outputFolder, "ExpansionTree_5.dot"));

        //    //do
        //    //{
        //    //    var node = ModaAlgorithms.GetNextNode(builder.VerticesSorted);
        //    //    var qGraph = node?.QueryGraph;
        //    //    if (qGraph == null) break;

        //    //    Visualizer.Visualize(qGraph, Path.Combine(outputFolder, node.NodeName + ".dot"));

        //    //    StdConsole.WriteLine("\tDrawn node {0} ", node.NodeName);
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

        //    StdConsole.WriteLine("Done!");
        //    StdConsole.ReadKey();
        //}
    }
}

using MODA.Impl;
using System.Diagnostics;
using System.IO;
using System;
using System.Text;
using System.Collections.Generic;

namespace ParaMODA
{
    public class MODATest
    {
        private const string OUTPUT_DIR = "MappingOutputs";
        private static StringBuilder sb = new StringBuilder("Processing Graph...");
        internal static void Run(string[] args)
        {
            var fgColor = Console.ForegroundColor;
            try
            {
                BaseVerb invokedVerbInstance = null;

                var parser = new CommandLine.Parser(with =>
                {
                    with.HelpWriter = Console.Error;
                    with.ParsingCulture = System.Globalization.CultureInfo.InvariantCulture;
                    with.IgnoreUnknownArguments = true;
                });
                if (!parser.ParseArguments(args, new Options(),
                  (verb, subOptions) =>
                  {
                      // if parsing succeeds the verb name and correct instance
                      // will be passed to onVerbCommand delegate (string,object)
                      switch (verb)
                      {
                          case "runone":
                              invokedVerbInstance = subOptions as RunOneVerb;
                              break;
                          case "runmany":
                              var many = subOptions as RunManyVerb;
                              if (string.IsNullOrWhiteSpace(many.QueryGraphFolder)
                                  && many.QueryGraphFiles == null)
                              {
                                  Console.WriteLine("One of the options -f (--folder) or -q (--querygraphs) has to be set.\n\n");
                                  many.GetUsage();
                                  Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                              }
                              else
                              {
                                  invokedVerbInstance = many;
                              }
                              break;
                          case "runall":
                              invokedVerbInstance = subOptions as RunAllVerb;
                              break;
                          default:
                              break;
                      }
                  }))
                {
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }

                #region Process input parameters
                if (invokedVerbInstance == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error. For more details, run the command: MODA.Console --help");

                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                ModaAlgorithms.UseModifiedGrochow = invokedVerbInstance.UseGrochow == false;

                var inputGraph = GraphProcessor.LoadGraph(invokedVerbInstance.InputGraphFile);
                Console.WriteLine("Input Graph (G): Nodes - {0}; Edges: {1}\n", inputGraph.VertexCount, inputGraph.EdgeCount);

                if (invokedVerbInstance.SaveOutputs && !Directory.Exists(OUTPUT_DIR))
                {
                    Directory.CreateDirectory(OUTPUT_DIR);
                }
                RunOneVerb runOneVerb = invokedVerbInstance as RunOneVerb;
                if (runOneVerb != null)
                {
                    #region Run One
                    QueryGraph queryGraph = GraphProcessor.LoadGraph(runOneVerb.QueryGraphFile, true) as QueryGraph;
                    Process(runOneVerb, inputGraph, queryGraph);
                    return;
                    #endregion
                }

                RunManyVerb runManyVerb = invokedVerbInstance as RunManyVerb;
                if (runManyVerb != null)
                {
                    #region Run Many
                    IList<QueryGraph> queryGraphs;
                    if (string.IsNullOrWhiteSpace(runManyVerb.QueryGraphFolder))
                    {
                        queryGraphs = new QueryGraph[runManyVerb.QueryGraphFiles.Length];
                        foreach (var gFile in runManyVerb.QueryGraphFiles)
                        {
                            QueryGraph qGraph = GraphProcessor.LoadGraph(gFile, true) as QueryGraph;

                            queryGraphs.Add(qGraph);
                        }
                    }
                    else
                    {
                        var files = Directory.GetFiles(runManyVerb.QueryGraphFolder);
                        queryGraphs = new QueryGraph[files.Length];
                        foreach (var gFile in files)
                        {
                            QueryGraph qGraph = GraphProcessor.LoadGraph(gFile, true) as QueryGraph;

                            queryGraphs.Add(qGraph);
                        }
                    }
                    foreach (var qGraph in queryGraphs)
                    {
                        Process(runManyVerb, inputGraph, qGraph);
                    }
                    return;
                    #endregion
                }

                RunAllVerb runAllVerb = invokedVerbInstance as RunAllVerb;
                if (runAllVerb != null)
                {
                    #region Run All
                    if (runAllVerb.SubgraphSize < 3 || runAllVerb.SubgraphSize > 5)
                    {
                        Console.WriteLine("Invalid input for -s (--size; subgraph size). Program currently supports sizes from 3 to 5.\n\n");
                        runAllVerb.GetUsage();
                        Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                    }
                    if (runAllVerb.SubgraphSize >= inputGraph.VertexCount)
                    {
                        Console.WriteLine("The specified subgraaph size is too large.\n\n\n");
                        runAllVerb.GetUsage();
                        Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                    }
                    ModaAlgorithms.BuildTree(runAllVerb.SubgraphSize);
                    Process(runAllVerb, inputGraph, subGraphSize: runAllVerb.SubgraphSize, saveTempMappingsToDisk: runAllVerb.SaveTempMappingsToDisk);
                    return;
                    #endregion
                }

                Console.ForegroundColor = ConsoleColor.Green;

                #endregion

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
            }
            Console.ForegroundColor = fgColor;
        }

        /// <summary>
        /// The understanding is that if <paramref name="queryGraph"/> is null, then <paramref name="subGraphSize"/> must be supplied.
        /// The converse is true too: if <paramref name="queryGraph"/> is supplied, then there's no need for <paramref name="subGraphSize"/>.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="inputGraph"></param>
        /// <param name="queryGraph"></param>
        /// <param name="subGraphSize"></param>
        /// <param name="saveTempMappingsToDisk">This will only apply when we're using the runall option (MODA), in which case this will tell us how to store the mappings found at an expansion tree node for reuse in a child node</param>
        private static void Process(BaseVerb options, QuickGraph.UndirectedGraph<int> inputGraph, QueryGraph queryGraph = null, int subGraphSize = -1, bool saveTempMappingsToDisk = false)
        {
            if (queryGraph != null)
            {
                Console.WriteLine("Query Graph (H): Nodes - {0}; Edges: {1}\n", queryGraph.VertexCount, queryGraph.EdgeCount);
            }
            if (saveTempMappingsToDisk == false)
            {
                var sw = Stopwatch.StartNew();

                var frequentSubgraphs = ModaAlgorithms.Algorithm1(inputGraph, queryGraph, subGraphSize, options.Threshold);

                sw.Stop();

                #region Process output
                int totalMappings = 0;
                sb.Append("\nCompleted. Result Summary\n");
                sb.AppendLine("-------------------------------------------\n");
                if (options.SaveOutputs == false)
                {
                    foreach (var qGraph in frequentSubgraphs)
                    {
                        if (qGraph.Value == null)
                        {
                            sb.AppendFormat("\tSub-graph: {0}\t Mappings: 0\t Is Frequent Subgraph? false\n", qGraph.Key.ToString());
                        }
                        else
                        {
                            int count = qGraph.Value.Count; //int.Parse(qGraph.Value.Split('#')[0]); // 
                            sb.AppendFormat("\tSub-graph: {0}\t Mappings: {1}\t Is Frequent Subgraph? {2}\n", qGraph.Key.ToString(), count, qGraph.Key.IsFrequentSubgraph);
                            totalMappings += count;
                        }
                    }
                }
                else // if (options.SaveOutputs == true)
                {
                    foreach (var qGraph in frequentSubgraphs)
                    {
                        var fileSb = new StringBuilder();
                        if (qGraph.Value == null)
                        {
                            sb.AppendFormat("\tSub-graph: {0}\t Is Frequent Subgraph? false\n", qGraph.Key.ToString());
                        }
                        else
                        {
                            int count = qGraph.Value.Count; //int.Parse(qGraph.Value.Split('#')[0]); // 
                            sb.AppendFormat("\tSub-graph: {0}\t Mappings: {1}\t Is Frequent Subgraph? {2}\n", qGraph.Key.ToString(), count, qGraph.Key.IsFrequentSubgraph);
                            foreach (var mapping in qGraph.Value)
                            {
                                fileSb.AppendLine(mapping.GetMappedNodes());
                            }
                            totalMappings += count;
                            try
                            {
                                File.WriteAllText(Path.Combine(OUTPUT_DIR, qGraph.Key.Label + ".txt"), fileSb.ToString());
                            }
                            catch (Exception ex)
                            {
                                Console.Error.WriteLine("Failed to write file containing mappings found for querygraph {0} to disk.\n\n{1}", qGraph.Key.Label, ex);
                            }
                        }
                    }
                }
                sb.AppendFormat("\nTime Taken: {0} ({1}ms)\nNetwork: Nodes - {2}; Edges: {3};\nTotal Mappings found: {4}\nSubgraph Size: {5}\n", sw.Elapsed, sw.ElapsedMilliseconds.ToString("N"), inputGraph.VertexCount, inputGraph.EdgeCount, totalMappings, subGraphSize);
                sb.AppendLine("-------------------------------------------\n");
                frequentSubgraphs.Clear();
                #endregion
            }
            else // if (saveTempMappingsToDisk == true)
            {
                var sw = Stopwatch.StartNew();

                var frequentSubgraphs = ModaAlgorithms.Algorithm1_C(inputGraph, queryGraph, subGraphSize, options.Threshold);

                sw.Stop();

                #region Process output
                int totalMappings = 0;
                sb.Append("\nCompleted. Result Summary\n");
                sb.AppendLine("-------------------------------------------\n");
                if (options.SaveOutputs == false)
                {
                    foreach (var qGraph in frequentSubgraphs)
                    {
                        if (qGraph.Value == null)
                        {
                            sb.AppendFormat("\tSub-graph: {0}\t Is Frequent Subgraph? false\n", qGraph.Key.ToString());
                        }
                        else
                        {
                            int count = int.Parse(qGraph.Value.Split('#')[0]);
                            sb.AppendFormat("\tSub-graph: {0}\t Mappings: {1}\t Is Frequent Subgraph? {2}\n", qGraph.Key.ToString(), count, qGraph.Key.IsFrequentSubgraph);
                            totalMappings += count;
                        }
                    }
                }
                else // if (options.SaveOutputs == true)
                {
                    foreach (var qGraph in frequentSubgraphs)
                    {
                        var fileSb = new StringBuilder();
                        if (qGraph.Value == null)
                        {
                            sb.AppendFormat("\tSub-graph: {0}\t Mappings: 0\t Is Frequent Subgraph? false\n", qGraph.Key.ToString());
                        }
                        else
                        {
                            int count = int.Parse(qGraph.Value.Split('#')[0]);
                            sb.AppendFormat("\tSub-graph: {0}\t Mappings: {1}\t Is Frequent Subgraph? {2}\n", qGraph.Key.ToString(), count, qGraph.Key.IsFrequentSubgraph);
                            //TODO: Big bug here. This line is so wrong. Redo!

                            var mappings = qGraph.Key.ReadMappingsFromFile(qGraph.Value);

                            foreach (var mapping in mappings)
                            {
                                fileSb.AppendLine(mapping.GetMappedNodes());
                            }
                            totalMappings += count;

                            try
                            {
                                File.WriteAllText(Path.Combine(OUTPUT_DIR, qGraph.Key.Label + ".txt"), fileSb.ToString());
                            }
                            catch (Exception ex)
                            {
                                Console.Error.WriteLine("Failed to write file containing mappings found for querygraph {0} to disk.\n\n{1}", qGraph.Key.Label, ex);
                            }
                        }
                    }
                }
                sb.AppendFormat("\nTime Taken: {0} ({1}ms)\nNetwork: Nodes - {2}; Edges: {3};\nTotal Mappings found: {4}\nSubgraph Size: {5}\n", sw.Elapsed, sw.ElapsedMilliseconds.ToString("N"), inputGraph.VertexCount, inputGraph.EdgeCount, totalMappings, subGraphSize);
                sb.AppendLine("-------------------------------------------\n");
                frequentSubgraphs.Clear();
                #endregion
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(sb);

            try
            {
                // Dump general result
                File.WriteAllText(Path.GetFileName(options.InputGraphFile) + ".out", sb.ToString());
            }
            catch { }
            sb.Clear();

        }
    }
}

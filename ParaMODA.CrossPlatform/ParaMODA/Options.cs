using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace ParaMODA
{
    [Verb("runone", HelpText = "Run program searching with JUST ONE query graph")]
    internal class RunOneVerb : BaseVerb
    {
        [Option('h', "querygraph", Required = true, HelpText = "The query graph file")]
        public string QueryGraphFile { get; set; }
    }

    [Verb("runmany", HelpText = "Run program searching with two or more query graph")]
    internal class RunManyVerb : BaseVerb
    {
        [Option('f', "folder", HelpText = "A folder containing all the subgraphs (as text files) you want to query. This may be more convenient that using the <q> (<querygraphs>) parameter")]
        public string QueryGraphFolder { get; set; }

        [Option('q', "querygraphs", Separator = ',', HelpText = "The list of files containing the query graphs. If more than one, separate with comma. If file name contains space, enclose in quotes.")]
        public string[] QueryGraphFiles { get; set; }
    }

    [Verb("runall", HelpText = "Run program searching for all possible subgraphs of the specified size. Currently supports sizes 3 to 6. More to come")]
    internal class RunAllVerb : BaseVerb
    {
        [Option('n', "size", Required = true, HelpText = "The subgraph size (i.e number of nodes)")]
        public int SubgraphSize { get; set; }

        [Option('d', "temptodisk", HelpText = "This will tell us how to store the mappings found at an expansion tree node for reuse in a child node")]
        public bool SaveTempMappingsToDisk { get; set; }
    }

    internal abstract class BaseVerb
    {
        /// <summary>
        /// Minimum number of mappings required to be found for a subgraph to be seen as a frequent subgraph
        /// </summary>
        [Option('t', "threshold", Default = 0, HelpText = "minimum number of mappings required to be found for a subgraph to be seen as a frequent subgraph")]
        public int Threshold { get; set; }

        /// <summary>
        /// The file containing the input graph. FUll or relative file path supported.
        /// </summary>
        [Option('g', "graph", Required = true, HelpText = "The file containing the input graph. FUll or relative file path supported.")]
        public string InputGraphFile { get; set; }

        /// <summary>
        /// If true, the output file containing the mappings for a query graph will be saved to disk
        /// </summary>
        [Option('s', "save", HelpText = "Boolean. If true, the output file containing the mappings for a query graph will be saved to disk")]
        public bool SaveOutputs { get; set; }

        /// <summary>
        /// If true, the program will use Grochow-Kellis' algorithm instead of the modification we proposed for mapping
        /// </summary>
        [Option('k', "gk", HelpText = "Boolean. If true, the program will use Grochow-Kellis' algorithm instead of the modification we proposed for mapping")]
        public bool UseGrochow { get; set; }

        ///// <summary>
        ///// Marking a property of type IParserState with ParserStateAttribute allows you to
        ///// receive an instance of ParserState (that contains a IList<ParsingError>).
        ///// This is equivalent from inheriting from CommandLineOptionsBase (of previous versions)
        ///// with the advantage to not propagating a type of the library.
        ///// </summary>
        //[ParserState]
        //public IParserState LastParserState { get; set; }

        public string GetUsage()
        {
            return ""; // HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}

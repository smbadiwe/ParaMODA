using QuickGraph;
using System.Collections.Generic;

namespace MODA.Impl
{
    public sealed class QueryGraph : UndirectedGraph<int>
    {
        public QueryGraph() : base()
        {

        }

        public QueryGraph(bool allowParralelEdges) : base(allowParralelEdges)
        {

        }
        
        /// <summary>
        /// A name to identify / refer to this query graph
        /// </summary>
        public string Label { get; set; }

        public bool IsFrequentSubgraph { get; set; }

        public IList<Mapping> ReadMappingsFromFile(string filename)
        {
            var mappings = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Mapping>>(Extensions.DecompressString(System.IO.File.ReadAllText(filename)));
            return mappings;
        }

        /// <summary>
        /// Write mappings to disk under this query graph. Returns the filename where it is written.
        /// The filename format used is this: $"{mappings.Count}#{Label}.ser"
        /// </summary>
        /// <param name="mappings"></param>
        /// <returns>filename where it is written</returns>
        public string WriteMappingsToFile(IList<Mapping> mappings)
        {
            var fileName = $"{mappings.Count}#{Label}.ser";
            System.IO.File.WriteAllText(fileName, Extensions.CompressString(Newtonsoft.Json.JsonConvert.SerializeObject(mappings)));
            return fileName;
        }

        public override int GetHashCode()
        {
            return Label.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Label.Equals(((QueryGraph)obj).Label);
        }
    }
}

using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl
{
    [Serializable]
    public class Map
    {
        public HashSet<Mapping> Mappings { get; set; }
        public UndirectedGraph<string, Edge<string>> QueryGraph { get; set; }
    }
}

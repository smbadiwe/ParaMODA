using QuickGraph;
using System;
using System.Collections.Generic;

namespace MODA.Impl
{
    
    public class Map
    {
        public List<Mapping> Mappings { get; set; }
        public UndirectedGraph<string, Edge<string>> QueryGraph { get; set; }
    }
}

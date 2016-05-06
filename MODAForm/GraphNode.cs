using System;
using System.Collections.Generic;
using System.Text;

namespace Motif1
{
    class GraphNode:Node
    {
        Edge newEdge;
        //unsafe GraphNode* upArrow;

        public GraphNode()
        {
            newEdge=new Edge(0,0);
        }
        public GraphNode(Edge NewEdge)
        {
            newEdge = NewEdge;
        }

        //List<Mapping> supportMaps(List<Mapping> maps)
        //{
        // //   for(int i=0;
        //}
    }
}

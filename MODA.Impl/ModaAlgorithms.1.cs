using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        /// <summary>
        /// Algo 1: Find subgraph frequency
        /// </summary>
        /// <param name="inputGraph"></param>
        /// <param name="subgraphSize"></param>
        /// <param name="thresholdValue"></param>
        /// <returns>Fg, frequent subgraph list</returns>
        public List<UndirectedGraph<int, Edge<int>>> Algorithm1(UndirectedGraph<int, Edge<int>> inputGraph, int subgraphSize, int thresholdValue = 0)
        {
            var builder = new ExpansionTreeBuilder<Edge<int>>(subgraphSize);
            builder.Build();

            var frequentSubgraphs = new List<UndirectedGraph<int, Edge<int>>>();
            var allMappings = new Dictionary<UndirectedGraph<int, Edge<int>>, List<Dictionary<int, int>>>();
            do
            {
                var qGraph = GetNextNode(builder.VerticesSorted).QueryGraph;
                List<Dictionary<int, int>> mappings;
                if (qGraph.EdgeCount == (subgraphSize - 1))
                {
                    //TODO: Mapping module - Grockow & Kellis
                    mappings = Algorithm2(qGraph, inputGraph);
                }
                else
                {
                    //TODO: Enumeration moodule - 
                    mappings = Algorithm3(qGraph, inputGraph, builder.ExpansionTree);
                }

                //TODO: Do we need to save to disk?
                allMappings.Add(qGraph, mappings); //Save mappings

                if (mappings.Count > thresholdValue)
                {
                    frequentSubgraphs.Add(qGraph);
                }

                //Check for complete-ness; if complete, break
                //  A Complete graph of n nodes has n(n-1)/2 edges
                if (qGraph.EdgeCount == ((subgraphSize * (subgraphSize - 1)) / 2))
                {
                    break;
                }
            }
            while (true);

            return frequentSubgraphs;
        }
        
        /// <summary>
        /// Helper method for algorithm 1
        /// </summary>
        /// <param name="extTreeNodesQueued"></param>
        /// <returns></returns>
        private ExpansionTreeNode<Edge<int>> GetNextNode(IDictionary<ExpansionTreeNode<Edge<int>>, GraphColor> extTreeNodesQueued)
        {
            foreach (var node in extTreeNodesQueued)
            {
                if (node.Value == GraphColor.White) continue;

                extTreeNodesQueued[node.Key] = GraphColor.White;
                return node.Key;
            }
            return null;
        }
    }
}

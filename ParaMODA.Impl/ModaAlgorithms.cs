using System.Runtime.CompilerServices;

namespace ParaMODA.Impl
{
    public partial class ModaAlgorithms
    {
        private static ExpansionTreeBuilder<int> _builder;
        internal static MappingNodesComparer MappingNodesComparer;
        static ModaAlgorithms()
        {
            MappingNodesComparer = new MappingNodesComparer();
        }
        
        /// <summary>
        /// If true, the program will use my modified Grochow's algorithm (Algo 2)
        /// </summary>
        public static bool UseModifiedGrochow { get; set; }
        
        public static void BuildTree(int subgraphSize)
        {
            _builder = new ExpansionTreeBuilder<int>(subgraphSize);
            _builder.Build();
        }

        /// <summary>
        /// Helper method for algorithm 1
        /// </summary>
        /// <param name="extTreeNodesQueued"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ExpansionTreeNode GetNextNode()
        {
            if (_builder.VerticesSorted.Count > 0)
            {
                return _builder.VerticesSorted.Dequeue();
            }
            return null;
        }
    }
}

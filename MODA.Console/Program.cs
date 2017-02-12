namespace MODA.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //For args values, see ReadMe.txt
#if DEBUG
            args = "../Debug QueryGraph.txt 3 0 y n".Split(' ');
            //args = "../Debug Scere20141001CR_idx.txt 3 0 y n".Split(' ');
            //args = "../Debug Ecoli20141001CR_idx.txt 4 0 y n".Split(' ');
#else
            //args = "C:\\SOMA\\Deeds\\UWFinaProjectWorks\\MODA\\MODA.Console\\bin\\Debug\\ QueryGraph.txt 3 0 y n".Split(' ');
            //args = "../Debug Scere20141001CR_idx.txt 4 0 y y".Split(' ');
            //args = "../Debug QueryGraph.txt 4 0 y y".Split(' ');
            //args = "../Debug Ecoli20141001CR_idx.txt 3 0 y n".Split(' ');
#endif
            MODATest.Run(args);
            //var graph = new MODA.Impl.Graph(4);
            //graph.AddEdge("0", "1");
            //graph.AddEdge("1", "2");
            //graph.AddEdge("2", "3");
            //var graph2 = new MODA.Impl.Graph(4);
            //graph2.AddEdge("0", "1");
            //graph2.AddEdge("1", "2");
            //graph2.AddEdge("1", "3");
            //var graph3 = new MODA.Impl.Graph(4);
            //graph3.AddEdge("0", "1");
            //graph3.AddEdge("1", "2");
            //graph3.AddEdge("2", "3");
            //graph3.AddEdge("0", "3");
#if DEBUG

            System.Console.ReadKey();
#endif
        }
    }
}
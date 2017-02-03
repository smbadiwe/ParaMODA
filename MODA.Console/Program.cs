namespace MODA.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //For args values, see ReadMe.txt
#if DEBUG
            args = "../Debug QueryGraph.txt 4 0 y n".Split(' ');
            //args = "../Debug Scere20141001CR_idx.txt 3 0 y n".Split(' ');
#else
            //args = "C:\\SOMA\\Deeds\\UWFinaProjectWorks\\MODA\\MODA.Console\\bin\\Debug\\ QueryGraph.txt 3 0 y n".Split(' ');
            args = "../Debug Scere20141001CR_idx.txt 4 0 y y".Split(' ');
            //args = "../Debug QueryGraph.txt 4 0 y y".Split(' ');
            //args = "../Debug Ecoli20141001CR_idx.txt 3 0 y n".Split(' ');
#endif
            MODATest.Run(args);
            //var g = new Impl.Graph(4);
            //g.AddNode("1");
            //g.AddNode("2");
            //g.AddNode("3");
            //g.AddNode("4");
            //g.AddEdge("1", "2");
            //g.AddEdge("1", "3");
            //g.AddEdge("1", "4");
            //g.AddEdge("3", "4");
            //var newG = g.Clone();
            //newG.RemoveVertex("3");
            //var cEdg = newG.ContainsEdge("1", "4");
            //g.AddNode("2");
            //g.AddNode("3");
#if DEBUG

            System.Console.ReadKey();
#endif
        }
    }
}
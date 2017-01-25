namespace MODA.Console
{
    class Program
    {
        static void Main(string[] args)
        {
#if !DEBUG
            args = "../Debug Ecoli20141001CR_idx.txt 4 0 y n".Split(' ');
            //args = "QueryGraph../Debug .txt 3 0 y n".Split(' ');Scere20141001CR_idx
#endif
            MODATest.Run(args);
#if DEBUG
            System.Console.ReadKey();
#endif
        }
    }
}
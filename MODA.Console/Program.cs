namespace MODA.Console
{
    class Program
    {
        static void Main(string[] args)
        {
#if !DEBUG
            args = "../Debug Scere20141001CR_idx.txt 4 0 y n".Split(' ');
            //args = "QueryGraph../Debug Ecoli20141001CR_idx.txt 3 0 y n".Split(' ');
#endif
            MODATest.Run(args);
        }
    }
}
namespace MODA.Console
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            args = "Files Scere20141001CR_idx.txt 4 0 y n".Split(' ');
            //args = "Files QueryGraph.txt 3 0 y n".Split(' ');
#endif
            MODATest.Run(args);
            
        }
    }
}
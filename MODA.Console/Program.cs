namespace MODA.Console
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            args = ". Scere20141001CR_idx.txt 5 0 y n".Split(' ');
#endif
            MODATest.Run(args);
            
        }
    }
}
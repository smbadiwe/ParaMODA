using System;

namespace ParaMODA
{
    class Program
    {
        static void Main(string[] args)
        {
            var fgColor = Console.ForegroundColor;
            MODATest.Run(args);
            Console.ForegroundColor = fgColor;
#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}

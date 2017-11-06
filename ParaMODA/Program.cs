using QuickGraph;
using System;

namespace ParaMODA
{
    class Program
    {
        static void Main(string[] args)
        {
            var fgColor = Console.ForegroundColor;
#if DEBUG
            string argsStr = "";
            argsStr = @"runall -g ..\Release\Inputs\SampleInputGraph.txt -n 4"; // -k uses expansion tree
            //argsStr = @"runall -g ..\Release\Inputs\Ecoli20141001CR_idx.txt -n 3 -k"; // -k uses expansion tree
            //argsStr = @"runone -g ..\Release\Inputs\SampleInputGraph.txt -h ..\Release\QueryGraphs\4\qg-5a.txt -n 4";
            args = argsStr.Split(' ');
#endif
            Console.WriteLine("args = {0}", string.Join(" ", args));
            MODATest.Run(args);
            Console.ForegroundColor = fgColor;
#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}

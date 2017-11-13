using System;

namespace ParaMODA
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            //string argsStr = "";
            //argsStr = @"runall -g ..\Release\Inputs\SampleInputGraph.txt -n 33 -k"; // -k uses expansion tree
            ////argsStr = @"runall -g ..\Release\Inputs\Ecoli20141001CR_idx.txt -n 5"; // -k uses expansion tree
            ////argsStr = @"runone -g ..\Release\Inputs\SampleInputGraph.txt -h ..\Release\QueryGraphs\4\qg-5a.txt -n 4 -k";
            //args = argsStr.Split(' ');
            //Console.WriteLine("args = {0}", string.Join(" ", args));
#endif
            MODATest.Run(args);
#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}

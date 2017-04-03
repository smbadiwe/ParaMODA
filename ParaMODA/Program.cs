using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

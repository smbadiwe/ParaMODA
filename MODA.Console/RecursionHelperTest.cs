using MODA.Impl;
using System.Diagnostics;
using System.Threading;

namespace MODA.Console
{
    public class RecursionHelperTest
    {
        public static void Run()
        {
            var arr = new[] { 1,5,5,5,5,5, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            RunRecursionHelper(arr);
            RunRecursion(arr);
            System.Console.ReadKey();
        }

        static void RunRecursion(int[] arr)
        {
            var sw = Stopwatch.StartNew();

            var ans = Factorial(20); // Sum(arr);

            sw.Stop();
            System.Console.WriteLine("RunRecursion done.\n Time taken: {0}s. Ans = {1}", sw.Elapsed, ans);
        }

        static void RunRecursionHelper(int[] arr)
        {
            var sw = Stopwatch.StartNew();

            var ans = FactorialWithHelper(20); //  SumWithHelper(arr);

            sw.Stop();
            System.Console.WriteLine("RunRecursionHelper done.\n Time taken: {0}s. Ans = {1}", sw.Elapsed, ans);
        }

        static long Factorial(long n)
        {
            //This is the termination condition
            if (n < 2)
            {
                //This is the returning value when termination condition is true
                Thread.Sleep(3000);
                return 1;
            }

            //This is the recursive call
            var fac = Factorial(n - 1);

            //This is the work to do with the current item and the
            //result of recursive call
            return n * fac;
        }

        static long FactorialWithHelper(long n)
        {
            return RecursionHelper<long>.CreateSingular(currentParam => currentParam < 2, currentVal => { Thread.Sleep(3000); return 1; })
                   .RecursiveCall((currentParam, rv) => currentParam - 1)
                   .Do((i, rv) => i * rv)
                   .Execute(n);
        }

        static int Sum(int[] array, int index = 0)
        {
            //This is the termination condition
            if (index >= array.Length)
                //This is the returning value when termination condition is true
                return 0;

            //This is the recursive call
            var sumofrest = Sum(array, index + 1);

            //This is the work to do with the current item and the
            //result of recursive call
            return array[index] + sumofrest;
        }

        static int SumWithHelper(int[] ar)
        {
            return RecursionHelper<int>.CreateSingular(i => i >= ar.Length, i => 0)
                .RecursiveCall((i, rv) => i + 1)
                .Do((i, rv) => ar[i] + rv)
                .Execute(0);
        }
    }
}

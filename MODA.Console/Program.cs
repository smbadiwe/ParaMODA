using MODA.Impl;
using System.Collections.Generic;

namespace MODA.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //var list1 = new HashSet<string>(new[] { "1", "2", "3" });
            //var list2 = new HashSet<string>(new[] { "1", "2", "3" });

            //System.Console.WriteLine("{0}", list1 == list2);
            //System.Console.WriteLine("{0}", list1.GetHashCode() == list2.GetHashCode());
            //System.Console.WriteLine("{0}", list1.Equals(list2));
            //System.Console.WriteLine("{0}", list1.SetEquals(list2));

            //List<Mapping> mappingsToSearch = new List<Mapping>();
            ////{
            ////    "a", "b", "c"
            ////};
            //var item = mappingsToSearch.Find(x => x.Equals(new Mapping(new Dictionary<string, string>())));
            
            //System.Console.ReadKey();

            MODATest.Run(args);
            //RecursionHelperTest.Run();
        }
    }
}

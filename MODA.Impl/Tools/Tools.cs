using MODA.Impl.Transformers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl.Tools
{
    public class Tools
    {
        public static HashSet<O> mapSet<I, O>(IEnumerable<I> c, Transformer<I, O> f)
        {
            HashSet<O> output = new HashSet<O>();

            foreach (I input in c)
            {
                output.Add(f.transform(input));
            }
            return output;
        }
    }
}

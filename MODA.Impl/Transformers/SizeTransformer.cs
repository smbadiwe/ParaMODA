using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl.Transformers
{
    public class SizeTransformer<TVertex> : Transformer<HashSet<TVertex>, int>
    {

        private static SizeTransformer<TVertex> theTransformer;

        public static SizeTransformer<TVertex> getSizeTransformer()
        {
            if (theTransformer == null)
            {
                theTransformer = new SizeTransformer<TVertex>();
            }

            return theTransformer;
        }

        private SizeTransformer()
        {
        }

        public int transform(HashSet<TVertex> c)
        {
            return c.Count;
        }
    }
}

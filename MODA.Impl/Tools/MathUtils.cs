using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl.Tools
{
    public class MathUtils
    {
        public static int modBySmaller(int x, int y)
        {
            int small = Math.Min(x, y);
            int big = Math.Max(x, y);
            if (small == 0) return big;

            return big % small;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA
{
    public class Map
    {
        public int Domain { get; set; }
        public int Range { get; set; }

        public Map() : this(0, 0)
        {

        }

        public Map(int domain, int range)
        {
            Domain = domain;
            Range = range;
        }
    }
}

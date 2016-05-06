using System;
using System.Collections.Generic;
using System.Text;

namespace Motif1
{
    class Map
    {
        public int domain;
        public int range;
        public Map()
        {
            domain = 0;
            range = 0;
        }
        public Map(int Domain, int Range)
        {
            domain = Domain;
            range = Range;
        }
        public int get_range()
        {
            return range;
        }
        public int get_domain()
        {
            return domain;
        }
        public void set_range(int newRange)
        {
            range = newRange;
        }
        public void set_domain(int newDomain)
        {
            domain = newDomain;
        }
    }
}

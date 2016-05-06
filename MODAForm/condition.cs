using System;
using System.Collections.Generic;
using System.Text;

namespace Motif1
{
    class condition
    {
        int left;
        List<int> rights;

        public condition()
        {
            rights = new List<int>();
        }

        public void set_left(int Left)
        {
            left = Left;
        }
        public void add_right(int r)
        {
            rights.Add(r);
        }

        public void set_rights(List<int> Rights)
        {
            for (int i = 0; i < Rights.Count; i++)
                rights.Add(Rights[i]);
        }

        public int get_left()
        {
            return left;
        }

        public List<int> get_rights()
        {
            return rights;
        }

    }
}

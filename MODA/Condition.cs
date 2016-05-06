using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA
{
    public class Condition
    {
        int left;
        List<int> rights;

        public Condition()
        {
            rights = new List<int>();
        }

        public void SetLeft(int Left)
        {
            left = Left;
        }

        public void AddRight(int r)
        {
            rights.Add(r);
        }

        public void setRights(List<int> Rights)
        {
            for (int i = 0; i < Rights.Count; i++)
            {
                rights.Add(Rights[i]);
            }
        }

        public int GetLeft()
        {
            return left;
        }

        public List<int> GetRights()
        {
            return rights;
        }

    }
}

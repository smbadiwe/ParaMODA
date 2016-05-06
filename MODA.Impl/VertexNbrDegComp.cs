using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl
{
    public class VertexNbrDegComp<TVertex> : IComparable<TVertex>
    {
        protected object key;
        protected bool safe = false;

        public VertexNbrDegComp(object key)
        {
            this.key = key;
        }

        public VertexNbrDegComp(object key, bool safe)
        {
            this.key = key;
            this.safe = safe;
        }

        public int CompareTo(TVertex other)
        {
            throw new NotImplementedException();
        }

        //public bool dominates(TVertex v1, TVertex v2)
        //{
        //    // dominates is already exception-free, so no need to check safe
        //    return v1.degree() >= v2.degree() && IntLexComp.getIntLexComp().dominates((List<Integer>)v1.getUserDatum(key), (List<Integer>)v2.getUserDatum(key));
        //}
    }
}

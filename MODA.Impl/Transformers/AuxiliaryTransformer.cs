using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl.Transformers
{
    public abstract class AuxiliaryTransformer<A, I, O> : Transformer<I, O>
    {
        protected A aux;

        public AuxiliaryTransformer(A aux)
        {
            this.aux = aux;
        }

        public A getAux()
        {
            return this.aux;
        }

        public abstract O transform(I input);
    }
}

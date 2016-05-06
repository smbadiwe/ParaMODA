using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl.Transformers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="I">The Input</typeparam>
    /// <typeparam name="O">The Output</typeparam>
    public interface Transformer<I, O>
    {
        O transform(I input);
    }
}

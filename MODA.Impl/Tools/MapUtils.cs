using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MODA.Impl.Transformers;

namespace MODA.Impl.Tools
{
    /// <summary>
    /// NB: VI => input vertex, VO => output vertex
    /// </summary>
    public class MapUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="VI">Input</typeparam>
        /// <typeparam name="VO">Output</typeparam>
        /// <param name="theMap"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Dictionary<K, VO> mapValues<K, VI, VO>(
            Dictionary<K, VI> theMap, Transformer<VI, VO> f)
        {
            Dictionary<K, VO> outMap = new Dictionary<K, VO>();
            foreach (K key in theMap.Keys)
            {
                outMap.put(key, f.transform(theMap.get(key)));
            }
            return outMap;
        }
    }
}

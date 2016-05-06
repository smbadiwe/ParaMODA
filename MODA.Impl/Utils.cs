using MODA.Impl.Tools;
using MODA.Impl.Transformers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;

namespace MODA.Impl
{
    public class Utils
    {
        public static void addNeighborDegreeLists<TVertex>(UndirectedGraph<TVertex, Edge<TVertex>> g, object key,
               CopyAction copyAct)
        {
            Transformer<TVertex, List<int>> nbrDegTrans = new NbrDegreeTransformer<TVertex>(g);
            addVertexData(g, key, copyAct, nbrDegTrans);
        }

        public static void addVertexData<TVertex>(UndirectedGraph<TVertex, Edge<TVertex>> g, object key, CopyAction copyAct,
        Transformer<TVertex, List<int>> trans)
        {
            //foreach (TVertex v in g.Vertices)
            //{
            //    if (!v.containsUserDatumKey(key))
            //        v.addUserDatum(key, trans.transform(v), copyAct);
            //}
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;
using MODA.Impl.Tools;

namespace MODA.Impl.Transformers
{
    public class GetEqualVertexTransformer<TVertex> : AuxiliaryTransformer<UndirectedGraph<TVertex, Edge<TVertex>>, TVertex, TVertex>
    {
        public GetEqualVertexTransformer(UndirectedGraph<TVertex, Edge<TVertex>> g) : base(g)
        {
        }

        public override TVertex transform(TVertex input)
        {
            return getAux().Vertices.FirstOrDefault(x => x.Equals(input));
            //return input;
            //return (TVertex)input.getEqualVertex(getAux());
        }
    }

    /// <summary>
    /// used when mode == Mode.FIND_ALL
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    public class GetOriginalVertexTransformer<TVertex> : AuxiliaryTransformer<GetEqualVertexTransformer<TVertex>, HashSet<TVertex>, HashSet<TVertex>>
    {
        public GetOriginalVertexTransformer(GetEqualVertexTransformer<TVertex> g) : base(g)
        {
        }

        public override HashSet<TVertex> transform(HashSet<TVertex> verts)
        {
            return Tools.Tools.mapSet(verts, getAux());
        }
    }

    /// <summary>
    /// used when mode == Mode.FIND_ALL_MAPS
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    public class GetOriginalVertexMapTransformer<TVertex> : AuxiliaryTransformer<GetEqualVertexTransformer<TVertex>, Dictionary<TVertex, TVertex>, Dictionary<TVertex, TVertex>>
    {
        public GetOriginalVertexMapTransformer(GetEqualVertexTransformer<TVertex> g) : base(g)
        {
        }

        public override Dictionary<TVertex, TVertex> transform(Dictionary<TVertex, TVertex> m)
        {
            return MapUtils.mapValues(m, getAux());
        }
    }
}

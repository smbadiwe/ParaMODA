using MODA.Impl.Transformers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl.Isomorphism
{
    public class OrbitSymmetryBreaker<Vertex, TObject> where TObject : IComparable<TObject>
    {
        private Vertex @fixed;

        private HashSet<Vertex> orbit;

        private Transformer<Vertex, TObject> vertexTransformer;

        public OrbitSymmetryBreaker(Vertex v, Collection<Vertex> orbit,
                Transformer<Vertex, TObject> vertexTransformer)
        {
            this.@fixed = v;
            this.orbit = new HashSet<Vertex>(orbit);
            this.vertexTransformer = vertexTransformer;
        }

        public bool evaluate(Dictionary<Vertex, Vertex> partialMap)
        {
            Vertex fixedImg = partialMap.get(@fixed);
            if (@fixedImg == null)
                return true;

            TObject fixedLabel = vertexTransformer.transform(@fixedImg);

            foreach (Vertex v in partialMap.Keys)
            {
                if (v.Equals(@fixed) || !involves(v))
                    continue;

                if (vertexTransformer.transform(partialMap.get(v)).CompareTo(@fixedLabel) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool evaluate(Dictionary<Vertex, Vertex> partialMap, Vertex v, Vertex img)
        {
            if (!involves(v)
                    || (!v.Equals(@fixed) && !partialMap.Keys.Contains(@fixed)))
			return true;

            bool newFixed = v.Equals(@fixed);

            TObject newLabel = vertexTransformer.transform(img);
            TObject fixedLabel = newFixed ? newLabel : vertexTransformer
                    .transform(partialMap.get(@fixed));

            if (newFixed)
            {
                // System.out.print(" newFixed ");
                foreach (Vertex orbitV in orbit)
                    if (partialMap.ContainsKey(orbitV)
                            && vertexTransformer.transform(partialMap.get(orbitV))
                                    .CompareTo(@fixedLabel) < 0)
                        return false;

                return true;
            }
            else
            {
                // System.out.print(" " + this + "(" + newLabel + " " + fixedLabel +
                // ")");
                return newLabel.CompareTo(@fixedLabel) >= 0;
            }
        }

        public bool involves(Vertex v)
        {
            return orbit.Contains(v);
        }

        public override string ToString()
        {
            return "(" + @fixed +" <= " + orbit + ")";
        }
    }
}

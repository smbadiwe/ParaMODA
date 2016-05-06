using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using MODA.Impl.Tools;
using MODA.Impl.Transformers;

namespace MODA.Impl
{
    public class SimpleDegreeDistribution<TVertex> : IEnumerator<int> // Dictionary<int, HashSet<TVertex>>
    {
        private Dictionary<int, HashSet<TVertex>> degreeToVertices;

        private int numVerts = 0;
        private int numEdges = 0;

        public Dictionary<int, HashSet<TVertex>>.KeyCollection.Enumerator GetEnumerator()
        {
            return degreeToVertices.Keys.GetEnumerator();
        }

        public int Current
        {
            get
            {
                return GetEnumerator().Current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return GetEnumerator().Current;
            }
        }

        public SimpleDegreeDistribution(UndirectedGraph<TVertex, Edge<TVertex>> g)
        {
            degreeToVertices = new Dictionary<int, HashSet<TVertex>>();
            foreach (TVertex v in new HashSet<TVertex>(g.Vertices))
            {
                numVerts++;
                int deg = g.AdjacentDegree(v);
                numEdges += deg;
                if (degreeToVertices.ContainsKey(deg))
                {
                    degreeToVertices[deg].Add(v);
                }
                else
                {
                    HashSet<TVertex> verts = new HashSet<TVertex>();
                    verts.Add(v);
                    degreeToVertices.Add(deg, verts);
                }
            }

            numEdges /= 2;
        }

        public override bool Equals(object obj)
        {
            SimpleDegreeDistribution<TVertex> dd = obj as SimpleDegreeDistribution<TVertex>;
            if (dd != null)
            {
                // Check number of vertices and edges
                if (numEdges != dd.numEdges || numVerts != dd.numVerts)
                {
                    return false;
                }
                // Check number of vertices of each Degree
                foreach (int d in degreeToVertices.Keys)
                {
                    if (numberWithDegree(d) != dd.numberWithDegree(d))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int[] primes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };

            int hash = 0, primeLength = primes.Length;

            List<int> degrees = degreeToVertices.Keys.OrderBy(x => x).ToList();

            for (int i = 0; i < degrees.Count; i++)
            {
                int deg = degrees[i];
                hash += primes[i % primeLength]
                        * MathUtils.modBySmaller(deg, numberWithDegree(deg));
            }

            return hash;
        }

        public int numberWithDegree(int deg)
        {
            HashSet<TVertex> verts = degreeToVertices.get(deg);
            return verts == null ? 0 : verts.Count;
        }

        public HashSet<TVertex> verticesWithDegree(int deg)
        {
            HashSet<TVertex> verts = degreeToVertices.get(deg);
            if (verts == null)
            {
                return null;
            }
            return new HashSet<TVertex>(verts);
        }

        public HashSet<int> getDegrees()
        {
            return new HashSet<int>(degreeToVertices.Keys);
        }

        public Dictionary<int, int> degreeToCountMap()
        {
            return MapUtils.mapValues(degreeToVertices, SizeTransformer<TVertex>.getSizeTransformer());
        }

        public override string ToString()
        {
            return degreeToCountMap().ToString();
        }

        //public Iterator<Integer> iterator()
        //{
        //    return degreeToVertices.keySet().iterator();
        //}

        public void Dispose()
        {

        }

        public bool MoveNext()
        {
            return degreeToVertices.Keys.GetEnumerator().MoveNext();
        }

        public void Reset()
        {

        }
    }
}

using MODA.Impl.Tools;
using MODA.Impl.Transformers;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl.Isomorphism
{
    public abstract class Isomorphism<TVertex, TGraph> where TGraph : UndirectedGraph<TVertex, Edge<TVertex>>
    {
        protected Isomorphism()
        {
        }

        public static Isomorphism<TVertex, TGraph> getInstance()
        {
            return SymmetryIsomorphism<TVertex, TGraph>.getInstance();
        }

        public static void UnknownMode(Mode m)
        {
            throw new InvalidOperationException("Unknown mode: " + m);
        }


        /////////////////////////////////////////////////////////
        // DERIVED FUNCTIONS
        /////////////////////////////////////////////////////////
        public bool isIsomorphic(TGraph g1, TGraph g2, Collection<Transformer<TGraph, object>> invariants)
        {
            foreach (Transformer<TGraph, object> invariant in invariants)
            {
                if (!invariant.transform(g1).Equals(invariant.transform(g2)))
                    return false;
            }
            return isIsomorphic(g1, g2);
        }
        
        public bool isIsomorphic(TGraph g1, TGraph g2)
        {
            return findIsomorphism(g1, g2) != null;
        }
        

        ////////////////////////////////////////////////////////////
        // BASIC FUNCTIONS BASED ON ABSTRACT METHODS
        ////////////////////////////////////////////////////////////

        ////////////////// COMPLETE MAP
        public Dictionary<TVertex, TVertex> completeMap(Dictionary<TVertex, TVertex> vertMap,
                TGraph smallGraph, TGraph bigGraph)
        {
            return (Dictionary<TVertex, TVertex>)completeMapsHelper(vertMap, smallGraph,
                    bigGraph, Mode.FIND, null).get(
                    Mode.FIND);
        }

        public List<Dictionary<TVertex, TVertex>> completeMaps(Dictionary<TVertex, TVertex> vertMap,
                TGraph smallGraph, TGraph bigGraph)
        {
            return (List<Dictionary<TVertex, TVertex>>)completeMapsHelper(vertMap,
                    smallGraph, bigGraph, Mode.FIND_ALL, null).get(Mode.FIND_ALL);
        }

        public int numCompleteMaps(Dictionary<TVertex, TVertex> vertMap, TGraph smallGraph,
                TGraph bigGraph)
        {
            return (int)completeMapsHelper(vertMap, smallGraph, bigGraph,
                    Mode.COUNT, null).get(Mode.COUNT);
        }

        public abstract Dictionary<Mode, Object> completeMapsHelper(
                Dictionary<TVertex, TVertex> vertMap, TGraph smallGraph, TGraph bigGraph,
                Mode mode, Dictionary<Mode, object> result);

        //////////////// FIND ISOMORPHISM
        public Dictionary<TVertex, TVertex> findIsomorphism(TGraph g1, TGraph g2)
        {
            return (Dictionary<TVertex, TVertex>)findIsomorphismHelper(g1, g2, Mode.FIND)
                    .get(Mode.FIND);
        }

        public int numIsomorphisms(TGraph g1, TGraph g2)
        {
            return (int)findIsomorphismHelper(g1, g2, Mode.COUNT).get(
                    Mode.COUNT);
        }

        public List<Dictionary<TVertex, TVertex>> findAllIsomorphisms(TGraph g1, TGraph g2)
        {
            return (List<Dictionary<TVertex, TVertex>>)findIsomorphismHelper(g1, g2,
                    Mode.FIND_ALL).get(Mode.FIND_ALL);
        }

        public abstract Dictionary<Mode, object> findIsomorphismHelper(TGraph g1,
                TGraph g2, Mode mode);

        ////////////////// INDUCED SUBGRAPH

        public abstract Dictionary<Mode, object> inducedSubgraphHelper(TGraph g1, TGraph g2, StandardVerbosifier verbose, Mode mode);

        ////////////////////////// ARBITRARY SUBGRAPH
        
    }

}

using MODA.Impl.Tools;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl.Isomorphism
{
    public abstract class Isomorphism<TVertex, TGraph> where TGraph : AdjacencyGraph<TVertex, Edge<TVertex>>
    {
        protected Isomorphism()
        {
        }

        public static Isomorphism<TVertex, TGraph> getInstance()
        {
            return SymmetryIsomorphism<TVertex>.getInstance();
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
            foreach (Transformer<Graph, object> invariant in invariants)
                if (!invariant.transform(g1).equals(invariant.transform(g2)))
                    return false;

            return isIsomorphic(g1, g2);
        }

        public bool isInducedSubgraph(TGraph smallGraph, TGraph bigGraph)
        {
            return inducedSubgraph(smallGraph, bigGraph) != null;
        }

        public bool isIsomorphic(TGraph g1, TGraph g2)
        {
            return findIsomorphism(g1, g2) != null;
        }

        public int numAutomorphisms(TGraph g)
        {
            if (g.containsUserDatumKey(Keys.AUT))
                return ((List)g.getUserDatum(Keys.AUT)).size();

            return numIsomorphisms(g, g);
        }

        public HashSet<TVertex> orbitRepresentatives(TGraph g)
        {
            return orbitRepresentatives(g, findAllIsomorphisms(g, g));
        }

        public HashSet<TVertex> orbitRepresentatives(TGraph g,
                Collection<Dictionary<TVertex, TVertex>> auts)
        {
            return MapUtils.partitionRepresentatives(orbits(g, auts));
        }

        public Dictionary<TVertex, int> orbits(TGraph g)
        {
            return orbits(g, findAllIsomorphisms(g, g));
        }

        public Dictionary<TVertex, int> orbits(TGraph g,
                Collection<Dictionary<TVertex, TVertex>> auts)
        {
            return MapUtils.orbits((HashSet<TVertex>)g.getVertices(), auts);
        }

        public List<Dictionary<TVertex, TVertex>> findAllAutomorphisms(TGraph g)
        {
            if (g.containsUserDatumKey(Keys.AUT))
                return Collections.unmodifiableList((List<Dictionary<TVertex, TVertex>>)g.getUserDatum(Keys.AUT));

            List<Dictionary<TVertex, TVertex>> aut = findAllIsomorphisms(g, g);
            g.addUserDatum(Keys.AUT, aut, UserData.REMOVE);

            return aut;
        }

        ////////////////////////////////////////////////////////////
        // BASIC FUNCTIONS BASED ON ABSTRACT METHODS
        ////////////////////////////////////////////////////////////

        ////////////////// COMPLETE MAP
        public Dictionary<TVertex, TVertex> completeMap(Dictionary<TVertex, TVertex> vertMap,
                TGraph smallGraph, TGraph bigGraph)
        {
            return (Dictionary<TVertex, TVertex>)completeMapsHelper(vertMap, smallGraph,
                    bigGraph, Isomorphism.Mode.FIND, null).get(
                    Isomorphism.Mode.FIND);
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

        public abstract Dictionary<Isomorphism.Mode, Object> completeMapsHelper(
                Dictionary<TVertex, TVertex> vertMap, TGraph smallGraph, TGraph bigGraph,
                Isomorphism.Mode mode, Dictionary<Isomorphism.Mode, Object> result);

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

        protected abstract Dictionary<Isomorphism.Mode, object> findIsomorphismHelper(TGraph g1,
                TGraph g2, Isomorphism.Mode mode);

        ////////////////// INDUCED SUBGRAPH
        public HashSet<HashSet<TVertex>> inducedSubgraphs(TGraph g1, TGraph g2)
        {
            return inducedSubgraphs(g1, g2, new StandardVerbosifier());
        }

        public HashSet<HashSet<TVertex>> inducedSubgraphs(TGraph g1, TGraph g2, StandardVerbosifier verbose)
        {
            return (HashSet<HashSet<TVertex>>)inducedSubgraphHelper(g1, g2, verbose, Mode.FIND_ALL).get(Mode.FIND_ALL);
        }

        public int numInducedSubgraphs(TGraph g1, TGraph g2)
        {
            return numInducedSubgraphs(g1, g2, new StandardVerbosifier());
        }

        public int numInducedSubgraphs(TGraph g1, TGraph g2, StandardVerbosifier verbose)
        {
            return (int)inducedSubgraphHelper(g1, g2, verbose, Mode.COUNT).get(Mode.COUNT);
        }

        public HashSet<TVertex> inducedSubgraph(TGraph g1, TGraph g2)
        {
            return inducedSubgraph(g1, g2, new StandardVerbosifier());
        }

        public HashSet<TVertex> inducedSubgraph(TGraph g1, TGraph g2, StandardVerbosifier verbose)
        {
            return (HashSet<TVertex>)inducedSubgraphHelper(g1, g2, verbose, Mode.FIND).get(Mode.FIND);
        }

        public List<Dictionary<TVertex, TVertex>> inducedSubgraphMaps(TGraph g1, TGraph g2)
        {
            return inducedSubgraphMaps(g1, g2, new StandardVerbosifier());
        }

        public List<Dictionary<TVertex, TVertex>> inducedSubgraphMaps(TGraph g1, TGraph g2, StandardVerbosifier verbose)
        {
            return (List<Dictionary<TVertex, TVertex>>)inducedSubgraphHelper(g1, g2, verbose, Mode.FIND_ALL_MAPS).get(Mode.FIND_ALL_MAPS);
        }

        public Dictionary<TVertex, TVertex> inducedSubgraphMap(TGraph g1, TGraph g2)
        {
            return inducedSubgraphMap(g1, g2, new StandardVerbosifier());
        }

        public Dictionary<TVertex, TVertex> inducedSubgraphMap(TGraph g1, TGraph g2, StandardVerbosifier verbose)
        {
            return (Dictionary<TVertex, TVertex>)inducedSubgraphHelper(g1, g2, verbose, Mode.FIND_MAP).get(Mode.FIND_MAP);
        }

        protected abstract Dictionary<Isomorphism.Mode, object> inducedSubgraphHelper(TGraph g1, TGraph g2, StandardVerbosifier verbose, Isomorphism.Mode mode);

        ////////////////////////// ARBITRARY SUBGRAPH
        public HashSet<HashSet<Edge<TVertex>>> subgraphs(TGraph g1, TGraph g2)
        {
            return subgraphs(g1, g2, new StandardVerbosifier());
        }

        public HashSet<HashSet<Edge<TVertex>>> subgraphs(TGraph g1, TGraph g2, StandardVerbosifier verbose)
        {
            return (HashSet<HashSet<Edge<TVertex>>>)subgraphHelper(g1, g2, verbose, Mode.FIND_ALL).get(Mode.FIND_ALL);
        }

        public int writeSubgraphs(TGraph g1, TGraph g2, OrderedPair<BufferedWriter, Transformer<HashSet<Edge>, String>> outputData)
        {
            return writeSubgraphs(g1, g2, new StandardVerbosifier(), outputData);
        }

        public int writeSubgraphs(TGraph g1, TGraph g2, StandardVerbosifier verbose, OrderedPair<BufferedWriter, Transformer<HashSet<Edge>, String>> outputData)
        {
            int num = (int)subgraphHelper(g1, g2, verbose, outputData, Mode.OUTPUT_FILE).get(Mode.OUTPUT_FILE);

            // These are necessary -- otherwise some data may not get output!
            IOUtils.safeFlush(outputData.getFirst());
            IOUtils.safeClose(outputData.getFirst());
            return num;
        }

        public int numSubgraphs(TGraph g1, TGraph g2)
        {
            return numSubgraphs(g1, g2, new StandardVerbosifier());
        }

        public int numSubgraphs(TGraph g1, TGraph g2, StandardVerbosifier verbose)
        {
            return (int)subgraphHelper(g1, g2, verbose, Mode.COUNT).get(Mode.COUNT);
        }

        public HashSet<TVertex> subgraph(TGraph g1, TGraph g2)
        {
            return subgraph(g1, g2, new StandardVerbosifier());
        }

        public HashSet<TVertex> subgraph(TGraph g1, TGraph g2, StandardVerbosifier verbose)
        {
            return (HashSet<TVertex>)subgraphHelper(g1, g2, verbose, Mode.FIND).get(Mode.FIND);
        }

        public List<Dictionary<Edge<TVertex>, Edge<TVertex>>> subgraphMaps(TGraph g1, TGraph g2)
        {
            return subgraphMaps(g1, g2, new StandardVerbosifier());
        }

        public List<Dictionary<Edge<TVertex>, Edge<TVertex>>> subgraphMaps(TGraph g1, TGraph g2, StandardVerbosifier verbose)
        {
            return (List<Dictionary<Edge<TVertex>, Edge<TVertex>>>)subgraphHelper(g1, g2, verbose, Mode.FIND_ALL_MAPS).get(Mode.FIND_ALL_MAPS);
        }

        public Dictionary<Edge<TVertex>, Edge<TVertex>> subgraphMap(TGraph g1, TGraph g2)
        {
            return subgraphMap(g1, g2, new StandardVerbosifier());
        }

        public Dictionary<Edge<TVertex>, Edge<TVertex>> subgraphMap(TGraph g1, TGraph g2, StandardVerbosifier verbose)
        {
            return (Dictionary<Edge<TVertex>, Edge<TVertex>>)subgraphHelper(g1, g2, verbose, Mode.FIND_MAP).get(Mode.FIND_MAP);
        }

        protected Dictionary<Isomorphism.Mode, object> subgraphHelper(TGraph g1, TGraph g2, StandardVerbosifier verbose, Isomorphism.Mode mode)
        {
            return subgraphHelper(g1, g2, verbose, null, mode);
        }

        protected abstract Dictionary<Isomorphism.Mode, object> subgraphHelper(TGraph g1,
            TGraph g2, StandardVerbosifier verbose,
            OrderedPair<BufferedWriter, Transformer<HashSet<Edge<TVertex>>, String>> outputData, Isomorphism.Mode mode);

    }

}

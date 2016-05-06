using MODA.Impl.Isomorphism;
using MODA.Impl.Tools;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl.Isomorphism
{
    public class SymmetryIsomorphism<Vertex> : Isomorphism<Vertex>
    {
        protected SymmetryIsomorphism()
        {
        }

        private static SymmetryIsomorphism<Vertex> theInstance;

        new public static SymmetryIsomorphism<Vertex> getInstance()
        {
            if (theInstance == null)
            {
                theInstance = new SymmetryIsomorphism<Vertex>();
            }

            return theInstance;
        }

        protected override Dictionary<Isomorphism.Mode, object> inducedSubgraphHelper(AdjacencyGraph<Vertex, Edge<Vertex>> smallGraph,
                AdjacencyGraph<Vertex, Edge<Vertex>> bigGraph, StandardVerbosifier verbose, Isomorphism.Mode mode)

        {
            Dictionary<Isomorphism.Mode, object> result = new Dictionary<Isomorphism.Mode, object>();
            switch (mode)
            {
                case Mode.COUNT:
                    result.put(mode, 0);
                    break;
                case Mode.FIND:
                case Mode.FIND_MAP:
                    result.put(mode, null);
                    break;
                case Mode.FIND_ALL:
                    result.put(mode, new HashSet<HashSet<Vertex>>());
                    break;
                case Mode.FIND_ALL_MAPS:
                    result.put(mode, new List<Dictionary<Vertex, Vertex>>());
                    break;
                default:
                    UnknownMode(mode);
                    break;
            }

            // must be constructed before bigGraph is locally replaced with a copy
            // of itself
            // or you have to create a variable to keep a local reference to the
            // original bigGraph
            GetEqualVertexTransformer getOrigVert = new GetEqualVertexTransformer(
                    bigGraph);

            // For efficiency, define these transformers OUTSIDE any loop
            // used when mode == Mode.FIND_ALL
            Transformer<HashSet<Vertex>, HashSet<Vertex>> getOrigVerts = new AuxiliaryTransformer<GetEqualVertexTransformer, Set<Vertex>, Set<Vertex>>(
                    getOrigVert)
            {
                    public HashSet<Vertex> transform(HashSet<Vertex> verts)
        {
            return Tools.mapSet(verts, getAux());
        }
    };

    // For efficiency, define these transformers OUTSIDE any loop
    // used when mode == Mode.FIND_ALL_MAPS
    Transformer<Dictionary<Vertex, Vertex>, Dictionary<Vertex, Vertex>> getOrigVertMap = new AuxiliaryTransformer<GetEqualVertexTransformer, Dictionary<Vertex, Vertex>, Dictionary<Vertex, Vertex>>(
            getOrigVert)
    {
                    public Dictionary<Vertex, Vertex> transform(Dictionary<Vertex, Vertex> m)
    {
        return MapUtils.mapValues(m, getAux());
    }
};

bigGraph = (Graph) bigGraph.copy(); // copy bigAdjacencyGraph<Vertex, Edge<Vertex>> so we can remove
        // vertices from it as we go without
        // damaging the input graph
        // ? smallGraph = (Graph) smallGraph.copy();

        Utils.addNeighborDegreeLists(bigGraph, Keys.NBR_DEGREES,
                UserData.REMOVE);
        Utils.addNeighborDegreeLists(smallGraph, Keys.NBR_DEGREES,
                UserData.REMOVE);

        VertexNbrDegComp allNbrDegComp = new VertexNbrDegComp(Keys.NBR_DEGREES,
                true);
Comparator<Vertex> comp = new LexicalComparator<Vertex>(
        VertexDegreeComparator.getInstance(), allNbrDegComp);

// Sort vertices in big AdjacencyGraph<Vertex, Edge<Vertex>> by ascending degree and then by nbg
// degrees
List<Vertex> bigVerts = new ArrayList<Vertex>(bigGraph.getVertices());

        if (SORT) {
            Collections.sort(bigVerts, comp);
        }

        for (int i = 0; i<bigVerts.size(); i++) {
            bigVerts.get(i).addUserDatum(Isomorphism.Keys.ORDER, i,
                    UserData.REMOVE);
        }

        verbose.setPrintEvery(bigVerts.size() / 20);

        Dictionary<Vertex, Set<OrbitSymmetryBreaker>> symBreakers = breakSymmetriesPerVertex(smallGraph); // usually
                                                                                                          // data
List<Vertex> smallVerts = new ArrayList<Vertex>(symBreakers.keySet());

        if (SORT) {
            Collections.sort(smallVerts, comp);
        }

        Dictionary<Vertex, Vertex> vertMap = new Dictionary<Vertex, Vertex>();

        while (!bigVerts.isEmpty()) {
            Vertex bigV = bigVerts.get(0);

verbose.printlnIter(bigVerts.size() + " " + bigV.degree());

            for (int smallVIndex = 0; smallVIndex<smallVerts.size(); smallVIndex++) {
                Vertex smallV = smallVerts.get(smallVIndex);

                if (allNbrDegComp.dominates(bigV, smallV)) {
                    vertMap.clear();
                    vertMap.put(smallV, bigV);
                    if (Isomorphism.DEBUG) {
                        System.out.println("Mapping " + smallV + " to " + bigV
                                + "(" + bigV.getUserDatum(Keys.ORDER) + ")");
                    }

                    Dictionary<Isomorphism.Mode, Object> partialResult = completeMapsHelper(
                            vertMap, smallGraph, bigGraph, mode, symBreakers
                            .get(smallV), null);
                    switch (mode) {
                        case COUNT:
                            result.put(mode, (Integer) result.get(mode)
                                    + (Integer) partialResult.get(mode));
                            break;
                        case FIND:
                        case FIND_MAP:
                            Object found = partialResult.get(mode);
                            if (found != null) {
                                if (mode == Mode.FIND) {
                                    result.put(mode, new HashSet<Vertex>(
                                            ((Dictionary<Vertex, Vertex>) partialResult
                                            .get(mode)).values()));
                                } else if (mode == Mode.FIND_MAP) {
                                    result.put(mode,
                                            (Dictionary<Vertex, Vertex>) partialResult
                                            .get(mode));
                                } else {
                                    unknownMode(mode);
                                }
                            }
                            return result;
                        case FIND_ALL:
                            List<Set<Vertex>> subgraphs = Tools
                                    .map(
                                            MapUtils
                                            .mapsToValueSets((List<Dictionary<Vertex, Vertex>>)partialResult
                                                    .get(mode)),
                                            getOrigVerts);
                            ((Set<Set<Vertex>>) result.get(mode)).addAll(subgraphs);
                            break;
                        case FIND_ALL_MAPS:
                            List<Dictionary<Vertex, Vertex>> subgraphMaps = Tools.map(
                                    (List<Dictionary<Vertex, Vertex>>)partialResult
                                    .get(mode), getOrigVertMap);
                            ((List<Dictionary<Vertex, Vertex>>) result.get(mode))
                                    .addAll(subgraphMaps);
                            break;
                        default:
                            unknownMode(mode);
                    }

                }
            }

            bigGraph.removeVertex(bigV);
            bigVerts.remove(0);
            verbose.incrementCount(1);
        }

        return result;
    }

    public Set<OrbitSymmetryBreaker> breakSymmetries(Graph g, Vertex v,
            Transformer<Vertex, ? extends Comparable> vertexTransformer)
{
    if (g.containsUserDatumKey(Keys.SYM_BREAKERS))
    {
        return ((Dictionary<Vertex, Set<OrbitSymmetryBreaker>>)g
                .getUserDatum(Keys.SYM_BREAKERS)).get(v);
    }

    return breakSymmetries(g, v, findAllAutomorphisms(g), vertexTransformer);
}

public Set<OrbitSymmetryBreaker> breakAllSymmetries(Graph g)
{
    return breakAllSymmetries(g, null);
}

public Set<OrbitSymmetryBreaker> breakAllSymmetries(Graph g,
        Transformer<Vertex, ? extends Comparable> vertexTransformer)
{
    if (vertexTransformer == null)
    {
        vertexTransformer = new VertexUserDatumTransformer<Integer>(
                Keys.ORDER);
    }

    Set<OrbitSymmetryBreaker> breakers = new HashSet<OrbitSymmetryBreaker>();
    List<Dictionary<Vertex, Vertex>> remainingSymmetries = new ArrayList<Dictionary<Vertex, Vertex>>(
            findAllAutomorphisms(g));

    Dictionary<Integer, Set<Vertex>> reverseOrbitMap = MapUtils.retractMap(orbits(
            g, remainingSymmetries));

    while (remainingSymmetries.size() > 1)
    {
        // System.out.println(remainingSymmetries);

        // Find largest remaining orbit
        Set<Vertex> curOrbit = null;
        for (Set<Vertex> thisOrbit : reverseOrbitMap.values())
        {
            if (curOrbit == null || thisOrbit.size() > curOrbit.size())
            {
                curOrbit = thisOrbit;
            }
        }
        Vertex curV = curOrbit.iterator().next();
        assert curOrbit.size() > 1;

        // System.out.println(curOrbit);
        breakers.add(new OrbitSymmetryBreaker(curV, curOrbit,
                vertexTransformer));
        remainingSymmetries = MapUtils
                .mapsFixing(remainingSymmetries, curV);
        reverseOrbitMap = MapUtils
                .retractMap(orbits(g, remainingSymmetries));
        // System.out.println(breakers);
    }

    return breakers;
}

public Dictionary<Vertex, Set<OrbitSymmetryBreaker>> breakSymmetriesPerVertex(
        Graph g)
{
    return breakSymmetriesPerVertex(g, null);
}

public Dictionary<Vertex, Set<OrbitSymmetryBreaker>> breakSymmetriesPerVertex(
        Graph g, Transformer<Vertex, ? extends Comparable> vertexTransformer)
{
    if (g.containsUserDatumKey(Keys.SYM_BREAKERS))
    {
        return (Dictionary<Vertex, Set<OrbitSymmetryBreaker>>)g
                .getUserDatum(Keys.SYM_BREAKERS);
    }

    Dictionary<Vertex, Set<OrbitSymmetryBreaker>> symBreakers = new Dictionary<Vertex, Set<OrbitSymmetryBreaker>>();

    List<Dictionary<Vertex, Vertex>> auts = findAllAutomorphisms(g);
    List<Vertex> smallVerts = new ArrayList<Vertex>(orbitRepresentatives(g,
            auts));

    if (vertexTransformer == null)
    {
        vertexTransformer = new VertexUserDatumTransformer<Integer>(
                Keys.ORDER);
    }

    for (Vertex v : smallVerts)
    {
        symBreakers.put(v, breakSymmetries(g, v, auts, vertexTransformer));
    }

    g.addUserDatum(Keys.SYM_BREAKERS, symBreakers, UserData.REMOVE);

    return symBreakers;
}

/**
 * Currently returns a set of OrbitSymmetryBreakers that share a reference
 * to vertexTransformer.
 */
public Set<OrbitSymmetryBreaker> breakSymmetries(Graph g, Vertex v,
        Collection<Dictionary<Vertex, Vertex>> symmetries,
        Transformer<Vertex, ? extends Comparable> vertexTransformer)
{
    Set<OrbitSymmetryBreaker> breakers = new HashSet<OrbitSymmetryBreaker>();

    List<Dictionary<Vertex, Vertex>> remainingSymmetries = MapUtils.mapsFixing(
            symmetries, v);

    Dictionary<Vertex, Integer> orbitMap = orbits(g, remainingSymmetries);
    Dictionary<Integer, Set<Vertex>> reverseOrbitMap = MapUtils
            .retractMap(orbitMap);

    List<Vertex> q = new ArrayList<Vertex>(v.getNeighbors());
    Set<Vertex> visited = Tools.setWithObjects(v);

    while (remainingSymmetries.size() > 1 && q.size() > 0)
    {
        // System.out.println(remainingSymmetries);
        Vertex curV = q.remove(0);
        visited.add(curV);
        Set<Vertex> toAdd = new HashSet<Vertex>(curV.getNeighbors());
        toAdd.removeAll(visited);
        q.addAll(toAdd);

        Set<Vertex> curOrbit = reverseOrbitMap.get(orbitMap.get(curV));
        // System.out.println(curOrbit);
        if (curOrbit.size() > 1)
        {
            breakers.add(new OrbitSymmetryBreaker(curV, curOrbit,
                    vertexTransformer));
            remainingSymmetries = MapUtils.mapsFixing(remainingSymmetries,
                    curV);
            orbitMap = orbits(g, remainingSymmetries);
            reverseOrbitMap = MapUtils.retractMap(orbitMap);
        }
        // System.out.println(breakers);
    }

    return breakers;
}

public void addSymmetryBreakers(Graph g)
{
    if (!g.containsUserDatumKey(Isomorphism.Keys.SYM_BREAKERS))
    {
        g.addUserDatum(Isomorphism.Keys.SYM_BREAKERS,
                breakSymmetriesPerVertex(g), UserData.REMOVE);
    }
}

// public Dictionary<Vertex, Vertex> completeMap(Dictionary<Vertex, Vertex> vertMap,
// AdjacencyGraph<Vertex, Edge<Vertex>> smallGraph, AdjacencyGraph<Vertex, Edge<Vertex>> bigGraph,
// Set<OrbitSymmetryBreaker> symBreakers) {
// return (Dictionary<Vertex, Vertex>) completeMapsHelper(vertMap, smallGraph,
// bigGraph, Isomorphism.Mode.FIND, symBreakers, null).get(
// Mode.FIND);
// }
//
// public List<Dictionary<Vertex, Vertex>> completeMaps(Dictionary<Vertex, Vertex>
// vertMap,
// AdjacencyGraph<Vertex, Edge<Vertex>> smallGraph, AdjacencyGraph<Vertex, Edge<Vertex>> bigGraph,
// Set<OrbitSymmetryBreaker> symBreakers) {
// return (List<Dictionary<Vertex, Vertex>>) completeMapsHelper(vertMap,
// smallGraph, bigGraph, Mode.FIND_ALL, symBreakers, null).get(
// Mode.FIND_ALL);
// }
//
// public int numCompleteMaps(Dictionary<Vertex, Vertex> vertMap, AdjacencyGraph<Vertex, Edge<Vertex>> smallGraph,
// AdjacencyGraph<Vertex, Edge<Vertex>> bigGraph, Set<OrbitSymmetryBreaker> symBreakers) {
// return (Integer) completeMapsHelper(vertMap, smallGraph, bigGraph,
// Mode.COUNT, symBreakers, null).get(Mode.COUNT);
// }
public Dictionary<Isomorphism.Mode, Object> completeMapsHelper(
        Dictionary<Vertex, Vertex> vertMap, AdjacencyGraph<Vertex, Edge<Vertex>> smallGraph, AdjacencyGraph<Vertex, Edge<Vertex>> bigGraph,
        Isomorphism.Mode mode, Dictionary<Isomorphism.Mode, Object> result)
{
    return completeMapsHelper(vertMap, smallGraph, bigGraph, mode,
            breakSymmetries(smallGraph, vertMap.keySet().iterator().next(),
                    null), result);
}

// Can use symBreakers from any mapped vertex, but must be consistent
// throughout recursion
public Dictionary<Isomorphism.Mode, Object> completeMapsHelper(
        Dictionary<Vertex, Vertex> vertMap, AdjacencyGraph<Vertex, Edge<Vertex>> smallGraph, AdjacencyGraph<Vertex, Edge<Vertex>> bigGraph,
        Isomorphism.Mode mode, Set<OrbitSymmetryBreaker> symBreakers,
        Dictionary<Isomorphism.Mode, Object> result)
{
    if (result == null)
    {
        result = new Dictionary<Isomorphism.Mode, Object>();
        switch (mode)
        {
            case COUNT:
                result.put(mode, 0);
                break;
            case FIND:
            case FIND_MAP:
                result.put(mode, null);
                break;
            case FIND_ALL:
            case FIND_ALL_MAPS:
                result.put(mode, new ArrayList<Dictionary<Vertex, Vertex>>());
                break;
            default:
                unknownMode(mode);
        }
    }

    if (vertMap.size() == smallGraph.numVertices())
    {
        switch (mode)
        {
            case COUNT:
                result.put(mode, ((Integer)result.get(mode)) + 1);
                return result;
            case FIND:
            case FIND_MAP:
                result.put(mode, vertMap);
                return result;
            case FIND_ALL:
            case FIND_ALL_MAPS:
                ((List<Dictionary<Vertex, Vertex>>)result.get(mode)).add(vertMap);
                return result;
            default:
                unknownMode(mode);
        }
    }
    else
    {
        Set<Vertex> mapped = vertMap.keySet();
        List<Vertex> unmapped = new ArrayList<Vertex>(smallGraph
                .getVertices());
        unmapped.removeAll(mapped);

        // System.out.println(mapped + " " + unmapped);
        // Find the unmapped vertex with the most mapped neighbors
        // Use lexical comparison of degrees and then neighbor degrees to
        // break ties
        VertexNbrDegComp nbrDegComp = new VertexNbrDegComp(
                Keys.NBR_DEGREES, true);
        Vertex best = null; // Tools.max(unmapped, nbrDegComp);
        int bestNumNbrs = 0;
        int bestNumSym = 0;
        Set<Vertex> bestNbrs = null;
        for (Vertex v : unmapped)
        {
            int numSymBreakers = 0;
            for (OrbitSymmetryBreaker b : symBreakers)
            {
                if (b.involves(v))
                {
                    numSymBreakers++;
                }
            }

            Set<Vertex> nbrs = new HashSet<Vertex>(v.getNeighbors());
            nbrs.retainAll(mapped);
            int numNbrs = nbrs.size();

            if (numNbrs > bestNumNbrs)
            {
                bestNumNbrs = numNbrs;
                best = v;
                bestNbrs = nbrs;
            }
            else if (numNbrs == bestNumNbrs)
            {
                if (numSymBreakers > bestNumSym)
                {
                    best = v;
                    bestNbrs = nbrs;
                    bestNumNbrs = numNbrs;
                }
                else
                {
                    if (best != null)
                    {
                        // GlobalTimer.startTime(bestNbrDominatesKey);
                        boolean dom = (v
                                .containsUserDatumKey(Keys.NBR_DEGREES) ? nbrDegComp
                                        .dominates(v, best)
                                        : false);
                        // GlobalTimer.recordTime(bestNbrDominatesKey);
                        if (dom)
                        {
                            best = v;
                            bestNbrs = nbrs;
                            bestNumNbrs = numNbrs;
                        }
                    }
                    else
                    {
                        best = v;
                        bestNbrs = nbrs;
                        bestNumNbrs = numNbrs;
                    }
                }
            }
        }

        // Set<Vertex> goodUnmapped = new HashSet<Vertex>(unmapped);
        // Tools.filter(goodUnmapped, new
        // AuxiliaryPredicate<Set<Vertex>>(mapped) {
        // public boolean evaluate(Object o) {
        // Vertex v = (Vertex)o;
        // for(Vertex m : aux)
        // if(m.isNeighborOf(v))
        // return true;
        //
        // return false;
        // }
        // });
        //
        // best = Tools.max(goodUnmapped, nbrDegComp);
        // bestNbrs = new HashSet<Vertex>(best.getNeighbors());
        // bestNbrs.retainAll(mapped);
        String debugPrefix = "";
        for (int i = 0; i < vertMap.size(); i++)
        {
            debugPrefix += "  ";
        }

        if (DEBUG)
        {
            System.out.println(debugPrefix + "Best: " + best + " "
                    + vertMap);
        }

        // Possibile vertices to map the best vertex to
        Set<Vertex> bigPossibilities = new HashSet<Vertex>(vertMap.get(
                bestNbrs.iterator().next()).getNeighbors());
        bigPossibilities.removeAll(vertMap.values());
        for (Vertex smallV : mapped)
        {
            Vertex bigV = vertMap.get(smallV);
            if (bestNbrs.contains(smallV))
            {
                if (MAKE_DIRECTED)
                {
                    boolean keepPreds = best.isPredecessorOf(smallV);
                    boolean keepSuccs = best.isSuccessorOf(smallV);
                    Set<Vertex> preds = bigV.getPredecessors();
                    Set<Vertex> succs = bigV.getSuccessors();
                    if (keepPreds)
                    {
                        bigPossibilities.retainAll(preds);
                    }
                    else
                    {
                        bigPossibilities.removeAll(preds);
                    }

                    if (keepSuccs)
                    {
                        bigPossibilities.retainAll(succs);
                    }
                    else
                    {
                        bigPossibilities.removeAll(succs);
                    }
                }
                else
                {
                    bigPossibilities.retainAll(bigV.getNeighbors());
                }
            }
            else
            {
                bigPossibilities.removeAll(bigV.getNeighbors());
            }
        }

        if (DEBUG && bigPossibilities.size() == 0)
        {
            System.out.println(debugPrefix + "  REJECTED B/C NO POSS");
        }

        poss:
        for (Vertex p : bigPossibilities)
        {
            if (DEBUG)
            {
                System.out.print(debugPrefix + "Poss: " + best + "->" + p
                        + "(" + p.getUserDatum(Keys.ORDER) + ")");
            }

            // if(nbrDegComp.dominates(p, best))
            // continue poss;
            for (OrbitSymmetryBreaker b : symBreakers)
            {
                if (!b.evaluate(vertMap, best, p))
                {
                    if (DEBUG)
                    {
                        System.out.println(" REJECTED");
                    }
                    continue poss;
                }
            }

            if (DEBUG)
            {
                System.out.println(" ACCEPTED");
            }

            Dictionary<Vertex, Vertex> newMap = new Dictionary<Vertex, Vertex>(
                    vertMap);
            newMap.put(best, p);
            completeMapsHelper(newMap, smallGraph, bigGraph, mode,
                    symBreakers, result);
            if ((mode == Mode.FIND || mode == Mode.FIND_MAP)
                    && result.get(mode) != null)
            {
                return result;
            }
        }
    }

    return result;
}

/**
 * Since findIsomorphismHelper must return all isomorphisms, including those
 * that differ only by an automorphism, simply used the method from
 * MappingIsomorphism.
 */
protected Dictionary<Mode, object> findIsomorphismHelper(Graph g1, Graph g2, Mode mode)
{
    return MappingIsomorphism.getInstance().findIsomorphismHelper(g1, g2,
            mode);
}

public Dictionary<Isomorphism.Mode, object> subgraphHelper(
        AdjacencyGraph<Vertex, Edge<Vertex>> smallGraph,
        AdjacencyGraph<Vertex, Edge<Vertex>> bigGraph,
        StandardVerbosifier verbose,
        OrderedPair<BufferedWriter, Transformer<Set<Edge>, String>> outputData,
        Isomorphism.Mode mode)
{
    Dictionary<Isomorphism.Mode, Object> result = new Dictionary<Isomorphism.Mode, Object>();
    switch (mode)
    {
        case OUTPUT_FILE:
        // nothing more to do here
        // purposefully falls through to case COUNT
        case COUNT:
            result.put(mode, 0);
            break;
        case FIND:
        case FIND_MAP:
            result.put(mode, null);
            break;
        case FIND_ALL:
            result.put(mode, new HashSet<Set<Edge>>());
            break;
        case FIND_ALL_MAPS:
            result.put(mode, new ArrayList<Dictionary<Edge, Edge>>());
            break;
        default:
            unknownMode(mode);
    }

    // must be constructed before bigGraph is locally replaced with a copy
    // of itself
    // or you have to create a variable to keep a local reference to the
    // original bigGraph
    GetEqualEdgeTransformer getOrigEdge = new GetEqualEdgeTransformer(
            bigGraph);

    // For efficiency, define these transformers OUTSIDE any loop
    // used when mode == Mode.FIND_ALL
    Transformer<Set<Edge>, Set<Edge>> getOrigEdges = new AuxiliaryTransformer<GetEqualEdgeTransformer, Set<Edge>, Set<Edge>>(
            getOrigEdge)
    {
                    public Set<Edge> transform(Set<Edge> verts)
{
    return Tools.mapSet(verts, getAux());
}
                };

        // For efficiency, define these transformers OUTSIDE any loop
        // used when mode == Mode.FIND_ALL_MAPS
        Transformer<Dictionary<Edge, Edge>, Dictionary<Edge, Edge>> getOrigEdgeMap = new AuxiliaryTransformer<GetEqualEdgeTransformer, Dictionary<Edge, Edge>, Dictionary<Edge, Edge>>(
                getOrigEdge)
        {
                    public Dictionary<Edge, Edge> transform(Dictionary<Edge, Edge> m)
{
    return MapUtils.mapValues(m, getAux());
}
                };

        bigGraph = (Graph) bigGraph.copy(); // copy bigAdjacencyGraph<Vertex, Edge<Vertex>> so we can remove
        // edges/vertices from it as we go without
        // damaging the input graph
        // ? smallGraph = (Graph) smallGraph.copy();

        Utils.addNeighborDegreeLists(bigGraph, Keys.NBR_DEGREES,
                UserData.REMOVE);
        Utils.addNeighborDegreeLists(smallGraph, Keys.NBR_DEGREES,
                UserData.REMOVE);

        VertexNbrDegComp allNbrDegComp = new VertexNbrDegComp(Keys.NBR_DEGREES,
                true);
Comparator<Vertex> comp = new LexicalComparator<Vertex>(
        VertexDegreeComparator.getInstance(), allNbrDegComp);

Comparator<Edge> edgeComp = new AuxiliaryComparator<Comparator<Vertex>, Edge>(
        comp)
{
                    public int compare(Edge o1, Edge o2)
{
    SimilarOrderedPair<Vertex> endpts1 = SimilarOrderedPair
    .ensureOrdered(Utils.getEndpoints(o1));
    if (getAux().compare(endpts1.getFirst(), endpts1.getSecond()) < 0)
    {
        endpts1 = endpts1.reverse();
    }

    SimilarOrderedPair<Vertex> endpts2 = SimilarOrderedPair
    .ensureOrdered(Utils.getEndpoints(o1));
    if (getAux().compare(endpts2.getFirst(), endpts2.getSecond()) < 0)
    {
        endpts2 = endpts2.reverse();
    }

    int compBig = getAux().compare(endpts1.getFirst(),
            endpts2.getFirst());
    if (compBig != 0)
    {
        return compBig;
    }
    else
    {
        return getAux().compare(endpts1.getSecond(),
                endpts2.getSecond());
    }
}
                };

        // Sort vertices in big AdjacencyGraph<Vertex, Edge<Vertex>> by ascending degree and then by nbr
        // degrees
        // List<Vertex> bigVerts = new
        // ArrayList<Vertex>(bigGraph.getVertices());
        List<Edge> bigEdges = new ArrayList<Edge>(bigGraph.getEdges());

        if (SORT) {
            // Collections.sort(bigVerts, comp);
            Collections.sort(bigEdges, edgeComp);
        }

        {
            // Makes isoOrder local to this block
            int isoOrder = 0;
            for (Vertex v : (Set<Vertex>) bigGraph.getVertices()) {
                v.addUserDatum(Isomorphism.Keys.ORDER, isoOrder++,
                        UserData.REMOVE);
                // System.out.println(v + " = " + (isoOrder - 1));
            }
        }

        VertexUserDatumTransformer isoOrderTransformer = new VertexUserDatumTransformer(
                Keys.ORDER);

verbose.setPrintEvery(bigEdges.size() / 20);

        Set<OrbitSymmetryBreaker> symBreakers = breakAllSymmetries(smallGraph);
        if (DEBUG) {
            System.out.println(symBreakers);
        }

        List<Vertex> smallVerts = new ArrayList<Vertex>(smallGraph
                .getVertices());
List<Edge> smallEdges = new ArrayList<Edge>(smallGraph.getEdges());
        if (SORT) {
            // Collections.sort(smallVerts, comp);
            Collections.sort(smallEdges, edgeComp);
        }

        EdgeMap edgeMap = new EdgeMap();

        while (!bigEdges.isEmpty()) {
            // Vertex bigV = bigVerts.get(0);
            Edge bigE = bigEdges.get(0);
SimilarOrderedPair<Vertex> bigEndpts = SimilarOrderedPair
        .ensureOrdered(Utils.getEndpoints(bigE));
boolean directed = bigE instanceof DirectedEdge; // NB: This
                                                 // algorithm
                                                 // DOES NOT
                                                 // CURRENTLY
                                                 // handle graphs
                                                 // with mixed
                                                 // (i.e. both
                                                 // directed and
                                                 // undirected)
                                                 // edges

verbose.printlnIter("" + bigEdges.size());

            for (int smallEIndex = 0; smallEIndex<smallEdges.size(); smallEIndex++) {
                Edge smallE = smallEdges.get(smallEIndex);
SimilarOrderedPair<Vertex> smallEndpts = SimilarOrderedPair
        .ensureOrdered(Utils.getEndpoints(smallE));
flipLoop:
                for (int flip = 0; flip< (directed? 1 : 2); flip++) {
                    if (flip == 1) {
                        smallEndpts = smallEndpts.reverse();
                    }

                    if (allNbrDegComp.dominates(bigEndpts.getFirst(),
                            smallEndpts.getFirst())
                            && allNbrDegComp.dominates(bigEndpts.getSecond(),
                                    smallEndpts.getSecond())) {
                        if (Isomorphism.DEBUG) {
                            System.out.println("Mapping " + smallEndpts
                                    + " to " + bigEndpts + "("
                                    + Tools.map(bigEndpts, isoOrderTransformer)
                                    + ")");
                        }

                        edgeMap.clear(); // necessary b/c edgeMap is not
                        // local to the loop
                        edgeMap.put(smallEndpts, bigEndpts);

                        for (OrbitSymmetryBreaker osb : symBreakers) {
                            Dictionary<Vertex, Vertex> vertMap = edgeMap.getVertMap();
                            if (DEBUG) {
                                System.out.println("CHECKING SYMBREAKER: "
                                        + osb + " against " + edgeMap);
                            }
                            if (!osb.evaluate(vertMap)) {
                                if (DEBUG) {
                                    System.out.println("  REJECTED");
                                }
                                continue flipLoop;
                            }
                        }

                        if (Isomorphism.DEBUG) {
                            System.out.println("Mapping " + smallEndpts
                                    + " to " + bigEndpts);
                        }

                        Dictionary<Isomorphism.Mode, Object> partialResult = completeEdgeMapsHelper(
                                edgeMap, smallGraph, bigGraph, mode,
                                symBreakers, null, outputData);

                        if (DEBUG) {
                            System.out.println("PARTIAL RESULT: "
                                    + partialResult);
                        }

                        switch (mode) {
                            case OUTPUT_FILE:
                            // nothing more to do here
                            // purposefully falls through to case COUNT
                            case COUNT:
                                result.put(mode, (Integer) result.get(mode)
                                        + (Integer) partialResult.get(mode));
                                break;
                            case FIND:
                            case FIND_MAP:
                                Object found = partialResult.get(mode);
                                if (found != null) {
                                    if (mode == Mode.FIND) {
                                        result.put(mode, new HashSet<Edge>(
                                                ((Dictionary<Edge, Edge>) partialResult
                                                .get(mode)).values()));
                                    } else if (mode == Mode.FIND_MAP) {
                                        result.put(mode,
                                                (Dictionary<Edge, Edge>) partialResult
                                                .get(mode));
                                    } else {
                                        unknownMode(mode);
                                    }
                                }
                                return result;
                            case FIND_ALL:
                                List<Set<Edge>> subgraphs = Tools
                                        .map(
                                                MapUtils
                                                .mapsToValueSets((List<Dictionary<Edge, Edge>>)partialResult
                                                        .get(mode)),
                                                getOrigEdges); // NB:
                                // partialResults
                                // will actually
                                // be of type
                                // List<EdgeMap>,
                                // but since
                                // EdgeMap
                                // implements
                                // Dictionary<Edge,
                                // Edge> this is
                                // OK. Also the
                                // generic type
                                // is not
                                // checked at
                                // runtime.
                                ((Set<Set<Edge>>) result.get(mode))
                                        .addAll(subgraphs);
                                break;
                            case FIND_ALL_MAPS:
                                List<Dictionary<Edge, Edge>> subgraphMaps = Tools.map(
                                        (List<Dictionary<Edge, Edge>>)partialResult
                                        .get(mode), getOrigEdgeMap);
                                ((List<Dictionary<Edge, Edge>>) result.get(mode))
                                        .addAll(subgraphMaps);
                                break;
                            default:
                                unknownMode(mode);
                        }
                    }
                }
            }

            if (!bigGraph.getEdges().contains(bigE)) {
                System.out.println("BAD EDGE: " + bigE);
            }
            bigGraph.removeEdge(bigE);
            // for (Vertex v : Utils.getEndpoints(bigE))
            // if (v.degree() == 0)
            // bigGraph.removeVertex(v);
            bigEdges.remove(0);
            verbose.incrementCount(1);
        }

        return result;
    }

    public Dictionary<Isomorphism.Mode, Object> completeEdgeMapsHelper(
            EdgeMap edgeMap,
            AdjacencyGraph<Vertex, Edge<Vertex>> smallGraph,
            AdjacencyGraph<Vertex, Edge<Vertex>> bigGraph,
            Isomorphism.Mode mode,
            Set<OrbitSymmetryBreaker> symBreakers,
            Dictionary<Isomorphism.Mode, Object> result,
            OrderedPair<BufferedWriter, Transformer<Set<Edge>, String>> outputData)
{

    // System.out.println(edgeMap);
    if (result == null)
    {
        result = new Dictionary<Isomorphism.Mode, Object>();
        switch (mode)
        {
            case OUTPUT_FILE:
            // nothing more to do here
            // purposefully falls through to case COUNT
            case COUNT:
                result.put(mode, 0);
                break;
            case FIND:
            case FIND_MAP:
                result.put(mode, null);
                break;
            case FIND_ALL:
            case FIND_ALL_MAPS:
                result.put(mode, new ArrayList<EdgeMap>());
                break;
            default:
                unknownMode(mode);
        }
    }

    if (edgeMap.numVertices() == smallGraph.numVertices())
    {
        boolean completeMap = true;
        if (edgeMap.numEdges() < smallGraph.numEdges())
        {
            for (Edge e : (Set<Edge>)smallGraph.getEdges())
            {
                if (!edgeMap.containsKey(e))
                {
                    SimilarPair<Vertex> endpts = Utils.getEndpoints(e);
                    Edge img = edgeMap
                            .getVertex(endpts.getFirst())
                            .findEdge(edgeMap.getVertex(endpts.getSecond()));
                    if (img != null)
                    {
                        edgeMap.forcePut(e, img);
                    }
                    else
                    {
                        completeMap = false;
                        break;
                    }
                }
            }
        }

        if (completeMap)
        {
            switch (mode)
            {
                case OUTPUT_FILE:
                    tools.IOUtils.writeLineSafe(outputData.getFirst(),
                            outputData.getSecond().transform(
                                    new HashSet<Edge>(edgeMap.values())));
                // purposefully falls through to case COUNT
                case COUNT:
                    result.put(mode, ((Integer)result.get(mode)) + 1);
                    return result;
                case FIND:
                case FIND_MAP:
                    result.put(mode, edgeMap);
                    return result;
                case FIND_ALL:
                case FIND_ALL_MAPS:
                    ((List<EdgeMap>)result.get(mode)).add(edgeMap);
                    return result;
                default:
                    unknownMode(mode);
            }
        }
    }
    else
    {
        Dictionary<Vertex, Vertex> vertMap = edgeMap.getVertMap();
        Set<Vertex> mapped = vertMap.keySet(); // in smallGraph
        List<Vertex> unmapped = new ArrayList<Vertex>(smallGraph
                .getVertices()); // in smallGraph
        unmapped.removeAll(mapped);

        // Find the unmapped vertex with the most mapped neighbors
        // Use lexical comparison of degrees and then neighbor degrees to
        // break ties
        VertexNbrDegComp nbrDegComp = new VertexNbrDegComp(
                Keys.NBR_DEGREES, true);
        Vertex best = null; // Tools.max(unmapped, nbrDegComp); // lies in
                            // smallGraph
        int bestNumNbrs = 0;
        int bestNumSym = 0;
        Set<Vertex> bestNbrsMapped = null; // in smallGraph
        for (Vertex v : unmapped)
        {
            int numSymBreakers = 0;
            for (OrbitSymmetryBreaker b : symBreakers)
            {
                if (b.involves(v))
                {
                    numSymBreakers++;
                }
            }

            Set<Vertex> nbrs = new HashSet<Vertex>(v.getNeighbors()); // in
                                                                      // smallGraph
            nbrs.retainAll(mapped);
            int numNbrs = nbrs.size();

            if (numNbrs > bestNumNbrs)
            {
                bestNumNbrs = numNbrs;
                best = v;
                bestNbrsMapped = nbrs;
            }
            else if (numNbrs == bestNumNbrs)
            {
                if (numSymBreakers > bestNumSym)
                {
                    best = v;
                    bestNbrsMapped = nbrs;
                    bestNumNbrs = numNbrs;
                }
                else
                {
                    if (best != null)
                    {
                        // GlobalTimer.startTime(bestNbrDominatesKey);
                        // dom=true iff we care about degree and nbr degree
                        // domination
                        boolean dom = (v
                                .containsUserDatumKey(Keys.NBR_DEGREES) ? nbrDegComp
                                        .dominates(v, best)
                                        : false);
                        // GlobalTimer.recordTime(bestNbrDominatesKey);
                        if (dom)
                        {
                            best = v;
                            bestNbrsMapped = nbrs;
                            bestNumNbrs = numNbrs;
                        }
                    }
                    else
                    {
                        best = v;
                        bestNbrsMapped = nbrs;
                        bestNumNbrs = numNbrs;
                    }
                }
            }
        }

        String debugPrefix = "";
        for (int i = 0; i < vertMap.size(); i++)
        {
            debugPrefix += "  ";
        }

        if (DEBUG)
        {
            System.out.println(debugPrefix + "Best: " + best + " "
                    + vertMap);
        }

        // Possible vertices to map the best vertex to
        Set<Vertex> bigPossibilities = new HashSet<Vertex>(vertMap.get(
                bestNbrsMapped.iterator().next()).getNeighbors());
        bigPossibilities.removeAll(vertMap.values());
        for (Vertex smallV : mapped)
        {
            Vertex bigV = vertMap.get(smallV);
            if (bestNbrsMapped.contains(smallV))
            {
                if (MAKE_DIRECTED)
                {
                    boolean keepPreds = best.isPredecessorOf(smallV);
                    boolean keepSuccs = best.isSuccessorOf(smallV);
                    Set<Vertex> preds = bigV.getPredecessors();
                    Set<Vertex> succs = bigV.getSuccessors();
                    if (keepPreds)
                    {
                        bigPossibilities.retainAll(preds);
                    }
                    // else
                    // bigPossibilities.removeAll(preds);

                    if (keepSuccs)
                    {
                        bigPossibilities.retainAll(succs);
                    }
                    // else
                    // bigPossibilities.removeAll(succs);
                }
                else
                {
                    bigPossibilities.retainAll(bigV.getNeighbors());
                }
            }
            else
            {
                // bigPossibilities.removeAll(bigV.getNeighbors());
            }
        }

        if (DEBUG && bigPossibilities.size() == 0)
        {
            System.out.println(debugPrefix + "  REJECTED B/C NO POSS");
        }

        poss:
        for (Vertex p : bigPossibilities)
        {
            if (DEBUG)
            {
                System.out.print(debugPrefix + "Poss: " + best + "->" + p
                        + "(" + p.getUserDatum(Keys.ORDER) + ")");
            }

            // NB: I'm pretty sure this is unnecessary, as its taken care of
            // by
            // the above restrictions on bigPossibilities -JAG 24 Dec 2006
            // [though the
            // code was commented out much earlier]
            // if(nbrDegComp.dominates(p, best))
            // continue poss;
            for (OrbitSymmetryBreaker b : symBreakers)
            {
                if (DEBUG)
                {
                    System.out.println("CHECKING SYMBREAKER: " + b
                            + " against " + vertMap + " + " + best + "="
                            + p);
                }
                if (!b.evaluate(vertMap, best, p))
                {
                    if (DEBUG)
                    {
                        System.out.println(" REJECTED");
                    }
                    continue poss;
                }
            }

            if (DEBUG)
            {
                System.out.println(" ACCEPTED");
            }

            EdgeMap newMap = new EdgeMap(edgeMap);
            for (Edge e : (Set<Edge>)best.getIncidentEdges())
            {
                SimilarPair<Vertex> endpts = Utils.getEndpoints(e);
                if (mapped.contains(endpts.getOther(best)))
                {
                    Vertex[] endptImgs = new Vertex[2];
                    for (int i = 0; i < 2; i++)
                    {
                        endptImgs[i] = vertMap.get(endpts.get(i));
                        if (endptImgs[i] == null
                                || endpts.get(i).equals(best))
                        {
                            endptImgs[i] = p;
                        }
                    }

                    newMap.put(SimilarOrderedPair.ensureOrdered(endpts),
                            new SimilarOrderedPair<Vertex>(endptImgs));
                }
            }

            completeEdgeMapsHelper(newMap, smallGraph, bigGraph, mode,
                    symBreakers, result, outputData);
            if ((mode == Mode.FIND || mode == Mode.FIND_MAP)
                    && result.get(mode) != null)
            {
                return result;
            }
        }
    }

    return result;
}
    }
}

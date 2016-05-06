using MODA.Impl.Isomorphism;
using MODA.Impl.Tools;
using MODA.Impl.Transformers;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl.Isomorphism
{
    public class SymmetryIsomorphism<TVertex, TGraph> : Isomorphism<TVertex, TGraph> where TGraph : UndirectedGraph<TVertex, Edge<TVertex>>
    {
        protected SymmetryIsomorphism()
        {

        }

        private static SymmetryIsomorphism<TVertex, TGraph> theInstance;

        new public static SymmetryIsomorphism<TVertex, TGraph> getInstance()
        {
            if (theInstance == null)
            {
                theInstance = new SymmetryIsomorphism<TVertex, TGraph>();
            }

            return theInstance;
        }

        public override Dictionary<Mode, object> completeMapsHelper(Dictionary<TVertex, TVertex> vertMap, TGraph smallGraph, TGraph bigGraph, Mode mode, Dictionary<Mode, object> result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Since findIsomorphismHelper must return all isomorphisms, including those
        /// that differ only by an automorphism, simply used the method from
        /// MappingIsomorphism.
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public override Dictionary<Mode, object> findIsomorphismHelper(TGraph g1, TGraph g2, Mode mode)
        {
            return MappingIsomorphism<TVertex, TGraph>.getInstance().findIsomorphismHelper(g1, g2, mode);
        }

        public override Dictionary<Mode, object> inducedSubgraphHelper(TGraph smallGraph, TGraph bigGraph, StandardVerbosifier verbose, Mode mode)
        {
            var result = new Dictionary<Mode, object>();
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
                    result.put(mode, new HashSet<HashSet<TVertex>>());
                    break;
                case Mode.FIND_ALL_MAPS:
                    result.put(mode, new List<Dictionary<TVertex, TVertex>>());
                    break;
                default:
                    UnknownMode(mode);
                    break;
            }

            // must be constructed before bigGraph is locally replaced with a copy
            // of itself
            // or you have to create a variable to keep a local reference to the
            // original bigGraph
            GetEqualVertexTransformer<TVertex> getOrigVert = new GetEqualVertexTransformer<TVertex>(
                    bigGraph);

            // For efficiency, define these transformers OUTSIDE any loop
            // used when mode == Mode.FIND_ALL
            Transformer<HashSet<TVertex>, HashSet<TVertex>> getOrigVerts = new GetOriginalVertexTransformer<TVertex>(getOrigVert);

            // For efficiency, define these transformers OUTSIDE any loop
            // used when mode == Mode.FIND_ALL_MAPS
            Transformer<Dictionary<TVertex, TVertex>, Dictionary<TVertex, TVertex>> getOrigVertMap = new GetOriginalVertexMapTransformer<TVertex>(getOrigVert);

            //bigGraph = bigGraph.Clone(); // copy bigGraph so we can remove
            //                             // vertices from it as we go without
            //                             // damaging the input graph
            //                             // ? smallGraph = (Graph) smallGraph.copy();

            //Utils.addNeighborDegreeLists(bigGraph, Keys.NBR_DEGREES,
            //        UserData.REMOVE);
            //Utils.addNeighborDegreeLists(smallGraph, Keys.NBR_DEGREES,
            //        UserData.REMOVE);

            //VertexNbrDegComp allNbrDegComp = new VertexNbrDegComp(Keys.NBR_DEGREES,
            //        true);
            //Comparator<TVertex> comp = new LexicalComparator<TVertex>(
            //        VertexDegreeComparator.getInstance(), allNbrDegComp);

            //// Sort vertices in big graph by ascending degree and then by nbg
            //// degrees
            //List<TVertex> bigVerts = new List<TVertex>(bigGraph.Vertices);

            //if (SORT)
            //{
            //    Collections.sort(bigVerts, comp);
            //}

            //for (int i = 0; i < bigVerts.Count; i++)
            //{
            //    //bigVerts[i].addUserDatum(Keys.ORDER, i,
            //    //        UserData.REMOVE);
            //}

            //verbose.setPrintEvery(bigVerts.Count / 20);

            //Dictionary<TVertex, HashSet<OrbitSymmetryBreaker<TVertex>>> symBreakers = breakSymmetriesPerVertex(smallGraph); // usually
            //                                                                                                                // data
            //List<TVertex> smallVerts = new List<TVertex>(symBreakers.Keys);

            //if (SORT)
            //{
            //    Collections.sort(smallVerts, comp);
            //}

            //Dictionary<TVertex, TVertex> vertMap = new Dictionary<TVertex, TVertex>();

            //while (bigVerts.Count > 0)
            //{
            //    TVertex bigV = bigVerts[0];

            //    verbose.printlnIter(bigVerts.Count + " " + bigV.degree());

            //    for (int smallVIndex = 0; smallVIndex < smallVerts.Count; smallVIndex++)
            //    {
            //        TVertex smallV = smallVerts[smallVIndex];

            //        if (allNbrDegComp.dominates(bigV, smallV))
            //        {
            //            vertMap.Clear();
            //            vertMap.put(smallV, bigV);
            //            if (Isomorphism.DEBUG)
            //            {
            //                System.out.println("Mapping " + smallV + " to " + bigV
            //                        + "(" + bigV.getUserDatum(Keys.ORDER) + ")");
            //            }

            //            Dictionary<Mode, object> partialResult = completeMapsHelper(
            //                    vertMap, smallGraph, bigGraph, mode, symBreakers
            //                    .get(smallV), null);
            //            switch (mode)
            //            {
            //                case Mode.COUNT:
            //                    result.put(mode, (int)result.get(mode)
            //                            + (int)partialResult.get(mode));
            //                    break;
            //                case Mode.FIND:
            //                case Mode.FIND_MAP:
            //                    Object found = partialResult.get(mode);
            //                    if (found != null)
            //                    {
            //                        if (mode == Mode.FIND)
            //                        {
            //                            result.put(mode, new HashSet<TVertex>(
            //                                    ((Dictionary<TVertex, TVertex>)partialResult
            //                                    .get(mode)).Values));
            //                        }
            //                        else if (mode == Mode.FIND_MAP)
            //                        {
            //                            result.put(mode,
            //                                    (Dictionary<TVertex, TVertex>)partialResult
            //                                    .get(mode));
            //                        }
            //                        else
            //                        {
            //                            UnknownMode(mode);
            //                        }
            //                    }
            //                    return result;
            //                case Mode.FIND_ALL:
            //                    List<HashSet<TVertex>> subgraphs = Tools
            //                            .map(
            //                                    MapUtils
            //                                    .mapsToValueSets((List<Dictionary<TVertex, TVertex>>)partialResult
            //                                            .get(mode)),
            //                                    getOrigVerts);
            //                    ((HashSet<HashSet<TVertex>>)result.get(mode)).0(subgraphs);
            //                    break;
            //                case Mode.FIND_ALL_MAPS:
            //                    List<Dictionary<TVertex, TVertex>> subgraphMaps = Tools.map(
            //                            (List<Dictionary<TVertex, TVertex>>)partialResult
            //                            .get(mode), getOrigVertMap);
            //                    ((List<Dictionary<TVertex, TVertex>>)result.get(mode))
            //                            .addAll(subgraphMaps);
            //                    break;
            //                default:
            //                    UnknownMode(mode);
            //                    break;
            //            }

            //        }
            //    }

            //    bigGraph.removeVertex(bigV);
            //    bigVerts.Remove(0);
            //    verbose.incrementCount(1);
            //}

            return result;
        }
    }
}
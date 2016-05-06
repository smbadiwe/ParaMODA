using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MODA.Impl.Tools;
using MODA.Impl;

namespace MODA.Impl.Isomorphism
{
    public class MappingIsomorphism<TVertex, TGraph> : Isomorphism<TVertex, TGraph> where TGraph : UndirectedGraph<TVertex, Edge<TVertex>>
    {
        protected MappingIsomorphism()
        {

        }

        private static MappingIsomorphism<TVertex, TGraph> theInstance;

        new public static MappingIsomorphism<TVertex, TGraph> getInstance()
        {
            if (theInstance == null)
            {
                theInstance = new MappingIsomorphism<TVertex, TGraph>();
            }

            return theInstance;
        }

        public override Dictionary<Mode, object> completeMapsHelper(Dictionary<TVertex, TVertex> vertMap, TGraph smallGraph, TGraph bigGraph, Mode mode, Dictionary<Mode, object> result)
        {
            if (result == null)
            {
                result = new Dictionary<Mode, object>();
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
                    case Mode.FIND_ALL_MAPS:
                        result.put(mode, new List<Dictionary<TVertex, TVertex>>());
                        break;
                    default:
                        UnknownMode(mode);
                        break;
                }
            }

            if (vertMap.Count == smallGraph.VertexCount)
            {
                switch (mode)
                {
                    case Mode.COUNT:
                        result.put(mode, ((int)result.get(mode)) + 1);
                        return result;
                    case Mode.FIND:
                    case Mode.FIND_MAP:
                        result.put(mode, vertMap);
                        return result;
                    case Mode.FIND_ALL:
                    case Mode.FIND_ALL_MAPS:
                        ((List<Dictionary<TVertex, TVertex>>)result.get(mode)).Add(vertMap);
                        return result;
                    default:
                        UnknownMode(mode);
                        break;
                }
            }
            else
            {
                #region Else
                //var mapped = vertMap.Keys;
                //List<TVertex> unmapped = new List<TVertex>(smallGraph.Vertices);
                //unmapped.RemoveAll(x => mapped.Contains(x));

                //// Console.WriteLine(mapped + " " + unmapped);
                //// Find the unmapped vertex with the most mapped neighbors
                //// Use lexical comparison of degrees and then neighbor degrees to
                //// break ties
                //VertexNbrDegComp nbrDegComp = new VertexNbrDegComp(
                //        Keys.NBR_DEGREES, true);
                //TVertex best; // Tools.max(unmapped, nbrDegComp);
                //int bestNumNbrs = 0;
                //HashSet<TVertex> bestNbrs = null;
                //foreach (TVertex v in unmapped)
                //{
                //    HashSet<TVertex> nbrs = new HashSet<TVertex>(v.getNeighbors());
                //    nbrs.retainAll(mapped);
                //    int numNbrs = nbrs.Count;
                //    if (numNbrs > bestNumNbrs)
                //    {
                //        bestNumNbrs = numNbrs;
                //        best = v;
                //        bestNbrs = nbrs;
                //    }
                //    else if (numNbrs == bestNumNbrs)
                //    {
                //        if (best != null)
                //        {
                //            // GlobalTimer.startTime(bestNbrDominatesKey);
                //            bool dom = (v.containsUserDatumKey(Keys.NBR_DEGREES) ? nbrDegComp
                //                    .dominates(v, best)
                //                    : false);
                //            // GlobalTimer.recordTime(bestNbrDominatesKey);
                //            if (dom)
                //            {
                //                best = v;
                //                bestNbrs = nbrs;
                //            }
                //        }
                //        else
                //        {
                //            best = v;
                //            bestNbrs = nbrs;
                //        }
                //    }
                //}

                //// Set<Vertex> goodUnmapped = new HashSet<Vertex>(unmapped);
                //// Tools.filter(goodUnmapped, new
                //// AuxiliaryPredicate<Set<Vertex>>(mapped) {
                //// public boolean evaluate(Object o) {
                //// Vertex v = (Vertex)o;
                //// for(Vertex m : aux)
                //// if(m.isNeighborOf(v))
                //// return true;
                ////
                //// return false;
                //// }
                //// });
                ////
                //// best = Tools.max(goodUnmapped, nbrDegComp);
                //// bestNbrs = new HashSet<Vertex>(best.getNeighbors());
                //// bestNbrs.retainAll(mapped);
                //if (bestNbrs.Count == 0)
                //{
                //    Console.WriteLine("ERROR WILL ROBINSON");
                //    Console.WriteLine("graph: " + Utils.graphDescription(smallGraph, "\n"));
                //    Console.WriteLine("mapped: " + mapped);
                //    Console.WriteLine("not mapped: " + unmapped);
                //}
                //// Possibile vertices to map the best vertex to
                //HashSet<TVertex> bigPossibilities = new HashSet<TVertex>(vertMap.get(bestNbrs.GetEnumerator().Current).getNeighbors());
                //bigPossibilities.RemoveWhere(x => vertMap.ContainsValue(x));
                //foreach (TVertex smallV in mapped)
                //{
                //    TVertex bigV = vertMap.get(smallV);
                //    if (bestNbrs.Contains(smallV))
                //    {
                //        if (MAKE_DIRECTED)
                //        {
                //            bool keepPreds = best.isPredecessorOf(smallV);
                //            bool keepSuccs = best.isSuccessorOf(smallV);
                //            HashSet<TVertex> preds = bigV.getPredecessors();
                //            HashSet<TVertex> succs = bigV.getSuccessors();
                //            if (keepPreds)
                //            {
                //                bigPossibilities.retainAll(preds);
                //            }
                //            else
                //            {
                //                bigPossibilities.RemoveWhere(x => preds.Contains(x));
                //            }

                //            if (keepSuccs)
                //            {
                //                bigPossibilities.retainAll(succs);
                //            }
                //            else
                //            {
                //                bigPossibilities.RemoveWhere(x => preds.Contains(x));
                //            }
                //        }
                //        else
                //        {
                //            bigPossibilities.retainAll(bigV.getNeighbors());
                //        }
                //    }
                //    else
                //    {
                //        bigPossibilities.removeAll(bigV.getNeighbors());
                //    }
                //}

                //// Console.WriteLine(" Possibilities: " + bigPossibilities);
                //foreach (TVertex p in bigPossibilities)
                //{
                //    // if(!nbrDegComp.dominates(p, best)) {
                //    // continue;
                //    Dictionary<TVertex, TVertex> newMap = new Dictionary<TVertex, TVertex>(vertMap);
                //    newMap.put(best, p);
                //    completeMapsHelper(newMap, smallGraph, bigGraph, mode, result);
                //    if ((mode == Mode.FIND || mode == Mode.FIND_MAP)
                //            && result.get(mode) != null)
                //    {
                //        return result;
                //    }
                //} 
                #endregion
            }

            return result;
        }

        public override Dictionary<Mode, object> findIsomorphismHelper(TGraph g1, TGraph g2, Mode mode)
        {
            Dictionary<Mode, object> result = new Dictionary<Mode, object>();

            switch (mode)
            {
                case Mode.FIND:
                case Mode.FIND_MAP:
                    result.put(mode, null);
                    break;
                case Mode.FIND_ALL:
                case Mode.FIND_ALL_MAPS:
                    result.put(mode, new List<Dictionary<TVertex, TVertex>>());
                    break;
                case Mode.COUNT:
                    result.put(mode, 0);
                    break;
                default:
                    UnknownMode(mode);
                    break;
            }
            TGraph[] graph = { g1, g2 };
            TVertex start1;
            HashSet<TVertex> verts0OfDegree = null;
            if (true) //if (PredicateUtils.enforcesUndirected(g1) && PredicateUtils.enforcesUndirected(g2))
            {
                SimpleDegreeDistribution<TVertex>[] dd = new SimpleDegreeDistribution<TVertex>[2];
                for (int i = 0; i < 2; i++)
                {
                    dd[i] = new SimpleDegreeDistribution<TVertex>(graph[i]);
                }

                if (!dd[0].Equals(dd[1]))
                {
                    return result;
                }

                int bestDeg = dd[0].Current; //.iterator().next();
                int numWithBestDeg = dd[0].numberWithDegree(bestDeg);
                foreach (var deg in dd[0])
                {
                    int numWithThisDeg = dd[0].numberWithDegree(deg);
                    if (numWithThisDeg < numWithBestDeg)
                    {
                        bestDeg = deg;
                        numWithBestDeg = numWithThisDeg;
                    }
                }

                // This is where we assume the graphs are connected
                // If the graphs weren't connected, we'd have to iterate over one
                // starting point per connected component of g2, and we'd have
                // to modify numCompleteMaps to return maps that are complete for
                // a single component
                start1 = dd[1].verticesWithDegree(bestDeg).GetEnumerator().Current; //.iterator().next();
                verts0OfDegree = dd[0].verticesWithDegree(bestDeg);
            }
            #region Else - Do Directed
            //else
            //{
            //    DirectedDegreeDistribution[] dd = new DirectedDegreeDistribution[2];
            //    for (int i = 0; i < 2; i++)
            //    {
            //        dd[i] = new DirectedDegreeDistribution(graph[i]);
            //    }

            //    if (!dd[0].equals(dd[1]))
            //    {
            //        return result;
            //    }

            //    Degree bestDeg = dd[0].getDegrees().iterator().next();
            //    int numWithBestDeg = dd[0].numberWithDegree(bestDeg);
            //    foreach (Degree deg in dd[0])
            //    {
            //        int numWithThisDeg = dd[0].numberWithDegree(deg);
            //        if (numWithThisDeg < numWithBestDeg)
            //        {
            //            bestDeg = deg;
            //            numWithBestDeg = numWithThisDeg;
            //        }
            //    }

            //    // This is where we assume the graphs are connected
            //    // If the graphs weren't connected, we'd have to iterate over one
            //    // starting point per connected component of g2, and we'd have
            //    // to modify numCompleteMaps to return maps that are complete for
            //    // a single component
            //    start1 = dd[1].verticesWithDegree(bestDeg).iterator().next();
            //    verts0OfDegree = dd[0].verticesWithDegree(bestDeg);
            //} 
            #endregion

            foreach (TVertex v in verts0OfDegree)
            {
                Dictionary<TVertex, TVertex> m = new Dictionary<TVertex, TVertex>();
                m.put(v, start1);
                switch (mode)
                {
                    case Mode.COUNT:
                        result.put(mode, Convert.ToInt32(result.get(mode))
                                + numCompleteMaps(m, graph[0], graph[1]));
                        break;
                    case Mode.FIND:
                    case Mode.FIND_MAP:
                        Dictionary<TVertex, TVertex> theCompleteMap = completeMap(m, graph[0], graph[1]);
                        if (theCompleteMap != null)
                        {
                            result.put(mode, theCompleteMap);
                            return result;
                        }
                        break;
                    case Mode.FIND_ALL:
                    case Mode.FIND_ALL_MAPS:
                        ((List<Dictionary<TVertex, TVertex>>)result.get(mode))
                                .AddRange(completeMaps(m, graph[0], graph[1]));
                        break;
                    default:
                        UnknownMode(mode);
                        break;
                }
            }

            return result;
        }

        public override Dictionary<Mode, object> inducedSubgraphHelper(TGraph g1, TGraph g2, StandardVerbosifier verbose, Mode mode)
        {
            throw new NotImplementedException();
        }
    }
}

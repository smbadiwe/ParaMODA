using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace QuickGraph
{
    public delegate bool EdgeEqualityComparer<TVertex>(Edge<TVertex> edge, TVertex source, TVertex target);

    [DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
    public class UndirectedGraph<TVertex>
    {
        private readonly bool allowParallelEdges = true;
        private int edgeCount = 0;
        private Dictionary<TVertex, List<TVertex>> edges;

        public UndirectedGraph()
            : this(true)
        { }

        public UndirectedGraph(bool allowParallelEdges)
        {
            this.allowParallelEdges = allowParallelEdges;
            this.edges = new Dictionary<TVertex, List<TVertex>>();
        }

        public int VertexCount
        {
            get { return this.edges.Count; }
        }

        public IEnumerable<TVertex> Vertices
        {
            get { return this.edges.Keys; }
        }

        public int EdgeCount
        {
            get { return this.edgeCount; }
        }

        public IEnumerable<Edge<TVertex>> Edges
        {
            get
            {
                var edgeColors = new List<Edge<TVertex>>(); //HashSet
                foreach (var vertsSet in this.edges)
                {
                    foreach (var vert in vertsSet.Value)
                    {
                        Edge<TVertex> edge = new Edge<TVertex>(vertsSet.Key, vert);
                        if (!edgeColors.Contains(edge))
                            edgeColors.Add(edge);
                    }
                }
                return edgeColors;
            }
        }

        /// <summary>
        /// This returns the neighbourhood of the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public IList<TVertex> GetNeighbors(TVertex vertex)
        {
            List<TVertex> adjEdges;
            if (this.edges.TryGetValue(vertex, out adjEdges))
            {
                return adjEdges;
            }
            return new TVertex[0];
        }

        /// <summary>
        /// NB: The degree sequence of an undirected graph is the non-increasing sequence of its vertex degrees;
        /// We return the vertices here instead of the vertex degrees 
        /// </summary>
        /// <param name="count">The expected number of items to return. This value is usually less than the <see cref="VertexCount"/></param>
        /// <returns></returns>
        public IList<TVertex> GetNodesSortedByDegree(int count)
        {
            var tempList = new Dictionary<TVertex, int>(count);
            foreach (var node in Vertices.Take(count))
            {
                tempList.Add(node, this.GetDegree(node));
            }

            var listToReturn = new List<TVertex>(count);
            foreach (var item in tempList.OrderByDescending(x => x.Value))
            {
                listToReturn.Add(item.Key);
            }

            tempList.Clear();
            tempList = null;
            return listToReturn;
        }

        /// <summary>
        /// NB: The degree sequence of an undirected graph is the non-increasing sequence of its vertex degrees;
        /// But here, we return the list of degrees in ascending order. The expected use case will not care about
        /// the order direction
        /// </summary>
        /// <returns></returns>
        public int[] GetReverseDegreeSequence()
        {
            var tempList = new int[VertexCount];
            int i = 0;
            foreach (var node in Vertices)
            {
                tempList[i++] = GetDegree(node);
            }
            Array.Sort(tempList);
            return tempList;
        }

        public UndirectedGraph<TVertex> Clone()
        {
            var inputGraphClone = new UndirectedGraph<TVertex>(allowParallelEdges)
            {
                edgeCount = edgeCount,
            };
            foreach (var edge in edges)
            {
                inputGraphClone.edges.Add(edge.Key, new List<TVertex>(edge.Value));
            }

            return inputGraphClone;
        }

        public override string ToString()
        {
            if (this.EdgeCount == 0) return "";
            var sb = new System.Text.StringBuilder("Graph-Edges_");
            foreach (var edge in this.Edges)
            {
                sb.AppendFormat("[{0}],", edge);
            }

            return sb.ToString();
        }

        public bool RemoveVertex(TVertex v)
        {
            this.ClearAdjacentEdges(v);
            return this.edges.Remove(v);
        }

        public void Clear()
        {
            this.edgeCount = 0;
            this.edges.Clear();
            this.edges = null;
        }

        public void ClearAdjacentEdges(TVertex v)
        {
            List<TVertex> ends = new List<TVertex>(this.edges[v]);
            this.edgeCount -= ends.Count;
            foreach (var end in ends)
            {
                List<TVertex> otherEnds;
                if (this.edges.TryGetValue(end, out otherEnds))
                {
                    otherEnds.Remove(v);
                }
            }
            ends.Clear();
            ends = null;
        }

        public bool TryGetEdge(TVertex source, TVertex target, out Edge<TVertex> edge)
        {
            List<TVertex> ends;
            if (this.edges.TryGetValue(source, out ends) && ends.Contains(target))
            {
                edge = new Edge<TVertex>(source, target);
                return true;
            }

            edge = default(Edge<TVertex>);
            return false;
        }

        public bool ContainsEdge(TVertex source, TVertex target)
        {
            List<TVertex> ends;
            return (this.edges.TryGetValue(source, out ends) && ends.Contains(target));
        }

        public bool AddVerticesAndEdge(TVertex source, TVertex target)
        {
            List<TVertex> nodesConnectedToSource, nodesConnectedToTarget;
            if (edges.TryGetValue(source, out nodesConnectedToSource))
            {
                //We've seen this source before. So...
                if (nodesConnectedToSource.Contains(target))
                {
                    return false; // already exists
                }

                nodesConnectedToSource.Add(target);
                //...and add the source to the list of targets too
                if (edges.TryGetValue(target, out nodesConnectedToTarget))
                {
                    if (!nodesConnectedToTarget.Contains(source))
                    {
                        nodesConnectedToTarget.Add(source);
                    }
                }
                else
                {
                    edges[target] = new List<TVertex>(1) { source };
                }
            }
            // since we don't care about direction, chech againby reversing them
            else if (edges.TryGetValue(target, out nodesConnectedToTarget))
            {
                if (nodesConnectedToTarget.Contains(source))
                {
                    return false; // already exists
                }

                nodesConnectedToTarget.Add(source);

                if (edges.TryGetValue(source, out nodesConnectedToSource))
                {
                    if (!nodesConnectedToSource.Contains(target))
                    {
                        nodesConnectedToSource.Add(target);
                    }
                }
                else
                {
                    edges[source] = new List<TVertex> { target };
                }
            }
            else // neither exists. So, add them
            {
                edges[source] = new List<TVertex>(1) { target };
                edges[target] = new List<TVertex>(1) { source };
            }

            this.edgeCount++;
            return true;
        }

        public bool AddVerticesAndEdge(Edge<TVertex> edge)
        {
            return AddVerticesAndEdge(edge.Source, edge.Target);
        }

        public int AddVerticesAndEdgeRange(IEnumerable<Edge<TVertex>> edges)
        {
            int count = 0;
            foreach (var edge in edges)
            {
                if (this.AddVerticesAndEdge(edge.Source, edge.Target))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Gets the degree of the given vertex, <paramref name="v"/>/
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public int GetDegree(TVertex v)
        {
            List<TVertex> edges;
            if (this.edges.TryGetValue(v, out edges))
            {
                return edges.Count;
            }
            return 0;
        }

        /// <summary>
        /// Adds the vertices and edge. This is different from <see cref="AddVerticesAndEdge(Edge{TVertex})" /> only in the sense that
        /// we allow duplicate items in the target lists temporarily.
        /// </summary>
        /// <param name="edge">The edge.</param>
        /// <returns></returns>
        private bool AddVerticesAndEdgeStraight(Edge<TVertex> edge)
        {
            TVertex source = edge.Source, target = edge.Target;
            List<TVertex> nodesConnectedToSource, nodesConnectedToTarget;
            if (edges.TryGetValue(source, out nodesConnectedToSource))
            {
                //We've seen this source before. So...
                if (nodesConnectedToSource.Contains(target))
                {
                    return false; // already exists
                }

                nodesConnectedToSource.Add(target);
                //...and add the source to the list of targets too
                if (edges.TryGetValue(target, out nodesConnectedToTarget))
                {
                    nodesConnectedToTarget.Add(source);
                }
                else
                {
                    edges[target] = new List<TVertex> { source };
                }
            }
            // since we don't care about direction, chech againby reversing them
            else if (edges.TryGetValue(target, out nodesConnectedToTarget))
            {
                if (nodesConnectedToTarget.Contains(source))
                {
                    return false; // already exists
                }

                nodesConnectedToTarget.Add(source);

                if (edges.TryGetValue(source, out nodesConnectedToSource))
                {
                    nodesConnectedToSource.Add(target);
                }
                else
                {
                    edges[source] = new List<TVertex> { target };
                }
            }
            else // neither exists. So, add them
            {
                edges[source] = new List<TVertex> { target };
                edges[target] = new List<TVertex> { source };
            }

            this.edgeCount++;
            return true;
        }
    }
}

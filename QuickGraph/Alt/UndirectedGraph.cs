using QuickGraph.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace QuickGraph
{
    public delegate bool EdgeEqualityComparer<TVertex>(Edge<TVertex> edge, TVertex source, TVertex target);
    
    [DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
    public class UndirectedGraph<TVertex>
    {
        private readonly bool allowParallelEdges = true;
        private int edgeCount = 0;
        private int edgeCapacity = 4;

        public UndirectedGraph()
            : this(true)
        { }

        public UndirectedGraph(bool allowParallelEdges)
        {
            this.allowParallelEdges = allowParallelEdges;
            this.edges2 = new Dictionary<TVertex, List<TVertex>>();
        }
        
        public int EdgeCapacity
        {
            get { return this.edgeCapacity; }
            set
            {
                this.edgeCapacity = value;
            }
        }

        #region Newly Added

        /// <summary>
        /// This returns the neighbourhood of the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public IList<TVertex> GetNeighbors(TVertex vertex)
        {
            List<TVertex> adjEdges;
            if (this.edges2.TryGetValue(vertex, out adjEdges))
            {
                return adjEdges;
            }
            return new TVertex[0];
        }

        /// <summary>
        /// NB: The degree sequence of an undirected graph is the non-increasing sequence of its vertex degrees;
        /// We return the vertices 
        /// </summary>
        /// <param name="count">The expected number of items to return. This value is usually less than the <see cref="VertexCount"/></param>
        /// <returns></returns>
        public IList<TVertex> GetNodesSortedByDegree(int count)
        {
            var tempList = new Dictionary<TVertex, int>(count);
            foreach (var node in Vertices.Take(count))
            {
                tempList.Add(node, this.AdjacentDegree(node));
            }

            var listToReturn = new List<TVertex>(count);
            foreach (var item in tempList.OrderByDescending(x => x.Value))
            {
                listToReturn.Add(item.Key);
            }

            tempList.Clear();
            return listToReturn;
        }

        /// <summary>
        /// NB: The degree sequence of an undirected graph is the non-increasing sequence of its vertex degrees;
        /// </summary>
        /// <returns></returns>
        public int[] GetDegreeSequence()
        {
            var listToReturn = new List<int>(VertexCount);
            foreach (var node in Vertices)
            {
                listToReturn.Add(this.AdjacentDegree(node));
            }
            
            return listToReturn.OrderByDescending(x => x).ToArray();
        }

        public UndirectedGraph<TVertex> Clone()
        {
            var inputGraphClone = new UndirectedGraph<TVertex>();
            foreach (var edge in this.Edges2)
            {
                inputGraphClone.AddVerticesAndEdge(edge.Source, edge.Target);
            }
            return inputGraphClone;
        }

        public override string ToString()
        {
            if (this.IsEdgesEmpty) return "";
            var sb = new System.Text.StringBuilder("Graph-Edges_");
            foreach (var edge in this.Edges2)
            {
                sb.AppendFormat("[{0}],", edge);
            }

            return sb.ToString();
        }
        #endregion

        #region IGraph<Vertex,Edge> Members
        public bool IsDirected
        {
            get { return false; }
        }

        public bool AllowParallelEdges
        {
            get { return this.allowParallelEdges; }
        }
        #endregion

        #region IMutableUndirected<Vertex,Edge> Members
        public bool RemoveVertex(TVertex v)
        {
            this.ClearAdjacentEdges(v);
            return this.edges2.Remove(v);
        }
        #endregion

        #region IMutableIncidenceGraph<Vertex,Edge> Members
        //public int RemoveAdjacenEdge<TVertex>If(TVertex v, EdgePredicate<TVertex> predicate)
        //{
        //    var ouEdge<TVertex>s = this.adjacentEdges[v];
        //    var edges = new List<Edge<TVertex>>(ouEdge<TVertex>s.Count);
        //    foreach (var edge in ouEdge<TVertex>s)
        //        if (predicate(edge))
        //            edges.Add(edge);

        //    this.RemoveEdges(edges);
        //    return edges.Count;
        //}

        //[ContractInvariantMethod]
        //void ObjectInvariant()
        //{
        //    Contract.Invariant(this.edgeCount >= 0);
        //}

        public void ClearAdjacentEdges(TVertex v)
        {
            List<TVertex> ends = new List<TVertex>(this.edges2[v]);
            this.edgeCount -= ends.Count;
            foreach (var end in ends)
            {
                List<TVertex> otherEnds;
                if (this.edges2.TryGetValue(end, out otherEnds))
                {
                    otherEnds.Remove(v);
                }
            }
            ends.Clear();
            //var edges = this.adjacentEdges[v].Clone();
            //this.edgeCount -= edges.Count;

            //foreach (var edge in edges)
            //{
            //    IEdgeList<TVertex> aEdges;
            //    if (this.adjacentEdges.TryGetValue(edge.Target, out aEdges))
            //        aEdges.Remove(edge);
            //    if (this.adjacentEdges.TryGetValue(edge.Source, out aEdges))
            //        aEdges.Remove(edge);
            //}
        }
        #endregion

        #region IMutableGraph<Vertex,Edge> Members - Cleared
        //public void TrimEdgeExcess()
        //{
        //    foreach (var edges in this.adjacentEdges.Values)
        //        edges.TrimExcess();
        //}

        //public void Clear()
        //{
        //    this.adjacentEdges.Clear();
        //    this.edgeCount = 0;
        //    this.OnCleared(EventArgs.Empty);
        //}

        //public event EventHandler Cleared;
        //private void OnCleared(EventArgs e)
        //{
        //    var eh = this.Cleared;
        //    if (eh != null)
        //        eh(this, e);
        //}
        #endregion

        #region IUndirectedGraph<Vertex,Edge> Members

        public bool TryGetEdge(TVertex source, TVertex target, out Edge<TVertex> edge)
        {
            List<TVertex> ends;
            if (this.edges2.TryGetValue(source, out ends) && ends.Contains(target))
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
            if (this.edges2.TryGetValue(source, out ends) && ends.Contains(target))
            {
                return true;
            }
            return false;
        }
        
        public bool IsVerticesEmpty
        {
            get { return this.edges2.Count == 0; }
        }

        public int VertexCount
        {
            get { return this.edges2.Count; }
        }

        public IEnumerable<TVertex> Vertices
        {
            get { return this.edges2.Keys; }
        }

        public bool ContainsVertex(TVertex vertex)
        {
            return this.edges2.ContainsKey(vertex);
        }
        #endregion

        private Dictionary<TVertex, List<TVertex>> edges2;
        public bool AddVerticesAndEdge(TVertex source, TVertex target)
        {
            List<TVertex> nodesConnectedToSource, nodesConnectedToTarget;
            if (edges2.TryGetValue(source, out nodesConnectedToSource))
            {
                //We've seen this source before. So...
                if (nodesConnectedToSource.Contains(target))
                {
                    return false; // already exists
                }
                else
                {
                    nodesConnectedToSource.Add(target);
                    //...and add the source to the list of targets too
                    if (edges2.TryGetValue(target, out nodesConnectedToTarget))
                    {
                        if (!nodesConnectedToTarget.Contains(source))
                        {
                            nodesConnectedToTarget.Add(source);
                        }
                    }
                    else
                    {
                        edges2[target] = new List<TVertex> { source };
                    }
                }
            }
            // since we don't care about direction, chech againby reversing them
            else if (edges2.TryGetValue(target, out nodesConnectedToTarget))
            {
                if (nodesConnectedToTarget.Contains(source))
                {
                    return false; // already exists
                }
                else
                {
                    nodesConnectedToTarget.Add(source);

                    if (edges2.TryGetValue(source, out nodesConnectedToSource))
                    {
                        if (!nodesConnectedToSource.Contains(target))
                        {
                            nodesConnectedToSource.Add(target);
                        }
                    }
                    else
                    {
                        edges2[source] = new List<TVertex> { target };
                    }
                }
            }
            else // neither exists. So, add them
            {
                edges2[source] = new List<TVertex> { target };
                edges2[target] = new List<TVertex> { source };
            }

            this.edgeCount++;
            return true;
        }

        #region IMutableEdgeListGraph<Vertex,Edge> Members
        public bool AddVerticesAndEdge(Edge<TVertex> edge)
        {
            return AddVerticesAndEdge(edge.Source, edge.Target);
        }

        public int AddVerticesAndEdgeRange(IEnumerable<Edge<TVertex>> edges)
        {
            int count = 0;
            foreach (var edge in edges)
                if (this.AddVerticesAndEdge(edge.Source, edge.Target))
                    count++;
            return count;
        }
        
        #endregion

        #region IEdgeListGraph<Vertex,Edge> Members
        public bool IsEdgesEmpty
        {
            get { return this.EdgeCount == 0; }
        }

        public int EdgeCount
        {
            get { return this.edgeCount; }
        }

        public IEnumerable<Edge<TVertex>> Edges2
        {
            get
            {
                var edgeColors = new Dictionary<Edge<TVertex>, GraphColor>(this.EdgeCount);
                foreach (var vertsSet in this.edges2)
                {
                    foreach (var vert in vertsSet.Value)
                    {
                        Edge<TVertex> edge = new Edge<TVertex>(vertsSet.Key, vert);
                        GraphColor c;
                        if (edgeColors.TryGetValue(edge, out c))
                            continue;

                        edgeColors.Add(edge, GraphColor.Black);
                        yield return edge;
                    }
                }
            }
        }
        
        #endregion

        #region IUndirectedGraph<Vertex,Edge> Members
        public int AdjacentDegree(TVertex v)
        {
            List<TVertex> edges;
            if (this.edges2.TryGetValue(v, out edges))
            {
                return edges.Count;
            }
            return 0;
        }

        public bool IsadjacentEdgesEmpty(TVertex v)
        {
            List<TVertex> edges;
            if (this.edges2.TryGetValue(v, out edges))
            {
                return edges.Count == 0;
            }
            return true;
        }
        #endregion
    }
}

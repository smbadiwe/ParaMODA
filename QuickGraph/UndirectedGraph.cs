using QuickGraph.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace QuickGraph
{
    public delegate bool EdgeEqualityComparer<TVertex, TEdge>(TEdge edge, TVertex source, TVertex target)
        where TEdge : IEdge<TVertex>;

    [DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
    public class UndirectedGraph<TVertex, TEdge>
        : IUndirectedGraph<TVertex, TEdge> //IMutableUndirectedGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        private readonly bool allowParallelEdges = true;
        private readonly VertexEdgeDictionary<TVertex, TEdge> adjacentEdges;
        private readonly EdgeEqualityComparer<TVertex, TEdge> edgeEqualityComparer;
        private int edgeCount = 0;
        private int edgeCapacity = 4;

        public UndirectedGraph()
            : this(true)
        { }

        public UndirectedGraph(bool allowParallelEdges)
        {
            this.allowParallelEdges = allowParallelEdges;
            this.edgeEqualityComparer = EdgeExtensions.GetUndirectedVertexEquality<TVertex, TEdge>();
            this.adjacentEdges = new VertexEdgeDictionary<TVertex, TEdge>(EqualityComparer<TVertex>.Default);
        }

        //public UndirectedGraph(bool allowParallelEdges, EdgeEqualityComparer<TVertex, TEdge> edgeEqualityComparer)
        //    : this(allowParallelEdges, edgeEqualityComparer, -1)
        //{
        //}

        //public UndirectedGraph(bool allowParallelEdges, EdgeEqualityComparer<TVertex, TEdge> edgeEqualityComparer, int vertexCapacity)
        //    : this(allowParallelEdges, edgeEqualityComparer, vertexCapacity, EqualityComparer<TVertex>.Default)
        //{ }

        //public UndirectedGraph(bool allowParallelEdges, EdgeEqualityComparer<TVertex, TEdge> edgeEqualityComparer, int vertexCapacity, IEqualityComparer<TVertex> vertexComparer)
        //{
        //    if (edgeEqualityComparer == null) throw new ArgumentNullException("edgeEqualityComparer");
        //    if (vertexComparer == null) throw new ArgumentNullException("vertexComparer");

        //    this.allowParallelEdges = allowParallelEdges;
        //    this.edgeEqualityComparer = edgeEqualityComparer;
        //    if (vertexCapacity > -1)
        //        this.adjacentEdges = new VertexEdgeDictionary<TVertex, TEdge>(vertexCapacity, vertexComparer);
        //    else
        //        this.adjacentEdges = new VertexEdgeDictionary<TVertex, TEdge>(vertexComparer);
        //}

        public EdgeEqualityComparer<TVertex, TEdge> EdgeEqualityComparer
        {
            get
            {
                return this.edgeEqualityComparer;
            }
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
        /// <param name="isG"></param>
        /// <returns></returns>
        public HashSet<TVertex> GetNeighbors(TVertex vertex)
        {
            IEdgeList<TVertex, TEdge> adjEdges;
            if (this.adjacentEdges.TryGetValue(vertex, out adjEdges))
            {
                var set = new HashSet<TVertex>();
                for (int i = 0; i < adjEdges.Count; i++)
                {
                    set.Add(adjEdges[i].Source);
                    set.Add(adjEdges[i].Target);
                }
                set.Remove(vertex);
                return set;
            }
            return new HashSet<TVertex>();
        }

        /// <summary>
        /// NB: The degree sequence of an undirected graph is the non-increasing sequence of its vertex degrees;
        /// </summary>
        /// <param name="count">The expected number of items to return. This value is usually less than the <see cref="VertexCount"/></param>
        /// <returns></returns>
        public IList<TVertex> GetDegreeSequence(int count)
        {
            if (this.IsVerticesEmpty) return new TVertex[0];

            var tempList = new Dictionary<TVertex, int>(count);
            int iter = 1;
            foreach (var node in Vertices)
            {
                tempList.Add(node, this.AdjacentDegree(node));
                iter++;
                if (iter == count) break;
            }

            var listToReturn = new List<TVertex>(count);
            foreach (var item in tempList.OrderByDescending(x => x.Value))
            {
                listToReturn.Add(item.Key);
            }

            tempList = null;
            return listToReturn; //.ToArray();
        }

        public UndirectedGraph<TVertex, TEdge> Clone()
        {
            var inputGraphClone = new UndirectedGraph<TVertex, TEdge>();
            inputGraphClone.AddVerticesAndEdgeRange(this.Edges);

            return inputGraphClone;
        }
        
        public override string ToString()
        {
            if (this.IsEdgesEmpty) return "";
            var sb = new System.Text.StringBuilder("Graph-Edges_");
            foreach (var edge in this.Edges)
            {
                sb.AppendFormat("[{0}],", edge);
            }
            //sb.AppendLine();
            return sb.ToString().TrimEnd();
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
        //public event VertexAction<TVertex> VertexAdded;
        //protected virtual void OnVertexAdded(TVertex args)
        //{
        //    this.VertexAdded?.Invoke(args);
        //}

        //public int AddVertexRange(IEnumerable<TVertex> vertices)
        //{
        //    int count = 0;
        //    foreach (var v in vertices)
        //        if (this.AddVertex(v))
        //            count++;
        //    return count;
        //}

        //public bool AddVertex(TVertex v)
        //{
        //    if (this.ContainsVertex(v))
        //        return false;

        //    var edges = this.EdgeCapacity < 0
        //        ? new EdgeList<TVertex, TEdge>()
        //        : new EdgeList<TVertex, TEdge>(this.EdgeCapacity);
        //    this.adjacentEdges.Add(v, edges);
        //    this.OnVertexAdded(v);
        //    return true;
        //}

        private IEdgeList<TVertex, TEdge> AddAndReturnEdges(TVertex v)
        {
            IEdgeList<TVertex, TEdge> edges;
            if (!this.adjacentEdges.TryGetValue(v, out edges))
                this.adjacentEdges[v] = edges = this.EdgeCapacity < 0
                    ? new EdgeList<TVertex, TEdge>()
                    : new EdgeList<TVertex, TEdge>(this.EdgeCapacity);

            return edges;
        }

        //public event VertexAction<TVertex> VertexRemoved;
        //protected virtual void OnVertexRemoved(TVertex args)
        //{
        //    this.VertexRemoved?.Invoke(args);
        //}

        public bool RemoveVertex(TVertex v)
        {
            this.ClearAdjacentEdges(v);
            bool result = this.adjacentEdges.Remove(v);

            //if (result)
            //    this.OnVertexRemoved(v);

            return result;
        }

        //public int RemoveVertexIf(VertexPredicate<TVertex> pred)
        //{
        //    var vertices = new List<TVertex>();
        //    foreach (var v in this.Vertices)
        //        if (pred(v))
        //            vertices.Add(v);

        //    foreach (var v in vertices)
        //        RemoveVertex(v);
        //    return vertices.Count;
        //}
        #endregion

        #region IMutableIncidenceGraph<Vertex,Edge> Members
        //public int RemoveAdjacentEdgeIf(TVertex v, EdgePredicate<TVertex, TEdge> predicate)
        //{
        //    var outEdges = this.adjacentEdges[v];
        //    var edges = new List<TEdge>(outEdges.Count);
        //    foreach (var edge in outEdges)
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
            var edges = this.adjacentEdges[v].Clone();
            this.edgeCount -= edges.Count;

            foreach (var edge in edges)
            {
                IEdgeList<TVertex, TEdge> aEdges;
                if (this.adjacentEdges.TryGetValue(edge.Target, out aEdges))
                    aEdges.Remove(edge);
                if (this.adjacentEdges.TryGetValue(edge.Source, out aEdges))
                    aEdges.Remove(edge);
            }
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
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            foreach (var e in this.AdjacentEdges(source))
            {
                if (this.edgeEqualityComparer(e, source, target))
                {
                    edge = e;
                    return true;
                }
            }

            edge = default(TEdge);
            return false;
        }

        public bool ContainsEdge(TVertex source, TVertex target)
        {
            TEdge edge;
            return this.TryGetEdge(source, target, out edge);
        }

        public TEdge AdjacentEdge(TVertex v, int index)
        {
            return this.adjacentEdges[v][index];
        }

        public bool IsVerticesEmpty
        {
            get { return this.adjacentEdges.Count == 0; }
        }

        public int VertexCount
        {
            get { return this.adjacentEdges.Count; }
        }

        public IEnumerable<TVertex> Vertices
        {
            get { return this.adjacentEdges.Keys; }
        }

        public bool ContainsVertex(TVertex vertex)
        {
            return this.adjacentEdges.ContainsKey(vertex);
        }
        #endregion

        #region IMutableEdgeListGraph<Vertex,Edge> Members
        public bool AddVerticesAndEdge(TEdge edge)
        {
            var sourceEdges = this.AddAndReturnEdges(edge.Source);
            var targetEdges = this.AddAndReturnEdges(edge.Target);
            //this.AddAndReturnEdges(edge.Target);

            if (!this.AllowParallelEdges)
            {
                if (this.ContainsEdgeBetweenVertices(sourceEdges, edge))
                    return false;
            }

            sourceEdges.Add(edge);
            if (!EdgeExtensions.IsSelfEdge<TVertex, TEdge>(edge))
                targetEdges.Add(edge);
            this.edgeCount++;
            //this.OnEdgeAdded(edge);

            return true;
        }

        public int AddVerticesAndEdgeRange(IEnumerable<TEdge> edges)
        {
            int count = 0;
            foreach (var edge in edges)
                if (this.AddVerticesAndEdge(edge))
                    count++;
            return count;
        }

        public bool AddEdge(TEdge edge)
        {
            var sourceEdges = this.adjacentEdges[edge.Source];
            if (!this.AllowParallelEdges)
            {
                if (this.ContainsEdgeBetweenVertices(sourceEdges, edge))
                    return false;
            }

            sourceEdges.Add(edge);
            if (!edge.Source.Equals(edge.Target)) // if it's not self-edge
            {
                var targetEdges = this.adjacentEdges[edge.Target];
                targetEdges.Add(edge);
            }
            this.edgeCount++;
            //this.OnEdgeAdded(edge);

            return true;
        }

        //public int AddEdgeRange(IEnumerable<TEdge> edges)
        //{
        //    int count = 0;
        //    foreach (var edge in edges)
        //        if (this.AddEdge(edge))
        //            count++;
        //    return count;
        //}

        //public event EdgeAction<TVertex, TEdge> EdgeAdded;
        //protected virtual void OnEdgeAdded(TEdge args)
        //{
        //    this.EdgeAdded?.Invoke(args);
        //}

        //public bool RemoveEdge(TEdge edge)
        //{
        //    bool removed = this.adjacentEdges[edge.Source].Remove(edge);
        //    if (removed)
        //    {
        //        if (!EdgeExtensions.IsSelfEdge<TVertex, TEdge>(edge))
        //            this.adjacentEdges[edge.Target].Remove(edge);
        //        this.edgeCount--;
        //        Contract.Assert(this.edgeCount >= 0);
        //        this.OnEdgeRemoved(edge);
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        //public event EdgeAction<TVertex, TEdge> EdgeRemoved;
        //protected virtual void OnEdgeRemoved(TEdge args)
        //{
        //    this.EdgeRemoved?.Invoke(args);
        //}

        //public int RemoveEdgeIf(EdgePredicate<TVertex, TEdge> predicate)
        //{
        //    List<TEdge> edges = new List<TEdge>();
        //    foreach (var edge in this.Edges)
        //    {
        //        if (predicate(edge))
        //            edges.Add(edge);
        //    }
        //    return this.RemoveEdges(edges);
        //}

        //public int RemoveEdges(IEnumerable<TEdge> edges)
        //{
        //    int count = 0;
        //    foreach (var edge in edges)
        //    {
        //        if (RemoveEdge(edge))
        //            count++;
        //    }
        //    return count;
        //}
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

        public IEnumerable<TEdge> Edges
        {
            get
            {
                var edgeColors = new Dictionary<TEdge, GraphColor>(this.EdgeCount);
                foreach (var edges in this.adjacentEdges.Values)
                {
                    foreach (TEdge edge in edges)
                    {
                        GraphColor c;
                        if (edgeColors.TryGetValue(edge, out c))
                            continue;
                        edgeColors.Add(edge, GraphColor.Black);
                        yield return edge;
                    }
                }
            }
        }

        public bool ContainsEdge(TEdge edge)
        {
            //var eqc = this.EdgeEqualityComparer;
            foreach (var e in this.AdjacentEdges(edge.Source))
                if (e.Equals(edge))
                    return true;
            return false;
        }

        private bool ContainsEdgeBetweenVertices(IEnumerable<TEdge> edges, TEdge edge)
        {
            if (edges == null) throw new ArgumentNullException("edges");
            if (edge == null) throw new ArgumentNullException("edge");

            var source = edge.Source;
            var target = edge.Target;
            foreach (var e in edges)
                if (this.EdgeEqualityComparer(e, source, target))
                    return true;
            return false;
        }
        #endregion

        #region IUndirectedGraph<Vertex,Edge> Members
        public IList<TEdge> AdjacentEdges(TVertex v)
        {
            IEdgeList<TVertex, TEdge> edges;
            if (this.adjacentEdges.TryGetValue(v, out edges))
            {
                return edges;
            }
            return new TEdge[0];
        }

        public int AdjacentDegree(TVertex v)
        {
            IEdgeList<TVertex, TEdge> edges;
            if (this.adjacentEdges.TryGetValue(v, out edges))
            {
                return edges.Count;
            }
            return 0;
        }

        public bool IsAdjacentEdgesEmpty(TVertex v)
        {
            IEdgeList<TVertex, TEdge> edges;
            if (this.adjacentEdges.TryGetValue(v, out edges))
            {
                return edges.Count == 0;
            }
            return true;
        }
        #endregion
    }
}

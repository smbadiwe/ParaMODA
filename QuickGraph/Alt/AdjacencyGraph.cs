using QuickGraph.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace QuickGraph
{
    /// <summary>
    /// A mutable directed graph data structure efficient for sparse
    /// graph representation where out-edge need to be enumerated only.
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    /// <typeparam name="TEdge">type of the edges</typeparam>
    [DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
    public class AdjacencyGraph<TVertex>
    {
        private readonly bool isDirected = true;
        private readonly bool allowParallelEdges;
        private readonly VertexEdgeDictionary<TVertex> vertexEdges;
        private int edgeCount = 0;
        private int edgeCapacity = -1;

        public AdjacencyGraph()
            :this(true)
        {}

        public AdjacencyGraph(bool allowParallelEdges)
            :this(allowParallelEdges,-1)
        {
        }

        public AdjacencyGraph(bool allowParallelEdges, int vertexCapacity)
            :this(allowParallelEdges, vertexCapacity, -1)
        {
        }

        public AdjacencyGraph(bool allowParallelEdges, int vertexCapacity, int edgeCapacity)
            :this(allowParallelEdges, vertexCapacity, edgeCapacity, EqualityComparer<TVertex>.Default)
        {
        }

        public AdjacencyGraph(bool allowParallelEdges, int vertexCapacity, int edgeCapacity, IEqualityComparer<TVertex> vertexComparer)
        {
            if(vertexComparer == null) throw new ArgumentNullException("vertexComparer");

            this.allowParallelEdges = allowParallelEdges;
            if (vertexCapacity > -1)
                this.vertexEdges = new VertexEdgeDictionary<TVertex>(vertexCapacity, vertexComparer);
            else
                this.vertexEdges = new VertexEdgeDictionary<TVertex>(vertexComparer);
            this.edgeCapacity = edgeCapacity;
        }

        public AdjacencyGraph(
            bool allowParallelEdges, 
            int capacity, 
            int edgeCapacity,
            Func<int, VertexEdgeDictionary<TVertex>> vertexEdgesDictionaryFactory)
        {
            if (vertexEdgesDictionaryFactory == null) throw new ArgumentNullException("vertexComparer");
            this.allowParallelEdges = allowParallelEdges;
            this.vertexEdges = vertexEdgesDictionaryFactory(capacity);
            this.edgeCapacity = edgeCapacity;
        }

        public bool IsDirected
        {
            get { return this.isDirected; }
        }

        public bool AllowParallelEdges
        {
            
            get { return this.allowParallelEdges; }
        }

        public int EdgeCapacity
        {
            get { return this.edgeCapacity; }
            set { this.edgeCapacity = value; }
        }

        public static Type EdgeType
        {
            get { return typeof(Edge<TVertex>); }
        }

        public bool IsVerticesEmpty
        {
            get { return this.vertexEdges.Count == 0; }
        }

        public int VertexCount
        {
            get { return this.vertexEdges.Count; }
        }

        public virtual IEnumerable<TVertex> Vertices
        {
            get { return this.vertexEdges.Keys; }
        }

        
        public bool ContainsVertex(TVertex v)
        {
            return this.vertexEdges.ContainsKey(v);
        }

        public bool IsOutEdgesEmpty(TVertex v)
        {
            return this.vertexEdges[v].Count == 0;
        }

        public int OutDegree(TVertex v)
        {
            return this.vertexEdges[v].Count;
        }

        public virtual IEnumerable<Edge<TVertex>> OutEdges(TVertex v)
        {
            return this.vertexEdges[v];
        }

        public virtual bool TryGetOutEdges(TVertex v, out IEnumerable<Edge<TVertex>> edges)
        {
            IEdgeList<TVertex> list;
            if (this.vertexEdges.TryGetValue(v, out list))
            {
                edges = list;
                return true;
            }

            edges = null;
            return false;
        }

        public Edge<TVertex> OutEdge(TVertex v, int index)
        {
            return this.vertexEdges[v][index];
        }

        /// <summary>
        /// Gets a value indicating whether this instance is edges empty.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is edges empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEdgesEmpty
        {
            
            get { return this.edgeCount == 0; }
        }

        /// <summary>
        /// Gets the edge count.
        /// </summary>
        /// <value>The edge count.</value>
        public int EdgeCount
        {
            get 
            {
                return this.edgeCount; 
            }
        }

        [ContractInvariantMethod]
        void ObjectInvariant()
        {
            Contract.Invariant(this.edgeCount >= 0);
        }

        /// <summary>
        /// Gets the edges.
        /// </summary>
        /// <value>The edges.</value>
        public virtual IEnumerable<Edge<TVertex>> Edges
        {
            
            get
            {
                foreach (var edges in this.vertexEdges.Values)
                    foreach (var edge in edges)
                        yield return edge;
            }
        }

        
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            IEnumerable<Edge<TVertex>> outEdges;
            if (!this.TryGetOutEdges(source, out outEdges))
                return false;
            foreach (var outEdge in outEdges)
                if (outEdge.Target.Equals(target))
                    return true;
            return false;
        }

        
        public bool ContainsEdge(Edge<TVertex> edge)
        {
            IEdgeList<TVertex> edges;
            return 
                this.vertexEdges.TryGetValue(edge.Source, out edges) &&
                edges.Contains(edge);
        }

        
        public bool TryGetEdge(
            TVertex source,
            TVertex target,
            out Edge<TVertex> edge)
        {
            IEdgeList<TVertex> edgeList;
            if (this.vertexEdges.TryGetValue(source, out edgeList) &&
                edgeList.Count > 0)
            {
                foreach (var e in edgeList)
                {
                    if (e.Target.Equals(target))
                    {
                        edge = e;
                        return true;
                    }
                }
            }
            edge = default(Edge<TVertex>);
            return false;
        }

        
        public virtual bool TryGetEdges(
            TVertex source,
            TVertex target,
            out IEnumerable<Edge<TVertex>> edges)
        {
            IEdgeList<TVertex> outEdges;
            if (this.vertexEdges.TryGetValue(source, out outEdges))
            {
                List<Edge<TVertex>> list = new List<Edge<TVertex>>(outEdges.Count);
                foreach (var edge in outEdges)
                    if (edge.Target.Equals(target))
                        list.Add(edge);

                edges = list;
                return true;
            }
            else
            {
                edges = null;
                return false;
            }
        }

        public virtual bool AddVertex(TVertex v)
        {
            if (this.ContainsVertex(v))
                return false;

            if (this.EdgeCapacity>0)
                this.vertexEdges.Add(v, new EdgeList<TVertex>(this.EdgeCapacity));
            else
                this.vertexEdges.Add(v, new EdgeList<TVertex>());
            this.OnVertexAdded(v);
            return true;
        }

        public virtual int AddVertexRange(IEnumerable<TVertex> vertices)
        {
            int count = 0;
            foreach (var v in vertices)
                if (this.AddVertex(v))
                    count++;
            return count;
        }

        public event VertexAction<TVertex> VertexAdded;
        protected virtual void OnVertexAdded(TVertex args)
        {
            this.VertexAdded?.Invoke(args);
        }

        public virtual bool RemoveVertex(TVertex v)
        {
            if (!this.ContainsVertex(v))
                return false;
            // remove outedges
            {
                var edges = this.vertexEdges[v];
                if (this.EdgeRemoved != null) // lazily notify
                {
                    foreach (var edge in edges)
                        this.OnEdgeRemoved(edge);
                }
                this.edgeCount -= edges.Count;
                edges.Clear();
            }

            // iterage over edges and remove each edge touching the vertex
            var edgeToRemove = new EdgeList<TVertex>();
            foreach (var kv in this.vertexEdges)
            {
                if (kv.Key.Equals(v)) continue; // we've already 
                // collect edge to remove
                foreach(var edge in kv.Value)
                {
                    if (edge.Target.Equals(v))
                        edgeToRemove.Add(edge);
                }

                // remove edges
                foreach (var edge in edgeToRemove)
                {
                    kv.Value.Remove(edge);
                    this.OnEdgeRemoved(edge);
                }
                // update count
                this.edgeCount -= edgeToRemove.Count;
                edgeToRemove.Clear();
            }

            Contract.Assert(this.edgeCount >= 0);
            this.vertexEdges.Remove(v);
            this.OnVertexRemoved(v);

            return true;
        }

        public event VertexAction<TVertex> VertexRemoved;
        protected virtual void OnVertexRemoved(TVertex args)
        {
            this.VertexRemoved?.Invoke(args);
        }

        public int RemoveVertexIf(VertexPredicate<TVertex> predicate)
        {
            var vertices = new VertexList<TVertex>();
            foreach (var v in this.Vertices)
                if (predicate(v))
                    vertices.Add(v);

            foreach (var v in vertices)
                this.RemoveVertex(v);

            return vertices.Count;
        }

        public virtual bool AddVerticesAndEdge(Edge<TVertex> e)
        {
            this.AddVertex(e.Source);
            this.AddVertex(e.Target);
            return this.AddEdge(e);
        }

        /// <summary>
        /// Adds a range of edges to the graph
        /// </summary>
        /// <param name="edges"></param>
        /// <returns>the count edges that were added</returns>
        public int AddVerticesAndEdgeRange(IEnumerable<Edge<TVertex>> edges)
        {
            int count = 0;
            foreach (var edge in edges)
                if (this.AddVerticesAndEdge(edge))
                    count++;
            return count;
        }

        /// <summary>
        /// Adds the edge to the graph
        /// </summary>
        /// <param name="e">the edge to add</param>
        /// <returns>true if the edge was added; false if it was already part of the graph</returns>
        public virtual bool AddEdge(Edge<TVertex> e)
        {
            if (!this.AllowParallelEdges)
            {
                if (this.ContainsEdge(e.Source, e.Target))
                    return false;
            }
            this.vertexEdges[e.Source].Add(e);
            this.edgeCount++;

            this.OnEdgeAdded(e);

            return true;
        }

        public int AddEdgeRange(IEnumerable<Edge<TVertex>> edges)
        {
            int count = 0;
            foreach (var edge in edges)
                if (this.AddEdge(edge))
                    count++;
            return count;
        }

        public event EdgeAction<TVertex> EdgeAdded;
        protected virtual void OnEdgeAdded(Edge<TVertex> args)
        {
            this.EdgeAdded?.Invoke(args);
        }

        public virtual bool RemoveEdge(Edge<TVertex> e)
        {
            IEdgeList<TVertex> edges;
            if (this.vertexEdges.TryGetValue(e.Source, out edges) &&
                edges.Remove(e))
            {
                this.edgeCount--;
                Contract.Assert(this.edgeCount >= 0);
                this.OnEdgeRemoved(e);
                return true;
            }
            else
                return false;
        }

        public event EdgeAction<TVertex> EdgeRemoved;
        protected virtual void OnEdgeRemoved(Edge<TVertex> args)
        {
            this.EdgeRemoved?.Invoke(args);
        }

        public int RemoveEdgeIf(EdgePredicate<TVertex> predicate)
        {
            var edges = new EdgeList<TVertex>();
            foreach (var edge in this.Edges)
                if (predicate(edge))
                    edges.Add(edge);

            foreach (var edge in edges)
                this.RemoveEdge(edge);

            return edges.Count;
        }

        public void ClearOutEdges(TVertex v)
        {
            var edges = this.vertexEdges[v];
            int count = edges.Count;
            if (this.EdgeRemoved != null) // call only if someone is listening
            {
                foreach (var edge in edges)
                    this.OnEdgeRemoved(edge);
            }
            edges.Clear();
            this.edgeCount -= count;
        }

        public int RemoveOutEdgeIf(TVertex v, EdgePredicate<TVertex> predicate)
        {
            var edges = this.vertexEdges[v];
            var edgeToRemove = new EdgeList<TVertex>(edges.Count);
            foreach (var edge in edges)
                if (predicate(edge))
                    edgeToRemove.Add(edge);

            foreach (var edge in edgeToRemove)
            {
                edges.Remove(edge);
                this.OnEdgeRemoved(edge);
            }
            this.edgeCount -= edgeToRemove.Count;

            return edgeToRemove.Count;
        }

        public void TrimEdgeExcess()
        {
            foreach (var edges in this.vertexEdges.Values)
                edges.TrimExcess();
        }

        public void Clear()
        {
            this.vertexEdges.Clear();
            this.edgeCount = 0;
            this.OnCleared(EventArgs.Empty);
        }

        public event EventHandler Cleared;
        private void OnCleared(EventArgs e)
        {
            this.Cleared?.Invoke(this, e);
        }

        #region ICloneable Members
        //private AdjacencyGraph(
        //    VertexEdgeDictionary<TVertex> vertexEdges,
        //    int edgeCount,
        //    int edgeCapacity,
        //    bool allowParallelEdges
        //    )
        //{
        //    Contract.Requires(vertexEdges != null);
        //    Contract.Requires(edgeCount >= 0);

        //    this.vertexEdges = vertexEdges;
        //    this.edgeCount = edgeCount;
        //    this.edgeCapacity = edgeCapacity;
        //    this.allowParallelEdges = allowParallelEdges;
        //}

        
        //public AdjacencyGraph<TVertex> Clone()
        //{
        //    return new AdjacencyGraph<TVertex>(
        //        this.vertexEdges.Clone(),
        //        this.edgeCount,
        //        this.edgeCapacity,
        //        this.allowParallelEdges
        //        );
        //}
        
        #endregion
    }
}

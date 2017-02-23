using QuickGraph.Algorithms.Services;
using QuickGraph.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms.Search
{
    /// <summary>
    /// A breath first search algorithm for directed graphs
    /// </summary>
    /// <reference-ref
    ///     idref="gross98graphtheory"
    ///     chapter="4.2"
    ///     />
    public sealed class BreadthFirstSearchAlgorithm<TVertex> 
        : RootedAlgorithmBase<TVertex, AdjacencyGraph<TVertex>>
        , IDistanceRecorderAlgorithm<TVertex>
        , IVertexColorizerAlgorithm<TVertex>
        , ITreeBuilderAlgorithm<TVertex>
    {
        private IDictionary<TVertex, GraphColor> vertexColors;
        private IQueue<TVertex> vertexQueue;
        private readonly Func<IEnumerable<Edge<TVertex>>, IEnumerable<Edge<TVertex>>> outEdgeEnumerator;

        public BreadthFirstSearchAlgorithm(AdjacencyGraph<TVertex> g)
            : this(g, new QuickGraph.Collections.Queue<TVertex>(), new Dictionary<TVertex, GraphColor>())
        {}

        public BreadthFirstSearchAlgorithm(
            AdjacencyGraph<TVertex> visitedGraph,
            IQueue<TVertex> vertexQueue,
            IDictionary<TVertex, GraphColor> vertexColors
            )
            : this(null, visitedGraph, vertexQueue, vertexColors)
        { }

        public BreadthFirstSearchAlgorithm(
            IAlgorithmComponent host,
            AdjacencyGraph<TVertex> visitedGraph,
            IQueue<TVertex> vertexQueue,
            IDictionary<TVertex, GraphColor> vertexColors
            )
            :this(host, visitedGraph, vertexQueue, vertexColors, e => e)
        {}

        public BreadthFirstSearchAlgorithm(
            IAlgorithmComponent host,
            AdjacencyGraph<TVertex> visitedGraph,
            IQueue<TVertex> vertexQueue,
            IDictionary<TVertex, GraphColor> vertexColors,
            Func<IEnumerable<Edge<TVertex>>, IEnumerable<Edge<TVertex>>> outEdgeEnumerator
            )
            : base(host, visitedGraph)
        {
            Contract.Requires(vertexQueue != null);
            Contract.Requires(vertexColors != null);
            Contract.Requires(outEdgeEnumerator != null);

            this.vertexColors = vertexColors;
            this.vertexQueue = vertexQueue;
            this.outEdgeEnumerator = outEdgeEnumerator;
        }

        public Func<IEnumerable<Edge<TVertex>>, IEnumerable<Edge<TVertex>>> OutEdgeEnumerator
        {
            get { return this.outEdgeEnumerator; }
        }

        public IDictionary<TVertex,GraphColor> VertexColors
        {
            get
            {
                return vertexColors;
            }
        }

        public GraphColor GetVertexColor(TVertex vertex)
        {
            return this.vertexColors[vertex];
        }

        public event VertexAction<TVertex> InitializeVertex;
        private void OnInitializeVertex(TVertex v)
        {
            var eh = this.InitializeVertex;
            if (eh != null)
                eh(v);
        }

        public event VertexAction<TVertex> StartVertex;
        private void OnStartVertex(TVertex v)
        {
            var eh = this.StartVertex;
            if (eh != null)
                eh(v);
        }

        public event VertexAction<TVertex> ExamineVertex;
        private void OnExamineVertex(TVertex v)
        {
            var eh = this.ExamineVertex;
            if (eh != null)
                eh(v);
        }

        public event VertexAction<TVertex> DiscoverVertex;
        private void OnDiscoverVertex(TVertex v)
        {
            var eh = this.DiscoverVertex;
            if (eh != null)
                eh(v);
        }

        public event EdgeAction<TVertex> ExamineEdge;
        private void OnExamineEdge(Edge<TVertex> e)
        {
            var eh = this.ExamineEdge;
            if (eh != null)
                eh(e);
        }

        public event EdgeAction<TVertex> TreeEdge;
        private void OnTreeEdge(Edge<TVertex> e)
        {
            var eh = this.TreeEdge;
            if (eh != null)
                eh(e);
        }

        public event EdgeAction<TVertex> NonTreeEdge;
        private void OnNonTreeEdge(Edge<TVertex> e)
        {
            var eh = this.NonTreeEdge;
            if (eh != null)
                eh(e);
        }

        public event EdgeAction<TVertex> GrayTarget;
        private void OnGrayTarget(Edge<TVertex> e)
        {
            var eh = this.GrayTarget;
            if (eh != null)
                eh(e);
        }

        public event EdgeAction<TVertex> BlackTarget;
        private void OnBlackTarget(Edge<TVertex> e)
        {
            var eh = this.BlackTarget;
            if (eh != null)
                eh(e);
        }

        public event VertexAction<TVertex> FinishVertex;
        private void OnFinishVertex(TVertex v)
        {
            var eh = this.FinishVertex;
            if (eh != null)
                eh(v);
        }

        protected override void Initialize()
        {
            base.Initialize();

            var cancelManager = this.Services.CancelManager;
            if (cancelManager.IsCancelling)
                return;
            // initialize vertex u
            foreach (var v in VisitedGraph.Vertices)
            {
                this.VertexColors[v] = GraphColor.White;
                OnInitializeVertex(v);
            }
        }

        protected override void InternalCompute()
        {
            if (this.VisitedGraph.VertexCount == 0)
                return;

            TVertex rootVertex;
            if (!this.TryGetRootVertex(out rootVertex))
            {
                // enqueue roots
                foreach (var root in AlgoExt.Roots(this.VisitedGraph))
                    this.EnqueueRoot(root);
            }
            else // enqueue select root only
            {
                this.EnqueueRoot(rootVertex);
            }
            this.FlushVisitQueue();
        }

        public void Visit(TVertex s)
        {
            this.EnqueueRoot(s);
            this.FlushVisitQueue();
        }

        private void EnqueueRoot(TVertex s)
        {
            this.OnStartVertex(s);

            this.VertexColors[s] = GraphColor.Gray;

            OnDiscoverVertex(s);
            this.vertexQueue.Enqueue(s);
        }

        private void FlushVisitQueue()
        {
            var cancelManager = this.Services.CancelManager;
            var oee = this.OutEdgeEnumerator;

            while (this.vertexQueue.Count > 0)
            {
                if (cancelManager.IsCancelling) return;

                var u = this.vertexQueue.Dequeue();
                this.OnExamineVertex(u);
                foreach (var e in oee(this.VisitedGraph.OutEdges(u)))
                {
                    TVertex v = e.Target;
                    this.OnExamineEdge(e);

                    var vColor = this.VertexColors[v];
                    if (vColor == GraphColor.White)
                    {
                        this.OnTreeEdge(e);
                        this.VertexColors[v] = GraphColor.Gray;
                        this.OnDiscoverVertex(v);
                        this.vertexQueue.Enqueue(v);
                    }
                    else
                    {
                        this.OnNonTreeEdge(e);
                        if (vColor == GraphColor.Gray)
                            this.OnGrayTarget(e);
                        else
                            this.OnBlackTarget(e);
                    }
                }
                this.VertexColors[u] = GraphColor.Black;
                this.OnFinishVertex(u);
            }
        }
    }
    
}

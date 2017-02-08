using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl
{
    /// <summary>
    /// Implemented using a modified Adjacency matrix version. DO NOT USE
    /// </summary>
    public class Graph
    {
        private bool[] adjMatrix;
        private int currentVertexCount;
        public int MaxSize { get; private set; }
        public string[] Vertices { get; private set; }
        public int MaxEdgeCount { get; private set; }
        public int VertexCount { get { return currentVertexCount; } }
        public int EdgeCount { get { return adjMatrix.Count(x => x == true); } }
        public Graph(int size)
        {
            MaxSize = size;
            MaxEdgeCount = size * (size - 1) / 2;
            adjMatrix = new bool[MaxEdgeCount];
            Vertices = new string[size];
        }

        public bool TryGetEdge(string source, string target, out Edge<string> edge)
        {
            int adjMatrixIndex = 0;
            for (int i = 0; i < MaxSize - 1; i++)
            {
                for (int j = i + 1; j < MaxSize; j++)
                {
                    if ((Vertices[i] == source && Vertices[j] == target)
                        || (Vertices[i] == target && Vertices[j] == source))
                    {
                        if (adjMatrix[adjMatrixIndex] == true)
                        {
                            edge = new Edge<string>(source, target);
                            return true;
                        }
                        else
                        {
                            edge = null;
                            return false;
                        }
                    }
                    adjMatrixIndex++;
                }
            }
            edge = null;
            return false;
        }

        public IList<Edge<string>> GetEdges()
        {
            int adjMatrixIndex = 0;
            var toReturn = new List<Edge<string>>(currentVertexCount - 1);
            for (int i = 0; i < MaxSize - 1; i++)
            {
                for (int j = i + 1; j < MaxSize; j++)
                {
                    if (adjMatrix[adjMatrixIndex] == true)
                    {
                        toReturn.Add(new Edge<string>(Vertices[i], Vertices[j]));
                    }

                    adjMatrixIndex++;
                }
            }
            return toReturn;
        }

        public bool AddNode(string node)
        {
            if (currentVertexCount < MaxSize)
            {
                if (!Vertices.Contains(node))
                {
                    Vertices[currentVertexCount] = node;
                    currentVertexCount++;
                    return true;
                }
            }
            return false;
        }

        public bool ContainsVertex(string node)
        {
            return Vertices.Contains(node);
        }

        public void AddVerticesAndEdgeRange(IEnumerable<Edge<string>> edges)
        {
            foreach (var edge in edges)
            {
                AddEdge(edge.Source, edge.Target);
            }
        }

        public IList<string> GetNeighbors(string node)
        {
            var indexOfNode = Array.IndexOf(Vertices, node);
            if (indexOfNode > -1)
            {
                int adjMatrixIndex = 0;
                var toReturn = new List<string>(currentVertexCount - 1);
                for (int i = 0; i < MaxSize - 1; i++)
                {
                    for (int j = i + 1; j < MaxSize; j++)
                    {
                        if (i == indexOfNode)
                        {
                            if (adjMatrix[adjMatrixIndex] == true)
                            {
                                toReturn.Add(Vertices[j]);
                            }
                        }
                        else if (j == indexOfNode)
                        {
                            if (adjMatrix[adjMatrixIndex] == true)
                            {
                                toReturn.Add(Vertices[i]);
                            }
                        }
                        adjMatrixIndex++;
                    }
                }
                return toReturn;
            }
            return new string[0];
        }

        public int AdjacentDegree(string node)
        {
            int indexOfNode = Array.IndexOf(Vertices, node);
            if (indexOfNode > -1)
            {
                int toReturn = 0;
                int adjMatrixIndex = 0;
                for (int i = 0; i < currentVertexCount - 1; i++)
                {
                    for (int j = i + 1; j < currentVertexCount; j++)
                    {
                        if (i == indexOfNode || j == indexOfNode)
                        {
                            if (adjMatrix[adjMatrixIndex] == true)
                            {
                                toReturn++;
                            }
                        }
                        adjMatrixIndex++;
                    }
                }
                return toReturn;
            }
            return 0;
        }

        public bool AddVerticesAndEdge(Edge<string> edge)
        {
            return AddEdge(edge.Source, edge.Target);
        }

        public bool AddEdge(string source, string target)
        {
            AddNode(source);
            AddNode(target);

            int adjMatrixIndex = 0;
            for (int i = 0; i < MaxSize - 1; i++)
            {
                for (int j = i + 1; j < MaxSize; j++)
                {
                    if ((Vertices[i] == source && Vertices[j] == target)
                        || (Vertices[i] == target && Vertices[j] == source))
                    {
                        adjMatrix[adjMatrixIndex] = true;
                        return true;
                    }
                    adjMatrixIndex++;
                }
            }
            return false;
        }

        public bool ContainsEdge(string source, string target)
        {
            Edge<string> edge;
            return TryGetEdge(source, target, out edge);
        }

        public Graph Clone()
        {
            var newG = new Graph(MaxSize);
            Array.Copy(this.Vertices, newG.Vertices, MaxSize);
            Array.Copy(this.adjMatrix, newG.adjMatrix, newG.MaxEdgeCount);
            return newG;
        }

        public bool RemoveVertex(string node)
        {
            //Removing an edge is tantamount to setting the corresponding element in the adjeacency matrix to false
            int indexOfNode = Array.IndexOf(Vertices, node);
            if (indexOfNode > -1)
            {
                int adjMatrixIndex = 0, vertexCount = Vertices.Length;
                int newAdjIndex = 0, newMaxEdgeCount = (MaxSize - 1) * (MaxSize - 2) / 2;
                var newAdjMatrix = new bool[newMaxEdgeCount]; //Remember we'll be removing one node

                for (int i = 0; i < MaxSize - 1; i++)
                {
                    if (vertexCount <= i) break;
                    for (int j = i + 1; j < MaxSize; j++)
                    {
                        if (vertexCount <= j) break;
                        if (i == indexOfNode || j == indexOfNode)
                        {
                            adjMatrixIndex++;
                            continue;
                        }

                        newAdjMatrix[newAdjIndex] = adjMatrix[adjMatrixIndex];
                        newAdjIndex++;

                        adjMatrixIndex++;
                    }
                }
                RemoveFromVerticesAt(indexOfNode);
                
                //Update values for the graph
                this.adjMatrix = newAdjMatrix;
                this.MaxSize--;
                this.MaxEdgeCount = newMaxEdgeCount;
                return true;
            }

            return false;
        }
        
        private void RemoveFromVerticesAt(int index)
        {
            if (index >= Vertices.Length) return;

            string[] newVerticesSet = new string[Vertices.Length - 1];
            int i = 0;
            int j = 0;
            while (i < Vertices.Length)
            {
                if (i != index)
                {
                    newVerticesSet[j] = Vertices[i];
                    j++;
                }

                i++;
            }
            Vertices = newVerticesSet;
            currentVertexCount--;
        }

        /// <summary>
        /// NB: The degree sequence of an undirected graph is the non-increasing sequence of its vertex degrees;
        /// </summary>
        /// <param name="count">The expected number of items to return. This value is usually less than the <see cref="VertexCount"/></param>
        /// <returns></returns>
        public IList<string> GetDegreeSequence(int count)
        {
            if (Vertices.Length == 0) return new string[0];

            var tempList = new Dictionary<string, int>(count);
            int iter = 1;
            foreach (var node in Vertices)
            {
                tempList.Add(node, this.AdjacentDegree(node));
                iter++;
                if (iter == count) break;
            }

            var listToReturn = new List<string>(count);
            foreach (var item in tempList.OrderByDescending(x => x.Value))
            {
                listToReturn.Add(item.Key);
            }

            tempList = null;
            return listToReturn;
        }
    }
}

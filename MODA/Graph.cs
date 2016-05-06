using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA
{
    /// <summary>
    /// The network / graph
    /// </summary>
    public class Graph
    {
        public List<bool> AdjacentMatrix { get; set; }
        public int NumberOfNodes { get; set; }
        public bool IsDirected { get; private set; }

        public Graph() : this(false)
        {

        }
        public Graph(bool isDirected)
        {
            IsDirected = IsDirected;
            AdjacentMatrix = new List<bool>();
        }

        #region methods
        public void PrepareAdjacentMatrix()
        {
            long n = (NumberOfNodes * (NumberOfNodes - 1)) / 2;
            for (long i = 0; i < n; i++)
            {
                AdjacentMatrix.Add(false);
            }
        }

        public List<int> FindNeighbors(int node)
        {
            List<int> result = new List<int>();
            int index = (node * (2 * NumberOfNodes - 1 - node) / 2);
            int i = 0;
            for (i = index; i < index + NumberOfNodes - 1 - node; i++)
            {
                if (AdjacentMatrix[i])
                {
                    result.Add(node + i - index + 1);
                }
            }
            for (i = 0; i < node; i++)
            {
                if (IsLinkBetween(node, i))
                {
                    result.Add(i);
                }
            }
            return result;
        }

        public bool IsLinkBetween(int node1, int node2)
        {
            int tmp_n1 = 0;
            int tmp_n2 = 0;
            if (node1 == node2) return false;

            if (node1 < node2)
            {
                tmp_n1 = node1;
                tmp_n2 = node2;
            }
            else
            {
                tmp_n1 = node2;
                tmp_n2 = node1;
            }


            int index = (tmp_n1 * (2 * NumberOfNodes - 1 - tmp_n1) / 2) +
                 (tmp_n2 - tmp_n1 - 1);
            return (AdjacentMatrix[index]);

        }

        public List<int> FindNeighborsWithFlag(int node, bool[] flag)
        {
            List<int> result = new List<int>();
            int index = (node * (2 * NumberOfNodes - 1 - node) / 2);
            int i = 0;
            for (i = index; i < index + NumberOfNodes - 1 - node; i++)
            {
                if (AdjacentMatrix[i] && !flag[node])
                {
                    result.Add(node + i - index + 1);
                }
            }
            for (i = 0; i < node; i++)
            {
                if (IsLinkBetween(node, i) && !flag[node])
                {
                    result.Add(i);
                }
            }
            return result;
        }

        public List<int> FindNeighborsGreaterThan(int node, int treshhold)
        {
            List<int> result = new List<int>();
            int index = (node * (2 * NumberOfNodes - 1 - node) / 2);
            int i = 0;
            int temp = 0;
            for (i = index; i < index + NumberOfNodes - 1 - node; i++)
            {
                temp = node + i - index + 1;
                if (AdjacentMatrix[i] && temp > treshhold)
                {
                    result.Add(node + i - index + 1);
                }
            }

            for (i = 0; i < node; i++)
            {
                if (IsLinkBetween(node, i) && i > treshhold)
                {
                    result.Add(i);
                }
            }
            return result;
        }

        public int DegreeOfNode(int node)
        {
            List<int> result = FindNeighbors(node);
            return result.Count;
        }

        public void RemoveEdge(int node)
        {
            int i = 0;
            for (i = 0; i < node; i++)
            {
                DeleteSingleEdge(i, node);
            }
            for (i = node + 1; i < NumberOfNodes; i++)
            {
                DeleteSingleEdge( node, i);
            }
        }

        private void DeleteSingleEdge(int node1, int node2)
        {
            int tmp_n1 = node1;
            int tmp_n2 = node2;
            int index = (tmp_n1 * (2 * NumberOfNodes - 1 - tmp_n1) / 2) +
                 (tmp_n2 - tmp_n1 - 1);
            AdjacentMatrix[index] = false;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA
{
    public class NewGraph
    {
        private List<Vertex> V;
        private List<Vertex> vSorted;
        private int _numberOfVertex;
        protected List<bool> _adjMatrix;
        protected List<bool> _copyAdjmatrix;

        public NewGraph(List<bool> adjMatrix)
        {
            _adjMatrix = adjMatrix;
            _numberOfVertex = GetNumberOfVertex(adjMatrix);
            V = new List<Vertex>();
            List<int> local_neighbors = new List<int>();
            Vertex local_vertex;
            List<int> local_neighbor_degree;

            for (int i = 0; i < _numberOfVertex; i++)
            {
                local_neighbors = FindNeighbors(i);
                local_vertex = new Vertex(i, local_neighbors);
                V.Add(local_vertex);

                //

                local_neighbor_degree = new List<int>();
                for (int k = 0; k < V[i].GetDegree(); k++)
                {
                    local_neighbor_degree.Add(V[V[i].getNeighbors()[k]].GetDegree());
                }
                V[i].SetNeighborsDegree(local_neighbor_degree);
            }

            vSorted = new List<Vertex>();
            _copyAdjmatrix = new List<bool>();

            //Copy adj_matrix 
            //for testing if section
            for (int i = 0; i < _adjMatrix.Count; i++)
            {
                _copyAdjmatrix.Add(_adjMatrix[i]);
            }

            vSorted = GetSortedByDegree(V);
        }

        private List<Vertex> GetSortedByDegree(List<Vertex> v)
        {
            List<Vertex> temp = new List<Vertex>();
            Vertex temp_vertex = new Vertex();

            for (int i = 0; i < v.Count; i++)
            {
                temp.Add(v[i]);
            }
            //sorted by buble sort method
            for (int i = 0; i < temp.Count; i++)
            {
                for (int j = i + 1; j < temp.Count; j++)
                {
                    if (temp[i].GetDegree() < temp[j].GetDegree())
                    {
                        temp_vertex = temp[i];
                        temp[i] = temp[j];
                        temp[j] = temp_vertex;
                    }
                }
            }
            return temp;

        }

        public void RemoveEdge(int node)
        {
            int i = 0;
            for (i = 0; i < node; i++)
            {
                DeleteSingleEdge(i, node);
            }
            for (i = node + 1; i < _numberOfVertex; i++)
            {
                DeleteSingleEdge(node, i);
            }
        }

        private void DeleteSingleEdge(int node1, int node2)
        {
            int tmp_n1 = node1;
            int tmp_n2 = node2;
            int index = (tmp_n1 * (2 * _numberOfVertex - 1 - tmp_n1) / 2) +
                 (tmp_n2 - tmp_n1 - 1);
            _adjMatrix[index] = false;
        }

        //Sort degrees should be added
        public void RemoveVertex(int vertextName, int vertexDegree)
        {
            List<int> local = new List<int>();
            local = V[vertextName].getNeighbors();
            for (int i = 0; i < local.Count; i++)
            {
                V[local[i]].DeleteNeighbor(vertextName, vertexDegree); //update neighbors of vertexName
                //
                List<int> neighbor_of_local = new List<int>();
                neighbor_of_local = V[local[i]].getNeighbors();

                for (int j = 0; j < neighbor_of_local.Count; j++)
                {
                    V[neighbor_of_local[j]].UpdateNeighborList(V[local[i]].GetDegree());
                }
            }

            V[vertextName].Delete();   //clear neighbors List and neighborsDegree List vertexName
            RemoveEdge(vertextName);  //for updating Adj-Matrix
            //convert false(or 0) any entries in Adj-matrix that 
            //vertexName there is
        }

        public List<int> FindNeighbors(int vertexName)
        {
            List<int> result = new List<int>();
            int number_node = _numberOfVertex;
            int node = vertexName;
            int index = (node * (2 * number_node - 1 - node) / 2);
            int i = 0;
            for (i = index; i < index + number_node - 1 - node; i++)
            {
                if (_adjMatrix[i])
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

        public int GetNumberOfVertex()
        {
            return _numberOfVertex;
        }

        public bool IsLinkBetween(int node1, int node2)
        {
            int tmp_n1 = 0;
            int tmp_n2 = 0;
            if (node1 == node2)
                return false;
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
            int index = (tmp_n1 * (2 * _numberOfVertex - 1 - tmp_n1) / 2) +
                 (tmp_n2 - tmp_n1 - 1);
            return (_adjMatrix[index]);

        }

        //This function works on copyAdj_matrix , 
        //This is for 'ifTesting' section
        //Added in 87.01.24
        public bool IsLinkBetween2(int node1, int node2)
        {
            int tmp_n1 = 0;
            int tmp_n2 = 0;
            if (node1 == node2)
                return false;
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
            int index = (tmp_n1 * (2 * _numberOfVertex - 1 - tmp_n1) / 2) +
                 (tmp_n2 - tmp_n1 - 1);
            return (_copyAdjmatrix[index]);

        }

        public Vertex GetVertex(int index)
        {
            return V[index];
        }

        public Vertex GetSoretedVertex(int index)
        {
            return vSorted[index];
        }

        private int GetNumberOfVertex(List<bool> adjMatrix)
        {
            int length = adjMatrix.Count;
            int sum = 0;
            for (int i = 1; ; i++)
            {
                sum += i;
                if (sum == length)
                {
                    return i + 1;
                }
            }
        }
    }
}

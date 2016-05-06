using System;
using System.Collections.Generic;
using System.Text;

namespace Motif1
{

    class NewGraph
    {
        List<Vertex> V;
        List<Vertex> vSorted;
        protected List<bool> Adj_matrix;
        protected List<bool> copyAdj_matrix;
        int vertex_number;

        public NewGraph(List<bool> Adj_Matrix)
        {
            Adj_matrix = new List<bool>();
            Adj_matrix = Adj_Matrix;
            vertex_number = getVertexNumber(Adj_Matrix);
            V = new List<Vertex>();
            List<int> local_neighbors = new List<int>();
            Vertex local_vertex;
            List<int> local_neighbor_degree;

            for (int i = 0; i < vertex_number; i++)
            {
                local_neighbors = find_neighbors(i);
                local_vertex = new Vertex(i, local_neighbors);
                V.Add(local_vertex);
            }

            for (int i = 0; i < vertex_number; i++)
            {
                local_neighbor_degree = new List<int>();
                for (int k = 0; k < V[i].getDegree(); k++)
                {
                    local_neighbor_degree.Add(V[V[i].getNeighbors()[k]].getDegree());
                }
                V[i].setNeighborsDegree(local_neighbor_degree);
            }


            vSorted = new List<Vertex>();
            copyAdj_matrix = new List<bool>();

            //Copy adj_matrix 
            //for testing if section
            for (int i = 0; i < Adj_matrix.Count; i++)
                copyAdj_matrix.Add(Adj_matrix[i]);
            //
            //

            vSorted = getSortedByDegree(V);
        }

        List<Vertex> getSortedByDegree(List<Vertex> v)
        {
            List<Vertex> temp = new List<Vertex>(v);
            Vertex temp_vertex = new Vertex();

            //for (int i = 0; i < v.Count; i++)
            //    temp.Add(v[i]);

            for (int i = 0; i < temp.Count; i++)    //sorted by buble sort method
            {
                for (int j = i + 1; j < temp.Count; j++)
                {
                    if (temp[i].getDegree() < temp[j].getDegree())
                    {
                        temp_vertex = temp[i];
                        temp[i] = temp[j];
                        temp[j] = temp_vertex;
                    }
                }
            }
            return temp;

        }

        public void remove_edge(int node)
        {
            int i = 0;
            for (i = 0; i < node; i++)
                delete_single_edge(i, node);
            for (i = node + 1; i < vertex_number; i++)
                delete_single_edge(node, i);
        }

        void delete_single_edge(int node1, int node2)
        {

            int tmp_n1 = node1;
            int tmp_n2 = node2;
            int index = (tmp_n1 * (2 * vertex_number - 1 - tmp_n1) / 2) +
                 (tmp_n2 - tmp_n1 - 1);
            Adj_matrix[index] = false;
        }
        //Sort degrees should be added
        public void removeVertex(int vertextName, int vertexDegree)
        {
            List<int> local = new List<int>();
            local = V[vertextName].getNeighbors();
            for (int i = 0; i < local.Count; i++)
            {
                V[local[i]].deleteNeighbor(vertextName, vertexDegree); //update neighbors of vertexName
                //
                List<int> neighbor_of_local = new List<int>();
                neighbor_of_local = V[local[i]].getNeighbors();

                for (int j = 0; j < neighbor_of_local.Count; j++)
                    V[neighbor_of_local[j]].updateNeighborList(V[local[i]].getDegree());
            }

            V[vertextName].delete();   //clear neighbors List and neighborsDegree List vertexName
            remove_edge(vertextName);  //for updating Adj-Matrix
            //convert false(or 0) any entries in Adj-matrix that 
            //vertexName there is
        }

        public List<int> find_neighbors(int vertexName)
        {
            List<int> result = new List<int>();
            int number_node = vertex_number;
            int node = vertexName;
            int index = (node * (2 * number_node - 1 - node) / 2);
            int i = 0;
            for (i = index; i < index + number_node - 1 - node; i++)
            {
                if (i > 0 && Adj_matrix[i])
                    result.Add(node + i - index + 1);
            }
            for (i = 0; i < node; i++)
            {
                if (is_link_between(node, i))
                    result.Add(i);
            }
            return result;
        }

        public int getNumberVertex()
        {
            return vertex_number;
        }

        public bool is_link_between(int node1, int node2)
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
            int index = (tmp_n1 * (2 * vertex_number - 1 - tmp_n1) / 2) +
                 (tmp_n2 - tmp_n1 - 1);
            return (index > 0 && Adj_matrix[index]);

        }

        //This function works on copyAdj_matrix , 
        //This is for 'ifTesting' section
        //Added in 87.01.24
        public bool is_link_between2(int node1, int node2)
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
            int index = (tmp_n1 * (2 * vertex_number - 1 - tmp_n1) / 2) +
                 (tmp_n2 - tmp_n1 - 1);
            return (copyAdj_matrix[index]);

        }
        //
        //

        public Vertex getVertex(int index)
        {
            return V[index];
        }
        public Vertex getSoretedVertex(int index)
        {
            return vSorted[index];
        }

        int getVertexNumber(List<bool> Adj_Matrix)
        {
            int length = Adj_Matrix.Count;
            int sum = 0;
            for (int i = 1; ; i++)
            {
                sum += i;
                if (sum == length)
                    return i + 1;
            }
            return -1;
        }



    }
}

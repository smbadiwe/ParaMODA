using System;
using System.Collections.Generic;
using System.Text;

namespace Motif1
{
    class graph
    {
        public int number_node;
        bool directed;
        public List<bool> Adj_matrix;
        
        public graph(bool is_directed)
        {
            init();
            directed = is_directed;
            prepare_AdjMatrix();
        }
        public int degree_of_node(int index_node)
        {
            List<int> result = new List<int>();
            result = find_neighbors(index_node);
            return result.Count;
        }
        public void remove_edge(int node)
        {
            int i = 0;
            for (i = 0; i < node; i++)
                delete_single_edge(i, node);
            for (i = node + 1; i < number_node; i++)
                delete_single_edge(node, i);
        }
        public string convert_adj_to_string()
        {
            string result;
            result = string.Empty;
            int i = 0;
            for (i = 0; i < Adj_matrix.Count; i++)
                result += Adj_matrix[i].ToString();
            return result;
        }
        void delete_single_edge(int node1, int node2)
        {

            int tmp_n1 = node1;
            int tmp_n2 = node2;
            int index = (tmp_n1 * (2 * number_node - 1 - tmp_n1) / 2) +
                 (tmp_n2 - tmp_n1 - 1);
            Adj_matrix[index] = false;
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
            int index = (tmp_n1 * (2 * number_node - 1 - tmp_n1) / 2) +
                 (tmp_n2 - tmp_n1 - 1);
            return (Adj_matrix[index]);

        }
        public List<int> find_neighbors(int node)
        {
            List<int> result = new List<int>();
            int index = (node * (2 * number_node - 1 - node) / 2);
            int i = 0;
            for (i = index; i < index + number_node - 1 - node; i++)
                if (Adj_matrix[i])
                    result.Add(node + i - index + 1);

            for (i = 0; i < node; i++)
                if (is_link_between(node, i))
                    result.Add(i);
            return result;
        }
        public List<int> find_neighbors_with_flag(int node, bool[] flag)
        {
            List<int> result = new List<int>();
            int index = (node * (2 * number_node - 1 - node) / 2);
            int i = 0;
            for (i = index; i < index + number_node - 1 - node; i++)
                if (Adj_matrix[i] && !flag[node])
                    result.Add(node + i - index + 1);

            for (i = 0; i < node; i++)
                if (is_link_between(node, i) && !flag[node])
                    result.Add(i);
            return result;
        }


        public List<int> find_neighbors_grater_than(int node, int treshhold)
        {
            List<int> result = new List<int>();
            int index = (node * (2 * number_node - 1 - node) / 2);
            int i = 0;
            int temp = 0;
            for (i = index; i < index + number_node - 1 - node; i++)
            {
                temp = node + i - index + 1;
                if (Adj_matrix[i] && temp > treshhold)
                    result.Add(node + i - index + 1);
            }

            for (i = 0; i < node; i++)
                if (is_link_between(node, i) && i > treshhold)
                    result.Add(i);
            return result;
        }
        public void prepare_AdjMatrix()
        {
            int i = 0;
            long n = (number_node * (number_node - 1)) / 2;
            for (i = 0; i < n; i++)
                Adj_matrix.Add(false);
        }

        public void add_link(int node1, int node2)
        {
            int index = 0;
            int temp = 0;

            //swap
            if (node2 < node1)
            {
                temp = node2;
                node2 = node1;
                node1 = temp;
            }

            //the first element in Adj_matrix is stored in index 0
            if (!directed)
            {
                index = (node1 * (2 * number_node - 1 - node1) / 2) +
                    (node2 - node1 - 1);
                if (index >= 0 && index < Adj_matrix.Count && !Adj_matrix[index])
                    Adj_matrix[index] = true;
            }
        }
        void init()
        {
            Adj_matrix = new List<bool>();
        }
    }
}

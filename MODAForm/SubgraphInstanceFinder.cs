using System;
using System.Collections.Generic;
using System.Text;

namespace Motif1
{
    class SubgraphInstanceFinder
    {
        List<List<int>> temp_size_k_graphs;
        List<List<int>> subgraph_maps_domain;
        List<List<int>> subgraph_maps_range;
        bool[] flag;
        int[] f;
        graph network;
        graph h;
        public SubgraphInstanceFinder(graph Network, graph H)
        {
            network = Network;
            h = H;
        }
        public void Find_subgraph_instances()
        {
            find_subgraph_instances(network, h);
        }
         void find_subgraph_instances(graph G, graph H)
        {
            int range_tag_size = 1000;
            temp_size_k_graphs = new List<List<int>>();
            List<int> function_domain = new List<int>();
            subgraph_maps_domain = new List<List<int>>();
            subgraph_maps_range = new List<List<int>>();
            List<int> used_range = new List<int>();
            flag = new bool[10000];
            bool[] domain_tag = new bool[50];
            bool[] range_tag = new bool[range_tag_size];
            int[] local_result = new int[50];
            int i = 0;
            int j = 0;

            for (i = 0; i < G.number_node; i++)
            {
                clear_domain_tag(domain_tag, 50);
                clear_domain_tag(range_tag, range_tag.Length);
                clear_function(f, 50);
                for (j = 0; j < H.number_node; j++)
                {
                    function_domain = new List<int>();
                    used_range = new List<int>();
                    if (can_support(i, G, j, H))
                    {
                        f[j] = i;
                        domain_tag[j] = true;
                        range_tag[i] = true;
                        function_domain.Add(j);
                        used_range.Add(i);
                        local_result = isomorphic_extensions(f, G, H, function_domain, used_range, domain_tag, range_tag);
                    }
                }

                network.remove_edge(i);
            }

        }

        

        void clear_domain_tag(bool[] domain_tag, int size)
        {
            for (int i = 0; i < size; i++)
                domain_tag[i] = false;
        }

        void clear_function(int[] function, int size)
        {
            for (int i = 0; i < size; i++)
                function[i] = -1;
        }

        bool can_support(int node_G, graph G, int node_H, graph H)
        {

            //neighboor condition satisfied must be added in futeure.
            if (G.degree_of_node(node_G) < H.degree_of_node(node_H))
                return false;

            return true;
        }

        int[] isomorphic_extensions(int[] function, graph G, graph H, List<int> domain, List<int> used_range, bool[] domain_tag, bool[] range_tag)
        {
            List<int> neighbor_range = new List<int>();
            List<int> domain_prim = new List<int>();
            List<int> used_range_prim = new List<int>();
            bool[] domain_tag_prim = new bool[50];
            int range_tag_prim_size = 1000;
            bool[] range_tag_prim = new bool[range_tag_prim_size];
            int[] function_prim = new int[50];

            int m = 0;

            if (domain.Count == H.number_node)
            {
                add_mapping_to_list(function, subgraph_maps_domain, subgraph_maps_range, domain.Count);
                return function;
            }
            m = choose_one_neighbor_of_domain(domain, H, domain_tag);
            if (m == -1)
                return null;

            neighbor_range = choose_neighbor_of_range(used_range, G, range_tag);

            function = function_prim;
            

            for (int i = 0; i < neighbor_range.Count; i++)
            {
                if (node_is_compatible(G, H, neighbor_range[i], m, domain, function))
                {

                    used_range_prim = copy_list(used_range);
                    function_prim[m] = neighbor_range[i];
                    used_range_prim.Add(neighbor_range[i]);
                    domain_prim = copy_list(domain);
                    domain_prim.Add(m);
                    copy_array(domain_tag, domain_tag_prim, domain_tag_prim.Length);
                    copy_array(range_tag, range_tag_prim, range_tag_prim.Length);
                    domain_tag_prim[m] = true;
                    range_tag_prim[neighbor_range[i]] = true;
                    isomorphic_extensions(function_prim, G, H, domain_prim, used_range_prim, domain_tag_prim, range_tag_prim);
                }

            }
            int[] t = new int[1];
            return t;
        }

        void copy_array(bool[] original, bool[] copy, int size)
        {
            for (int i = 0; i < size; i++)
                copy[i] = original[i];
        }
        void copy_array(int[] original, int[] copy, int size)
        {
            for (int i = 0; i < size; i++)
                copy[i] = original[i];
        }

        List<int> copy_list(List<int> original)
        {
            List<int> copied = new List<int>();
            int i = 0;
            copied = new List<int>();
            for (i = 0; i < original.Count; i++)
                copied.Add(original[i]);
            return copied;
        }

        bool node_is_compatible(graph G, graph H, int g_node, int h_node, List<int> domain_in_H, int[] function)
        {
            List<int> neighbor = new List<int>();
            List<int> non_neighbor = new List<int>();
            int i = 0;


            for (i = 0; i < 50; i++)
                if (function[i] == g_node)
                    return false;
            //no repeated mapping in range is allowed

           
            neighbor = H.find_neighbors(h_node);


            //split into two sets
            int local_counter = 0;
            for (i = 0; i < neighbor.Count + local_counter; i++)
                if (!domain_in_H.Contains(neighbor[i - local_counter]))
                {

                    neighbor.Remove(neighbor[i - local_counter]);
                    local_counter++;
                }
            for (i = 0; i < domain_in_H.Count; i++)
                if (!neighbor.Contains(domain_in_H[i]))
                {
                    non_neighbor.Add(domain_in_H[i]);
                }


            List<int> local = new List<int>();
            local = G.find_neighbors_with_flag(g_node, flag);
            for (i = 0; i < neighbor.Count; i++)
            {
                if (!local.Contains(function[neighbor[i]]))
                    return false;
            }

            //Inja paper eshtebah dasht
            //for (i = 0; i <non_neighbor.Count; i++)
            //{
            //    if (local.Contains(function[non_neighbor[i]]))
            //        return false;
            //}

            return true;
        }

       


        List<int> choose_neighbor_of_range(List<int> used_range, graph G, bool[] range_tag)
        {
            List<int> local = new List<int>();
            List<int> result = new List<int>();
            int local_counter = 0;
            for (int i = 0; i < used_range.Count; i++)
            {
                local = G.find_neighbors(used_range[i]);
                if (local.Count == 0)
                    return result;
                local_counter = 0;
                for (int j = 0; j < local.Count + local_counter; j++)
                    if (range_tag[local[j - local_counter]] || flag[local[j - local_counter]])
                    {
                        local.Remove(local[j - local_counter]);
                        local_counter++;
                    }
                add_list(result, local);
            }
            return result;
        }

        void add_list(List<int> original, List<int> added_list)
        {
            int i = 0;
            for (i = 0; i < added_list.Count; i++)
                if (!original.Contains(added_list[i]))
                    original.Add(added_list[i]);
        }



        //choose next vertex that is neighbor of domain
        int choose_one_neighbor_of_domain(List<int> used_domain, graph H, bool[] domain_tag)
        {
            List<int> local = new List<int>();
            List<int> result = new List<int>();
            int local_counter = 0;
            for (int i = 0; i < used_domain.Count; i++)
            {
                local = H.find_neighbors(used_domain[i]);
                local_counter = 0;
                for (int j = 0; j < local.Count + local_counter; j++)
                    if (domain_tag[local[j - local_counter]])
                    {
                        local.Remove(local[j - local_counter]);
                        local_counter++;
                    }
                add_list(result, local);
            }
            if (result.Count == 0)
                return -1;
            else
                return result[0];
        }

        void add_mapping_to_list(int[] func, List<List<int>> DOMAIN, List<List<int>> RANGE, int counter)
        {
            int i = 0;
            List<int> local_domain = new List<int>();
            List<int> local_range = new List<int>();
            for (i = 0; i < counter; i++)
            {
                local_domain.Add(i);
                local_range.Add(func[i]);
            }
            DOMAIN.Add(local_domain);
            RANGE.Add(local_range);
        }



    }
}

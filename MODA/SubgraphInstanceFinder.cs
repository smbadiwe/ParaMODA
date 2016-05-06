using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA
{
    public class SubgraphInstanceFinder
    {
        private List<List<int>> temp_size_k_graphs;
        private List<List<int>> subgraph_maps_domain;
        private List<List<int>> subgraph_maps_range;
        private bool[] flag;
        private int[] f;
        private Graph network;
        private Graph h;
        public SubgraphInstanceFinder(Graph Network, Graph H)
        {
            network = Network;
            h = H;
            subgraph_maps_domain = new List<List<int>>();
            subgraph_maps_range = new List<List<int>>();
            temp_size_k_graphs = new List<List<int>>();
        }

        public void Find_subgraph_instances()
        {
            int range_tag_size = 1000;
            List<int> function_domain = new List<int>();
            List<int> used_range = new List<int>();
            flag = new bool[10000];
            bool[] domain_tag = new bool[50];
            bool[] range_tag = new bool[range_tag_size];
            int[] local_result = new int[50];
            int i = 0;
            int j = 0;

            for (i = 0; i < network.NumberOfNodes; i++)
            {
                ClearDomainTag(domain_tag, 50);
                ClearDomainTag(range_tag, range_tag.Length);
                ClearFunction(f, 50);
                for (j = 0; j < h.NumberOfNodes; j++)
                {
                    function_domain = new List<int>();
                    used_range = new List<int>();
                    if (CanSupport(i, network, j, h))
                    {
                        f[j] = i;
                        domain_tag[j] = true;
                        range_tag[i] = true;
                        function_domain.Add(j);
                        used_range.Add(i);
                        local_result = IsomorphicExtensions(f, network, h, function_domain, used_range, domain_tag, range_tag);
                    }
                }

                network.RemoveEdge(i);
            }

        }

        private void ClearDomainTag(bool[] domain_tag, int size)
        {
            for (int i = 0; i < size; i++)
            {
                domain_tag[i] = false;
            }
        }

        private void ClearFunction(int[] function, int size)
        {
            for (int i = 0; i < size; i++)
            {
                function[i] = -1;
            }
        }

        private bool CanSupport(int node_G, Graph G, int node_H, Graph H)
        {
            //neighboor condition satisfied must be added in futeure.
            return (G.DegreeOfNode(node_G) >= H.DegreeOfNode(node_H));
        }

        private int[] IsomorphicExtensions(int[] function, Graph G, Graph H, List<int> domain, List<int> used_range, bool[] domain_tag, bool[] range_tag)
        {
            List<int> neighbor_range = new List<int>();
            List<int> domain_prim = new List<int>();
            List<int> used_range_prim = new List<int>();
            bool[] domain_tag_prim = new bool[50];
            int range_tag_prim_size = 1000;
            bool[] range_tag_prim = new bool[range_tag_prim_size];
            int[] function_prim = new int[50];

            int m = 0;

            if (domain.Count == H.NumberOfNodes)
            {
                AddMappingToList(function, subgraph_maps_domain, subgraph_maps_range, domain.Count);
                return function;
            }
            m = ChooseOneNeighborOfDomain(domain, H, domain_tag);
            if (m == -1)
                return null;

            neighbor_range = ChooseNeighborOfRange(used_range, G, range_tag);

            function = function_prim;


            for (int i = 0; i < neighbor_range.Count; i++)
            {
                if (NodeIsCompatible(G, H, neighbor_range[i], m, domain, function))
                {

                    used_range_prim = CopyList(used_range);
                    function_prim[m] = neighbor_range[i];
                    used_range_prim.Add(neighbor_range[i]);
                    domain_prim = CopyList(domain);
                    domain_prim.Add(m);
                    CopyArray(domain_tag, domain_tag_prim, domain_tag_prim.Length);
                    CopyArray(range_tag, range_tag_prim, range_tag_prim.Length);
                    domain_tag_prim[m] = true;
                    range_tag_prim[neighbor_range[i]] = true;
                    IsomorphicExtensions(function_prim, G, H, domain_prim, used_range_prim, domain_tag_prim, range_tag_prim);
                }

            }
            int[] t = new int[1];
            return t;
        }

        private void CopyArray(bool[] original, bool[] copy, int size)
        {
            for (int i = 0; i < size; i++)
            {
                copy[i] = original[i];
            }
        }

        private void CopyArray(int[] original, int[] copy, int size)
        {
            for (int i = 0; i < size; i++)
            {
                copy[i] = original[i];
            }
        }

        private List<int> CopyList(List<int> original)
        {
            List<int> copied = new List<int>();
            copied.AddRange(original);
            return copied;
        }

        private bool NodeIsCompatible(Graph G, Graph H, int g_node, int h_node, List<int> domain_in_H, int[] function)
        {
            List<int> neighbor = new List<int>();
            List<int> non_neighbor = new List<int>();
            int i = 0;

            for (i = 0; i < 50; i++)
            {
                if (function[i] == g_node) return false;
            }
            //no repeated mapping in range is allowed

            neighbor = H.FindNeighbors(h_node);

            //split into two sets
            int local_counter = 0;
            for (i = 0; i < neighbor.Count + local_counter; i++)
            {
                if (!domain_in_H.Contains(neighbor[i - local_counter]))
                {
                    neighbor.Remove(neighbor[i - local_counter]);
                    local_counter++;
                }
            }
            for (i = 0; i < domain_in_H.Count; i++)
            {
                if (!neighbor.Contains(domain_in_H[i]))
                {
                    non_neighbor.Add(domain_in_H[i]);
                }
            }

            List<int> local = G.FindNeighborsWithFlag(g_node, flag);
            for (i = 0; i < neighbor.Count; i++)
            {
                if (!local.Contains(function[neighbor[i]]))
                    return false;
            }

            return true;
        }

        private List<int> ChooseNeighborOfRange(List<int> used_range, Graph G, bool[] range_tag)
        {
            List<int> local = new List<int>();
            List<int> result = new List<int>();
            int local_counter = 0;
            for (int i = 0; i < used_range.Count; i++)
            {
                local = G.FindNeighbors(used_range[i]);
                if (local.Count == 0) return result;

                local_counter = 0;
                for (int j = 0; j < local.Count + local_counter; j++)
                {
                    if (range_tag[local[j - local_counter]] || flag[local[j - local_counter]])
                    {
                        local.Remove(local[j - local_counter]);
                        local_counter++;
                    }
                }
                AddList(result, local);
            }
            return result;
        }

        private void AddList(List<int> original, List<int> added_list)
        {
            int i = 0;
            for (i = 0; i < added_list.Count; i++)
            {
                if (!original.Contains(added_list[i]))
                {
                    original.Add(added_list[i]);
                }
            }
        }

        /// <summary>
        /// choose next vertex that is neighbor of domain
        /// </summary>
        /// <param name="used_domain"></param>
        /// <param name="H"></param>
        /// <param name="domain_tag"></param>
        /// <returns></returns>
        private int ChooseOneNeighborOfDomain(List<int> used_domain, Graph H, bool[] domain_tag)
        {
            List<int> local = new List<int>();
            List<int> result = new List<int>();
            int local_counter = 0;
            for (int i = 0; i < used_domain.Count; i++)
            {
                local = H.FindNeighbors(used_domain[i]);
                local_counter = 0;
                for (int j = 0; j < local.Count + local_counter; j++)
                {
                    if (domain_tag[local[j - local_counter]])
                    {
                        local.Remove(local[j - local_counter]);
                        local_counter++;
                    }
                }
                AddList(result, local);
            }

            if (result.Count == 0) return -1;

            return result[0];
        }

        private void AddMappingToList(int[] func, List<List<int>> DOMAIN, List<List<int>> RANGE, int counter)
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

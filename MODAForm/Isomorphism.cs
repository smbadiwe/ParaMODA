using System;
using System.Collections.Generic;
using System.Text;

namespace Motif1
{
    class Isomorphism
    {
        NewGraph network;
        QueryGraph query;
        int numberOfMappings;
        List<Mapping> mappings;


        public Isomorphism(NewGraph Network, QueryGraph Query)
        {
            network = Network;
            query = Query;
            numberOfMappings = 0;
            mappings = new List<Mapping>();
        }

        //
        //
        // testing if module
        //

        public int testingIf(int first, int second)
        {
            int sum = 0;
            int n1 = 0;
            int n2 = 0;
            foreach (Mapping M in mappings)
            {
                n1 = M.getRange(first);
                n2 = M.getRange(second);

                if (network.is_link_between2(n1, n2))
                    sum++;

            }
            return sum;
        }
        //
        //


        public Mapping getSinlgeMapping(int index)
        {
            return mappings[index];
        }
        public void generateMapping()
        {
            int network_vertex = network.getNumberVertex();
            int query_vertex = query.getNumberVertex();
            Mapping temp_mapping = new Mapping();
            Map temp_map = new Map();
            // for handling index of sorted vertex
            int index = 0;
            int index_degree = 0; //it is for second invarient
            //

            for (int i = 0; i < network_vertex - 1; i++)    //we can doing this with random schema picking of network's node
            {
                // index = network.getSoretedVertex(i).getName(); //it is added for first invarient
                //on the vertex degree
                // index_degree = network.getVertex(index).getDegree(); //for second invarient


                for (int k = 0; k < query_vertex; k++)
                {
                    temp_mapping = new Mapping();
                    index = i;
                    if (support(network.getVertex(index), query.getVertex(k)))
                    {
                        temp_map = new Map(k, index);
                        temp_mapping.AddMap(temp_map);
                        temp_mapping = IsomorphicExtensions(temp_mapping);

                    }//end if support

                }//end for k

                network.removeVertex(index, index_degree);

            }//end for i

        }



        int getMostConstraint(List<int> domain, QueryGraph q)
        {

            List<bool> flag = new List<bool>();
            int size = q.getNumberVertex();
            int i = 0;
            for (/*int i=0*/; i < size; i++)
                flag.Add(false);
            for (i = 0; i < domain.Count; i++)
                flag[domain[i]] = true;

            List<int> neighbors_domain = new List<int>();
            List<int> temp = new List<int>();

            for (i = 0; i < domain.Count; i++)
            {
                temp.Clear();
                temp = q.find_neighbors(domain[i]);
                for (int k = 0; k < temp.Count; k++)
                    if (!flag[temp[k]])
                        neighbors_domain.Add(temp[k]);
            }

            try
            {
                return neighbors_domain[0]; //it must be refined with most constraint policy
            }
            catch (Exception e)
            {
                return -1;
            }

        }
        List<int> getMostConstraint(List<int> domain) //polymorphism of methode for geting renge from Network
        {

            List<bool> flag = new List<bool>();
            int size = network.getNumberVertex();
            int i = 0;
            for (/*int i=0*/; i < size; i++)
                flag.Add(false);
            for (i = 0; i < domain.Count; i++)
                flag[domain[i]] = true;

            List<int> neighbors_domain = new List<int>();
            List<int> temp = new List<int>();

            for (i = 0; i < domain.Count; i++)
            {
                temp.Clear();
                temp = network.find_neighbors(domain[i]);
                for (int k = 0; k < temp.Count; k++)
                    if ((!flag[temp[k]]) && (!neighbors_domain.Contains(temp[k])))
                        neighbors_domain.Add(temp[k]);
            }
            return neighbors_domain;
            //return neighbors_domain[0]; //it must be refined with most constraint policy

        }

        Mapping IsomorphicExtensions(Mapping current_mapping)
        {

            List<int> domain = new List<int>();
            domain = current_mapping.getDomain();
            Mapping current_mapping_prim = new Mapping();
            Map single_map;



            if (domain.Count == query.getNumberVertex())
            {
                Mapping tmp = new Mapping();
                for (int k = 0; k < current_mapping.get_mapping().Count; k++)
                    tmp.AddMap(current_mapping.get_map(k));
                mappings.Add(tmp);
                return current_mapping;
            }

            int m = getMostConstraint(domain, query);
            if (m == -1)
                return null;

            List<int> n = new List<int>();
            n = getMostConstraint(current_mapping.getRange());

            for (int i = 0; i < n.Count; i++)
            {

                //add violate conditions 
                if (compatible(current_mapping, m, n[i]) /*&& breaking(current_mapping,m,n[i])*/ )
                {
                    current_mapping_prim.set_mapping(current_mapping.get_mapping());
                    single_map = new Map(m, n[i]);
                    current_mapping_prim.AddMap(single_map);
                    IsomorphicExtensions(current_mapping_prim);
                }
            }  //end for i for n


            return current_mapping;


        }

        bool breaking(Mapping f, int m, int n) //this function is for testof breaking condition
        {
            if (m == 0)
            {
                int index = f.findMap(3);
                int renge_of_3 = 0;
                if (index != -1)
                {
                    renge_of_3 = f.get_map(index).get_range();
                    if (renge_of_3 < n)
                        return false;
                }
            }
            else
                if (m == 3)
            {
                int index = f.findMap(0);
                int renge_of_0 = 0;
                if (index != -1)
                {
                    renge_of_0 = f.get_map(index).get_range();
                    if (renge_of_0 > n)
                        return false;
                }
            }
            return true;
        }


        bool compatible(Mapping f, int m, int n)
        {
            List<int> domain = f.getDomain();
            bool isLink = false;
            for (int i = 0; i < domain.Count; i++)
            {
                isLink = query.is_link_between(m, domain[i]);
                if (isLink)
                {
                    if (!network.is_link_between(n, f.getRange()[i]))
                        return false;
                }
                else
                {
                    //saeed
                    //if (network.is_link_between(n, f.getRange()[i]))
                    //return false;
                }
            }
            return true;
        }

        /// <summary>
        /// In Algo 2: line 5: if kv >= ku
        /// </summary>
        /// <param name="network_vertex"></param>
        /// <param name="query_vertex"></param>
        /// <returns></returns>
        bool support(Vertex network_vertex, Vertex query_vertex)
        {
            if (network_vertex.getDegree() < query_vertex.getDegree())
                return false;

            var temp_list_netwok = network_vertex.getNeighborsDegrees();
            var temp_list_query = query_vertex.getNeighborsDegrees();

            for (int i = temp_list_query.Count - 1; i >= 0; i--)
            {
                if (temp_list_netwok[i] < temp_list_query[i])  return false;
            }

            return true;
        }

        public int getNumberMapping()
        {
            return mappings.Count;
        }

    }
}

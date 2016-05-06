using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA
{
    public class Isomorphism
    {
        NewGraph _network;
        QueryGraph _query;
        int _numberOfMappings;
        List<Mapping> _mappings;
        
        public Isomorphism(NewGraph network, QueryGraph query)
        {
            _network = network;
            _query = query;
            _numberOfMappings = 0;
            _mappings = new List<Mapping>();
        }

        //
        //
        // testing if module
        //
        public int TestingIf(int first, int second)
        {
            int sum = 0;
            int n1 = 0;
            int n2 = 0;
            foreach (Mapping M in _mappings)
            {
                n1 = M.GetRange(first);
                n2 = M.GetRange(second);

                if (_network.IsLinkBetween2(n1, n2))
                {
                    sum++;
                }
            }
            return sum;
        }

        public Mapping GetSinlgeMapping(int index)
        {
            return _mappings[index];
        }

        /// <summary>
        /// Algorithm 2: Mapping Module
        /// </summary>
        public void GenerateMapping()
        {
            int networkVertex = _network.GetNumberOfVertex();
            int queryVertex = _query.GetNumberOfVertex();
            Mapping tempMapping = new Mapping();
            Map tempMap = new Map();
            // for handling index of sorted vertex
            int index = 0;
            int index_degree = 0; //it is for second invarient
            //

            for (int i = 0; i < networkVertex - 1; i++)    //we can doing this with random schema picking of network's node
            {
                // index = network.getSoretedVertex(i).getName(); //it is added for first invarient
                //on the vertex degree
                // index_degree = network.getVertex(index).getDegree(); //for second invarient


                for (int k = 0; k < queryVertex; k++)
                {
                    //temp_mapping = new Mapping();
                    index = i;
                    if (Support(_network.GetVertex(index), _query.GetVertex(k)))
                    {
                        tempMap = new Map(k, index);
                        tempMapping.AddMap(tempMap);
                        tempMapping = IsomorphicExtensions(tempMapping);

                    }//end if support

                }//end for k

                _network.RemoveVertex(index, index_degree);

            }//end for i

        }
        
        int GetMostConstraint(List<int> domain, QueryGraph q)
        {

            List<bool> flag = new List<bool>();
            int size = q.GetNumberOfVertex();
            int i = 0;
            for (/*int i=0*/; i < size; i++)
                flag.Add(false);
            for (i = 0; i < domain.Count; i++)
                flag[domain[i]] = true;

            List<int> neighborsDomain = new List<int>();
            List<int> temp = new List<int>();

            for (i = 0; i < domain.Count; i++)
            {
                temp.Clear();
                temp = q.FindNeighbors(domain[i]);
                for (int k = 0; k < temp.Count; k++)
                {
                    if (!flag[temp[k]])
                    {
                        neighborsDomain.Add(temp[k]);
                    }
                }
            }

            try
            {
                return neighborsDomain[0]; //it must be refined with most constraint policy
            }
            catch (Exception e)
            {
                return -1;
            }

        }

        private List<int> GetMostConstraint(List<int> domain) //polymorphism of methode for geting renge from Network
        {
            List<bool> flag = new List<bool>();
            int size = _network.GetNumberOfVertex();
            int i = 0;
            for (/*int i=0*/; i < size; i++)
            {
                flag.Add(false);
            }
            for (i = 0; i < domain.Count; i++)
            {
                flag[domain[i]] = true;
            }
            List<int> neighbors_domain = new List<int>();
            List<int> temp = new List<int>();

            for (i = 0; i < domain.Count; i++)
            {
                temp.Clear();
                temp = _network.FindNeighbors(domain[i]);
                for (int k = 0; k < temp.Count; k++)
                {
                    if ((!flag[temp[k]]) && (!neighbors_domain.Contains(temp[k])))
                    {
                        neighbors_domain.Add(temp[k]);
                    }
                }
            }
            return neighbors_domain;
            //return neighbors_domain[0]; //it must be refined with most constraint policy

        }

        private Mapping IsomorphicExtensions(Mapping currentMapping)
        {
            List<int> domain = currentMapping.GetDomain();
            if (domain.Count == _query.GetNumberOfVertex())
            {
                Mapping tmp = new Mapping();
                for (int k = 0; k < currentMapping.GetMapping().Count; k++)
                {
                    tmp.AddMap(currentMapping.GetMap(k));
                }
                _mappings.Add(tmp);
                return currentMapping;
            }

            int m = GetMostConstraint(domain, _query);
            if (m == -1) return null;

            List<int> n = GetMostConstraint(currentMapping.GetRange());

            Mapping current_mapping_prim;
            Map single_map;

            for (int i = 0; i < n.Count; i++)
            {
                //add violate conditions 
                if (Compatible(currentMapping, m, n[i]) /*&& breaking(current_mapping,m,n[i])*/ )
                {
                    current_mapping_prim = new Mapping(currentMapping.GetMapping());
                    single_map = new Map(m, n[i]);
                    current_mapping_prim.AddMap(single_map);
                    IsomorphicExtensions(current_mapping_prim);
                }
            }  //end for i for n


            return currentMapping;
        }

        private bool breaking(Mapping f, int m, int n) //this function is for testof breaking condition
        {
            if (m == 0)
            {
                int index = f.FindMap(3);
                int renge_of_3 = 0;
                if (index != -1)
                {
                    renge_of_3 = f.GetMap(index).Range;
                    if (renge_of_3 < n) return false;
                }
            }
            else if (m == 3)
            {
                int index = f.FindMap(0);
                int renge_of_0 = 0;
                if (index != -1)
                {
                    renge_of_0 = f.GetMap(index).Range;
                    if (renge_of_0 > n) return false;
                }
            }
            return true;
        }

        private bool Compatible(Mapping f, int m, int n)
        {
            List<int> domain = new List<int>();
            domain = f.GetDomain();
            bool isLink = false;
            for (int i = 0; i < domain.Count; i++)
            {
                isLink = _query.IsLinkBetween(m, domain[i]);
                if (isLink)
                {
                    if (!_network.IsLinkBetween(n, f.GetRange()[i]))
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

        private bool Support(Vertex networkVertex, Vertex queryVertex)
        {
            if (networkVertex.GetDegree() < queryVertex.GetDegree()) return false;

            var temp_list_netwok = networkVertex.GetNeighborsDegrees();
            var temp_list_query = queryVertex.GetNeighborsDegrees();

            for (int i = temp_list_query.Count - 1; i >= 0; i--)
            {
                if (temp_list_query[i] > temp_list_netwok[i]) return false;
            }

            return true;
        }

        public int getNumberMapping()
        {
            return _mappings.Count;
        }

    }
}

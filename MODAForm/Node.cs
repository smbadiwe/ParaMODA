using System;
using System.Collections.Generic;
using System.Text;

namespace Motif1
{
    class Node
    {
        List<Mapping> mappings;
        List<bool> cam_code;
        List<condition> breaking_conditions;
        List<Node> childs;

        public Node()
        {
            mappings = new List<Mapping>();
            cam_code = new List<bool>();
            breaking_conditions = new List<condition>();
            childs = new List<Node>();
        }

        public Node(List<bool> Cam_code)
        {
            mappings = new List<Mapping>();
            cam_code = new List<bool>();
            breaking_conditions = new List<condition>();
            childs = new List<Node>();
            cam_code = Cam_code;

        }

        public List<Node> generateSubgraphs()
        {
            List<Node> result = new List<Node>();
            List<bool> temp=new List<bool>();
            Node temp_child;
            for (int i = 0; i < cam_code.Count; i++)
            {
                temp = cam_code;
                if (!temp[i])
                {
                    temp[i] = true;
                    temp_child=new Node(temp);
                    result.Add(temp_child);
                }
            }
            return result;
        }

        List<List<int>> find_equivalents(List<List<int>> A)
        {
            int n = A[0].Count;
            List<int> V = new List<int>();
            List<List<int>> result = new List<List<int>>();
            List<int> local;
            int i = 0;
            for (; i < n; i++)
                V.Add(i);
            i = 0;
            while (V.Count != 0 && i < n)
            {
                local = new List<int>();
                for (int k = 0; k < A.Count; k++)
                {
                    if (!local.Contains(A[k][i]))
                    {
                        local.Add(A[k][i]);
                        V.Remove(A[k][i]);
                    }

                }
                result.Add(local);
                i++;
            }
            return result;
        }

        public List<List<int>> generateEquivalent(List<Mapping> mappings)
        {
            int n = mappings[0].get_length();
            List<int> V = new List<int>();
            List<List<int>> result = new List<List<int>>();
            List<int> local;
            int i = 0;
            for (; i < n; i++)
                V.Add(i);        // baraye inke hamishe har graph shamele vertex haye 0 ta n mibashad.
            i = 0;
            while (V.Count != 0 && i < n)
            {
                local = new List<int>();
                for (int k = 0; k < mappings.Count; k++)
                {
                    if (!local.Contains(mappings[k].get_map(i).get_range()))
                    {
                        local.Add(mappings[k].get_map(i).get_range());
                        V.Remove(mappings[k].get_map(i).get_range());
                    }

                }
                result.Add(local);
                i++;
            }
            
            return result;

        }

        List<Mapping> refine(List<Mapping> A, int n)
        {
            List<Mapping> result = new List<Mapping>();
            for (int i = 0; i < A.Count; i++)
                if (A[i].get_map(n).get_range() == n)
                    result.Add(A[i]);

            return result;
        }

        public List<condition> generateCondition()
        {
            int i=0;
            int n = 0;
            int max = 0;
            condition local;
            List<condition> result = new List<condition>();
            List<Mapping> A = new List<Mapping>();
            /// A=getAutomorphism(cam_code)     tavasote farid bayad neveshte shavad
            while (A.Count != 1)
            {
                List<List<int >> equvalent = generateEquivalent(A);
                max = get_largest_index(equvalent);
                n = equvalent[max][0];
                local=new condition();
                local.set_left(n);
                for (i = 1; i < equvalent[max].Count; i++)
                    local.add_right(equvalent[max][i]);
                result.Add(local);
                A = refine(A, n);
            }
            return result;
        }

        int get_largest_index(List<List<int>> T)
        {
            int index = 0;
            for (int i = 0; i < T.Count; i++)
                if (T[i].Count > index)
                    index = i;
            return index;
        }

        public List<Mapping> generateMapping(graph Network)
        { 
            //farid minevisadesh
            List<Mapping> result = new List<Mapping>();
            return result;
        }


       //public List<Mapping> getAutomorphism()   
       //{

       //} 
    }
}

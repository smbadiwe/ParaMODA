using System;
using System.Collections.Generic;
using System.Text;

namespace Motif1
{
    class QueryGraph:NewGraph
    {
        List<condition> breaking_conditions;

        public QueryGraph(List<bool> Adj_Matrix):base(Adj_Matrix)
        {
            breaking_conditions = new List<condition>();
            //breaking_conditions = generateCondition();
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
            int i = 0;
            int n = 0;
            int max = 0;
            condition local;
            List<condition> result = new List<condition>();
            List<Mapping> A = new List<Mapping>();
            /// A=getAutomorphism(cam_code)     tavasote farid bayad neveshte shavad
            while (A.Count != 1)
            {
                List<List<int>> equvalent = generateEquivalent(A);
                max = get_largest_index(equvalent);
                n = equvalent[max][0];
                local = new condition();
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

        public List<condition> getBreakingCondition()
        {
            return breaking_conditions;
        }
    }
}

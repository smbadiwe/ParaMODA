using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA
{
    public class QueryGraph : NewGraph
    {
        private List<Condition> _breakingConditions;

        public QueryGraph(List<bool> adjMatrix) : base(adjMatrix)
        {
            _breakingConditions = new List<Condition>();
        }

        public List<List<int>> GenerateEquivalent(List<Mapping> mappings)
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
                    if (!local.Contains(mappings[k].GetMap(i).Range))
                    {
                        local.Add(mappings[k].GetMap(i).Range);
                        V.Remove(mappings[k].GetMap(i).Range);
                    }

                }
                result.Add(local);
                i++;
            }

            return result;

        }

        private List<Mapping> Refine(List<Mapping> A, int n)
        {
            List<Mapping> result = new List<Mapping>();
            for (int i = 0; i < A.Count; i++)
            {
                if (A[i].GetMap(n).Range == n)
                {
                    result.Add(A[i]);
                }
            }
            return result;
        }

        public List<Condition> GenerateCondition()
        {
            int i = 0;
            int n = 0;
            int max = 0;
            Condition local;
            List<Condition> result = new List<Condition>();
            List<Mapping> A = new List<Mapping>();
            /// A=getAutomorphism(cam_code)     tavasote farid bayad neveshte shavad
            while (A.Count != 1)
            {
                List<List<int>> equvalent = GenerateEquivalent(A);
                max = GetLargestIndex(equvalent);
                n = equvalent[max][0];
                local = new Condition();
                local.SetLeft(n);
                for (i = 1; i < equvalent[max].Count; i++)
                {
                    local.AddRight(equvalent[max][i]);
                }
                result.Add(local);
                A = Refine(A, n);
            }
            return result;
        }

        private int GetLargestIndex(List<List<int>> T)
        {
            int index = 0;
            for (int i = 0; i < T.Count; i++)
            {
                if (T[i].Count > index)
                {
                    index = i;
                }
            }
            return index;
        }

        public List<Condition> GetBreakingCondition()
        {
            return _breakingConditions;
        }
    }
}

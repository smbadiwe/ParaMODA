using System.Collections.Generic;

namespace MODA.Impl
{
    /// <summary>
    /// This compares two <seealso cref="Mapping"/> nodes are equal. Used in dictionary where these nodes form the key
    /// </summary>
    public class MappingNodesComparer : EqualityComparer<string[]>
    {
        public override bool Equals(string[] x, string[] y)
        {
            return new HashSet<string>(x).SetEquals(y);
        }

        public override int GetHashCode(string[] obj)
        {
            int hash = 0;
            for (int i = 0; i < obj.Length; i++)
            {
                hash += obj[i].GetHashCode();
            }
            return hash;
        }
    }
}

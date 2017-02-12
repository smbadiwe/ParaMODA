using System.Collections.Generic;

namespace MODA.Impl
{
    /// <summary>
    /// This compares two <see cref="Mapping"/> nodes are equal. Used in dictionary where these nodes form the key.
    /// The node set of interest is that of the mapping images on G; i.e. the set that form the nodes in the <see cref="Mapping.InducedSubGraph"/>
    /// </summary>
    public sealed class MappingNodesComparer : EqualityComparer<string[]>
    {
        public override bool Equals(string[] x, string[] y)
        {
            return new HashSet<string>(x).SetEquals(y);
        }

        public override int GetHashCode(string[] obj)
        {
            int hash = 0;
            for (int i = obj.Length - 1; i > -1; i--)
            {
                hash += obj[i].GetHashCode();
            }
            return hash;
        }
    }
}

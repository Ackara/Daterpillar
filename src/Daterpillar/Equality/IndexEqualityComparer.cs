using System.Collections.Generic;

namespace Acklann.Daterpillar.Equality
{
    internal class IndexEqualityComparer : IEqualityComparer<Index>
    {
        public bool Equals(Index x, Index y)
        {
            return x.GetName() == y.GetName()
                && x.Type == y.Type
                && (string.Join(string.Empty, x.Columns)) == (string.Join(string.Empty, y.Columns))

                // Some servers will automatically flag the index as unique if it is a primary so I'm doing the same here.
                && (x.Type == IndexType.PrimaryKey ? true : x.IsUnique) == (y.Type == IndexType.PrimaryKey ? true : y.IsUnique);
        }

        public int GetHashCode(Index obj)
        {
            return obj.GetName().GetHashCode() ^ obj.Type.GetHashCode() ^ obj.IsUnique.GetHashCode() ^ (string.Join(string.Empty, obj.Columns)).GetHashCode();
        }
    }
}
using System.Collections.Generic;

namespace Ackara.Daterpillar.Equality
{
    internal class IndexEqualityComparer : IEqualityComparer<Index>
    {
        public bool Equals(Index x, Index y)
        {
            return
                x.Name == y.Name &&
                x.IsUnique == y.IsUnique &&
                x.Type == y.Type &&
                (string.Join("", x.Columns)) == (string.Join("", y.Columns));
        }

        public int GetHashCode(Index obj)
        {
            return obj.Name.GetHashCode() ^ obj.Type.GetHashCode() ^ obj.IsUnique.GetHashCode() ^ (string.Join("", obj.Columns)).GetHashCode();
        }
    }
}
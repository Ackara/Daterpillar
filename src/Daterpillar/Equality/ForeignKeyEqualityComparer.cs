using Acklann.Daterpillar.Configuration;
using System.Collections.Generic;

namespace Acklann.Daterpillar.Equality
{
    internal class ForeignKeyEqualityComparer : IEqualityComparer<ForeignKey>
    {
        public bool Equals(ForeignKey x, ForeignKey y)
        {
            return
                string.Equals(x.Name, y.Name, System.StringComparison.OrdinalIgnoreCase) &&
                x.OnUpdate == y.OnUpdate &&
                x.OnDelete == y.OnDelete;
        }

        public int GetHashCode(ForeignKey obj)
        {
            return obj.Name.GetHashCode() ^ obj.LocalTable.GetHashCode() ^ obj.LocalColumn.GetHashCode() ^ obj.ForeignTable.GetHashCode() ^ obj.ForeignColumn.GetHashCode() ^ obj.OnUpdate.GetHashCode() ^ obj.OnDelete.GetHashCode();
        }
    }
}
using Acklann.Daterpillar.Configuration;
using System.Collections.Generic;

namespace Acklann.Daterpillar.Equality
{
    internal class ForeignKeyEqualityComparer : IEqualityComparer<ForeignKey>
    {
        public bool Equals(ForeignKey x, ForeignKey y)
        {
            return
                x.Name.Equals(y.Name, System.StringComparison.CurrentCultureIgnoreCase) &&
                x.LocalTable == y.LocalTable &&
                x.LocalColumn == y.LocalColumn &&
                x.ForeignTable == y.ForeignTable &&
                x.ForeignColumn == y.ForeignColumn &&
                x.OnUpdate == y.OnUpdate &&
                x.OnDelete == y.OnDelete;
        }

        public int GetHashCode(ForeignKey obj)
        {
            return obj.Name.GetHashCode() ^ obj.LocalTable.GetHashCode() ^ obj.LocalColumn.GetHashCode() ^ obj.ForeignTable.GetHashCode() ^ obj.ForeignColumn.GetHashCode() ^ obj.OnUpdate.GetHashCode() ^ obj.OnDelete.GetHashCode();
        }
    }
}
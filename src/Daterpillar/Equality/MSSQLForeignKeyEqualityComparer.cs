namespace Acklann.Daterpillar.Equality
{
    internal class MSSQLForeignKeyEqualityComparer : System.Collections.Generic.IEqualityComparer<ForeignKey>
    {
        public bool Equals(ForeignKey x, ForeignKey y)
        {
            return
                x.Name.Equals(y.Name, System.StringComparison.CurrentCultureIgnoreCase) &&
                x.LocalTable == y.LocalTable &&
                x.LocalColumn == y.LocalColumn &&
                x.ForeignTable == y.ForeignTable &&
                x.ForeignColumn == y.ForeignColumn &&
                (x.OnUpdate == ReferentialAction.Restrict ? ReferentialAction.NoAction : x.OnUpdate) == (y.OnUpdate == ReferentialAction.Restrict ? ReferentialAction.NoAction : y.OnUpdate) &&
                (x.OnDelete == ReferentialAction.Restrict ? ReferentialAction.NoAction : x.OnDelete) == (y.OnDelete == ReferentialAction.Restrict ? ReferentialAction.NoAction : y.OnDelete);
        }

        public int GetHashCode(ForeignKey obj)
        {
            return obj.GetName().GetHashCode() ^ obj.LocalTable.GetHashCode() ^ obj.LocalColumn.GetHashCode() ^ obj.ForeignTable.GetHashCode() ^ obj.ForeignColumn.GetHashCode() ^ obj.OnUpdate.GetHashCode() ^ obj.OnDelete.GetHashCode();
        }
    }
}
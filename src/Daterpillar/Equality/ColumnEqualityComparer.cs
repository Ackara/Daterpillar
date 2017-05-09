using System.Collections.Generic;

namespace Ackara.Daterpillar.Equality
{
    internal class ColumnEqualityComparer : IEqualityComparer<Column>
    {
        public bool Equals(Column x, Column y)
        {
            if (x == null && y == null) return true;
            else if (x == null || y == null) return false;
            else
            {
                return x.Name == y.Name
                && x.DataType == y.DataType
                && x.IsNullable == y.IsNullable
                && x.AutoIncrement == y.AutoIncrement
                && x.Comment == y.Comment
                && x.OrdinalPosition == y.OrdinalPosition;
            }
        }

        public int GetHashCode(Column obj)
        {
            return obj.Name.GetHashCode() ^ obj.DataType.GetHashCode() ^ obj.IsNullable.GetHashCode() ^ obj.AutoIncrement.GetHashCode() ^ obj.OrdinalPosition.GetHashCode() ^ obj.Comment.GetHashCode();
        }
    }
}
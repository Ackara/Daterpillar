using Acklann.Daterpillar.Configuration;
using System.Collections.Generic;

namespace Acklann.Daterpillar.Equality
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
                && Equals(x.DataType, y.DataType)
                && x.IsNullable == y.IsNullable
                && x.AutoIncrement == y.AutoIncrement
                && x.OrdinalPosition == y.OrdinalPosition
                && ((x.Comment == y.Comment) || (string.IsNullOrEmpty(x.Comment) || string.IsNullOrEmpty(y.Comment)));
            }
        }

        public bool Equals(DataType x, DataType y)
        {
            return x.Name.Equals(y.Name, System.StringComparison.CurrentCultureIgnoreCase)
                && (x.Scale == 0 ? y.Scale : x.Scale) == (y.Scale == 0 ? x.Scale : y.Scale)
                && (x.Precision == 0 ? y.Precision : x.Precision) == (y.Precision == 0 ? x.Precision : y.Precision);
        }

        public int GetHashCode(Column obj)
        {
            return obj.Name.GetHashCode() ^ obj.DataType.GetHashCode() ^ obj.IsNullable.GetHashCode() ^ obj.AutoIncrement.GetHashCode() ^ obj.OrdinalPosition.GetHashCode() ^ obj.Comment.GetHashCode();
        }

        #region Private Members

        private Compilation.Resolvers.SQLiteTypeResolver _typeResolver = new Compilation.Resolvers.SQLiteTypeResolver();

        #endregion Private Members
    }
}
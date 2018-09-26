using Acklann.Daterpillar.Configuration;
using System.Collections.Generic;

namespace Acklann.Daterpillar.Equality
{
    internal class SQLiteColumnEqualityComparer : IEqualityComparer<Column>
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
                && x.OrdinalPosition == y.OrdinalPosition
                && ((x.Comment == y.Comment) || (string.IsNullOrEmpty(x.Comment) || string.IsNullOrEmpty(y.Comment)));
            }
        }

        public bool Equals(DataType x, DataType y)
        {
            DataType left = x, right = y;
            left.Name = _typeResolver.GetTypeName(x);
            right.Name = _typeResolver.GetTypeName(y);

            return left.Name.Equals(right.Name, System.StringComparison.CurrentCultureIgnoreCase)
                && (left.Scale == 0 ? right.Scale : left.Scale) == (right.Scale == 0 ? left.Scale : right.Scale)
                && (left.Precision == 0 ? right.Precision : left.Precision) == (right.Precision == 0 ? left.Precision : right.Precision);
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
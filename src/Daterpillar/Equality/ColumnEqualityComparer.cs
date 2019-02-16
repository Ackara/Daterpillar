using Acklann.Daterpillar.Configuration;
using System.Collections.Generic;

namespace Acklann.Daterpillar.Equality
{
    internal class ColumnEqualityComparer : IEqualityComparer<ColumnDeclaration>
    {
        public bool Equals(ColumnDeclaration x, ColumnDeclaration y)
        {
            if (x == null && y == null) return true;
            else if (x == null || y == null) return false;
            else
            {
                return x.Name == y.Name
                && x.DataType == y.DataType
                && x.IsNullable == y.IsNullable
                && x.DefaultValue == y.DefaultValue
                && x.AutoIncrement == y.AutoIncrement
                && ((x.Comment == y.Comment) || (string.IsNullOrEmpty(x.Comment) || string.IsNullOrEmpty(y.Comment)));
            }
        }

        public int GetHashCode(ColumnDeclaration obj)
        {
            return obj.Name.GetHashCode() ^ obj.DataType.GetHashCode() ^ obj.IsNullable.GetHashCode() ^ obj.AutoIncrement.GetHashCode() ^ obj.OrdinalPosition.GetHashCode() ^ obj.Comment.GetHashCode();
        }

        #region Private Members

        private Compilation.Resolvers.SQLiteTypeResolver _typeResolver = new Compilation.Resolvers.SQLiteTypeResolver();

        #endregion Private Members
    }
}
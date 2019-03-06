using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Translators;
using System.Collections.Generic;

namespace Acklann.Daterpillar.Migration
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
                && x.DefaultValue == y.DefaultValue
                && x.AutoIncrement == y.AutoIncrement
                && ((x.Comment == y.Comment) || (string.IsNullOrEmpty(x.Comment) || string.IsNullOrEmpty(y.Comment)));
            }
        }

        public int GetHashCode(Column obj)
        {
            return obj.Name.GetHashCode() ^ obj.DataType.GetHashCode() ^ obj.IsNullable.GetHashCode() ^ obj.AutoIncrement.GetHashCode() ^ obj.OrdinalPosition.GetHashCode() ^ obj.Comment.GetHashCode();
        }

        #region Private Members

        private SQLiteTranslator _typeResolver = new SQLiteTranslator();

        #endregion Private Members
    }
}
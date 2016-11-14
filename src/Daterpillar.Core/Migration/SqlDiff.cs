using System;

namespace Gigobyte.Daterpillar.Migration
{
    public struct SqlDiff : IEquatable<SqlDiff>
    {
        #region Operators

        public static bool operator ==(SqlDiff left, SqlDiff right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SqlDiff left, SqlDiff right)
        {
            return !left.Equals(right);
        }

        #endregion Operators

        public int Changes { get; set; }

        public SqlDiffSummary Summary { get; set; }

        public bool Equals(SqlDiff other)
        {
            return Changes == other.Changes && Summary == other.Summary;
        }

        public override bool Equals(object obj)
        {
            if (obj is SqlDiff) return Equals(obj);
            else return false;
        }

        public override int GetHashCode()
        {
            return Changes.GetHashCode() ^ Summary.GetHashCode();
        }
    }
}
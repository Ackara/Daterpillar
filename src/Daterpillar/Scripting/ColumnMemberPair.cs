using System.Collections.Generic;
using System.Reflection;

namespace Acklann.Daterpillar.Scripting
{
    internal class ColumnMemberPair
    {
        public ColumnMemberPair(string columnName, MemberInfo member)
        {
            ColumnName = columnName;
            Member = member;
        }

        public MemberInfo Member { get; set; }

        public string MemberName
        {
            get => Member.Name;
        }

        public string ColumnName { get; set; }
    }

    internal class ColumnMemberPairEqualityComparer : IEqualityComparer<ColumnMemberPair>
    {
        public bool Equals(ColumnMemberPair x, ColumnMemberPair y) => string.Equals(x.MemberName, y.MemberName);

        public int GetHashCode(ColumnMemberPair obj) => obj.MemberName.GetHashCode();
    }
}
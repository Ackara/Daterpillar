using Acklann.Daterpillar.Modeling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Acklann.Daterpillar.Foo
{
    internal static class ColumnMap
    {
        public static void Register(Type recordType)
        {
            string tableName = recordType.GetTableName();

            if (_map.ContainsKey(tableName)) return;
            else _map.Add(tableName, tableName);

            IEnumerable<MemberInfo> members = from m in Modeling.Helper.GetColumns(recordType)
                                              let attr = m.GetCustomAttribute<Annotations.ColumnAttribute>()
                                              where (attr?.AutoIncrement ?? false) == false
                                              select m;

            foreach (MemberInfo member in members)
            {
                string qualifiedName = GetFullQualifiedName(tableName, member.GetColumnName());
                if (_map.ContainsKey(qualifiedName) == false)
                {
                    _map.Add(qualifiedName, member);
                }
            }

            _map.Add(GetFullQualifiedName(tableName, nameof(IInsertable.GetColumns)), (from p in members select p.GetColumnName()).ToArray());
            _map.Add(GetFullQualifiedName(tableName, nameof(IInsertable.GetValues)), (from x in members select x).ToArray());
        }

        public static MemberInfo GetMember(string tableName, string columnName)
        {
            return _map[GetFullQualifiedName(tableName, columnName)] as MemberInfo;
        }

        public static MemberInfo[] GetMembers(string tableName)
        {
            return (MemberInfo[])_map[GetFullQualifiedName(tableName, nameof(IInsertable.GetValues))];
        }

        public static string[] GetColumns(string tableName)
        {
            return (string[])_map[GetFullQualifiedName(tableName, nameof(IInsertable.GetColumns))];
        }

        public static string GetFullQualifiedName(string tableName, string columnName) => string.Concat(tableName, '.', columnName);

        private static readonly Hashtable _map = new Hashtable();
    }
}
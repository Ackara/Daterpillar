using Acklann.Daterpillar.Modeling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Acklann.Daterpillar.Scripting
{
    internal static class ColumnMap
    {
        public static void Register(Type recordType)
        {
            string tableName = recordType.GetTableName();

            if (_map.ContainsKey(tableName)) return;
            else _map.Add(tableName, tableName);

            IEnumerable<MemberInfo> members = (from m in TypeInfoExtensions.GetColumns(recordType)
                                               where m.IsAutoColumn() == false
                                               select m).Distinct();

            var columns = new List<string>();
            foreach (MemberInfo member in members)
            {
                foreach (string columnName in member.GetColumnNames())
                {
                    string qualifiedName = GetFullQualifiedName(tableName, columnName);
                    if (!_map.ContainsKey(qualifiedName))
                    {
                        _map.Add(qualifiedName, member);
                        columns.Add(columnName);
                    }
                }
            }

            _map.Add(GetFullQualifiedName(tableName, nameof(GetColumns)), columns.ToArray());
            _map.Add(GetFullQualifiedName(tableName, nameof(GetMembers)), members.ToArray());
        }

        public static MemberInfo GetMember(string tableName, string columnName)
        {
            return _map[GetFullQualifiedName(tableName, columnName)] as MemberInfo;
        }

        public static MemberInfo[] GetMembers(string tableName)
        {
            return (MemberInfo[])_map[GetFullQualifiedName(tableName, nameof(GetMembers))];
        }

        public static string[] GetColumns(string tableName)
        {
            return (string[])_map[GetFullQualifiedName(tableName, nameof(GetColumns))];
        }

        public static string GetFullQualifiedName(string tableName, string columnName) => string.Concat(tableName, '.', columnName);

        private static readonly Hashtable _map = new Hashtable();
    }
}
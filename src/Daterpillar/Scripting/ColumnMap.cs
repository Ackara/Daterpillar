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
                                                   //where m.IsAutoColumn() == false
                                               select m);

            var columns = new List<string>();
            var keys = new List<(string, MemberInfo)>();
            var nonKeys = new List<(string, MemberInfo)>();
            var all = new List<(string, MemberInfo)>();

            foreach (MemberInfo member in members)
            {
                foreach (string columnName in member.GetColumnNames())
                {
                    string key = CreateKey(tableName, columnName);
                    if (!_map.ContainsKey(key))
                    {
                        _map.Add(key, member);
                        columns.Add(columnName);
                    }

                    if (!member.IsAutoColumn())
                    {
                        all.Add((columnName, member));
                    }

                    if (member.IsKey())
                    {
                        keys.Add((columnName, member));
                    }
                    else
                    {
                        nonKeys.Add((columnName, member));
                    }
                }
            }

            _map.Add(CreateKey(tableName, nameof(GetColumnNames)), columns.ToArray());
            _map.Add(CreateKey(tableName, nameof(GetMembers)), members.ToArray());

            _map.Add(CreateKey(tableName, nameof(GetColumns)), all.ToArray());
            _map.Add(CreateKey(tableName, nameof(GetIdentityColumns)), keys.ToArray());
            _map.Add(CreateKey(tableName, nameof(GetNonIdentityColumns)), nonKeys.ToArray());
        }

        public static MemberInfo GetMember(string tableName, string columnName)
        {
            return _map[CreateKey(tableName, columnName)] as MemberInfo;
        }

        public static IEnumerable<(string, MemberInfo)> GetColumns(string tableName)
        {
            return (IEnumerable<(string, MemberInfo)>)_map[CreateKey(tableName, nameof(GetColumns))];
        }

        public static IEnumerable<(string, MemberInfo)> GetIdentityColumns(string tableName)
        {
            return (IEnumerable<(string, MemberInfo)>)_map[CreateKey(tableName, nameof(GetIdentityColumns))];
        }

        public static IEnumerable<(string, MemberInfo)> GetNonIdentityColumns(string tableName)
        {
            return (IEnumerable<(string, MemberInfo)>)_map[CreateKey(tableName, nameof(GetNonIdentityColumns))];
        }

        public static MemberInfo[] GetMembers(string tableName)
        {
            return (MemberInfo[])_map[CreateKey(tableName, nameof(GetMembers))];
        }

        public static string[] GetColumnNames(string tableName)
        {
            return (string[])_map[CreateKey(tableName, nameof(GetColumnNames))];
        }

        public static string CreateKey(string tableName, string columnName) => string.Concat(tableName, '.', columnName);

        private static readonly Hashtable _map = new Hashtable();
    }
}
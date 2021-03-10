using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acklann.Daterpillar.Modeling
{
    internal class ColumnMap
    {
        public static void Register(Type recordType)
        {
            string tableName = recordType.GetCustomAttribute<Attributes.TableAttribute>()?.Name;
            if (string.IsNullOrEmpty(tableName)) { System.Diagnostics.Debug.WriteLine($"'{recordType.FullName}' do not have a {nameof(Attributes.TableAttribute)}"); return; }

            if (_map.ContainsKey(tableName)) return;
            else _map.Add(tableName, null);



            foreach (PropertyInfo prop in recordType.GetColumns())
            {
                string qualifiedName = string.Concat(tableName, '.');
                if (_map.ContainsKey(qualifiedName) == false)
                {
                    _map.Add(qualifiedName, prop);
                }
            }
        }

        private static readonly IDictionary<string, PropertyInfo> _map = new Dictionary<string, PropertyInfo>();
    }
}
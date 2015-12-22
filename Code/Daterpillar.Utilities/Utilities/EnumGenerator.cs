using Gigobyte.Daterpillar.Transformation;
using System;
using System.Collections.Generic;
using System.Data;

namespace Gigobyte.Daterpillar.Utilities
{
    public sealed class EnumGenerator : IDisposable
    {
        public EnumGenerator(IDbConnection connection)
        {
            _connection = connection;
        }

        public Enumeration FetchEnumeration(string table, string nameColumn, string valueColumn)
        {
            var enumeration = new Enumeration() { Name = table };
            enumeration.Values = new List<KeyValuePair<string, int>>();

            if (_connection.State != ConnectionState.Open) _connection.Open();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = $"SELECT {nameColumn}, {valueColumn} FROM {table};";
                using (var results = new DataTable())
                {
                    results.Load(command.ExecuteReader());
                    foreach (DataRow row in results.Rows)
                    {
                        string name = Convert.ToString(row[nameColumn]);
                        int value = Convert.ToInt32(row[valueColumn]);

                        enumeration.Values.Add(new KeyValuePair<string, int>(name, value));
                    }
                }
            }

            return enumeration;
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
            }
        }

        #region Private Members

        private IDbConnection _connection;

        #endregion Private Members
    }
}
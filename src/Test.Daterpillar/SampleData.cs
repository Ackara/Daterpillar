using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.TextTransformation;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Tests.Daterpillar.Constants;

namespace Tests.Daterpillar
{
    public static partial class SampleData
    {
        public static FileInfo GetFile(string filename)
        {
            filename = Path.GetFileName(filename);
            string ext = "*" + Path.GetExtension(filename);
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            return new DirectoryInfo(baseDirectory).GetFiles(ext, SearchOption.AllDirectories)
                .First(x => x.Name == filename);
        }

        public static Table CreateTable([CallerMemberName]string name = null)
        {
            return new Table(name);
        }

        public static Table CreateTableSchema(string name = "Employee")
        {
            var table = new Table() { Name = name };

            // Define columns
            var id = new Column();
            id.Name = "Id";
            id.AutoIncrement = true;
            id.DataType = new DataType() { Name = "int" };

            var fullName = new Column();
            fullName.Name = "Full_Name";
            fullName.Comment = "The first name column.";
            fullName.DataType = new DataType() { Name = "varchar", Scale = 64 };
            fullName.Modifiers = new List<string>(new string[] { "default 'n/a'" });

            var salary = new Column();
            salary.Name = "Salary";
            salary.Comment = "The salary column";
            salary.DataType = new DataType() { Name = "decimal", Scale = 12, Precision = 2 };

            // Define foreign keys
            var fKey = new ForeignKey();
            fKey.Name = "fkey1";
            fKey.LocalColumn = "Id";
            fKey.ForeignTable = "Card";
            fKey.ForeignColumn = "Id";
            fKey.OnUpdate = ForeignKeyRule.SET_NULL;
            fKey.OnDelete = ForeignKeyRule.CASCADE;

            // Define index
            var pKey = new Index();
            pKey.Table = table.Name;
            pKey.Type = IndexType.PrimaryKey;
            pKey.Columns = new List<IndexColumn>() { new IndexColumn() { Name = "Id" } };

            var idx1 = new Index();
            idx1.Name = $"{name}_idx".ToLower();
            idx1.Type = IndexType.Index;
            idx1.Unique = true;
            idx1.Table = table.Name;
            idx1.Columns = new List<IndexColumn>() { new IndexColumn() { Name = fullName.Name, Order = SortOrder.DESC } };

            // Put it all together
            table.Columns = new List<Column>() { id, fullName, salary };
            table.ForeignKeys = new List<ForeignKey>() { fKey };
            table.Indexes = new List<Index>() { pKey, idx1 };

            return table;
        }

        public static Schema CreateSchema([CallerMemberName]string name = null)
        {
            var schema = new Schema();
            schema.Name = name;
            schema.Author = "johnDoe@example.com";
            schema.Description = "This is a useful description";
            schema.CreatedOn = new DateTime(2016, 9, 9);

            var employeeTable = CreateTableSchema();
            schema.Tables.Add(employeeTable);
            schema.Script = "-- Script Goes Here";

            return schema;
        }

        public static Schema CreateMockSchema(string filename = KnownFile.MockSchemaXML)
        {
            return Schema.Load(GetFile(filename).OpenRead());
        }

        #region Database Methods

        public static bool TryCreateDatabase(IDbConnection connection, ITemplate template)
        {
            var schema = Schema.Load(GetFile(KnownFile.MockSchemaXML).OpenRead());
            return TryCreateDatabase(connection, schema, template);
        }

        public static bool TryCreateDatabase(IDbConnection connection, Schema schema, ITemplate template)
        {
            try
            {
                using (connection)
                {
                    if (connection.State != ConnectionState.Open) connection.Open();

                    schema.Name = connection.Database;
                    TryTruncateDatabase(connection, schema, false);
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = template.Transform(schema);
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (System.Data.Common.DbException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.ErrorCode}");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool TryTruncateDatabase(IDbConnection connection, Schema schema, bool shouldDisposeConnection = true)
        {
            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();
                using (var command = connection.CreateCommand())
                {
                    for (int i = schema.Tables.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            command.CommandText = $"DROP TABLE {schema.Tables[i].Name};";
                            command.ExecuteNonQuery();
                        }
                        catch { }
                    }
                }
                return true;
            }
            catch (System.Data.Common.DbException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.ErrorCode}");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                if (shouldDisposeConnection) connection?.Dispose();
            }
        }

        #endregion Database Methods
    }
}
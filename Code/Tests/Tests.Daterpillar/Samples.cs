using Ackara.Daterpillar.Transformation;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Tests.Daterpillar
{
    public static class Samples
    {
        public static FileInfo GetFile(string filename)
        {
            throw new System.NotImplementedException();
        }

        public static Schema GetSchema([CallerMemberName]string name = null)
        {
            var schema = new Schema();
            schema.Name = name ?? "SchemaName";
            schema.Author = "johnDoe@example.com";

            // Define tables
            var employee = new Table();
            employee.Name = "Employee";

            // Define columns
            var id = new Column();
            id.Name = "Id";
            id.Comment = "The id column.";
            id.DataType = new DataType() { Name = "int" };
            id.Modifiers = new List<string>(new string[] { "NOT NULL", "PRIMARY KEY AUTO_INCREMENT" });

            var fullName = new Column();
            fullName.Name = "Full_Name";
            fullName.Comment = "The first name column.";
            fullName.DataType = new DataType() { Name = "VARCHAR", Scale = 64 };
            fullName.Modifiers = new List<string>(new string[] { "not null", "default 'n/a'" });

            var salary = new Column();
            salary.Name = "Salary";
            salary.Comment = "The salary column";
            salary.DataType = new DataType() { Name = "decimal", Scale = 12, Precision = 2 };
            salary.Modifiers = new List<string>(new string[] { "not null" });

            // Define foreign keys
            var fKey = new ForeignKey();
            fKey.Name = "fkey1";
            fKey.LocalColumn = "Id";
            fKey.ForeignTable = "Card";
            fKey.ForeignColumn = "Id";
            fKey.OnUpdate = ForeignKeyRule.SetNull;
            fKey.OnDelete = ForeignKeyRule.Cascade;

            // Define index
            var pKey = new Index();
            pKey.Table = employee.Name;
            pKey.Type = "primaryKey";
            pKey.Columns = new List<IndexColumn>() { new IndexColumn() { Name = "Id" } };

            var idx1 = new Index();
            idx1.Name = "index1";
            idx1.Type = "index";
            idx1.Unique = true;
            idx1.Table = employee.Name;
            idx1.Columns = new List<IndexColumn>() { new IndexColumn() { Name = fullName.Name, Order = SortOrder.DESC } };

            // Put it all together
            employee.Columns = new List<Column>() { id, fullName, salary };
            employee.ForeignKeys = new List<ForeignKey>() { fKey };
            employee.Indexes = new List<Index>() { pKey, idx1 };

            schema.Tables.Add(employee);

            return schema;
        }
    }
}
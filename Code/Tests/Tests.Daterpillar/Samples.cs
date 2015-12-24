using Gigobyte.Daterpillar.Transformation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Tests.Daterpillar.Sample;

namespace Tests.Daterpillar
{
    public static class Samples
    {
        public static FileInfo GetFile(string filename)
        {
            string ext = "*" + Path.GetExtension(filename);
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            return new DirectoryInfo(baseDirectory).GetFiles(ext, SearchOption.AllDirectories)
                .First(x => x.Name == filename);
        }

        public static string GetFileContent(string filename)
        {
            return File.ReadAllText(GetFile(filename).FullName);
        }

        public static Song GetSong([CallerMemberName]string name = null)
        {
            return new Song()
            {
                Id = 100,
                Name = name,
                Length = 12,
                Price = 1.29M,
                AlbumId = 1,
                ArtistId = 22,
                GenreId = 1
            };
        }

        public static Schema GetSchema([CallerMemberName]string name = null)
        {
            var schema = new Schema();
            schema.Name = name;
            schema.Author = "johnDoe@example.com";

            var employeeTable = GetTableSchema();
            schema.Tables.Add(employeeTable);

            return schema;
        }

        public static Table GetTableSchema(string name = "Employee")
        {
            var table = new Table() { Name = name };

            // Define columns
            var id = new Column();
            id.Name = "Id";
            id.AutoIncrement = true;
            id.DataType = new DataType() { Name = "int" };
            id.Modifiers = new List<string>(new string[] { "NOT NULL" });

            var fullName = new Column();
            fullName.Name = "Full_Name";
            fullName.Comment = "The first name column.";
            fullName.DataType = new DataType() { Name = "varchar", Scale = 64 };
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
            fKey.OnUpdateRule = ForeignKeyRule.SET_NULL;
            fKey.OnDeleteRule = ForeignKeyRule.CASCADE;

            // Define index
            var pKey = new Index();
            pKey.Table = table.Name;
            pKey.Type = "primaryKey";
            pKey.Columns = new List<IndexColumn>() { new IndexColumn() { Name = "Id" } };

            var idx1 = new Index();
            idx1.Name = $"{name}_idx".ToLower();
            idx1.Type = "index";
            idx1.Unique = true;
            idx1.Table = table.Name;
            idx1.Columns = new List<IndexColumn>() { new IndexColumn() { Name = fullName.Name, Order = SortOrder.DESC } };

            // Put it all together
            table.Columns = new List<Column>() { id, fullName, salary };
            table.ForeignKeys = new List<ForeignKey>() { fKey };
            table.Indexes = new List<Index>() { pKey, idx1 };

            return table;
        }
    }
}
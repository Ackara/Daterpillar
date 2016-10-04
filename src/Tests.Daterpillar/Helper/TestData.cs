using Gigobyte.Daterpillar;
using System;
using System.IO;
using System.Linq;

namespace Tests.Daterpillar.Helper
{
    public static class TestData
    {
        public static FileInfo GetFile(string filename)
        {
            filename = Path.GetFileName(filename);
            string ext = "*" + Path.GetExtension(filename);
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            return new DirectoryInfo(baseDirectory).GetFiles(ext, SearchOption.AllDirectories)
                .First(x => x.Name == filename);
        }

        public static Table CreateColumn(string name)
        {
            return new Table()
            {
                Name = name,
                Comment = "This is a new table"
            };
        }

        public static Column CreateColumn(string name, string tableName = "tb1")
        {
            return CreateColumn(name, new DataType("varchar", 32, 0), tableName);
        }

        public static Column CreateColumn(string name, DataType type, string tableName)
        {
            return new Column()
            {
                Name = name,
                DataType = type,
                IsNullable = false,
                TableRef = new Table(tableName)
            };
        }

        public static Index CreateIndex(string name, string tableName = "tb1", params IndexColumn[] columns)
        {
            return new Index()
            {
                TableRef = new Table(tableName),

                Name = name,
                Unique = false,
                Table = tableName,
                IndexType = IndexType.Index,
                Columns = new System.Collections.Generic.List<IndexColumn>(columns)
            };
        }

        public static ForeignKey CreateForeignKey(string name)
        {
            return new ForeignKey()
            {
            };
        }
    }
}
using Ackara.Daterpillar;
using System;
using System.IO;
using System.Linq;

namespace MSTest.Daterpillar
{
    public partial class MockData
    {
        public const string Samples = "Samples";
        public const string daterpillar = "daterpillar.xsd";

        public static Schema CreateSchema()
        {
            var schema = new Schema();
            schema.Add(
                new Table("card_type",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("varchar", 64))),

                new Table("card",
                    new Column("Id", new DataType("int")),
                    new Column("Name", new DataType("varchar", 64)),
                    new Column("Text", new DataType("text")),
                    new Column("Level", new DataType("int")),
                    new Column("Card_Type_Id", new DataType("int")),

                    new Index(true, IndexType.Index, new ColumnName("Name")),
                    new Index(IndexType.Index, new ColumnName("Level")),
                    new Index(IndexType.Index, new ColumnName("Card_Type_Id")),

                    new ForeignKey("Card_Type_Id", "card_type", "Id", onUpdate: ReferentialAction.Cascade, onDelete: ReferentialAction.Restrict)),

                new Script("-- header"),
                new Script("-- seed data", Syntax.SQLite, "seed"));

            return schema;
        }

        public static FileInfo GetFile(string relativePath, string pattern = "*")
        {
            var deploymentDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            var target = (
                from file in (deploymentDir.GetFiles(pattern, SearchOption.AllDirectories))
                where file.FullName.EndsWith(relativePath)
                select file).First();

            return target;
        }

        public static string GetFilePath(string relativePath, string pattern = "*")
        {
            return GetFile(relativePath, pattern).FullName;
        }
    }
}
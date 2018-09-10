using Acklann.Daterpillar;
using System;
using System.IO;
using System.Linq;

namespace MSTest.Daterpillar
{
    public partial class MockData
    {
        public static Schema GetSchema()
        {
            var schema = new Schema();
            schema.Add(
                new Table("card_type",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("varchar", 64))),

                new Table("attribute",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("varchar", 64))),

                new Table("monster_type",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("varchar", 64))),

                new Table("card",
                    new Column("Id", new DataType("int")),
                    new Column("Name", new DataType("varchar", 256)),
                    new Column("Text", new DataType("text")),
                    new Column("Pendulum_Text", new DataType("text"), autoIncrement: false, nullable: true),
                    new Column("Attribute_Id", new DataType("int")),
                    new Column("Card_Type_Id", new DataType("int")),
                    new Column("Monster_Type_Id", new DataType("int")),

                    new ForeignKey("Attribute_Id", "attribute", "Id", ReferentialAction.Cascade, ReferentialAction.Restrict),
                    new ForeignKey("Card_Type_Id", "card_type", "Id", ReferentialAction.Cascade, ReferentialAction.Restrict),
                    new ForeignKey("Monster_Type_Id", "monster_type", "Id", ReferentialAction.Cascade, ReferentialAction.NoAction),

                    new Index(IndexType.PrimaryKey, new ColumnName("Id")),
                    new Index(IndexType.Index, true, new ColumnName("Name", Order.Descending)),
                    new Index(IndexType.Index, true, new ColumnName("Text"), new ColumnName("Pendulum_Text"))),

                new Script("-- header"),
                new Script("-- seed data", Syntax.SQLite, "seed"));

            return schema;
        }

        public static Schema GetSchema(string relativePath)
        {
            var file = GetFile(relativePath, "*.xml");
            return Schema.Load(file.OpenRead());
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
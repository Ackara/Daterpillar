using Acklann.Daterpillar;
using Acklann.Daterpillar.Scripting;
using ApprovalTests;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(FName.X86)]
    [DeploymentItem(FName.Samples)]
    [DeploymentItem(FName.daterpillarXSD)]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class SchemaTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void RemoveTable_should_remove_a_specified_table_from_a_schema_when_a_name_is_passed()
        {
            // Arrange
            var sut = MockData.GetSchema();

            // Act
            int totalTablesBefore = sut.Tables.Count;
            sut.RemoveTable("card");
            int totalTables = sut.Tables.Count;

            // Assert
            totalTables.ShouldBe((totalTablesBefore - 1));
        }

        [TestMethod]
        public void Save_should_serialize_a_schema_object_when_invoked()
        {
            // Arrange
            Schema sut = MockData.GetSchema();

            // Act
            bool documentIsValid;
            string xml = "", errorMsg;

            using (var memoryStream = new MemoryStream())
            {
                sut.Save(memoryStream);
                documentIsValid = Helper.ValidateXml(memoryStream, out errorMsg);
                xml = Encoding.UTF8.GetString(memoryStream.GetBuffer());
            }

            // Assert
            sut.Tables.ShouldNotBeNull();
            xml.ShouldNotBeNullOrEmpty();
            documentIsValid.ShouldBeTrue(errorMsg);
            //Approvals.VerifyXml(xml);
        }

        [TestMethod]
        public void Load_should_deserialize_a_schema_object_when_invoked()
        {
            // Arrange
            var file = MockData.GetFile(FName.schemaTest_mock_schema1XML);

            // Act
            var sut = Schema.Load(file.OpenRead());

            // Assert
            sut.Tables.ShouldNotBeEmpty();
            sut.Scripts.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void Merge_should_append_the_sql_objects_of_another_schema_when_invoked()
        {
            // Arrange
            var deployDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            var fragments = from f in deployDir.GetFiles("*.xml", SearchOption.AllDirectories)
                            where f.Name.StartsWith("fragment")
                            select Schema.Load(f.OpenRead());

            var sut = new Schema();
            sut.Add(
                new Table("user",
                new Column("Id", new DataType("int"), autoIncrement: true),
                new Column("Username", new DataType("varchar", 64)),
                new Column("PasswordHash", new DataType("text")),
                new Column("Email", new DataType("varchar", 128))));

            // Act
            sut.Merge(fragments.ToArray());
            var xml = sut.ToXml();

            // Assert
            sut.Tables.Count.ShouldBe(4);
            sut.Scripts.Count.ShouldBe(1);
            Approvals.VerifyXml(xml);
        }

        [TestMethod]
        public void Sort_should_rearrange_a_schema_tables_so_referenced_tables_precede_their_dependants_when_invoked()
        {
            // Arrange
            string script, errorMsg;
            bool scriptWasExecutedSuccessfully;
            var testCases = new TestCase[]
            {
                Case0(),
                Case1(),
                Case2(),
                Case3(),
                Case4()
            };

            // Act
            foreach (var sample in testCases)
            {
                System.Diagnostics.Debug.WriteLine($"******************** {sample.Name} ********************");
                System.Diagnostics.Debug.WriteLine($"input   : [{string.Join(", ", sample.Schema.Tables.Select(x => x.Name))}]");
                System.Diagnostics.Debug.WriteLine($"expected: [{string.Join(", ", sample.Expected)}]");
                System.Diagnostics.Debug.WriteLine("----------");

                sample.Schema.Sort();
                var results = sample.Schema.Tables.Select(x => x.Name).ToArray();

                using (var connection = ConnectionFactory.CreateMySQLConnection("dtpl_sort"))
                {
                    connection.UseEmptyDatabase();
                    script = new MySQLScriptBuilder().Append(sample.Schema).GetContent();
                    scriptWasExecutedSuccessfully = connection.TryExecuteScript(script, out errorMsg);
                    if (scriptWasExecutedSuccessfully == false) System.Diagnostics.Debug.WriteLine(script);
                }

                // Assert
                results.ShouldBe(sample.Expected);
                scriptWasExecutedSuccessfully.ShouldBeTrue(errorMsg);
            }
        }

        #region Sorting Cases

        private static TestCase Case0()
        {
            return new TestCase() { Name = nameof(Case0), Schema = new Schema(), Expected = new string[0] };
        }

        private static TestCase Case1()
        {
            var expected = new string[] { "a" };

            var schema = new Schema();
            schema.Add(
                new Table("a",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("int"))
                    ));

            return new TestCase() { Name = nameof(Case1), Schema = schema, Expected = expected };
        }

        private static TestCase Case2()
        {
            var expected = new string[] { "f", "e", "d", "a", "b", "c" };

            var schema = new Schema();
            schema.Add(
                new Table("a",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("LinkD", new DataType("int")),
                    new Column("LinkE", new DataType("int")),
                    new Column("LinkF", new DataType("int")),
                    new ForeignKey("LinkE", "e", "Id"),
                    new ForeignKey("LinkF", "f", "Id"),
                    new ForeignKey("LinkD", "d", "Id")
                    ),

                new Table("b",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("LinkD", new DataType("int")),
                    new ForeignKey("LinkD", "d", "Id")
                    ),

                new Table("c",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("LinkF", new DataType("int")),
                    new ForeignKey("LinkF", "f", "Id")
                    ),

                new Table("d",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("varchar", 64))
                    ),

                new Table("e",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("LinkF", new DataType("int")),
                    new ForeignKey("LinkF", "f", "Id")
                    ),

                new Table("f",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("varchar", 64))
                    ));

            return new TestCase() { Name = nameof(Case2), Schema = schema, Expected = expected };
        }

        private static TestCase Case3()
        {
            var expected = new string[] { "ab", "at", "ct", "i", "mt", "l", "c", "r", "pt", "p", "cn" };

            var schema = new Schema();
            schema.Add(
                new Table("ab",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("varchar"))),

                new Table("at",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("varchar"))),

                new Table("c",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("_ab", new DataType("int")),
                    new Column("_at", new DataType("int")),
                    new Column("_ct", new DataType("int")),
                    new Column("_i", new DataType("int")),
                    new Column("_mt", new DataType("int")),
                    new Column("_l", new DataType("int")),
                    new ForeignKey("_ct", "ct", "Id"),
                    new ForeignKey("_ab", "ab", "Id"),
                    new ForeignKey("_at", "at", "Id"),
                    new ForeignKey("_i", "i", "Id"),
                    new ForeignKey("_mt", "mt", "Id"),
                    new ForeignKey("_l", "l", "Id")),

                new Table("cn",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("int")),
                    new Column("_r", new DataType("int")),
                    new Column("_c", new DataType("int")),
                    new Column("_p", new DataType("int")),
                    new ForeignKey("_r", "r", "Id"),
                    new ForeignKey("_c", "c", "Id"),
                    new ForeignKey("_p", "p", "Id")
                    ),

                new Table("ct",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("int"))),

                new Table("i",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("int"))),

                new Table("l",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("int"))),

                new Table("mt",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("int"))),

                new Table("p",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("int")),
                    new Column("_pt", new DataType("int")),
                    new ForeignKey("_pt", "pt", "Id")),

                new Table("pt",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("int"))),

                new Table("r",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("int"))));

            return new TestCase()
            {
                Name = nameof(Case3),
                Schema = schema,
                Expected = expected
            };
        }

        private static TestCase Case4()
        {
            var expected = new string[]
            {
                "ability",
                "attribute",
                "card_type",
                "icon",
                "monster_type",
                "legality",
                "card",
                "product_type",
                "pack",
                "rarity",
                "card_number",
                "link",
            };
            return new TestCase()
            {
                Expected = expected,
                Name = nameof(Case4),
                Schema = MockData.GetSchema(FName.schemaTest_mock_schema2XML)
            };
        }

        private struct TestCase { public string Name; public Schema Schema; public string[] Expected; }

        #endregion Sorting Cases
    }
}
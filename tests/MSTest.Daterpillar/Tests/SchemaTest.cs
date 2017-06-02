﻿using Acklann.Daterpillar;
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
            var testCases = new TestCase[] { Case0(), Case1(), Case2() };

            // Act
            foreach (var sample in testCases)
            {
                System.Diagnostics.Debug.WriteLine($"******************** {sample.Name} ********************");

                sample.Schema.Sort();
                var results = sample.Schema.Tables.Select(x => x.Name).ToArray();

                using (var connection = ConnectionFactory.CreateMySQLConnection("dtpl_sort"))
                {
                    connection.UseEmptyDatabase();
                    script = new MySQLScriptBuilder().Append(sample.Schema).GetContent();
                    scriptWasExecutedSuccessfully = connection.TryExecuteScript(script, out errorMsg);
                    //System.Diagnostics.Debug.WriteLine(script);
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

        private struct TestCase { public string Name; public Schema Schema; public string[] Expected; }

        #endregion Sorting Cases
    }
}
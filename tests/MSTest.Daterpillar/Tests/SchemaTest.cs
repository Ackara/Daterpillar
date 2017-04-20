using Ackara.Daterpillar;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.IO;
using System.Linq;
using System.Text;
using static MSTest.Daterpillar.MockData;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(FileLauncherReporter), typeof(ClipboardReporter))]
    public class SchemaTest
    {
        [TestMethod]
        public void Save_should_serialize_a_schema_object_when_invoked()
        {
            // Arrange
            Schema sut = CreateSchema();

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
            var file = GetFile(schemaTest_mock_schema1XML);

            // Act
            var sut = Schema.Load(file.OpenRead());

            // Assert
            sut.Tables.ShouldNotBeEmpty();
            sut.Scripts.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void Load_should_deserialize_and_combine_all_specified_documents_passed_as_one_schema_object()
        {
            // Arrange
            var samplesDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Samples));
            var sampleFiles = (
                from file in (samplesDir.GetFiles("*fragment*.xml", SearchOption.AllDirectories))
                select file.FullName).ToArray();

            // Act
            var sut = Schema.Load(sampleFiles);

            string xml = "";
            using (var memory = new MemoryStream())
            {
                sut.Save(memory);
                xml = Encoding.UTF8.GetString(memory.GetBuffer());
            }

            // Assert
            xml.ShouldNotBeNullOrWhiteSpace();
            Approvals.VerifyXml(xml);
        }

        [TestMethod]
        public void Join_should_concatenate_the_sql_objects_of_another_schema_object_when_invoked()
        {
            // Arrange
            var schema1 = new Schema();
            schema1.Add(
                new Table("card_info",
                new Column("Id", new DataType("int"), autoIncrement: true),
                new Column("Number", new DataType("int")),
                new Column("ExpDate", new DataType("date")),
                new Column("CVV", new DataType("int"))));

            var schema2 = new Schema();
            schema2.Add(
                new Table("Customer",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("UserId", new DataType("varchar", 64)),
                    new Column("Password", new DataType("varchar", 64)),
                    new Column("Card_Id", new DataType("int"))),

                new Script("-- header"),
                new Script("-- footer"));

            // Act
            schema1.Join(schema2);

            // Assert
            schema1.Tables.Count.ShouldBe(2);
            schema1.Scripts.Count.ShouldBe(2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Join_should_throw_an_exception_when_two_sql_objects_has_the_same_name()
        {
            // Arrange
            var schema1 = new Schema();
            schema1.Add(
                new Table("Category",
                new Column("Id", new DataType("int"), autoIncrement: true),
                new Column("Name", new DataType("varchar", 64))));

            var schema2 = new Schema();
            schema2.Add(
                new Table("Category",
                new Column("Id", new DataType("int"), autoIncrement: true),
                new Column("Value", new DataType("varchar", 64))));

            // Act
            schema1.Join(schema2);
        }
    }
}
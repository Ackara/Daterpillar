using Ackara.Daterpillar;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.IO;
using System.Text;
using static MSTest.Daterpillar.MockData;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(Samples)]
    [DeploymentItem(daterpillarXSD)]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(FileLauncherReporter), typeof(ClipboardReporter))]
    public class SchemaTest
    {
        [TestMethod]
        public void Save_should_serialize_a_schema_object_when_invoked()
        {
            // Arrange
            Schema sut = GetMockSchema();

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
        public void Join_should_append_the_sql_objects_of_another_schema_when_invoked()
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
    }
}
using Gigobyte.Daterpillar.Transformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [DeploymentItem(Artifact.XDDL)]
    [DeploymentItem(SampleData.MusicXddlXML)]
    public class SchemaTest
    {
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void WriteTo_should_serialize_its_schema_object_into_a_stream()
        {
            // Arrange
            Schema sut = SampleData.CreateSchema();
            using (var stream = new MemoryStream())
            {
                // Act
                sut.WriteTo(stream);

                var validator = new XmlValidator();
                validator.Load(stream);

                // Assert
                Assert.IsTrue(validator.XmlDocIsValid, validator.GetErrorLog());
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void Assert_that_the_schema_object_can_be_deserialized_by_the_xml_serializer()
        {
            // Arrange
            var schemaFile = SampleData.GetFile(SampleData.MusicXddlXML);

            // Act
            using (var stream = schemaFile.OpenRead())
            {
                var obj = Schema.Load(stream);

                // Assert
                Assert.IsNotNull(obj);
                Assert.IsNotNull(obj.Tables);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void Parse_should_return_a_schema_object_from_a_xml_formatted_string()
        {
            // Arrange
            var schemaFile = SampleData.GetFile(SampleData.MusicXddlXML);

            // Act
            var obj = Schema.Parse(File.ReadAllText(schemaFile.FullName));

            // Assert
            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.Tables);
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void RemoveTable_should_delete_a_table_object_from_a_schema_object_when_a_valid_name_is_given()
        {
            // Arrange
            var schema = SampleData.CreateSchema();
            var nameOfTableToRemove = "RemoveMe";
            var tableToRemove = SampleData.CreateTableSchema(nameOfTableToRemove);

            schema.Tables.Add(tableToRemove);

            // Act
            schema.RemoveTable(nameOfTableToRemove);

            // Assert
            Assert.IsNull(schema.Tables.FirstOrDefault(x => x.Name == nameOfTableToRemove));
        }
    }
}
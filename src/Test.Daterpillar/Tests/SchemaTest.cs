using Acklann.Daterpillar;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Tests.Daterpillar.Constants;
using Tests.Daterpillar.Helpers;

namespace Tests.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(KnownFile.XSD)]
    [DeploymentItem(DDT.Samples)]
    public class SchemaTest
    {
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void WriteTo_should_serialize_a_schema_object_into_a_stream()
        {
            // Arrange
            var sut = new Schema()
            {
            };

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
        public void Load_should_deserialize_a_schema_object_when_xml_data_is_passed()
        {
            // Arrange
            var schemaFile = TestData.GetFile(KnownFile.MockSchema1XML);

            // Act
            var obj = Schema.Load(File.ReadAllText(schemaFile.FullName));

            // Assert
            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.Tables);
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void Join_should_append_the_child_item_of_another_schema_to_its_designated_parent_when_invoked()
        {
            // Arrange
            var sut = new Schema();
            sut.CreateTable("tbl1");

            var otherSchema = new Schema();
            otherSchema.CreateTable("tbl2");
            otherSchema.CreateTable("tbl3");
            otherSchema.Script = "-- a script";

            // Act
            sut.Join(otherSchema);

            var numberOfTables = sut.Tables.Count;

            // Assert
            Assert.AreEqual(3, numberOfTables);
            Assert.IsFalse(string.IsNullOrEmpty(sut.Script), "The 'Script' property was not set.");
        }
    }
}
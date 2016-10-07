using Gigobyte.Daterpillar;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Tests.Daterpillar.Globals;
using Tests.Daterpillar.Helper;

namespace Tests.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(Test.File.XDDL)]
    [DeploymentItem(Test.Data.Samples)]
    public class SchemaTest
    {
        [TestMethod]
        [Owner(Test.Dev.Ackara)]
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
        [Owner(Test.Dev.Ackara)]
        public void Load_should_deserialize_a_schema_object_when_xml_data_is_passed()
        {
            // Arrange
            var schemaFile = TestData.GetFile(KnownFile.MockSchemaXML);

            // Act
            var obj = Schema.Load(File.ReadAllText(schemaFile.FullName));

            // Assert
            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.Tables);
        }
    }
}
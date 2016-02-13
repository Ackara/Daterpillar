using Gigobyte.Daterpillar.Transformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [DeploymentItem(Artifact.XDDL)]
    [DeploymentItem(Artifact.SamplesFolder + Artifact.SampleSchema)]
    public class SchemaTest
    {
        /// <summary>
        /// Validate <see cref="Schema.WriteTo(Stream)"/> serialize the <see cref="Schema"/> object
        /// into a <see cref="Artifact.XDDL"/> xml document.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void SerializeSchemaObjectToStream()
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

        /// <summary>
        /// Assert <see cref="Schema"/> can be deserialized by the <see
        /// cref="System.Xml.Serialization.XmlSerializer"/> using a <see cref="Stream"/>.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void DeserializeSchemaObjectFromStream()
        {
            // Arrange
            var schemaFile = SampleData.GetFile(Artifact.SampleSchema);

            // Act
            using (var stream = schemaFile.OpenRead())
            {
                var obj = Schema.Load(stream);

                // Assert
                Assert.IsNotNull(obj);
                Assert.IsNotNull(obj.Tables);
            }
        }

        /// <summary>
        /// Assert <see cref="Schema"/> can be deserialized by the <see
        /// cref="System.Xml.Serialization.XmlSerializer"/> with a string.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void DeserializeSchemaObjectFromString()
        {
            // Arrange
            var schemaFile = SampleData.GetFile(Artifact.SampleSchema);

            // Act
            var obj = Schema.Parse(File.ReadAllText(schemaFile.FullName));

            // Assert
            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.Tables);
        }

        /// <summary>
        /// Assert <see cref="Schema.RemoveTable(string)"/> can find and remove an existing <see cref="Table"/> object from an instance.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void RemoveTableFromSchema()
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
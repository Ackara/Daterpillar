using Acklann.Diffa;
using Acklann.Diffa.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.IO;
using Schema = Acklann.Daterpillar.Configuration.Schema;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class SerializationTest
    {
        [TestMethod]
        public void Can_deserialize_a_schema_xml_file()
        {
            // Arrange
            bool result1, result2;

            // Act
            result1 = Schema.TryLoad(TestData.GetMusicXML().FullName, out Schema schema1, out string errorMsg);
            result2 = Schema.TryLoad(TestData.GetBad_SchemaXML().FullName, out Schema schema2, out errorMsg);

            // Assert
            result1.ShouldBeTrue(errorMsg);
            result2.ShouldBeFalse(errorMsg);

            Diff.Approve(schema1, ".xml");
        }

        [TestMethod]
        [Use(typeof(FileReporter))]
        public void Can_serialize_a_schema_object()
        {
            // Arrange
            string xml = null;
            var resultFile = Path.Combine(Path.GetTempPath(), $"{nameof(Daterpillar)}-test.xml");
            var xsd = TestData.GetFile($"{nameof(Daterpillar)}.xsd".ToLowerInvariant()).FullName;

            // Act
            if (Schema.TryLoad(TestData.GetMusicXML().FullName, out Schema schema, out string errorMsg))
            {
                schema.Save(resultFile);
                xml = File.ReadAllText(resultFile);
            }
            else Assert.Fail(errorMsg);

            // Assert
            Diff.ApproveXml(xml, xsd, Schema.XMLNS);
        }
    }
}
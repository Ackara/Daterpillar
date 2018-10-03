using Acklann.Diffa;
using Acklann.Diffa.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.IO;
using System.Text;
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
            var error = new StringBuilder();
            bool result1, result2;
            void handler(object s, System.Xml.Schema.ValidationEventArgs e)
            {
                error.AppendLine($"[{e.Severity}]  {e.Message}");
            }

            // Act
            result1 = Schema.TryLoad(TestData.GetMusicXML().FullName, out Schema schema1, handler);
            result2 = Schema.TryLoad(TestData.GetBad_SchemaXML().FullName, out Schema schema2, handler);

            // Assert
            result1.ShouldBeTrue(error.ToString());
            result2.ShouldBeFalse(error.ToString());

            Diff.Approve(schema1);
        }

        [TestMethod]
        [Use(typeof(FileReporter))]
        public void Can_serialize_a_schema_object()
        {
            // Arrange
            string xml = null;
            var error = new StringBuilder();
            var resultFile = Path.Combine(Path.GetTempPath(), $"{nameof(Daterpillar)}-test.xml");
            var xsd = TestData.GetFile($"{nameof(Daterpillar)}.xsd".ToLowerInvariant()).FullName;
            void handler(object s, System.Xml.Schema.ValidationEventArgs e)
            {
                error.AppendLine($"[{e.Severity}]  {e.Message}");
            }

            // Act
            if (Schema.TryLoad(TestData.GetMusicXML().FullName, out Schema schema, handler))
            {
                schema.Save(resultFile);
                xml = File.ReadAllText(resultFile);
            }
            else Assert.Fail(error.ToString());

            // Assert
            Diff.ApproveXml(xml, xsd, Schema.XMLNS);
        }
    }
}
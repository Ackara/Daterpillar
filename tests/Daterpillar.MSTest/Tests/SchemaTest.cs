using Acklann.Diffa;
using Acklann.Diffa.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Shouldly;
using System.IO;
using Schema = Acklann.Daterpillar.Configuration.Schema;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class SchemaTest
    {
        [TestMethod]
        public void Can_deserialize_a_schema_xml_file()
        {
            // Act
            var result1 = Schema.TryLoad(TestData.GetSakilaXML().FullName, out Schema schema1);
            var result2 = Schema.TryLoad(TestData.GetBad_SchemaXML().FullName, out Schema schema2);

            // Assert
            result1.ShouldBeTrue();
            result2.ShouldBeFalse();
            Diff.Approve(schema1);
        }

        [TestMethod]
        [Use(typeof(FileReporter))]
        public void Can_serialize_a_schema_object()
        {
            var resultFile = Path.Combine(Path.GetTempPath(), $"{nameof(Daterpillar)}-test.xml");
            var xsd = TestData.GetFile($"{nameof(Daterpillar)}.xsd").FullName;
            var schema = TestData.CreateInstance();

            schema.Save(resultFile);
            var xml = File.ReadAllText(resultFile);

            Diff.ApproveXml(xml, xsd, Schema.XMLNS);
        }
    }
}
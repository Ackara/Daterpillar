using Acklann.Diffa;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.IO;
using Schema = Acklann.Daterpillar.Configuration.Schema;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    //[Reporter(typeof(FileReporter))]
    public class SerializationTest
    {
        [TestMethod]
        public void Can_deserialize_a_schema_xml_file()
        {
            // Arrange + Act
            var result1 = Schema.TryLoad(TestData.GetMusicXML().FullName, out Schema schema1);
            var result2 = Schema.TryLoad(TestData.GetBad_SchemaXML().FullName, out Schema schema2);

            // Assert
            result1.ShouldBeTrue();
            result2.ShouldBeFalse();

            Diff.Approve(schema1, ".xml");
        }

        //[TestMethod]
        public void Can_deserialize_a_schema_xml_file_with_no_xmlns()
        {
            // Arrange + Act
            var result = Schema.TryLoad(TestData.GetNoNsXML().FullName, out Schema schema);

            // Assert
            result.ShouldBeTrue();

            Diff.Approve(schema, ".xml");
        }

        [TestMethod]
        public void Can_serialize_a_schema_to_xml()
        {
            // Arrange
            string xml = null;
            var resultFile = Path.Combine(Path.GetTempPath(), $"{nameof(Daterpillar)}-test.xml");

            // Act
            if (Schema.TryLoad(TestData.GetMusicXML().FullName, out Schema schema, out string errorMsg))
            {
                schema.Save(resultFile);
                xml = File.ReadAllText(resultFile);
            }
            else Assert.Fail(errorMsg);

            // Assert
            Diff.Approve(xml, ".xml");
        }

        [TestMethod]
        public void Can_validate_a_schema_for_errors()
        {
            // Arrange
            var sample = TestData.GetBad_SchemaXML();
            bool isValid, returedLineNo = false, returnedColumnNo = false;

            // Act
            using (var stream = sample.OpenRead())
            {
                isValid = Schema.Validate(stream, (severity, ex) =>
                {
                    if (!returedLineNo) returedLineNo = (ex.LineNumber > 0);
                    if (!returnedColumnNo) returnedColumnNo = (ex.LinePosition > 0);
                });
            }

            // Assert
            isValid.ShouldBeFalse();
            returedLineNo.ShouldBeTrue();
            returnedColumnNo.ShouldBeTrue();
        }

        [TestMethod]
        public void Can_merge_multiple_schemas()
        {
            // Arrange
            var sut = Schema.Load(TestData.GetMusicXML().FullName);

            // Act
            sut.Merge();

            // Assert
            sut.Scripts.ShouldNotBeEmpty();
            Diff.Approve(sut, ".xml");
        }
    }
}
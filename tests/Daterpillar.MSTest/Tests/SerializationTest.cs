using Acklann.Daterpillar.Linq;
using Acklann.Diffa;
using ApprovalTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.IO;
using System.Xml.Schema;
using Schema = Acklann.Daterpillar.Serialization.Schema;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class SerializationTest
    {
        [TestMethod]
        public void Can_read_schema_from_xml_file()
        {
            void print(XmlSeverityType s, XmlSchemaException ex) => System.Console.WriteLine($"[{s}] {ex.Message}");

            // Arrange + Act
            var result1 = Schema.TryLoad(Sample.GetMusicXML().FullName, out Schema schema1, print);
            var result2 = Schema.TryLoad(Sample.GetBad_SchemaXML().FullName, out Schema schema2);

            // Assert
            result1.ShouldBeTrue();
            result2.ShouldBeFalse();

            Approvals.VerifyXml(schema1.ToXml());
        }

        [TestMethod]
        public void Can_write_schema_to_xml_file()
        {
            // Arrange
            string xml = null;
            var resultFile = Path.Combine(Path.GetTempPath(), $"{nameof(Daterpillar)}-test.xml");

            // Act
            if (Schema.TryLoad(Sample.GetMusicXML().FullName, out Schema schema, out string errorMsg))
            {
                schema.Save(resultFile);
                xml = File.ReadAllText(resultFile);
            }
            else Assert.Fail(errorMsg);

            // Assert
            Approvals.VerifyXml(xml);
        }

        [TestMethod]
        public void Can_validate_a_schema_for_errors()
        {
            // Arrange
            var sample = Sample.GetBad_SchemaXML();
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
            var sut = Schema.Load(Sample.GetMusicXML().FullName);

            // Act
            sut.Merge();

            // Assert
            sut.Scripts.ShouldNotBeEmpty();
            Diff.Approve(sut.ToXml(), ".xml");
        }

        [DataTestMethod]
        [DataRow("", "''")]
        [DataRow(22, "'22'")]
        [DataRow(null, "null")]
        [DataRow("foo", "'foo'")]
        [DataRow(12.54f, "'12.54'")]
        [DataRow(DayOfWeek.Friday, "'5'")]
        [DataRow("abc ' '' def", "'abc '' '''' def'")]
        [DataRow("2015-1-1 1:1:1", "'2015-01-01 01:01:01'")]
        public void Can_serialize_an_object_to_a_sql_value(object input, string expectedValue)
        {
            if (DateTime.TryParse(input?.ToString(), out DateTime dt))
                input = dt;

            SqlComposer.Serialize(input).ShouldBe(expectedValue);
        }

        
    }
}
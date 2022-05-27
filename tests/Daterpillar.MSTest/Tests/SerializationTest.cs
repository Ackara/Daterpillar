using Acklann.Daterpillar.Annotations;
using Acklann.Daterpillar.Modeling;
using Acklann.Daterpillar.Scripting;
using ApprovalTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Schema;
using Schema = Acklann.Daterpillar.Modeling.Schema;

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
            Approvals.VerifyXml(sut.ToXml());
        }

        [TestMethod]
        [DynamicData(nameof(SqlConversionCases), DynamicDataSourceType.Method)]
        public void Can_serialize_an_object_to_a_sql_value(object input, string expectedValue)
        {
            if (DateTime.TryParse(input?.ToString(), out DateTime dt))
                input = dt;

            SqlExtensions.Serialize(input).ShouldBe(expectedValue);
        }

        [TestMethod]
        [DynamicData(nameof(GetEnumerationCases), DynamicDataSourceType.Method)]
        public void Should_enumerate_tables_by_dependencies(string[] relationships, string expectedSequence)
        {
            // Arrange
            Table table(string name) => new Table(name, new Column("id", SchemaType.VARCHAR), new Column("name", SchemaType.VARCHAR));

            var a = table("a");
            var b = table("b");
            var c = table("c");
            var d = table("d");
            var e = table("e");

            var schema = new Schema();
            schema.Add(a, b, c, d, e);

            Table get(string key)
            {
                switch (key)
                {
                    case "a": return a;
                    case "b": return b;
                    case "c": return c;
                    case "d": return d;
                    case "e": return e;
                    default: throw new NotImplementedException();
                }
            }

            void join(Table x, Table y)
            {
                x.Add(new ForeignKey
                {
                    ForeignTable = y.Name,
                    ForeignColumn = "id",
                    LocalTable = x.Name,
                    LocalColumn = "id"
                });
            }

            // Act
            foreach (string item in relationships)
            {
                string[] pair = item.Split(">");
                join(get(pair[0]), get(pair[1]));
            }

            var result = string.Join(" ", from x in schema.EnumerateTables() select x.Name);

            // Assert
            result.ShouldBe(expectedSequence);
        }

        #region Backing Members

        private static IEnumerable<object[]> GetEnumerationCases()
        {
            //yield return new object[] { new string[] { }, "a b c d e" };
            //yield return new object[] { new string[] { "a>b" }, "b a c d e" };
            yield return new object[] { new string[] { "a>b", "a>c" }, "b c a d e" };
            //yield return new object[] { new string[] { "a>b", "a>c", "b>d" }, "d b c a e" };
            //yield return new object[] { new string[] { "a>d", "a>b", "b>c" }, "d c b a e" };
        }

        private static IEnumerable<object[]> SqlConversionCases()
        {
            yield return new object[] { "", "''" };
            yield return new object[] { 22, "'22'" };
            yield return new object[] { null, "null" };
            yield return new object[] { "foo", "'foo'" };
            yield return new object[] { 12.54f, "'12.54'" };
            yield return new object[] { DayOfWeek.Friday, "'5'" };
            yield return new object[] { "abc ' '' def", "'abc '' '''' def'" };
            yield return new object[] { "2015-1-1 1:1:1", "'2015-01-01 01:01:01'" };
        }

        #endregion Backing Members
    }
}
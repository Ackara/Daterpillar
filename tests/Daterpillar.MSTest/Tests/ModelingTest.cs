using Acklann.Daterpillar.Annotations;
using Acklann.Daterpillar.Modeling;
using ApprovalTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class ModelingTest
    {
        [DataTestMethod, DynamicData(nameof(GetTypesToConvertToTable), DynamicDataSourceType.Method)]
        public void Can_convert_type_to_table(Type type)
        {
            // Arrange
            string name = type.Name;
            using var scenario = ApprovalTests.Namers.ApprovalResults.ForScenario(type.Name);
            using var stream = new MemoryStream();

            var serializer = new XmlSerializer(typeof(Table));

            // Act
            var result = SchemaFactory.CreateFrom(type);

            serializer.Serialize(stream, result);
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream);
            string xml = reader.ReadToEnd();

            // Assert
            Approvals.VerifyXml(xml);
        }

        [TestMethod]
        [DynamicData(nameof(GetTypesToConvertToTable), DynamicDataSourceType.Method)]
        public void Can_enumerate_all_columns(Type type)
        {
            using var scenario = ApprovalTests.Namers.ApprovalResults.ForScenario(type.Name);
            var columns = Acklann.Daterpillar.Modeling.Helper.GetColumns(type).Select(x => x.Name);
            var results = string.Join("\r\n", columns);
            Approvals.Verify(results);
        }

        #region Backing Members

        private static IEnumerable<object[]> GetTypesToConvertToTable()
        {
            var cases = from t in typeof(ModelingTest).GetNestedTypes()
                        select t;
            foreach (var type in cases)
            {
                yield return new object[] { type };
            }
        }

        #endregion Backing Members

        #region Types To Convert

        [Acklann.Daterpillar.Annotations.Table]
        public class Empty
        {
        }

        [System.ComponentModel.DataAnnotations.Schema.Table("offset_table")]
        public class OffsetTable
        {
            public string Id { get; set; }

            public DateTimeOffset Date { get; set; }
        }

        [System.ComponentModel.DataAnnotations.Schema.Table("inffered-columns-table")]
        public class InferredTable
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public int IntValue { get; set; }

            public decimal DecimalValue { get; set; }

            public DateTime Date { get; set; }

            public bool Boolean { get; set; }

            public TimeSpan Time { get; set; }

            public float FloatValue { get; set; }

            public DayOfWeek EnumValue { get; set; }

            public string ReadOnlyProp { get => Id; }

            public int PublicField;

            private string PrivateField;
        }

        [System.ComponentModel.DataAnnotations.Schema.Table("dataAnnotation")]
        public class DataAnnotatedTable
        {
            [System.ComponentModel.DataAnnotations.Key]
            public string Id { get; set; }

            public string Name { get; set; }

            [System.ComponentModel.DataAnnotations.MaxLength(123)]
            [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
            public string Hash { get; set; }
        }

        [Annotations.Table]
        public class MultiKey
        {
            public string Name { get; set; }

            [System.ComponentModel.DataAnnotations.Key]
            public string Part1 { get; set; }

            [Annotations.Key]
            public string Part2 { get; set; }

            [Annotations.Key]
            public string Part3 { get; set; }
        }

        [Annotations.Table]
        public class MultiKey2
        {
            [System.ComponentModel.DataAnnotations.Key]
            public string Name { get; set; }

            [System.ComponentModel.DataAnnotations.Key]
            [Annotations.Index("name_unique_idx", IndexType.Index, Unique = true)]
            public string Part1 { get; set; }

            public string Part2 { get; set; }

            [Annotations.Index("name_unique_idx", IndexType.Index, Unique = true)]
            public string Part3 { get; set; }
        }

        [Annotations.Table]
        public class UniqueIndex
        {
            public string Id { get; set; }

            public string Name { get; set; }

            [Annotations.Index(Unique = true)]
            public string Part2 { get; set; }
        }

        public class Parent
        {
            [Annotations.Column("dln")]
            public int Id { get; set; }

            public string Name { get; set; }
        }

        public class Child
        {
            public int Id { get; set; }

            public string Name { get; set; }

            [Annotations.ForeignKey(typeof(Parent))]
            public int ParentId { get; set; }
        }

        [Annotations.Table]
        public class ComplexType
        {
            public int Id { get; set; }

            public string Name { get; set; }

            [Annotations.Column(SchemaType.VARCHAR)]
            public ReadonlyValue Value { get; }
        }

        public readonly struct ReadonlyValue
        {
            public ReadonlyValue(string value)
            {
                Value = value;
            }

            [Annotations.Column]
            public string Value { get; }
        }

        public class HiddenColumns
        {
            public int Id { get; set; }

            public string Name { get; set; }

            [Annotations.Column(SchemaType.VARCHAR)]
            private string _secret;
        }

        #endregion Types To Convert
    }
}
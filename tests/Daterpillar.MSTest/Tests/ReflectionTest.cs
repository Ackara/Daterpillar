using Acklann.Daterpillar.Serialization;
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
    public class ReflectionTest
    {
        [TestMethod]
        [DynamicData(nameof(GetTypesToConvertToTable), DynamicDataSourceType.Method)]
        public void Can_convert_type_to_table(Type type)
        {
            // Arrange
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

        #region Backing Members

        private static IEnumerable<object[]> GetTypesToConvertToTable()
        {
            var cases = from t in typeof(ReflectionTest).GetNestedTypes()
                            //where t.Name == nameof(MultiKey)
                        select t;
            foreach (var type in cases)
            {
                yield return new object[] { type };
            }
        }

        #endregion Backing Members

        #region Types To Convert

        [Acklann.Daterpillar.Modeling.Attributes.Table]
        public class Empty
        {
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

        [Modeling.Attributes.Table]
        public class MultiKey
        {
            public string Name { get; set; }

            [System.ComponentModel.DataAnnotations.Key]
            public string Part1 { get; set; }

            [Modeling.Attributes.Key]
            public string Part2 { get; set; }

            [Modeling.Attributes.Key]
            public string Part3 { get; set; }
        }

        public class Parent
        {
            [Modeling.Attributes.Column("dln")]
            public int Id { get; set; }

            public string Name { get; set; }
        }

        public class Child
        {
            public int Id { get; set; }

            public string Name { get; set; }

            [Modeling.Attributes.ForeignKey(typeof(Parent))]
            public int ParentId { get; set; }
        }

        #endregion Types To Convert
    }
}
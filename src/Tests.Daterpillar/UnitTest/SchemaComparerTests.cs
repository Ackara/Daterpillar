using Gigobyte.Daterpillar.Compare;
using Gigobyte.Daterpillar.Transformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    public class SchemaComparerTests
    {
        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void GenerateReport_should_return_equal_when_the_specified_schemas_are_the_same()
        {
            // Arrange
            var sourceMockAggregator = Mock.Create<ISchemaAggregator>();
            sourceMockAggregator.Arrange(x => x.FetchSchema())
                .Returns(Test.Data.CreateSchema())
                .OccursOnce();

            var targetMockAggregator = Mock.Create<ISchemaAggregator>();
            targetMockAggregator.Arrange(x => x.FetchSchema())
                .Returns(Test.Data.CreateSchema())
                .OccursOnce();

            var sut = Mock.Create<SchemaComparer>();

            // Act
            var report = sut.GenerateReport(sourceMockAggregator, targetMockAggregator);

            // Assert
            Assert.AreEqual(0, report.Discrepancies.Count);
            Assert.AreEqual(Outcome.Equal, report.Summary);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void GenerateReport_should_return_not_equal_when_the_specified_schemas_are_not_the_same()
        {
            // Arrange
            var source = Test.Data.CreateSchema();
            var target = Test.Data.CreateSchema();

            var sut = new SchemaComparer();

            // Act
            var report = sut.GenerateReport(source, target);

            // Assert
            Assert.AreEqual(3, report.Discrepancies.Count);
            Assert.AreEqual(Outcome.NotEqual, report.Summary);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void GenerateReport_should_determine_the_specified_schemas_are_empty_when_they_have_no_tables()
        {
            // Arrange
            var sut = Mock.Create<SchemaComparer>();

            // Act
            var report = sut.GenerateReport(new Schema(), new Schema());

            // Assert
            Assert.AreEqual(0, report.Discrepancies.Count);
            Assert.AreEqual(Outcome.SourceEmpty, report.Summary);
            Assert.AreEqual(Outcome.TargetEmpty, report.Summary);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Compare_should_return_zero_when_the_specified_schemas_are_equal()
        {
            var result = new SchemaComparer().Compare(Test.Data.CreateSchema(), Test.Data.CreateSchema());

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Compare_should_return_an_int_less_then_zero_when_the_left_schema_has_less_DB_objects()
        {
            // Arrange
            var schemaA = Test.Data.CreateSchema();

            var schemaB = Test.Data.CreateSchema();
            schemaB.Tables.Add(CreateMockTable("anotherOne"));

            var sut = new SchemaComparer();

            // Act
            var result = sut.Compare(schemaA, schemaB);

            // Assert
            Assert.IsTrue(result < 0);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Compare_should_return_an_int_greater_then_zero_when_the_right_schema_has_more_DB_objects()
        {
            // Arrange
            var schemaA = Test.Data.CreateSchema();
            schemaA.Tables.Add(CreateMockTable("anotherOne"));

            var schemaB = Test.Data.CreateSchema();

            var sut = new SchemaComparer();

            // Act
            var result = sut.Compare(schemaA, schemaB);

            // Assert
            Assert.IsTrue(result > 0);
        }

        #region Private Members

        private static Table CreateMockTable(string name)
        {
            var table = new Table();
            table.Name = name;
            table.Columns = new List<Column>();
            table.Columns.Add(new Column()
            {
                Name = "Id",
                AutoIncrement = true,
                DataType = new DataType("INT")
            });
            table.Columns.Add(new Column()
            {
                Name = "Name",
                DataType = new DataType("VARCHAR")
            });

            return table;
        }

        #endregion Private Members
    }
}
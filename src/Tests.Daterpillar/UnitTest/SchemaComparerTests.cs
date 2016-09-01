using Gigobyte.Daterpillar.Aggregation;
using Gigobyte.Daterpillar.Migration;
using Gigobyte.Daterpillar.TextTransformation;
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
        public void GetChanges_should_return_equal_when_the_specified_schemas_are_the_same()
        {
            // Arrange
            var sourceMockAggregator = Mock.Create<ISchemaAggregator>();
            sourceMockAggregator.Arrange(x => x.FetchSchema())
                .Returns(SampleData.CreateSchema())
                .OccursOnce();

            var targetMockAggregator = Mock.Create<ISchemaAggregator>();
            targetMockAggregator.Arrange(x => x.FetchSchema())
                .Returns(SampleData.CreateSchema())
                .OccursOnce();

            var sut = Mock.Create<SchemaComparer>();

            // Act
            var report = sut.GetChanges(sourceMockAggregator, targetMockAggregator);

            // Assert
            Assert.AreEqual(0, report.Discrepancies);
            Assert.AreEqual(ComparisonReportConclusions.Equal, report.Summary);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void GetChanges_should_return_not_equal_when_the_specified_schemas_are_not_the_same()
        {
            // Arrange
            var source = SampleData.CreateSchema();
            var target = SampleData.CreateSchema();
            target.Tables.Add(CreateMockTable("anotherOne"));

            var sut = new SchemaComparer();

            // Act
            var report = sut.GetChanges(source, target);

            // Assert
            Assert.AreEqual(1, report.Discrepancies);
            Assert.AreEqual(ComparisonReportConclusions.NotEqual, report.Summary);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void GetChanges_should_determine_the_specified_schemas_are_empty_when_they_have_no_tables()
        {
            // Arrange
            var sut = Mock.Create<SchemaComparer>();

            // Act
            var report = sut.GetChanges(new Schema(), new Schema());

            // Assert
            Assert.AreEqual(0, report.Discrepancies);
            Assert.IsTrue(report.Summary.HasFlag(ComparisonReportConclusions.Equal));
            Assert.IsTrue(report.Summary.HasFlag(ComparisonReportConclusions.SourceEmpty));
            Assert.IsTrue(report.Summary.HasFlag(ComparisonReportConclusions.TargetEmpty));
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
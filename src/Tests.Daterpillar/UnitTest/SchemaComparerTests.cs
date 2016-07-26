using Gigobyte.Daterpillar.Compare;
using Gigobyte.Daterpillar.Transformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    public class SchemaComparerTests
    {
        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Compare_should_return_equal_when_the_specified_schemas_are_the_same()
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
            var report = sut.Compare(sourceMockAggregator, targetMockAggregator);

            // Assert
            Assert.AreEqual(0, report.Discrepancies.Count);
            Assert.AreEqual(Outcome.Equal, report.Summary);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Compare_should_return_not_equal_when_the_specified_schemas_are_not_the_same()
        {
            // Arrange
            var source = Test.Data.CreateSchema();
            var target = Test.Data.CreateSchema();

            var sut = new SchemaComparer();

            // Act
            var report = sut.Compare(source, target);

            // Assert
            Assert.AreEqual(3, report.Discrepancies.Count);
            Assert.AreEqual(Outcome.NotEqual, report.Summary);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Compare_should_determine_the_specified_schemas_are_empty_when_they_have_no_tables()
        {
            // Arrange
            var sut = Mock.Create<SchemaComparer>();

            // Act
            var report = sut.Compare(new Schema(), new Schema());

            // Assert
            Assert.AreEqual(0, report.Discrepancies.Count);
            Assert.AreEqual(Outcome.SourceEmpty, report.Summary);
            Assert.AreEqual(Outcome.TargetEmpty, report.Summary);
        }
    }
}
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using Gigobyte.Daterpillar.Management;
using System.Linq;
using Gigobyte.Daterpillar.Transformation;
using System.Runtime.Serialization;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    public class SchemaComparerBaseTests
    {
        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void TestMethod1()
        {
            // Arrange
            var serializer = new DataContractSerializer(typeof(Schema));
            var source = serializer.ReadObject(Test.Data.GetFile("").OpenRead());
            var target = serializer.ReadObject(Test.Data.GetFile("").OpenRead());
            
            var sourceMockAggregator = Mock.Create<ISchemaAggregator>();
            sourceMockAggregator.Arrange(x => x.FetchSchema())
                .Returns(source)
                .OccursOnce();

            var targetMockAggregator = Mock.Create<ISchemaAggregator>();
            targetMockAggregator.Arrange(x => x.FetchSchema())
                .Returns(target)
                .OccursOnce();

            var sut = Mock.Create<SchemaComparerBase>();

            // Act
            var report = sut.Compare(sourceMockAggregator, targetMockAggregator);

            // Assert
            Assert.AreEqual(0, report.Discrepancies.Count);
            Assert.AreEqual(Outcome.Equal, report.Summary);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void MyTestMethod()
        {
            // Arrange
            var sut = Mock.Create<SchemaComparerBase>();

            // Act
            var report = sut.Compare(null, null);

            // Assert
            Assert.IsTrue(report.Discrepancies.Count > 0);
            Assert.AreEqual(Outcome.NotEqual, report.Summary);
            Assert.AreEqual(Outcome.SourceEmpty, report.Summary);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void MyTestMethod2()
        {
            // Arrange
            var sut = Mock.Create<SchemaComparerBase>();

            // Act
            var report = sut.Compare(null, null);

            // Assert
            Assert.AreEqual(Outcome.NotEqual, report.Summary);
            Assert.AreEqual(Outcome.TargetEmpty, report.Summary);
            CollectionAssert.AllItemsAreNotNull(report.Discrepancies.ToArray());
        }

        
    }
}

using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.Aggregation;
using Gigobyte.Daterpillar.Migration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class SynchronizerBaseTest
    {
        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void GenerateScript_should_return_an_empty_byte_array_when_both_schemas_are_equal()
        {
            // Arrange
            var source = GetOldSchema();
            var target = GetOldSchema();

            var sut = new FakeSynchronizer();

            // Act
            var actual = sut.GenerateScript(source, target);

            // Assert
            Approvals.VerifyBinaryFile(actual, "sql");
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void GenerateScript_should_return_the_bytes_of_the_update_script_when_the_schemas_passed_are_not_equal()
        {
            // Arrange
            var source = Mock.Create<ISchemaAggregator>();
            source.Arrange(x => x.FetchSchema())
                .Returns(GetNewSchema())
                .OccursOnce();

            var target = Mock.Create<ISchemaAggregator>();
            target.Arrange(x => x.FetchSchema())
                .Returns(GetOldSchema())
                .OccursOnce();

            var sut = new FakeSynchronizer();

            // Act
            var result = sut.GenerateScript(source, target);

            // Assert
            Approvals.VerifyBinaryFile(result, "sql");
            source.Assert();
            target.Assert();
        }

        #region Private Members

        private static Schema GetOldSchema()
        {
            var schema = new Schema();

            return schema;
        }

        public static Schema GetNewSchema()
        {
            var schema = new Schema();

            return schema;
        }

        private class FakeSynchronizer : SynchronizerBase
        {
        }

        #endregion Private Members
    }
}
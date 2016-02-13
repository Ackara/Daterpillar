using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class SQLiteTemplateTest
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        /// <summary>
        /// Assert <see cref="SQLiteTemplate.Transform(Schema)"/> returns a valid SQLite schema with
        /// comments enabled.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GenerateSQLiteSchemaWithComments()
        {
            // Arrange
            var mockTypeResolver = Mock.Create<ITypeNameResolver>();
            mockTypeResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("INTEGER")
                .OccursAtLeast(1);

            var sample = SampleData.CreateSchema();
            sample.Tables.Add(SampleData.CreateTableSchema("Manager"));

            var settings = new SQLiteTemplateSettings()
            {
                CommentsEnabled = true,
                DropTableIfExist = true
            };

            var sut = new SQLiteTemplate(settings, mockTypeResolver);
            // Act
            var result = sut.Transform(sample);

            // Assert
            mockTypeResolver.Assert();
            Approvals.Verify(result);
        }

        /// <summary>
        /// Assert <see cref="SQLiteTemplate.Transform(Schema)"/> returns a valid SQLite schema with
        /// comments disabled.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GenerateSQLiteSchemaWithoutComments()
        {
            // Arrange
            var mockTypeResolver = Mock.Create<ITypeNameResolver>();
            mockTypeResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("INTEGER")
                .OccursAtLeast(1);

            var sample = SampleData.CreateSchema();
            sample.Tables.Add(SampleData.CreateTableSchema("Manager"));

            var settings = new SQLiteTemplateSettings()
            {
                CommentsEnabled = false,
                DropTableIfExist = false
            };

            var sut = new SQLiteTemplate(settings, mockTypeResolver);
            // Act
            var result = sut.Transform(sample);

            // Assert
            mockTypeResolver.Assert();
            Approvals.Verify(result);
        }
    }
}
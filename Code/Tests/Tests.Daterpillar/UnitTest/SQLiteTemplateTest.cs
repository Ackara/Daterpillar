using Ackara.Daterpillar.Transformation;
using Ackara.Daterpillar.Transformation.Template;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [UseApprovalSubdirectory(Str.ApprovalsDir)]
    [UseReporter(typeof(FileLauncherReporter), typeof(ClipboardReporter))]
    public class SQLiteTemplateTest
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        /// <summary>
        /// Verify the <see cref="SQLiteTemplate.Transform(Schema)"/> method outputs a Schema object as a SQLite schema when the default settings is passed.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ReturnSQLiteSchemaWithAllSettings()
        {
            // Arrange
            var settings = new SqlTemplateSettings()
            {
                Indent = true,
                ShowComments = true
            };

            var mockTypeResolver = Mock.Create<ITypeNameResolver>();
            mockTypeResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("DATA_TYPE")
                .OccursAtLeast(1);

            var sut = new SQLiteTemplate(settings, mockTypeResolver);
            var sample = Samples.GetSchema();

            // Act
            var result = sut.Transform(sample);

            // Assert
            mockTypeResolver.Assert();
            Approvals.Verify(result);
        }
    }
}
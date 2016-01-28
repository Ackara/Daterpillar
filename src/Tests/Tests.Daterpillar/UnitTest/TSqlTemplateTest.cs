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
    [UseApprovalSubdirectory(Dev.ApprovalsDir)]
    [UseReporter(typeof(FileLauncherReporter), typeof(ClipboardReporter))]
    public class TSqlTemplateTest
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        /// <summary>
        /// Assert <see cref="TSqlTemplate.Transform(Schema)"/> returns a valid T-SQL schema with
        /// comments enabled.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GenerateTSqlSchemaWithSettingsEnabled()
        {
            // Arrange
            var settings = new TSqlTemplateSettings()
            {
                CommentsEnabled = true,
                DropSchema = true
            };

            var schema = Samples.GetSchema();
            var managerTable = Samples.GetTableSchema("Manager");
            managerTable.Comment = "this is a table";
            schema.Tables.Add(managerTable);

            var mockResolver = Mock.Create<ITypeNameResolver>();
            mockResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("int")
                .OccursAtLeast(1);

            var sut = new TSqlTemplate(settings, mockResolver);

            // Act
            var sql = sut.Transform(schema);

            // Assert
            mockResolver.Assert();
            Approvals.Verify(sql);
        }

        /// <summary>
        /// Assert <see cref="TSqlTemplate.Transform(Schema)"/> returns a valid T-SQL schema with
        /// comments disabled.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GenerateTSqlSchemaWithSettingsDisabled()
        {
            // Arrange
            var settings = new TSqlTemplateSettings()
            {
                CommentsEnabled = false,
                DropSchema = false
            };

            var schema = Samples.GetSchema();
            var managerTable = Samples.GetTableSchema("Manager");
            schema.Tables.Add(managerTable);

            var mockResolver = Mock.Create<ITypeNameResolver>();
            mockResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("int")
                .OccursAtLeast(1);

            var sut = new TSqlTemplate(settings, mockResolver);

            // Act
            var tsql = sut.Transform(schema);

            // Assert
            mockResolver.Assert();
            Approvals.Verify(tsql);
        }
    }
}
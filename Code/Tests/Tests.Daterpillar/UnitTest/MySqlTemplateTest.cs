using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
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
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class MySqlTemplateTest
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        [TestMethod]
        [Owner(Str.Ackara)]
        public void GenerateMySqlSchemaWithSettingsEnabled()
        {
            // Arrange
            var settings = new MySqlTemplateSettings()
            {
                CommentsEnabled = true,
                DropSchemaAtBegining = true
            };

            var schema = Samples.GetSchema();
            var managerTable = Samples.GetTableSchema("Manager");
            managerTable.Comment = "this is a table";
            schema.Tables.Add(managerTable);

            var mockResolver = Mock.Create<ITypeNameResolver>();
            mockResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("int")
                .OccursAtLeast(1);

            var sut = new MySqlTemplate(settings, mockResolver);

            // Act
            var mysql = sut.Transform(schema);

            // Assert
            mockResolver.Assert();
            Approvals.Verify(mysql);
        }

        [TestMethod]
        [Owner(Str.Ackara)]
        public void GenerateMySqlSchemaWithSettingsDisabled()
        {
            // Arrange
            var settings = new MySqlTemplateSettings()
            {
                CommentsEnabled = false,
                DropSchemaAtBegining = false
            };

            var schema = Samples.GetSchema();
            var managerTable = Samples.GetTableSchema("Manager");
            schema.Tables.Add(managerTable);

            var mockResolver = Mock.Create<ITypeNameResolver>();
            mockResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("int")
                .OccursAtLeast(1);

            var sut = new MySqlTemplate(settings, mockResolver);

            // Act
            var mysql = sut.Transform(schema);

            // Assert
            mockResolver.Assert();
            Approvals.Verify(mysql);
        }
    }
}
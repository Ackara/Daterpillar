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
    public class MySqlTemplateTest
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GenerateMySqlSchemaWithSettingsEnabled()
        {
            // Arrange
            var settings = new MySqlTemplateSettings()
            {
                CommentsEnabled = true,
                DropDataIfExist = true
            };

            var schema = SampleData.CreateSchema();
            var managerTable = SampleData.CreateTableSchema("Manager");
            managerTable.Comment = "this is a table";
            schema.Tables.Add(managerTable);

            var mockResolver = Mock.Create<ITypeNameResolver>();
            mockResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("int")
                .OccursAtLeast(1);

            var sut = new MySqlTemplate(settings, mockResolver);

            // Act
            var result = sut.Transform(schema);

            // Assert
            mockResolver.Assert();
            Approvals.Verify(result);
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GenerateMySqlSchemaWithSettingsDisabled()
        {
            // Arrange
            var settings = new MySqlTemplateSettings()
            {
                CommentsEnabled = false,
                DropDataIfExist = false
            };

            var schema = SampleData.CreateSchema();
            var managerTable = SampleData.CreateTableSchema("Manager");
            schema.Tables.Add(managerTable);

            var mockResolver = Mock.Create<ITypeNameResolver>();
            mockResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("int")
                .OccursAtLeast(1);

            var sut = new MySqlTemplate(settings, mockResolver);

            // Act
            var result = sut.Transform(schema);

            // Assert
            mockResolver.Assert();
            Approvals.Verify(result);
        }
    }
}
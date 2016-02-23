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
                DropDatabaseIfExist = true
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
                DropDatabaseIfExist = false
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

        /// <summary>
        /// Assert <see cref="MySqlTemplate.Transform(Schema)"/> assigns the <see cref="Index.Table"/> property if null or empty.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void AssignMySqlIndexTableNameFieldIfMissing()
        {
            // Arrange
            var sample = SampleData.CreateSchema();
            sample.Tables[0].Indexes[1].Table = string.Empty;

            var mockTypeResolver = Mock.Create<ITypeNameResolver>();
            mockTypeResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("INT")
                .OccursAtLeast(1);

            var sut = new MySqlTemplate(new MySqlTemplateSettings(), mockTypeResolver);

            // Act
            var script = sut.Transform(sample);

            // Assert
            mockTypeResolver.AssertAll();
            Approvals.Verify(script);
        }
    }
}
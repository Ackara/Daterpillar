using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.CompilerServices;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class SqlTemplateTest
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        /// <summary>
        /// Assert <see cref="SqlTemplate.Transform(Schema)"/> returns a well formatted T-SQL schema
        /// when all settings are enbaled.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GenerateSqlSchemaWithSettingsEnabled()
        {
            // Arrange
            var settings = new SqlTemplateSettings()
            {
                AddScript = true,
                CommentsEnabled = true,
                DropDatabaseIfExist = true,
            };

            RunTemplateTest(settings);
        }

        /// <summary>
        /// Assert <see cref="SqlTemplate.Transform(Schema)"/> returns a well formatted T-SQL schema
        /// when all settings are disabled.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GenerateSqlSchemaWithSettingsDisabled()
        {
            // Arrange
            var settings = new SqlTemplateSettings()
            {
                AddScript = false,
                CommentsEnabled = false,
                DropDatabaseIfExist = false,
            };

            RunTemplateTest(settings);
        }

        /// <summary>
        /// Assert <see cref="SqlTemplate.Transform(Schema)"/> assign a value to <see
        /// cref="Index.Table"/> if null or empty.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void AutoFillSqlIndexTableNameIfMissing()
        {
            // Arrange
            var sut = new SqlTemplate(new SqlTemplateSettings(), Mock.Create<ITypeNameResolver>());
            var schema = SampleData.CreateSchema();
            schema.Tables[0].Indexes[0].Table = "";

            // Act
            var script = sut.Transform(schema);

            // Assert
            Approvals.Verify(script);
        }

        private void RunTemplateTest(SqlTemplateSettings settings, [CallerMemberName]string name = null)
        {
            // Arrange
            var schema = SampleData.CreateSchema(name);
            var managerTable = SampleData.CreateTableSchema("Manager");
            managerTable.Comment = "this is a comment";
            schema.Tables.Add(managerTable);

            var mockTypeResolver = Mock.Create<ITypeNameResolver>();
            mockTypeResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("int")
                .OccursAtLeast(1);

            var sut = new SqlTemplate(settings, mockTypeResolver);

            // Act
            var tsql = sut.Transform(schema);

            // Assert
            mockTypeResolver.AssertAll();
            Approvals.Verify(tsql);
        }
    }
}
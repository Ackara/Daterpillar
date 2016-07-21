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

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Transform_should_generate_a_sqlite_schema_when_all_template_settings_are_enabled()
        {
            // Arrange
            var mockTypeResolver = Mock.Create<ITypeNameResolver>();
            mockTypeResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("INTEGER")
                .OccursAtLeast(1);

            var sample = Test.Data.CreateSchema();
            sample.Tables.Add(Test.Data.CreateTableSchema("Manager"));

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

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Transform_should_generate_a_sqlite_schema_when_all_template_settings_are_disabled()
        {
            // Arrange
            var mockTypeResolver = Mock.Create<ITypeNameResolver>();
            mockTypeResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("INTEGER")
                .OccursAtLeast(1);

            var sample = Test.Data.CreateSchema();
            sample.Tables.Add(Test.Data.CreateTableSchema("Manager"));

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

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Transform_should_assign_the_sqlite_schema_index_tableName_property_when_null()
        {
            // Arrange
            var sample = Test.Data.CreateSchema();
            sample.Tables[0].Indexes[1].Table = string.Empty;

            var mockTypeResolver = Mock.Create<ITypeNameResolver>();
            mockTypeResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("INT")
                .OccursAtLeast(1);

            var sut = new SQLiteTemplate(new SQLiteTemplateSettings(), mockTypeResolver);

            // Act
            var script = sut.Transform(sample);

            // Assert
            mockTypeResolver.AssertAll();
            Approvals.Verify(script);
        }
    }
}
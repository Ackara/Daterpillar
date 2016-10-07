using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using Tests.Daterpillar.Constants;

namespace Tests.Daterpillar.Tests
{
    [TestClass]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class CSharpTemplateTest
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void Transform_should_generate_csharp_classes_when_all_template_settings_are_enabled()
        {
            // Arrange
            Schema schema = SampleData.CreateSchema();
            schema.Tables.Add(SampleData.CreateTableSchema("Manager"));

            var settings = new CSharpTemplateSettings()
            {
                Namespace = Schema.Xmlns,
                CommentsEnabled = true,
                DataContractsEnabled = true,
                SchemaAttributesEnabled = true,
                VirtualPropertiesEnabled = true
            };

            var mockResolver = Mock.Create<ITypeNameResolver>();
            mockResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("string")
                .OccursAtLeast(1);

            var sut = new CSharpTemplate(settings, mockResolver);

            // Act
            var csharp = sut.Transform(schema);

            // Assert
            mockResolver.Assert();
            Approvals.Verify(csharp);
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void Transform_should_generate_csharp_classes_when_all_template_settings_are_disabled()
        {
            // Arrange
            Schema schema = SampleData.CreateSchema();
            schema.Tables.Add(SampleData.CreateTableSchema("Manager"));

            var settings = new CSharpTemplateSettings()
            {
                Namespace = string.Empty,

                CommentsEnabled = false,
                DataContractsEnabled = false,
                SchemaAttributesEnabled = false,
                VirtualPropertiesEnabled = false
            };

            var mockResolver = Mock.Create<ITypeNameResolver>();
            mockResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("string")
                .OccursAtLeast(1);

            var sut = new CSharpTemplate(settings, mockResolver);

            // Act
            var csharp = sut.Transform(schema);

            // Assert
            mockResolver.Assert();
            Approvals.Verify(csharp);
        }
    }
}
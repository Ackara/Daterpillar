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
    public class NotifyPropertyChangedTemplateTest
    {
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        /// <summary>
        /// Assert <see cref="NotifyPropertyChangedTemplate.Transform(Schema)"/> returns valid C# when all settings
        /// are enabled.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GenerateNotifyPropertyChangedClassesWithSettingsEnabled()
        {
            // Arrange
            Schema schema = SampleData.CreateSchema();
            schema.Tables.Add(SampleData.CreateTableSchema("Manager"));

            var settings = new NotifyPropertyChangedTemplateSettings()
            {
                Namespace = Schema.Xmlns,

                CommentsEnabled = true,
                DataContractsEnabled = true,
                SchemaAnnotationsEnabled = true,
                VirtualPropertiesEnabled = true,
                PartialRaisePropertyChangedMethodEnabled = true
            };

            var mockResolver = Mock.Create<ITypeNameResolver>();
            mockResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("string")
                .OccursAtLeast(1);

            var sut = new NotifyPropertyChangedTemplate(settings, mockResolver);

            // Act
            var csharp = sut.Transform(schema);

            // Assert
            mockResolver.Assert();
            Approvals.Verify(csharp);
        }

        /// <summary>
        /// Assert <see cref="NotifyPropertyChangedTemplate.Transform(Schema)"/> returns valid C# when all settings
        /// are disabled.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GenerateNotifyPropertyChangedClassesWithSettingsDisabled()
        {
            // Arrange
            Schema schema = SampleData.CreateSchema();
            schema.Tables.Add(SampleData.CreateTableSchema("Manager"));

            var settings = new NotifyPropertyChangedTemplateSettings()
            {
                Namespace = string.Empty,

                CommentsEnabled = false,
                DataContractsEnabled = false,
                SchemaAnnotationsEnabled = false,
                VirtualPropertiesEnabled = false,
                PartialRaisePropertyChangedMethodEnabled = false
            };

            var mockResolver = Mock.Create<ITypeNameResolver>();
            mockResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("string")
                .OccursAtLeast(1);

            var sut = new NotifyPropertyChangedTemplate(settings, mockResolver);

            // Act
            var csharp = sut.Transform(schema);

            // Assert
            mockResolver.Assert();
            Approvals.Verify(csharp);
        }
    }
}
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
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class CsharpTemplateTest
    {
        /// <summary>
        /// Assert <see cref="CsharpTemplate.Transform(Schema)"/> returns valid C# when all settings
        /// are enabled.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void GenerateCsharpClassesWithSettingsEnabled()
        {
            // Arrange
            Schema schema = Samples.GetSchema();
            schema.Tables.Add(Samples.GetTableSchema("Manager"));

            var settings = new CsharpTemplateSettings()
            {
                Namespace = Schema.Xmlns,

                CommentsEnabled = true,
                DataContractsEnabled = true,
                SchemaAnnotationsEnabled = true,
                VirtualPropertiesEnabled = true
            };

            var mockResolver = Mock.Create<ITypeNameResolver>();
            mockResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("string")
                .OccursAtLeast(1);

            var sut = new CsharpTemplate(settings, mockResolver);

            // Act
            var csharp = sut.Transform(schema);

            // Assert
            mockResolver.Assert();
            Approvals.Verify(csharp);
        }

        /// <summary>
        /// Assert <see cref="CsharpTemplate.Transform(Schema)"/> returns valid C# when all settings
        /// are disabled.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void GenerateCsharpClassesWithSettingsDisabled()
        {
            // Arrange
            Schema schema = Samples.GetSchema();
            schema.Tables.Add(Samples.GetTableSchema("Manager"));

            var settings = new CsharpTemplateSettings()
            {
                Namespace = string.Empty,

                CommentsEnabled = false,
                DataContractsEnabled = false,
                SchemaAnnotationsEnabled = false,
                VirtualPropertiesEnabled = false
            };

            var mockResolver = Mock.Create<ITypeNameResolver>();
            mockResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("string")
                .OccursAtLeast(1);

            var sut = new CsharpTemplate(settings, mockResolver);

            // Act
            var csharp = sut.Transform(schema);

            // Assert
            mockResolver.Assert();
            Approvals.Verify(csharp);
        }
    }
}
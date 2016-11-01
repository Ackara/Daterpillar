using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using Tests.Daterpillar.Constants;

namespace Test.Daterpillar.Tests
{
    [TestClass]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class CSharpTemplateBuilderTest
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GetContent_should_return_csharp_class_declarations_when_all_template_settings_are_enabled()
        {
            RunTest(new CSharpScriptBuilderSettings()
            {
                Namespace = "https://static.testing.com/v1/schema.xsd",
                AppendComments = true,
                AppendDataContracts = true,
                AppendSchemaInformation = true,
                AppendVirtualProperties = true
            });
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GetContent_should_return_csharp_class_declarations_when_all_template_settings_are_disabled()
        {
            RunTest(new CSharpScriptBuilderSettings()
            {
                AppendComments = false,
                AppendDataContracts = false,
                AppendSchemaInformation = false,
                AppendVirtualProperties = false
            });
        }

        internal void RunTest(CSharpScriptBuilderSettings settings)
        {
            // Arrange
            var mockTypeResolver = Mock.Create<ITypeNameResolver>();

            mockTypeResolver.Arrange(x => x.GetName(Arg.IsAny<DataType>()))
                .Returns("string")
                .OccursAtLeast(1);

            var sut = new CSharpScriptBuilder(settings, mockTypeResolver);

            // Act
            sut.Create(GetSampleSchema());
            var code = sut.GetContent();

            var syntax = CSharpSyntaxTree.ParseText(code);
            var errorList = syntax.GetDiagnostics();
            int numberOfErrors = 0;

            foreach (var error in syntax.GetDiagnostics())
            {
                numberOfErrors++;
                System.Diagnostics.Debug.WriteLine($"[{error.Severity}] {error.GetMessage()} ");
            }

            // Assert
            Approvals.Verify(code);
            Assert.AreEqual(0, numberOfErrors, "The generated code has errors.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(code), "No classes were generated.");
        }

        #region Helpers

        private Schema GetSampleSchema()
        {
            var schema = new Schema();
            var table1 = schema.CreateTable("table1");
            table1.CreateColumn("Id", new DataType("INT"), autoIncrement: true, nullable: false, comment: "This is a comment");
            table1.CreateColumn("Name", new DataType("VARCHAR", 64));
            table1.CreateColumn("Date_Created");

            var table2 = schema.CreateTable("table2");
            table2.CreateColumn("Id", new DataType("INT"), autoIncrement: true, nullable: false, comment: "This is a comment");
            table2.CreateColumn("Name", new DataType("long"), autoIncrement: false, nullable: true);


            var table3 = schema.CreateTable("table3");
            table3.CreateColumn("Ref1");
            table3.CreateColumn("Ref2");
            table3.CreateIndex("thekey", IndexType.PrimaryKey, true, new IndexColumn("Ref1"));
            table3.CreateIndex("thekey", IndexType.PrimaryKey, true, new IndexColumn("Ref2"));

            return schema;
        }

        #endregion Helpers
    }
}
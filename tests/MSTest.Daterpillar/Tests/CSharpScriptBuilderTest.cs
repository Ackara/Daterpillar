using Ackara.Daterpillar.Scripting;
using ApprovalTests;
using ApprovalTests.Reporters;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    [UseReporter(typeof(FileLauncherReporter), typeof(ClipboardReporter))]
    public class CSharpScriptBuilderTest
    {
        [TestMethod]
        public void Append_should_return_a_csharp_script_that_defines_a_class_when_all_settings_are_enabled()
        {
            // Arrange
            var schema = MockData.GetSchema(FName.scriptingTest_partial_schemaXML);
            var sut = new CSharpScriptBuilder(new CSharpScriptBuilderSettings()
            {
                AddConstants = true,
                IgnoreComments = false,
                AddSchemaAttributes = true,
                UseVirtualProperties = true,
                InheritEntityBaseClass = true,
                AddDataContractAttributes = true,
            });

            // Act
            sut.Append(schema);
            var script = sut.GetContent();
            var codeIsCompilable = TryCompilation(ref script, out string errorMsg);

            // Assert
            script.ShouldNotBeNullOrEmpty();
            Approvals.Verify(script);
            codeIsCompilable.ShouldBeTrue(errorMsg);
        }

        [TestMethod]
        public void Append_should_return_a_csharp_script_that_defines_a_class_when_all_settings_are_disabled()
        {
            // Arrange
            var schema = MockData.GetSchema(FName.scriptingTest_partial_schemaXML);
            var sut = new CSharpScriptBuilder(new CSharpScriptBuilderSettings()
            {
                AddConstants = false,
                IgnoreComments = true,
                AddSchemaAttributes = false,
                UseVirtualProperties = false,
                InheritEntityBaseClass = false,
                AddDataContractAttributes = false,
            });

            // Act
            sut.Append(schema);
            var script = sut.GetContent();
            var codeIsCompilable = TryCompilation(ref script, out string errorMsg);

            // Assert
            script.ShouldNotBeNullOrEmpty();
            Approvals.Verify(script);
            codeIsCompilable.ShouldBeTrue(errorMsg);
        }

        [TestMethod]
        public void Append_should_return_a_csharp_script_that_defines_a_enum()
        {
            // Arrange
            var sample = new Dictionary<string, int>()
            {
                {"Red", 0 },
                {"Green", 1 },
                {"Blue", 2 },
                {"Dark Gray", 4 },
            };

            var sut = new CSharpScriptBuilder(new CSharpScriptBuilderSettings()
            {
                IgnoreComments = false,
                AddSchemaAttributes = true,
                UseVirtualProperties = true,
                InheritEntityBaseClass = true,
            });

            // Act
            sut.Append("Color", sample);
            var code = sut.GetContent();
            var codeIsCompilable = TryCompilation(ref code, out string errorMsg);

            // Assert
            code.ShouldNotBeNullOrEmpty();
            Approvals.Verify(code);
            codeIsCompilable.ShouldBeTrue(errorMsg);
        }

        [TestMethod]
        public void Append_should_return_a_csharp_script_that_defines_a_enum_that_supports_flags()
        {
            // Arrange
            var sample = new Dictionary<string, int>()
            {
                {"Red", 0 },
                {"Green", 1 },
                {"Blue", 2 },
                {"Dark Gray", 4 },
            };

            var sut = new CSharpScriptBuilder(new CSharpScriptBuilderSettings()
            {
                IgnoreComments = false,
                AddSchemaAttributes = true,
                UseVirtualProperties = true,
                InheritEntityBaseClass = true,
            });

            // Act
            sut.Append("Color", sample, flag: true);
            var code = sut.GetContent();
            var codeIsCompilable = TryCompilation(ref code, out string errorMsg);

            // Assert
            code.ShouldNotBeNullOrEmpty();
            Approvals.Verify(code);
            codeIsCompilable.ShouldBeTrue(errorMsg);
        }

        private static bool TryCompilation(ref string code, out string errorMsg)
        {
            var errors = CSharpSyntaxTree.ParseText(code).GetDiagnostics();
            errorMsg = string.Join(Environment.NewLine, errors.Select(x => $"[{x.Severity}] {x.GetMessage()}"));

            if (errors.Count() > 0)
            {
                code = string.Format("/*{0}ERRORS!!!{0}{1}{0}*/{0}{0}{2}", Environment.NewLine, errorMsg, code);
            }

            return errors.Count() == 0;
        }
    }
}
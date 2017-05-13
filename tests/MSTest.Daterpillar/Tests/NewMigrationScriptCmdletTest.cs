using Ackara.Daterpillar;
using Ackara.Daterpillar.Cmdlets;
using Ackara.Daterpillar.Migration;
using Ackara.Daterpillar.Scripting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.Linq;
using System.Management.Automation;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    public class NewMigrationScriptCmdletTest
    {
        [TestMethod]
        public void Invoke_should_return_a_psobject_with_the_fields_script_state_and_mods()
        {
            // Arrange
            var schemaA = MockData.GetSchema(FName.cmdlets_source_schemaXML);
            var schemaB = MockData.GetSchema(FName.cmdlets_target_schemaXML);

            var sut = new NewMigrationScriptCmdlet()
            {
                Source = schemaA,
                Target = schemaB,
                Syntax = nameof(Syntax.MySQL)
            };

            // Act
            var results = sut.Invoke<PSObject>().ToArray();
            var obj = results[0];

            // Assert
            results.Length.ShouldBe(1);
            obj.Members["Script"].ToString().ShouldNotBeNullOrWhiteSpace();
            obj.Members["State"].Value.ShouldBeOfType<MigrationState>();
            obj.Members["Modifications"].ShouldNotBeNull();
        }

        [TestMethod]
        public void Invoke_should_return_a_psobject_when_a_script_builder_object_is_passed()
        {
            // Arrange
            var schemaA = MockData.GetSchema(FName.cmdlets_source_schemaXML);
            var schemaB = MockData.GetSchema(FName.cmdlets_target_schemaXML);

            var sut = new NewMigrationScriptCmdlet()
            {
                Source = schemaA,
                Target = schemaB,
                ScriptBuilder = new NullScriptBuilder()
            };

            // Act
            var results = sut.Invoke<PSObject>().ToArray();

            // Assert
            results.Length.ShouldBe(1);
        }
    }
}
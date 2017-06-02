using Acklann.Daterpillar;
using Acklann.Daterpillar.Cmdlets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.Linq;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    public class ConvertToScriptCmdletTest
    {
        [TestMethod]
        public void Invoke_should_return_a_script_when_a_schema_object_is_passed()
        {
            // Arrange
            var sample = MockData.GetSchema();
            var sut = new ConvertToScriptCmdlet()
            {
                InputObject = sample,
                Syntax = Syntax.SQLite.ToString()
            };

            // Act
            var results = sut.Invoke<string>().ToArray();

            // Assert
            results.Length.ShouldBe(1);
            results[0].ShouldNotBeNullOrWhiteSpace();
        }
    }
}
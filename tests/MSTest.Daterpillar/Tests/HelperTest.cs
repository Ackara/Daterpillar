using Ackara.Daterpillar;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(FName.Samples)]
    public class HelperTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataSource(SourceName.text_formats)]
        public void ToPascalCase_should_return_a_pascal_case_string_when_invoked()
        {
            // Arrange
            string sample = Convert.ToString(TestContext.DataRow["TEXT"]);
            string expectedValue = Convert.ToString(TestContext.DataRow["PASCAL"]);

            // Act
            string result = sample.ToPascalCase();

            // Assert
            result.ShouldBe(expectedValue);
        }

        [TestMethod]
        [DataSource(SourceName.text_formats)]
        public void ToCamelCase_should_return_a_camel_case_string_when_invoked()
        {
            // Arrange
            string sample = Convert.ToString(TestContext.DataRow["TEXT"]);
            string expectedValue = Convert.ToString(TestContext.DataRow["CAMEL"]);

            // Act
            string result = sample.ToCamelCase();

            // Assert
            result.ShouldBe(expectedValue);
        }
    }
}
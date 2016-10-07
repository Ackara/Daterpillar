using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gigobyte.Daterpillar.TextTransformation;

namespace Tests.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(Test.Data.Samples)]
    public class TemplateExtensionsTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        [DataSource(Test.Data.Provider, (Test.Data.Directory + Test.File.TextFormatCSV), Test.File.TextFormatCSV, DataAccessMethod.Sequential)]
        public void ToPascalCase_should_format_a_string_into_pascal_case()
        {
            // Arrange
            var text = TestContext.DataRow["TEXT"].ToString();
            var expected = TestContext.DataRow["PASCAL"].ToString();

            // Act
            var result = text.ToPascalCase();
            TestContext.WriteLine("input: {0}", text);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        [DataSource(Test.Data.Provider, (Test.Data.Directory + Test.File.TextFormatCSV), Test.File.TextFormatCSV, DataAccessMethod.Sequential)]
        public void ToCamelCase_should_format_a_string_into_camel_case()
        {
            // Arrange
            var text = TestContext.DataRow["TEXT"].ToString();
            var expected = TestContext.DataRow["CAMEL"].ToString();

            // Act
            var result = text.ToCamelCase();
            TestContext.WriteLine("input: {0}", text);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
using Ackara.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [DeploymentItem(Artifact.DataXLSX)]
    public class TemplateExtensionsTests
    {
        public TestContext TestContext { get; set; }

        /// <summary>
        /// <see cref="TemplateExtensions.ToPascalCase(string, char[])"/> should return every word passed to it in pascal case.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        [DataSource(Data.ExcelProvider, Data.ExcelConnStr, "Text_Formats$", DataAccessMethod.Sequential)]
        public void ConvertStringToPascalCase()
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

        /// <summary>
        /// <see cref="TemplateExtensions.ToCamelCase(string, char[])"/> should return every word passed to it in camel case.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        [DataSource(Data.ExcelProvider, Data.ExcelConnStr, "Text_Formats$", DataAccessMethod.Sequential)]
        public void ConvertStringToCamelCase()
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
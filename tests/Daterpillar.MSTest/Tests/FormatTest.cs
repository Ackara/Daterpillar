using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class FormatTest
    {
        [DataRow("", "")]
        [DataRow(null, null)]
        [DataRow("camelCase", "CamelCase")]
        [DataRow("PascalCase", "PascalCase")]
        [DataRow("snake_case", "SnakeCase")]
        [DataRow("A Short Title", "AShortTitle")]
        [DataRow("a short title", "AShortTitle")]
        [DataRow(" a short title ", "AShortTitle")]
        [DataRow(" a  short _title ", "AShortTitle")]
        [DataTestMethod]
        public void Can_convert_text_to_pascal_case(string sample, string expectedValue)
        {
            sample.ToPascal().ShouldBe(expectedValue);
        }

        [DataRow("", "")]
        [DataRow(null, null)]
        [DataRow("camelCase", "camelCase")]
        [DataRow("PascalCase", "pascalCase")]
        [DataRow("snake_case", "snakeCase")]
        [DataRow("A Long Title", "aLongTitle")]
        [DataRow("a long title", "aLongTitle")]
        [DataTestMethod]
        public void Can_convert_text_to_camel_case(string sample, string expectedValue)
        {
            sample.ToCamel().ShouldBe(expectedValue);
        }

        [DataRow("", "")]
        [DataRow(null, null)]
        [DataRow("camelCase", "camel_case")]
        [DataRow("PascalCase", "pascal_case")]
        [DataRow("snake_case", "snake_case")]
        [DataRow("A Short Title", "a_short_title")]
        [DataRow("a short title", "a_short_title")]
        [DataRow(" a  short  title ", "_a__short__title_")]
        [DataTestMethod]
        public void Can_convert_text_to_snake_case(string sample, string expectedValue)
        {
            sample.ToSnake().ShouldBe(expectedValue);
        }
    }
}
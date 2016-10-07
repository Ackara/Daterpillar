using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tests.Daterpillar.Constants;

namespace Tests.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(Test.Data.Samples)]
    public class CSharpTypeNameResolverTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [DataSource(Test.Data.Provider, (Test.Data.Directory + Test.File.DataTypesCSV), Test.File.DataTypesCSV, DataAccessMethod.Sequential)]
        public void GetName_should_return_a_valid_csharp_type_name_when_data_type_is_passed()
        {
            // Arrange
            var dataType = new DataType(typeName: Convert.ToString(TestContext.DataRow["Type"]));
            var expected = TestContext.DataRow["C#"].ToString();
            var sut = new CSharpTypeNameResolver();

            // Act
            var result = sut.GetName(dataType);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
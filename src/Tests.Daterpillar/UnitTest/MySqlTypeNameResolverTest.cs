using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [DeploymentItem(Test.Data.Samples)]
    public class MySqlTypeNameResolverTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        [DataSource(Test.Data.Provider, (Test.Data.Directory + Test.File.DataTypesCSV), Test.File.DataTypesCSV, DataAccessMethod.Sequential)]
        public void GetName_should_return_a_valid_mysql_type_when_a_data_type_is_passed()
        {
            // Arrange
            var dataType = new DataType(typeName: Convert.ToString(TestContext.DataRow["Type"]));
            var expected = TestContext.DataRow["MySQL"].ToString();
            var sut = new MySqlTypeNameResolver();

            // Act
            var result = sut.GetName(dataType);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
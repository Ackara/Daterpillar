using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [DeploymentItem(Test.Data.Samples)]
    public class MSSQLTypeNameResolverTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        [DataSource(Test.Data.Provider, (Test.Data.Directory + Test.File.DataTypesCSV), Test.File.DataTypesCSV, DataAccessMethod.Sequential)]
        public void GetName_should_return_a_valid_mssql_type_when_a_data_type_is_passed()
        {
            // Arrange
            var dataType = new DataType(typeName: Convert.ToString(TestContext.DataRow["Type"]));
            var expected = Convert.ToString(TestContext.DataRow["T-SQL"]);
            var sut = new MSSQLTypeNameResolver();

            // Act
            var result = sut.GetName(dataType);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
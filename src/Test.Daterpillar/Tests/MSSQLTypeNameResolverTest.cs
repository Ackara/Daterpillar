using Acklann.Daterpillar;
using Acklann.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tests.Daterpillar.Constants;

namespace Tests.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(DDT.Samples)]
    public class TSQLTypeNameResolverTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [DataSource(DDT.Provider, (DDT.Directory + KnownFile.DataTypesCSV), KnownFile.DataTypesCSV, DataAccessMethod.Sequential)]
        public void GetName_should_return_a_valid_tsql_type_when_a_data_type_is_passed()
        {
            // Arrange
            var dataType = new DataType(typeName: Convert.ToString(TestContext.DataRow["Type"]));
            var expected = Convert.ToString(TestContext.DataRow["T-SQL"]);
            var sut = new MSSQLTypeNameResolver();

            // Act
            TestContext.WriteLine("Arg: {0}", dataType);
            var result = sut.GetName(dataType);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
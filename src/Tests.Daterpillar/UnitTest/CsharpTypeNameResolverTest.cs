using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [DeploymentItem(SampleData.DataTypesCSV)]
    public class CSharpTypeNameResolverTest
    {
        public TestContext TestContext { get; set; }
        

        [TestMethod]
        [Owner(Dev.Ackara)]
        [DataSource(Test.Data.Provider, SampleData.DataTypesConnStr, SampleData.DataTypesTable, DataAccessMethod.Sequential)]
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
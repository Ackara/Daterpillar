using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [DeploymentItem(Artifact.DataXLSX)]
    [DeploymentItem(Artifact.XDDL)]
    public class SQLiteTypeNameResolverTest : TypeNameResolverTestBase
    {
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void PreTestValidation(TestContext context)
        {
            AssertTestDataIsValid();
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [DataSource(Data.ExcelProvider, Data.ExcelConnStr, Data.DataTypesSheet, DataAccessMethod.Sequential)]
        public void GetName_should_return_a_valid_sqlite_type_when_a_data_type_is_passed()
        {
            // Arrange
            var dataType = new DataType(typeName: Convert.ToString(TestContext.DataRow["Type"]));
            var expected = TestContext.DataRow["SQLite"].ToString();
            var sut = new SQLiteTypeNameResolver();

            // Act
            var result = sut.GetName(dataType);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
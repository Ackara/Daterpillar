using Ackara.Daterpillar.Transformation;
using Ackara.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [DeploymentItem(Filename.DataXLSX)]
    [DeploymentItem(Filename.XSML)]
    public class SQLiteTypeNameResolverTest : TypeNameResolverTestBase
    {
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void PreTestValidation(TestContext context)
        {
            AssertTestDataIsValid();
        }

        /// <summary>
        /// <see cref="SQLiteTypeNameResolver.GetName(DataType)"/> should return the SQLite type
        /// name that best match the specified data type.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        [DataSource(Data.ExcelProvider, Data.ExcelConnStr, "DataTypes$", DataAccessMethod.Sequential)]
        public void ResolveSQLiteTypeName()
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
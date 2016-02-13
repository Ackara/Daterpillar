using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [DeploymentItem(Artifact.XDDL)]
    [DeploymentItem(Artifact.DataXLSX)]
    public class SqlTypeNameResolverTest : TypeNameResolverTestBase
    {
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            AssertTestDataIsValid();
        }

        /// <summary>
        /// Assert <see cref="SqlTypeNameResolver.GetName(DataType)"/> should return the T-SQL data
        /// type that best match the specified type.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        [DataSource(Data.ExcelProvider, Data.ExcelConnStr, Data.DataTypesSheet, DataAccessMethod.Sequential)]
        public void ResolveSqlTypeName()
        {
            // Arrange
            var dataType = new DataType(typeName: Convert.ToString(TestContext.DataRow["Type"]));
            var expected = Convert.ToString(TestContext.DataRow["T-SQL"]);
            var sut = new SqlTypeNameResolver();

            // Act
            var result = sut.GetName(dataType);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
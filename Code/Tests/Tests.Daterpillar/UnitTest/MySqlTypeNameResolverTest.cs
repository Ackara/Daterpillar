using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [DeploymentItem(Artifact.XSML)]
    [DeploymentItem(Artifact.DataXLSX)]
    public class MySqlTypeNameResolverTest : TypeNameResolverTestBase
    {
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void PreTestValidation(TestContext context)
        {
            AssertTestDataIsValid();
        }

        /// <summary>
        /// Assert <see cref="MySqlTypeNameResolver.GetName(DataType)"/> returns a valid MySQL type
        /// name that best match the specified data type.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        [DataSource(Data.ExcelProvider, Data.ExcelConnStr, "DataTypes$", DataAccessMethod.Sequential)]
        public void ResolveMySqlTypeName()
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
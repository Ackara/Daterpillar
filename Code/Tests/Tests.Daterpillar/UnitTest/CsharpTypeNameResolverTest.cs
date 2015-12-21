using Ackara.Daterpillar.Transformation;
using Ackara.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    public class CsharpTypeNameResolverTest : TypeNameResolverTestBase
    {
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void PreTestValidation(TestContext context)
        {
            AssertTestDataIsValid();
        }

        /// <summary>
        /// Assert <see cref="CsharpTypeNameResolver.GetName(DataType)"/> returns a valid C# type
        /// name that best match the specified data type.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        [DataSource(Data.ExcelProvider, Data.ExcelConnStr, "DataTypes$", DataAccessMethod.Sequential)]
        public void ResolveCsharpTypeName()
        {
            // Arrange
            var dataType = new DataType(typeName: Convert.ToString(TestContext.DataRow["Type"]));
            var expected = TestContext.DataRow["C#"].ToString();
            var sut = new CsharpTypeNameResolver();

            // Act
            var result = sut.GetName(dataType);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
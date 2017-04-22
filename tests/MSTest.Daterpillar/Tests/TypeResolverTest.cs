using Ackara.Daterpillar;
using Ackara.Daterpillar.TypeResolvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using static MSTest.Daterpillar.MockData;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(Samples)]
    public class TypeResolverTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataSource(dataTypeTable)]
        public void GetTypeName_should_return_valid_csharp_type_when_invoked()
        {
            // Arrange
            var typeName = Convert.ToString(TestContext.DataRow[Col.Type]);
            var expectedValue = Convert.ToString(TestContext.DataRow[Col.CSharp]);
            var sut = new CSharpTypeResolver();

            // Act
            var result = sut.GetTypeName(new DataType(typeName));

            // Assert
            result.ShouldBe(expectedValue, customMessage: $"Arg: '{typeName}'");
        }

        [TestMethod]
        [DataSource(dataTypeTable)]
        public void GetTypeName_should_return_valid_mysql_type_when_invoked()
        {
            // Arrange
            var typeName = Convert.ToString(TestContext.DataRow[Col.Type]);
            var expectedValue = Convert.ToString(TestContext.DataRow[Col.MySQL]);
            var sut = new MySQLTypeResolver();

            // Act
            var result = sut.GetTypeName(new DataType(typeName));

            // Assert
            result.ShouldBe(expectedValue, customMessage: $"Arg: '{typeName}'");
        }

        [TestMethod]
        [DataSource(dataTypeTable)]
        public void GetTypeName_should_return_valid_mssql_type_when_invoked()
        {
            // Arrange
            var typeName = Convert.ToString(TestContext.DataRow[Col.Type]);
            var expectedValue = Convert.ToString(TestContext.DataRow[Col.MSSQL]);
            var sut = new MSSQLTypeResolver();

            // Act
            var result = sut.GetTypeName(new DataType(typeName));

            // Assert
            result.ShouldBe(expectedValue, customMessage: $"Arg: '{typeName}'");
        }

        [TestMethod]
        [DataSource(dataTypeTable)]
        public void GetTypeName_should_return_valid_sqlite_type_when_invoked()
        {
            // Arrange
            var typeName = Convert.ToString(TestContext.DataRow[Col.Type]);
            var expectedValue = Convert.ToString(TestContext.DataRow[Col.SQLite]);
            var sut = new SQLiteTypeResolver();

            // Act
            var result = sut.GetTypeName(new DataType(typeName));
            
            // Assert
            result.ShouldBe(expectedValue, customMessage: $"Arg: '{typeName}'");
        }

        #region Private Members

        private struct Col
        {
            public const string
                Type = "Type",
                CSharp = "C#",
                MSSQL = "MSSQL",
                MySQL = "MySQL",
                SQLite = "SQLite";
        }

        #endregion Private Members
    }
}
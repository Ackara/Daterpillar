using Acklann.Daterpillar;
using Acklann.Daterpillar.Scripting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    public class ScriptBuilderFactoryTest
    {
        [TestMethod]
        public void Create_should_return_a_mssql_script_builder()
        {
            // Arrange
            var sut = new ScriptBuilderFactory();

            // Act
            var result1 = sut.Create(Syntax.MSSQL);
            var result2 = sut.Create(nameof(MSSQLScriptBuilder));

            // Assert
            result1.ShouldBeOfType<MSSQLScriptBuilder>();
            result2.ShouldBeOfType<MSSQLScriptBuilder>();
        }

        [TestMethod]
        public void Create_should_return_a_mysql_script_builder()
        {
            // Arrange
            var sut = new ScriptBuilderFactory();

            // Act
            var result1 = sut.Create(Syntax.MySQL);
            var result2 = sut.Create(nameof(MySQLScriptBuilder));

            // Assert
            result1.ShouldBeOfType<MySQLScriptBuilder>();
            result2.ShouldBeOfType<MySQLScriptBuilder>();
        }

        [TestMethod]
        public void Create_should_return_a_sqlite_script_builder()
        {
            // Arrange
            var sut = new ScriptBuilderFactory();

            // Act
            var result1 = sut.Create(Syntax.SQLite);
            var result2 = sut.Create(nameof(SQLiteScriptBuilder));

            // Assert
            result1.ShouldBeOfType<SQLiteScriptBuilder>();
            result2.ShouldBeOfType<SQLiteScriptBuilder>();
        }

        [TestMethod]
        public void Create_should_return_a_csharp_script_builder()
        {
            // Arrange
            var sut = new ScriptBuilderFactory();

            // Act
            var result1 = sut.Create(nameof(CSharpScriptBuilder));

            // Assert
            result1.ShouldBeOfType<CSharpScriptBuilder>();
        }
    }
}
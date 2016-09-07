using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Daterpillar.IntegrationTest
{
    [TestClass]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class MSSQLScriptBuilderTest
    {
        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Create_should_return_a_tsql_command_for_adding_a_new_table()
        {
            // Arrange
            var newTable = new Table("a", new Column[]
            {
                SampleData.CreateIdColumn(),
                SampleData.CreateDateTimeColumn("Date"),
                SampleData.CreateStringColumn("Name")
            });

            var sut = new MSSQLScriptBuilder();

            // Act

            // Assert
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Create_should_return_a_tsql_command_for_adding_a_new_index()
        {
            // Arrange
            var sut = new MSSQLScriptBuilder();

            // Act

            // Assert
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Create_should_return_a_tsql_command_for_adding_a_foreignKey()
        {
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Create_should_return_a_tsql_command_for_adding_a_column_to_an_table()
        {
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Create_should_return_a_tsql_command_for_dropping_a_table()
        {
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Create_should_return_a_tsql_command_for_dropping_an_index()
        {
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Create_should_return_a_tsql_command_for_dropping_a_foreignKey()
        {
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Create_should_return_a_tsql_command_for_dropping_a_column()
        {
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Create_should_return_a_tsql_command_for_altering_a_table()
        {
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Create_should_return_a_tsql_command_altering_table_column()
        {
        }
    }
}
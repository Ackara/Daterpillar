using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class MSSQLScriptBuilderTest
    {
        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Create_should_return_a_tsql_command_for_adding_a_schema()
        {
            // Arrange
            var schema = SampleData.CreateSchema();
            schema.Tables.Add(SampleData.CreateTableSchema("supervisor"));

            var sut = new MSSQLScriptBuilder();

            // Act
            sut.Create(schema);

            // Assert
            Approvals.Verify(sut.GetContent());
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Create_should_return_a_tsql_command_for_adding_a_new_table()
        {
            // Arrange
            var sut = new MSSQLScriptBuilder();

            // Act
            sut.Create(SampleData.CreateTableSchema());

            // Assert
            Approvals.Verify(sut.GetContent());
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Create_should_return_a_tsql_command_for_adding_a_new_index()
        {
            // Arrange
            var index = new Index()
            {
                Name = "idx1",
                IndexType = IndexType.Index,
                Table = "tblA",
                Unique = true
            };
            index.Columns = new List<IndexColumn>(new IndexColumn[] { new IndexColumn("Name"), new IndexColumn("Age", SortOrder.DESC) });

            var sut = new MSSQLScriptBuilder();

            // Act
            sut.Create(index);

            // Assert
            Approvals.Verify(sut.GetContent());
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Create_should_return_a_tsql_command_for_adding_a_foreignKey()
        {
            // Arrange
            var sut = new MSSQLScriptBuilder();

            // Act
            sut.Create(new ForeignKey()
            {
                LocalTable = "dept",
                LocalColumn = "Name",
                ForeignTable = "location",
                ForeignColumn = "street",
                OnDeleteRule = ForeignKeyRule.RESTRICT,
                OnUpdateRule = ForeignKeyRule.CASCADE
            });

            // Assert
            Approvals.Verify(sut.GetContent());
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Create_should_return_a_tsql_command_for_adding_a_column_to_a_table()
        {
            // Arrange
            var sut = new MSSQLScriptBuilder();

            // Act
            sut.Create(new Column()
            {
                Name = "Name",
                DataType = new DataType("varchar", 64, 0),
                Table = "User",
                Comment = "This is a comment",
                AutoIncrement = true
            });

            // Assert
            Approvals.Verify(sut.GetContent());
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Drop_should_return_a_tsql_command_for_dropping_a_table()
        {
            // Arrange
            var sut = new MSSQLScriptBuilder();

            // Act
            sut.Drop(SampleData.CreateTableSchema());

            // Assert
            Approvals.Verify(sut.GetContent());
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Drop_should_return_a_tsql_command_for_dropping_an_index()
        {
            // Arrange
            var index = new Index()
            {
                Name = "idx1",
                IndexType = IndexType.Primary,
                Table = "tblA"
            };
            index.Columns = new List<IndexColumn>(new IndexColumn[] { new IndexColumn("Id") });

            var sut = new MSSQLScriptBuilder();

            // Act
            sut.Drop(index);

            // Assert
            Approvals.Verify(sut.GetContent());
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Drop_should_return_a_tsql_command_for_dropping_a_foreignKey()
        {
            // Arrange
            var sut = new MSSQLScriptBuilder();

            // Act
            sut.Drop(new ForeignKey()
            {
                Name = "fk1",
                LocalTable = "dept",
                LocalColumn = "Name",
                ForeignTable = "location",
                ForeignColumn = "street",
                OnDeleteRule = ForeignKeyRule.RESTRICT,
                OnUpdateRule = ForeignKeyRule.CASCADE
            });

            // Assert
            Approvals.Verify(sut.GetContent());
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Drop_should_return_a_tsql_command_for_dropping_a_column()
        {
            // Arrange
            var schema = SampleData.CreateSchema();
            var column = schema.Tables[0].Columns[0];

            var sut = new MSSQLScriptBuilder();

            // Act
            sut.Drop(schema, column);

            // Assert
            Approvals.Verify(sut.GetContent());
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void Drop_should_return_a_tsql_command_for_dropping_a_schema()
        {
            // Arrange
            var sut = new MSSQLScriptBuilder();

            // Act
            sut.Drop(SampleData.CreateSchema());

            // Assert
            Approvals.Verify(sut.GetContent());
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void AlterTable_should_return_a_tsql_command_for_altering_a_table()
        {
            // Arrange
            var oldTable = SampleData.CreateTableSchema();
            var newTable = SampleData.CreateTableSchema("Supervisor");
            newTable.Columns[1].DataType = new DataType("short", 16, 0);

            var sut = new MSSQLScriptBuilder();

            // Act
            sut.AlterTable(oldTable, newTable);

            // Assert
            Approvals.Verify(sut.GetContent());
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void AlterTable_should_return_a_tsql_command_altering_table_column()
        {
            // Arrange
            var oldTable = new Column()
            {
                Name = "Name",
                DataType = new DataType("varchar", 64, 0),
                Table = "User",
                Comment = "This is a comment",
                AutoIncrement = true
            };

            var newTable = new Column()
            {
                Name = "Fullname",
                DataType = new DataType("varchar", 64, 0),
                Table = "User",
                IsNullable = true,
                Comment = "This is a comment",
                AutoIncrement = true
            };

            var sut = new MSSQLScriptBuilder();

            // Act
            sut.AlterTable(oldTable, newTable);

            // Assert
            Approvals.Verify(sut.GetContent());
        }
    }
}
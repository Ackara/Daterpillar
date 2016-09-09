using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.Migration;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Telerik.JustMock.Helpers;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class SynchronizerTest
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void GenerateScript_should_return_an_empty_byte_array_when_both_schemas_are_equal()
        {
            // Arrange
            var source = new Schema();
            source.Tables.AddRange(new Table[]
            {
                SampleData.CreateTable("a"),
                SampleData.CreateTable("b"),
                SampleData.CreateTable("c")
            });

            var target = new Schema();
            target.Tables.AddRange(new Table[]
            {
                SampleData.CreateTable("a"),
                SampleData.CreateTable("b"),
                SampleData.CreateTable("c")
            });
            target.Tables.Reverse();

            var sut = new Synchronizer(new FakeScriptBuilder());

            // Act
            var results = sut.GenerateScript(source, target);

            // Assert
            Assert.IsTrue(string.IsNullOrWhiteSpace(results));
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void GenerateScript_should_return_a_update_script_when_the_schemas_passed_tables_are_not_equal()
        {
            var source = new Schema();
            source.Tables.AddRange(new Table[]
            {
                SampleData.CreateTable("a"),
                SampleData.CreateTable("b"),
                SampleData.CreateTable("d"),
                SampleData.CreateTable("e"),
            });

            var target = new Schema();
            target.Tables.AddRange(new Table[]
            {
                SampleData.CreateTable("a"),
                SampleData.CreateTable("b"),
                SampleData.CreateTable("c"),
            });

            var sut = new Synchronizer(new FakeScriptBuilder());

            // Act
            var results = sut.GenerateScript(source, target);

            // Assert
            Approvals.Verify(results);
            source.Assert();
            target.Assert();
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void GenerateScript_should_return_the_bytes_of_a_truncate_script_when_no_tables_from_the_source_match_the_target()
        {
            // Arrange
            var source = new Schema();
            source.Tables.Add(SampleData.CreateTable("a"));

            var target = new Schema();
            target.Tables.Add(SampleData.CreateTable("b"));
            target.Tables.Add(SampleData.CreateTable("c"));

            var sut = new Synchronizer(new FakeScriptBuilder());

            // Act
            var results = sut.GenerateScript(source, target);

            // Assert
            Approvals.Verify(results);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void GenerateScript_should_return_a_update_script_when_the_schemas_passed_columns_are_not_equal()
        {
            // Arrange
            string tableName = "a";

            var source = new Schema();
            source.Tables.Add(new Table(tableName, new Column[]
            {
                SampleData.CreateIntegerColumn("c1", tableName),
                SampleData.CreateIntegerColumn("c2", tableName),
                SampleData.CreateIntegerColumn("c4", tableName),
                SampleData.CreateIntegerColumn("c5", tableName),
            }));

            var target = new Schema();
            target.Tables.Add(new Table("a", new Column[]
            {
                SampleData.CreateIntegerColumn("c1" , tableName),
                SampleData.CreateStringColumn("c2"  , tableName),
                SampleData.CreateDateTimeColumn("c3", tableName),
            }));

            var sut = new Synchronizer(new FakeScriptBuilder());

            // Act
            var result = sut.GenerateScript(source, target);

            // Assert
            Approvals.Verify(result);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void GenerateScript_should_return_the_bytes_of_a_truncate_script_when_no_columns_from_the_source_match_the_target()
        {
            // Arrange
            string tableName = "a";

            var source = new Schema();
            source.Tables.Add(new Table("a", new Column[]
            {
                SampleData.CreateIntegerColumn("c1", tableName)
            }));

            var target = new Schema();
            target.Tables.Add(new Table("a", new Column[]
            {
                SampleData.CreateIntegerColumn("c1", tableName),
                SampleData.CreateIntegerColumn("c2", tableName),
                SampleData.CreateIntegerColumn("c3", tableName),
            }));

            var sut = new Synchronizer(new FakeScriptBuilder());

            // Act
            var results = sut.GenerateScript(source, target);

            // Assert
            Approvals.Verify(results);
        }

        #region Fakes

        private class FakeScriptBuilder : IScriptBuilder
        {
            internal StringBuilder _text = new StringBuilder();

            public void AlterTable(Table tableA, Table tableB)
            {
                _text.AppendLine($"alter {tableA.Name} to {tableB.Name}");
            }

            public void Append(string text)
            {
                _text.Append(text);
            }

            public void AppendLine(string text)
            {
                _text.AppendLine(text);
            }

            public void Create(Table table)
            {
                _text.AppendLine($"create table [{table.Name}]");
            }

            public void Drop(Table table)
            {
                _text.AppendLine($"drop table [{table.Name}]");
            }

            public string GetContent()
            {
                return _text.ToString();
            }

            public void Create(Column column)
            {
                _text.AppendLine($"add column [{column.Name}] to [{column.Table}]");
            }

            public void Drop(Schema schema, Column column)
            {
                _text.AppendLine($"drop [{column.Name}] column from [{column.Table}] table");
            }

            public void AlterTable(Column oldColumn, Column newColumn)
            {
                _text.AppendLine($"alter [{oldColumn.Table}] table [{oldColumn.Name}] column");
            }

            public void Create(Index index)
            {
                _text.AppendLine($"create index [{index.Name}]");
            }

            public void Create(ForeignKey foreignKey)
            {
                _text.AppendLine($"create foreignkey {foreignKey.Name}");
            }

            public void Drop(Index index)
            {
                _text.AppendLine($"drop index [{index.Name}]");
            }

            public void Drop(ForeignKey foreignKey)
            {
                _text.AppendLine($"drop foreignkey {foreignKey.Name}");
            }

            public void Drop(Schema schema)
            {
                _text.Append($"drop {schema.Name} schema");
            }

            public void Create(Schema schema)
            {
                _text.AppendLine($"create schema [{schema.Name}]");
            }
        }

        #endregion Fakes
    }
}
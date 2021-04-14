using Acklann.Daterpillar.Serialization;
using Acklann.Daterpillar.Linq;
using Acklann.Daterpillar.Prototyping;
using Acklann.Daterpillar.Scripting.Writers;
using Acklann.Diffa;
using ApprovalTests.Namers;
using AutoBogus;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class ScriptingTest
    {
        [TestMethod]
        [UseApprovalSubdirectory("../test-cases/approved-results")]
        [DynamicData(nameof(GetMigrationCases), DynamicDataSourceType.Method)]
        public void Can_write_migration_scripts(string label, Language dialect, Schema oldSchema, Schema newSchema)
        {
            // Arrange
            var sut = new Serialization.Migrator();
            string fileName = $"{label}.{dialect}.sql".ToLower();
            string scriptFile = Path.Combine(Path.GetTempPath(), nameof(Daterpillar), nameof(ScriptingTest), fileName);

            using var stream = new MemoryStream();
            using var connection = SqlValidator.ClearDatabase(dialect);
            using var scenario = ApprovalResults.ForScenario(fileName);
            using var writer = new SqlWriterFactory().CreateInstance(dialect, stream);

            // Act
            sut.GenerateMigrationScript(dialect, new Schema(), oldSchema, scriptFile);
            var sql = File.ReadAllText(scriptFile);
            if (!SqlValidator.TryExecute(connection, sql, out string error)) Assert.Fail(error);

            var changes = sut.GenerateMigrationScript(dialect, oldSchema, newSchema, scriptFile);
            sql = File.ReadAllText(scriptFile);
            var migrationWasSuccessful = SqlValidator.TryExecute(connection, sql, out error);

            // Assert
            migrationWasSuccessful.ShouldBeTrue(error);
            if (!string.IsNullOrWhiteSpace(sql)) ApprovalTests.Approvals.VerifyFile(scriptFile);
            changes.ShouldNotBeNull();
        }

        [TestMethod]
        [DataRow(Language.TSQL)]
        [DataRow(Language.MySQL)]
        [DataRow(Language.SQLite, "main")]
        public void Can_clear_test_databases(Language connectionType, string expectedDatabaseName = default)
        {
            using var connection = SqlValidator.ClearDatabase(connectionType);
            connection.ShouldNotBeNull();
            connection.Database.ShouldBe(expectedDatabaseName ?? nameof(Daterpillar));
        }

        [DataTestMethod]
        [DataRow(Language.TSQL)]
        [DataRow(Language.MySQL)]
        [DataRow(Language.SQLite)]
        public void Can_generate_insert_commands(Language kind)
        {
            // Arrange
            var script = new StringBuilder();
            var tableName = "foo";
            var createStatement = $"create table {tableName}(id int, name varchar(64), age int);";
            var columns = new string[] { "id", "name", "age" };
            var values1 = new object[] { 1, "sally", 12 };
            var values2 = new object[] { 2, "peggy", 21 };
            var values3 = new object[] { 3, "don", 40 };

            var sample1 = A.Fake<IEntity>();
            A.CallTo(() => sample1.GetTableName()).Returns(tableName);
            A.CallTo(() => sample1.GetColumnList()).Returns(columns);

            var sample2 = A.Fake<IEntity>();
            A.CallTo(() => sample2.GetTableName()).Returns(tableName);
            A.CallTo(() => sample2.GetColumnList()).Returns(columns);

            // Act
            var case1 = SqlComposer.GenerateInsertStatements();

            A.CallTo(() => sample1.GetValueList()).Returns(new object[] { 1, "'sally'", 12 });
            var case2 = SqlComposer.GenerateInsertStatements(sample1);

            A.CallTo(() => sample1.GetValueList()).Returns(new object[] { 2, "'mark'", 21 });
            A.CallTo(() => sample2.GetValueList()).Returns(new object[] { 3, "'mary'", 25 });
            var case3 = SqlComposer.GenerateInsertStatements(sample1, sample2);

            A.CallTo(() => sample1.GetValueList()).Returns(new object[] { 4, "'jim'", 40 });
            A.CallTo(() => sample2.GetValueList()).Returns(new object[] { 5, "'sal'", 50 });
            var case4 = SqlComposer.GenerateJoinedInsertStatements(sample1, sample2);

            using (var connection = TestDatabase.CreateConnection(kind))
            {
                System.Diagnostics.Debug.WriteLine($"connection: {connection.ConnectionString}");
                TestDatabase.TryExecute(connection, $"drop table {tableName};", out string errorMsg);
                bool failed = !TestDatabase.TryExecute(connection, createStatement, out errorMsg);
                if (failed) Assert.Fail($"Failed to create {tableName} table.\n\n{errorMsg}");

                var separator = string.Concat(Enumerable.Repeat('=', 50));
                foreach (var item in (case2.Concat(case3).Append(case4)))
                {
                    TestDatabase.TryExecute(connection, item, out errorMsg);
                    script.Append(errorMsg)
                          .AppendLine(item)
                          .AppendLine();
                }
            }

            // Assert
            case1.ShouldBeEmpty();
            Diff.Approve(script, ".sql", kind);
        }

        [TestMethod]
        public void Can_query_database()
        {
            // Arrange
            Contact[] results = null;
            var database = TestDatabase.CreateDatabase<Contact>();
            var records = AutoFaker.Generate<Contact>(100).ToArray();

            // Act
            if (database.State != ConnectionState.Open) database.Open();

            using (database)
            using (var command = database.CreateCommand())
            {
                if (!TestDatabase.TryExecute(database, SqlComposer.GenerateJoinedInsertStatements(records), out string errorMsg))
                    Assert.Fail(errorMsg);

                results = database.Select<Contact>($"select * from {nameof(Contact)};").ToArray();
            }

            // Assert
            results.ShouldNotBeEmpty();
            records.ShouldAllBe(x => !string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(x.Email));
            records.ShouldAllBe(x => x.TimeBorn != default);
        }

        [TestMethod]
        public void Can_build_select_statement()
        {
            // Arrange
            var sut = new QueryBuilder();

            // Act
            sut.SelectAll()
                .From("profile");
            var case1 = sut.ToString();

            sut.Select("id", "name")
                .From("profile")
                .Where($"id = 234");
            var case2 = sut.ToString();

            sut.OrderBy("name")
                .Limit(10);
            var case3 = sut.ToString(Language.TSQL);

            sut.Limit(5).Offset(10);
            var case4 = sut.ToString(Language.TSQL);

            var case5 = sut.ToString(Language.MySQL);

            var case6 = sut.ToString();

            // Assert
            case1.ShouldMatch(@"(?i)select\s+\*\s+from profile\s*;");
            case2.ShouldMatch(@"(?i)select\s+id, name\s+from profile\s+where id = 234\s+;");
            case3.ShouldMatch(@"(?i)select top 10\s+id, name\s+from profile\s+where id = 234\s+order by\s+name\s+;");
            case4.ShouldMatch(@"(?i)select\s+id, name\s+from profile\s+where id = 234\s+order by\s+name\s+offset \d+ rows fetch next \d+ rows only\s+;");
            case5.ShouldMatch(@"(?i)select\s+id, name\s+from profile\s+where id = 234\s+order by\s+name\s+limit \d+\s+offset \d+\s+;");
        }

        [TestMethod]
        public void Can_build_update_statement()
        {
            // Arrange
            var builder = new UpdateBuilder("foo");

            // Act
            builder.Set("id", 123)
                .Set("name", "abc")
                .Predicate("id", "test");
            var case1 = builder.ToString();

            builder.Predicate("id", "test").And("name", "jane");
            var case2 = builder.ToString();

            builder.Set("age", 12);
            var case3 = builder.ToString();

            // Assert
            case1.ShouldMatch(@"(?i)update foo set id='?123'?, name='abc' where id='test';");
            case2.ShouldMatch(@"(?i)update foo set id='?123'?, name='abc' where id='test' and name='jane';");
            case3.ShouldMatch(@"(?i)update foo set id='?123'?, name='abc', age='?12'? where id='test' and name='jane';");
        }

        [DataTestMethod]
        [DataRow("", "''")]
        [DataRow(22, "'22'")]
        [DataRow(null, "null")]
        [DataRow("foo", "'foo'")]
        [DataRow(12.54f, "'12.54'")]
        [DataRow(DayOfWeek.Friday, "'5'")]
        [DataRow("abc ' '' def", "'abc '' '''' def'")]
        [DataRow("2015-1-1 1:1:1", "'2015-01-01 01:01:01'")]
        public void Can_serialize_an_object_to_a_sql_value(object input, string expectedValue)
        {
            if (DateTime.TryParse(input?.ToString(), out DateTime dt))
                input = dt;

            SqlComposer.Serialize(input).ShouldBe(expectedValue);
        }

        #region Backing Members

        private static void TestScript(string scriptFile, Language syntax, out string results, out bool scriptExecutionWasSuccessful)
        {
            string schemaName = "dtp_scripting_test";
            using (var database = TestDatabase.CreateConnection(syntax, schemaName))
            {
                database.RebuildSchema(schemaName);

                results = File.ReadAllText(scriptFile);
                scriptExecutionWasSuccessful = TestDatabase.TryExecute(database, results, out string error);
                var nl = string.Concat(Enumerable.Repeat(Environment.NewLine, 3));
                results = string.Format("{0}SYNTAX: {1}{3}{2}", error, syntax, results, nl);
            }
        }

        private static Schema CreateSchemaInstance()
        {
            var schema = new Schema();
            schema.Add(
                new Table("zombie"),

                new Table("placeholder",
                    new Column("Id", SchemaType.INT, autoIncrement: true),
                    new Column("Name", SchemaType.VARCHAR, nullable: true)
                ),

                new Table("service",
                    new Column("Id", SchemaType.INT, autoIncrement: true),
                    new Column("Name", SchemaType.VARCHAR),
                    new Column("Subscribers", SchemaType.INT),
                    new Column("Zombie", SchemaType.VARCHAR),
                    new Column("Zombie_fk", SchemaType.INT),

                    new ForeignKey("Zombie_fk", "placeholder", "Id"),
                    new Index(IndexType.Index, new ColumnName("Subscribers"))
                )
                );

            return schema;
        }

        private static IEnumerable<object[]> GetMigrationCases()
        {
#if DEBUG
            const string pattern = "*";
#else
            const string pattern = "*";
#endif
            IEnumerable<string> caseFolder = Directory.EnumerateDirectories(Path.Combine(AppContext.BaseDirectory, "test-cases"), pattern, SearchOption.AllDirectories);

            var languages = new Language[]
            {
                Language.SQLite,
                Language.MySQL,
                Language.TSQL,
            };

            foreach (Language lang in languages)
                foreach (string folder in caseFolder)
                {
                    string label = Path.GetFileName(folder);
                    Schema oldSchema = Schema.Load(Path.Combine(folder, "old.xml"));
                    Schema newSchema = Schema.Load(Path.Combine(folder, "new.xml"));

                    yield return new object[] { label, lang, oldSchema, newSchema };
                }
        }

        #endregion Backing Members
    }
}
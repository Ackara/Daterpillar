using Acklann.Daterpillar.Linq;
using Acklann.Daterpillar.Prototyping;
using Acklann.Daterpillar.Scripting;
using Acklann.Daterpillar.Scripting.Writers;
using Acklann.Daterpillar.Serialization;
using ApprovalTests.Namers;
using AutoBogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class ScriptingTest
    {
        [ClassInitialize]
        public static void Setup(TestContext _)
        {
            SqlValidator.CreateDatabase(_languages);
        }

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

        [TestMethod]
        [DynamicData(nameof(GetInsertionCases), DynamicDataSourceType.Method)]
        public void Can_execute_insert_command(Modeling.IInsertable model, Language connectionType)
        {
            // Arrange
            var label = string.Concat(model.GetType().Name, '-', connectionType).ToLower();
            using var connection = SqlValidator.CreateConnection(connectionType);
            using var scenario = ApprovalTests.Namers.ApprovalResults.ForScenario(label);

            // Act
            var command = SqlComposer2.ToInsertCommand(model, connectionType);
            var result = SqlCommandHelper.ExecuteCommand(connection, command, connectionType);

            // Assert
            result.Changes.ShouldBe(1, result.ErrorMessage);
            result.ErrorCode.ShouldBe(0);
            result.ErrorMessage.ShouldBeNullOrEmpty();
            command.ShouldNotBeNullOrWhiteSpace();
        }

        [TestMethod]
        public void Can_fetch_database_records(Modeling.ISelectable model, Language connectionType)
        {
            // Arrange
            var label = string.Concat(model.GetType().Name, '-', connectionType).ToLower();
            using var connection = SqlValidator.CreateConnection(connectionType);
            using var scenario = ApprovalTests.Namers.ApprovalResults.ForScenario(label);

            // Act

            // Assert
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

        #region Backing Members

        private static readonly Language[] _languages = new Language[]
        {
            //Language.TSQL,
            //Language.SQLite,
            Language.MySQL,
        };

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

        private static IEnumerable<object[]> GetInsertionCases()
        {
            for (int i = 0; i < _languages.Length; i++)
            {
                yield return new object[] { AutoFaker.Generate<Artist>(), _languages[i] };
            }
        }

        #endregion Backing Members
    }
}
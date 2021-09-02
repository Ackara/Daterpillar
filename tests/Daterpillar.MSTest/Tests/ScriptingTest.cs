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
        [TestInitialize]
        public void Setup()
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
        [DynamicData(nameof(GetSampleRecords), DynamicDataSourceType.Method)]
        public void Can_execute_insert_command(Modeling.IInsertable model, Language connectionType)
        {
            // Arrange
            var label = string.Concat(model.GetType().Name, '-', connectionType).ToLower();
            using var connection = SqlValidator.CreateConnection(connectionType);
            using var scenario = ApprovalResults.ForScenario(label);

            // Act
            var command = SqlComposer.ToInsertCommand(model, connectionType);
            var result = SqlCommandHelper.ExecuteCommand(connection, command, connectionType);

            // Assert
            result.ErrorCode.ShouldBe(0, result.ErrorMessage);
            result.ErrorMessage.ShouldBeNullOrEmpty(result.ErrorMessage);
            result.Changes.ShouldBe(1, result.ErrorMessage);
            command.ShouldNotBeNullOrWhiteSpace();
        }

        [TestMethod]
        [DynamicData(nameof(GetLanguages), DynamicDataSourceType.Method)]
        public void Can_execute_select_command(Language connectionType)
        {
            // Arrange
            var model = AutoFaker.Generate<Artist>();

            var label = string.Concat(model.GetType().Name, '-', connectionType).ToLower();
            using var connection = SqlValidator.CreateConnection(connectionType);
            using var scenario = ApprovalResults.ForScenario(label);

            // Act
            var insertResult = SqlCommandHelper.Insert(connection, connectionType, model);
            if (insertResult == false) Assert.Fail(insertResult.ErrorMessage);

            var query = new QueryBuilder()
                .SelectAll()
                .From(model.GetTableName())
                .Where("Name", model.Name);

            var resultSet1 = SqlCommandHelper.Select<Artist>(connection, query);
            var resultSet2 = SqlCommandHelper.Select(connection, query, model.GetType());

            // Assert
            resultSet1.ErrorMessage.ShouldBeNullOrWhiteSpace(resultSet1.ErrorMessage);
            resultSet1.Data.ShouldNotBeNull();
            resultSet1.Data.ShouldAllBe(x => x.Name == model.Name);

            resultSet2.Data.ShouldNotBeNull();
            resultSet2.Data.ShouldBeAssignableTo(typeof(IEnumerable<Modeling.ISelectable>));
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
                .Where("id", "test");
            var case1 = builder.ToString();

            builder.Where("id", "test").And("name", "jane");
            var case2 = builder.ToString();

            builder.Set("age", 12);
            var case3 = builder.ToString();

            // Assert
            case1.ShouldMatch(@"(?i)update foo set id='?123'?, name='abc' where id='test';");
            case2.ShouldMatch(@"(?i)update foo set id='?123'?, name='abc' where id='test' and name='jane';");
            case3.ShouldMatch(@"(?i)update foo set id='?123'?, name='abc', age='?12'? where id='test' and name='jane';");
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
        [DataRow(typeof(System.Data.SqlClient.SqlConnection), Language.TSQL)]
        [DataRow(typeof(System.Data.SQLite.SQLiteConnection), Language.SQLite)]
        [DataRow(typeof(MySql.Data.MySqlClient.MySqlConnection), Language.MySQL)]
        public void Can_determine_language_from_the_connection_type(Type connectionType, Language excepectedValue)
        {
            Scripting.SqlCommandHelper.GetLanguage(connectionType).ShouldBe(excepectedValue);
        }

        #region Backing Members

        private static readonly Language[] _languages = new Language[]
        {
            //Language.TSQL,
            //Language.SQLite,
            Language.MySQL,
        };

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

        private static IEnumerable<object[]> GetSampleRecords()
        {
            for (int i = 0; i < _languages.Length; i++)
            {
                yield return new object[] { AutoFaker.Generate<Artist>(), _languages[i] };

                //yield return new object[] { }
            }
        }

        private static IEnumerable<object[]> GetLanguages()
        {
            foreach (Language item in _languages)
            {
                yield return new object[] { item };
            }
        }

        #endregion Backing Members
    }
}
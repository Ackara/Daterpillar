using Acklann.Daterpillar.Annotations;
using Acklann.Daterpillar.Modeling;
using Acklann.Daterpillar.Prototyping;
using Acklann.Daterpillar.Scripting.Writers;
using ApprovalTests.Namers;
using AutoBogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Index = Acklann.Daterpillar.Modeling.Index;
using SchemaType = Acklann.Daterpillar.Annotations.SchemaType;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class DdlOperationsTest
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
            var sut = new Modeling.Migrator();
            string fileName = $"{label}.{dialect}.sql".ToLower();
            string scriptFile = Path.Combine(Path.GetTempPath(), nameof(Daterpillar), nameof(DdlOperationsTest), fileName);

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
        [DataRow(typeof(System.Data.SqlClient.SqlConnection), Language.TSQL)]
        [DataRow(typeof(System.Data.SQLite.SQLiteConnection), Language.SQLite)]
        [DataRow(typeof(MySql.Data.MySqlClient.MySqlConnection), Language.MySQL)]
        public void Can_determine_language_from_the_connection_type(Type connectionType, Language excepectedValue)
        {
            Scripting.DbConnectionExtensions.GetLanguage(connectionType).ShouldBe(excepectedValue);
        }

        #region Backing Members

        private static readonly Language[] _languages = new Language[]
        {
            Language.TSQL,
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
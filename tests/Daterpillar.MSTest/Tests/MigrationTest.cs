using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Linq;
using Acklann.Daterpillar.Migration;
using Acklann.Daterpillar.Writers;
using Acklann.Diffa;
using ApprovalTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class MigrationTest
    {
        [ClassInitialize]
        public static void Setup(TestContext _)
        {
        }

        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataRow(Language.TSQL)]
        [DataRow(Language.MySQL)]
        [DataRow(Language.SQLite)]
        public void Can_create_new_database_schema(Language dialect)
        {
            // Arrange
            using var database = TestDatabase2.CreateConnection(dialect);
            TestDatabase2.ClearSchema(database, dialect);

            Schema schema = CreateSchema();

            // Act
            string sql;
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            {
                var writer = new SqlWriterFactory().CreateInstance(dialect, stream);
                writer.Create(schema);
                writer.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                sql = reader.ReadToEnd();
            }

            var result = TestDatabase.TryExecute(database, sql, out string error);

            // Assert
            sql.ShouldNotBeNullOrWhiteSpace();
            result.ShouldBeTrue(error);
        }

        [TestMethod]
        [DynamicData(nameof(GetMigrations), DynamicDataSourceType.Method)]
        public void Can_generate_migration_script(string label, Language dialect, Schema remote, Schema local)
        {
            // Arrange
            using var database = TestDatabase2.CreateConnection(dialect);
            using var scenario = ApprovalTests.Namers.ApprovalResults.ForScenario(label, dialect);

            string resultFile = Path.Combine(Path.GetTempPath(), $"daterpillar-{Guid.NewGuid()}.sql");

            var sut = new Migrator();

            // Act
            var results = sut.GenerateMigrationScript(dialect, remote, local, resultFile);
            var script = File.ReadAllText(resultFile);
            System.Diagnostics.Debug.WriteLine(script);
            var success = TestDatabase2.TryExecute2(database, script, out string error);

            // Assert
            success.ShouldBeTrue(error);
            results.ShouldNotBeNull();
            Approvals.VerifyFile(resultFile);
        }

        [TestMethod]
        [DataRow(Language.TSQL)]
        [DataRow(Language.MySQL)]
        [DataRow(Language.SQLite)]
        public void Can_generate_a_migration_script(Language syntax)
        {
            // Arrange
            var results = new Tuple<bool, string>[2];
            var baseDir = Path.Combine(Path.GetTempPath(), "dtp");
            var migrationsDir = Path.Combine(baseDir, "migrations");
            var snapshotFile = Path.Combine(baseDir, "snapshot.schema.xml");
            var activeFile = Path.Combine(baseDir, "active.schema.xml");
            var outFile = Path.Combine(migrationsDir, "V1.1__alter_schema.sql");

            var sut = new Migrator();
            var factory = new SqlWriterFactory();

            if (Directory.Exists(baseDir)) Directory.Delete(baseDir, recursive: true);
            Directory.CreateDirectory(baseDir);

            // Act
            // Case 1: First migration; left (snapshot) is empty.
            Sample.GetMusicXML().CopyTo(activeFile);
            Sample.GetMusicDataXML().CopyTo(Path.Combine(baseDir, Sample.File.MusicDataXML));

            var oldSchema = new Schema();
            if (Schema.TryLoad(activeFile, out Schema newSchema, out string errorMsg) == false)
                Assert.Fail(errorMsg);

            newSchema.Merge();
            var case1 = sut.GenerateMigrationScript(syntax, oldSchema, newSchema, outFile).Length;

            File.Copy(outFile, Path.Combine(migrationsDir, $"V1.0__init.{syntax}.sql"));

            // Case 2: No migrations/changes.
            newSchema.Merge();
            newSchema.Save(snapshotFile);
            oldSchema = Schema.Load(snapshotFile);
            var case2 = sut.GenerateMigrationScript(syntax, oldSchema, newSchema, outFile).Length;

            // Case 3: Migrations exists.
            if (Schema.TryLoad(Sample.GetMusicRevisionsXML().FullName, out Schema revisions, out errorMsg) == false)
                Assert.Fail(errorMsg);

            revisions.Save(activeFile);
            var case3 = sut.GenerateMigrationScript(syntax, oldSchema, revisions, outFile).Length;

            // === Results === //

            int index = 0;
            string schemaName = "dtp-migrate-test";
            using (var db = TestDatabase.CreateConnection(syntax, schemaName))
            {
                db.RebuildSchema(schemaName, false);
                foreach (var file in Directory.EnumerateFiles(migrationsDir))
                {
                    var script = File.ReadAllText(file);
                    var passed = db.TryExecute(script, out errorMsg);

                    if (!passed) script = (errorMsg + script);
                    script = $"file: {Path.GetFileName(file)}\n\n\n{script}";
                    results[index++] = Tuple.Create(passed, script);
                }
            }

            // Assert
            index = 0;
            foreach ((bool executionWasSuccessful, string script) in results)
            {
                script.ShouldNotBeNullOrEmpty($"Script {index + 1} is empty.");
                Diff.Approve(script, ".sql", syntax, ++index);
                executionWasSuccessful.ShouldBeTrue();
            }

            case1.ShouldBeGreaterThan(0, "The 1st migration failed.");
            case2.ShouldBe(0, "The 2nd migration failed.");
            case3.ShouldBeGreaterThan(0, "The 3rd migration failed.");
        }

        [TestMethod]
        public void Should_merge_referenced_schema_when_applied()
        {
            // Arrange
            var totalTablesBeforeMerge = 0;
            var inputFile = Sample.GetSakilaInventoryXML().FullName;

            var city = new Table("city",
                new Column("Population", new DataType(SchemaType.INT)), /* add */
                new Column("city_id", new DataType(SchemaType.SMALLINT), true), /* update */

                new ForeignKey("placeholder", "fake", "Id", ReferentialAction.Cascade, ReferentialAction.Cascade), /* add */
                new ForeignKey("country_id", "country", "country_id", ReferentialAction.Cascade, ReferentialAction.Cascade), /* update */

                new Index(IndexType.Index, new ColumnName("Population")), /* add */
                new Index(IndexType.Index, new ColumnName("country_id", Order.DESC)) /* update */
                );
            var revisions = new Schema() { Tables = new System.Collections.Generic.List<Table> { city } };

            // Act
            if (Schema.TryLoad(inputFile, out Schema schema))
            {
                totalTablesBeforeMerge = schema.Tables.Count;

                schema.Merge();
                schema.Merge(revisions);
            }
            var result = schema.Tables.Find(x => x.Name == city.Name);

            // Assert
            schema.ShouldNotBeNull();
            //schema.Imports.ShouldBeNull();
            totalTablesBeforeMerge.ShouldBeLessThan(schema.Tables.Count);

            result.Columns.Find(x => x.Name == city.Columns[0].Name).ShouldNotBeNull();
            result.Columns.Find(x => x.Name == city.Columns[1].Name).DataType.ShouldBe(new DataType(SchemaType.SMALLINT));

            result.ForeignKeys.Find(x => x.Name == city.ForeignKeys[0].Name).ShouldNotBeNull();
            result.ForeignKeys.Find(x => x.Name == city.ForeignKeys[1].Name).OnDelete.ShouldBe(ReferentialAction.Cascade);

            result.Indecies.Find(x => x.Name == city.Indecies[0].Name).ShouldNotBeNull();
            result.Indecies.Find(x => x.Name == city.Indecies[1].Name).Columns[0].Order.ShouldBe(Order.DESC);

            Diff.Approve(schema.ToXml(), ".xml");
        }

        [TestMethod]
        [DataRow(typeof(System.Data.SqlClient.SqlConnection), Language.TSQL)]
        [DataRow(typeof(System.Data.SQLite.SQLiteConnection), Language.SQLite)]
        [DataRow(typeof(MySql.Data.MySqlClient.MySqlConnection), Language.MySQL)]
        public void Can_determine_language_from_the_connection_type(Type connectionType, Language excepectedValue)
        {
            IDbConnectionExtensions.GetLanguage(connectionType).ShouldBe(excepectedValue);
        }

        // ==================== ENUMERATOR ==================== //

        [TestMethod]
        [DataRow(0, "")]
        [DataRow(1, "a")]
        [DataRow(2, "a b")]
        [DataRow(9, "a b")]
        [DataRow(3, "b a c d")]
        [DataRow(4, "b a d c")]
        [DataRow(5, "b c a d")]
        [DataRow(6, "a c b d")]
        [DataRow(7, "d a c b")]
        [DataRow(8, "a d c b")]
        public void Can_sort_tables_by_dependencies(int caseNo, string exceptedValue)
        {
            var sut = GetEnumeratorCase(caseNo);
            var results = string.Join(" ", sut.EnumerateTables().Select(x => x.Name));
            results.ShouldBe(exceptedValue);
        }

        #region Backing Members

        private static Schema CreateSchema()
        {
            string assemblyFile = typeof(MigrationTest).Assembly.Location;
            return SchemaFactory.CreateFrom(assemblyFile);
        }



        private static Schema GetEnumeratorCase(int index)
        {
            var a = new Table("a"); var b = new Table("b");
            var c = new Table("c"); var d = new Table("d");
            var e = new Table("e"); var f = new Table("f");

            void join(Table x, Table y) => x.Add(new ForeignKey("", y.Name, ""));

            var s = new Schema();

            switch (index)
            {
                case 0: return s;

                case 1:
                    s.Add(a);
                    return s;

                case 2:
                    s.Add(a, b);
                    return s;

                case 3:
                    s.Add(a, b, c, d);
                    join(a, b);
                    return s;

                case 4:
                    s.Add(a, b, c, d);
                    join(a, b); join(c, d);
                    return s;

                case 5:
                    s.Add(a, b, c, d);
                    join(a, b); join(a, c);
                    return s;

                case 6:
                    s.Add(a, b, c, d);
                    join(b, c); join(c, a);
                    return s;

                case 7:
                    s.Add(a, b, c, d);
                    join(a, d); join(b, c); join(c, a);
                    return s;

                case 8:
                    s.Add(a, b, c, d);
                    join(b, c); join(c, d);
                    join(b, a); join(c, a); join(d, a);
                    return s;

                case 9:
                    s.Add(a, b);
                    join(a, b); join(b, a);
                    return s;
            }

            throw new IndexOutOfRangeException();

        }

        private static IEnumerable<object[]> GetMigrations()
        {
            TestDatabase2.ClearSchema();

            var cases = new List<(string, Schema, Schema)>();
            cases.Add(("init", new Schema(), CreateSchema()));

            /* ********** */

            foreach ((string label, Schema remote, Schema local) in cases)
                foreach (var lang in TestDatabase2.GetSupportedLanguages())
                {
                    yield return new object[] { label, lang, remote, local };
                }
        }

        #endregion Backing Members
    }
}
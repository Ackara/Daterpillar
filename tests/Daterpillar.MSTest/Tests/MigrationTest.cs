using Acklann.Daterpillar.Writers;
using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Migration;
using Acklann.Diffa;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.IO;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class MigrationTest
    {
        [TestMethod]
        public void Can_build_a_schema_from_an_assembly()
        {
            // Arrange
            var assemblyFile = typeof(MigrationTest).Assembly.Location;

            // Act
            var schema = SchemaFactory.CreateFrom(assemblyFile);

            // Assert
            schema.Version.ShouldNotBeNullOrEmpty();
            Diff.Approve(schema, ".xml");
        }

        [TestMethod]
        public void Should_merge_referenced_schema_when_applied()
        {
            // Arrange
            var totalTablesBeforeMerge = 0;
            var inputFile = TestData.GetSakilaInventoryXML().FullName;

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
            schema.Import.ShouldBeNull();
            totalTablesBeforeMerge.ShouldBeLessThan(schema.Tables.Count);

            result.Columns.Find(x => x.Name == city.Columns[0].Name).ShouldNotBeNull();
            result.Columns.Find(x => x.Name == city.Columns[1].Name).DataType.ShouldBe(new DataType(SchemaType.SMALLINT));

            result.ForeignKeys.Find(x => x.Name == city.ForeignKeys[0].Name).ShouldNotBeNull();
            result.ForeignKeys.Find(x => x.Name == city.ForeignKeys[1].Name).OnDelete.ShouldBe(ReferentialAction.Cascade);

            result.Indecies.Find(x => x.Name == city.Indecies[0].Name).ShouldNotBeNull();
            result.Indecies.Find(x => x.Name == city.Indecies[1].Name).Columns[0].Order.ShouldBe(Order.DESC);

            Diff.Approve(schema, ".xml");
        }

        [DataTestMethod]
        [DataRow(Syntax.TSQL)]
        [DataRow(Syntax.MySQL)]
        [DataRow(Syntax.SQLite)]
        public void Can_generate_a_migration_script(Syntax syntax)
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
            TestData.GetMusicXML().CopyTo(activeFile);
            TestData.GetMusicDataXML().CopyTo(Path.Combine(baseDir, TestData.File.MusicDataXML));

            var oldSchema = new Schema();
            if (Schema.TryLoad(activeFile, out Schema newSchema, out string errorMsg) == false)
                Assert.Fail(errorMsg);

            newSchema.Merge();
            var case1 = sut.GenerateMigrationScript(outFile, oldSchema, newSchema, syntax).Length;

            File.Copy(outFile, Path.Combine(migrationsDir, $"V1.0__init.{syntax}.sql"));

            // Case 2: No migrations/changes.
            newSchema.Merge();
            newSchema.Save(snapshotFile);
            oldSchema = Schema.Load(snapshotFile);
            var case2 = sut.GenerateMigrationScript(outFile, oldSchema, newSchema, syntax).Length;

            // Case 3: Migrations exists.
            if (Schema.TryLoad(TestData.GetMusicRevisionsXML().FullName, out Schema revisions, out errorMsg) == false)
                Assert.Fail(errorMsg);

            revisions.Save(activeFile);
            var case3 = sut.GenerateMigrationScript(outFile, oldSchema, revisions, syntax).Length;

            // === Results === //

            int index = 0;
            using (var db = new Database(syntax, "dtp-migrate-test"))
            {
                db.Refresh();
                foreach (var file in Directory.EnumerateFiles(migrationsDir))
                {
                    var script = File.ReadAllText(file);
                    var passed = db.TryExecute(script, out errorMsg);

                    if (!passed) script = (errorMsg + script);
                    script = $"file: {Path.GetFileName(file)}\r\n\r\n\r\n{script}";
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

        //[TestMethod]
        public void Can_execute_migration_script_with_flyway()
        {
            // Arrange
            var baseDirectory = Path.Combine(Path.GetTempPath(), "dtp-exe-migrations");
            var database = new FileInfo(Path.Combine(baseDirectory, "dtp-test.db"));
            var migrationsFolder = Path.Combine(baseDirectory, "migrations");
            var connectionString = $"database={database.FullName};";

            // Act
            if (Directory.Exists(baseDirectory)) Directory.Delete(baseDirectory, recursive: true);
            Directory.CreateDirectory(migrationsFolder);
            TestData.GetInitSQL().CopyTo(Path.Combine(migrationsFolder, "init.sql"));
            database.Create().Dispose();

            var sizeBeforeMigrate = database.Length;
            Flyway.Migrate(Syntax.SQLite, connectionString, migrationsFolder);

            // Assert
            database.Length.ShouldBeGreaterThan(sizeBeforeMigrate);
        }

        [TestMethod]
        public void Should_download_flyway_when_not_installed()
        {
            // Arrange
            string version = "5.2.4";
            string baseDirectory = Path.Combine(Path.GetTempPath(), "dtp-flyway");
            string expectedPath = Path.Combine(baseDirectory, "flyway", version, "flyway");

            // Act
            //if (File.Exists(expectedPath)) Directory.Delete(baseDirectory, recursive: true);
            var flywayPath = Flyway.Install(version, baseDirectory);

            // Assert
            flywayPath.ShouldContain(expectedPath, Case.Insensitive);
            File.Exists(expectedPath).ShouldBeTrue($"{expectedPath} was not downloaded.");
        }
    }
}
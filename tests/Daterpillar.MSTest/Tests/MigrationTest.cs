using Acklann.Daterpillar.Configuration;
using Acklann.Diffa;
using Acklann.Diffa.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.IO;
using System.Linq;

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
            var sut = new Commands.BuildCommand(assemblyFile);

            // Act
            var resultFile = Path.ChangeExtension(assemblyFile, ".schema.xml");
            var exitCode = sut.Execute();

            // Assert
            exitCode.ShouldBe(0);
            Diff.ApproveFile(resultFile);
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
            if (Schema.TryLoad(inputFile, out Schema schema, out string errorMsg))
            {
                totalTablesBeforeMerge = schema.Tables.Count;

                schema.Merge();
                schema.Merge(revisions);
            }
            var result = schema.Tables.Find(x => x.Name == city.Name);

            // Assert
            schema.ShouldNotBeNull();
            schema.Include.ShouldBeNull();
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

            var sut = new Commands.MigrateCommand(snapshotFile, activeFile, migrationsDir, "1.1",
                syntax: syntax,
                fileNameFormat: "V{0}__Update.{2}.sql",
                omitDropStatements: false
                );

            if (Directory.Exists(baseDir)) Directory.Delete(baseDir, recursive: true);
            Directory.CreateDirectory(baseDir);

            // Act
            // Case 1: First migration; left (snapshot) is empty.
            TestData.GetMusicXML().CopyTo(activeFile);
            TestData.GetMusicDataXML().CopyTo(Path.Combine(baseDir, TestData.File.MusicDataXML));

            if (Schema.TryLoad(activeFile, out Schema schema, out string errorMsg) == false)
                Assert.Fail(errorMsg);

            var exitCode1 = sut.Execute();
            var outFile = Directory.EnumerateFiles(migrationsDir).First();
            File.Copy(outFile, Path.Combine(migrationsDir, $"V1.0__init.{syntax}.sql"));

            // Case 2: No migrations/changes.
            schema.Merge();
            schema.Save(snapshotFile);
            var exitCode2 = sut.Execute();

            // Case 3: Migrations exists.
            if (Schema.TryLoad(TestData.GetMusicRevisionsXML().FullName, out Schema revisions, out errorMsg) == false)
                Assert.Fail(errorMsg);

            revisions.Save(activeFile);
            var exitCode3 = sut.Execute();

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

            exitCode1.ShouldBe(0, "The 1st migration failed.");
            exitCode2.ShouldBe(0, "The 2nd migration failed.");
            exitCode3.ShouldBe(0, "The 3rd migration failed.");
        }

        //[DataTestMethod]
        [DataRow(Syntax.SQLite, null, null, null)]
        [Use(typeof(FileReporter), doNotPauseIfTestFails: true)]
        public void Can_generate_a_migration_script_from(Syntax syntax, string activeFile, string snapshotFile, string connectionStirng)
        {
            // Arrange
            var baseDir = Path.Combine(Path.GetTempPath(), "dtp-debug-env");
            var migrationDir = Path.Combine(baseDir, "migrations");
            var oldSchema = Path.Combine(baseDir, "old.schema.xml");
            var newSchema = Path.Combine(baseDir, "new.schema.xml");

            var sut = new Commands.MigrateCommand(
                oldSchema,
                newSchema,
                migrationDir,
                "1.1",
                syntax);

            // Act
            if (Directory.Exists(baseDir)) Directory.Delete(baseDir, true);
            Directory.CreateDirectory(baseDir);

            if (File.Exists(snapshotFile))
                File.Copy(snapshotFile, oldSchema, true);

            if (File.Exists(activeFile))
                File.Copy(activeFile, newSchema, true);
            else
                File.WriteAllText(newSchema, (new Schema().ToString()));

            var exitCode = sut.Execute();

            using (var db = new Database(syntax, connectionStirng))
            {
                bool pass = db.TryExecute("", out string errorMsg);
            }

            // Assert
            exitCode.ShouldBe(0);
        }
    }
}
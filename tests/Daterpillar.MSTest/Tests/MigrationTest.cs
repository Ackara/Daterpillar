using Acklann.Daterpillar.Configuration;
using Acklann.Diffa;
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
                fileNameFormat: "V{0}__Update.{2}.sql",
                syntax: syntax,
                omitDropStatements: false
                );

            // Act
            /* Case 1: No migrations. */
            if (Directory.Exists(baseDir)) Directory.Delete(baseDir, recursive: true);
            Directory.CreateDirectory(baseDir);
            TestData.GetMusicXML().CopyTo(activeFile);
            if (Schema.TryLoad(activeFile, out Schema schema, out string errorMsg) == false)
                Assert.Fail(errorMsg);

            var exitCode1 = sut.Execute();
            var outFile = Directory.EnumerateFiles(migrationsDir).First();
            File.Copy(outFile, Path.Combine(migrationsDir, $"V1.0__init.{syntax}.sql"));

            /* Case 2: Migrations already exists. */
            schema.Merge();
            schema.Save(snapshotFile);

            if (Schema.TryLoad(TestData.GetMusicRevisionsXML().FullName, out Schema revisions, out errorMsg) == false)
                Assert.Fail(errorMsg);

            revisions.Save(schema.Path);
            var exitCode2 = sut.Execute();

            /* Case 3: Clean*/

            /* === Results === */

            int index = 0;
            foreach (var file in Directory.EnumerateFiles(migrationsDir))
            {
                using (var db = new Database(syntax))
                {
                    db.Refresh();

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
                Diff.Approve(script, ".sql", syntax, ++index);
                executionWasSuccessful.ShouldBeTrue();
            }

            exitCode1.ShouldBe(0, "The 1st migration failed.");
            exitCode2.ShouldBe(0, "The 2nd migration failed.");
        }
    }
}
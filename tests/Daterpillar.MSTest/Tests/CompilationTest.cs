using Acklann.Daterpillar.Configuration;
using Acklann.Diffa;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.IO;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class CompilationTest
    {
        [TestMethod]
        public void Can_build_a_schema_from_an_assembly()
        {
            // Arrange
            var assemblyFile = typeof(CompilationTest).Assembly.Location;
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

        [TestMethod]
        public void Can_generate_a_migration_script()
        {
            // Arrange
            var migrationsDir = Path.Combine(Path.GetTempPath(), "dtp-migrations");
            var snapshotFile = Path.Combine(migrationsDir, "snapshot.schema.xml");

            if (Schema.TryLoad(TestData.GetMusicXML().FullName, out Schema schema, out string errorMsg) == false)
                throw new System.Xml.Schema.XmlSchemaValidationException(errorMsg);

            if (Schema.TryLoad(TestData.GetMusicRevisionsXML().FullName, out Schema revisions, out errorMsg) == false)
                throw new System.Xml.Schema.XmlSchemaValidationException(errorMsg);

            var sut = new Commands.MigrateCommand(snapshotFile, schema.Path, "1.1", migrationsDir, Syntax.Generic, "V{0}__Update_{1:m.s}.sql");

            // Act

            /* Case 1: No migrations. */
            if (Directory.Exists(migrationsDir)) Directory.Delete(migrationsDir, recursive: true);
            if (File.Exists(snapshotFile)) File.Delete(snapshotFile);
            var exitCode1 = sut.Execute();

            /* Case 2: Migrations already exists. */
            revisions.Save(schema.Path);
            var exitCode2 = sut.Execute();

            // Assert
            exitCode1.ShouldBe(0);
            exitCode2.ShouldBe(0);

            int idx = 0;
            foreach (var file in Directory.EnumerateFiles(migrationsDir))
            {
                Diff.ApproveFile(file, idx++);
            }
        }
    }
}
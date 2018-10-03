using Acklann.Daterpillar.Configuration;
using Acklann.Diffa;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.IO;
using System.Text;

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
            var error = new StringBuilder();
            var inputFile = TestData.GetSakilaInventoryXML().FullName;

            var city = new Table("city",
                new Column("Population", new DataType(SchemaType.INT)), /* add */
                new Column("city_id", new DataType(SchemaType.SMALLINT)), /* update */

                new ForeignKey("placeholder", "fake", "Id", ReferentialAction.Cascade, ReferentialAction.Cascade), /* add */
                new ForeignKey("country_id", "country", "country_id", ReferentialAction.Cascade, ReferentialAction.Cascade), /* update */

                new Index(IndexType.Index, new ColumnName("Population")), /* add */
                new Index(IndexType.Index, new ColumnName("country_id", Order.DESC)) /* update */
                );
            var revisions = new Schema() { Tables = new System.Collections.Generic.List<Table> { city } };

            // Act
            if (Schema.TryLoad(inputFile, out Schema schema, out string errorMsg))
            {
                schema.Merge();
                schema.Merge(revisions);
            }
            var result = schema.Tables.Find(x => x.Name == city.Name);

            // Assert
            schema.ShouldNotBeNull();
            schema.Include.ShouldBeNull();

            result.Columns.Find(x => x.Name == city.Columns[0].Name).ShouldNotBeNull();
            result.Columns.Find(x => x.Name == city.Columns[1].Name).DataType.ShouldBe(new DataType(SchemaType.SMALLINT));

            result.ForeignKeys.Find(x => x.Name == city.ForeignKeys[0].Name).ShouldNotBeNull();
            result.ForeignKeys.Find(x => x.Name == city.ForeignKeys[1].Name).OnDelete.ShouldBe(ReferentialAction.Cascade);

            result.Indexes.Find(x => x.Name == city.Indexes[0].Name).ShouldNotBeNull();
            result.Indexes.Find(x => x.Name == city.Indexes[1].Name).Columns[0].Order.ShouldBe(Order.DESC);

            Diff.Approve(schema, ".xml");
        }
    }
}
using Ackara.Daterpillar;
using Ackara.Daterpillar.Migration;
using Ackara.Daterpillar.Scripting;
using ApprovalTests;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Data;
using System.Linq;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(FName.X86)]
    [DeploymentItem(FName.Samples)]
    public class SchemaComparerTest
    {
        [TestMethod]
        public void Compare_should_report_that_the_specified_identical_schemas_are_equal()
        {
            // Arrange
            var source = MockData.GetSchema(FName.scriptingTest_partial_schemaXML);
            var target = MockData.GetSchema(FName.scriptingTest_partial_schemaXML);

            var builder = CreateNewScriptBuilder();
            var sut = new SchemaComparer();

            // Act
            var result = sut.Compare(source, target, builder);
            var changeScript = builder.GetContent();

            // Assert
            result.ShouldBe(MigrationState.NoChanges);
            changeScript.ShouldBeNullOrEmpty();
        }

        [TestMethod]
        public void Compare_should_report_that_the_specified_non_identical_schemas_are_not_equal()
        {
            // Arrange
            var source = MockData.GetSchema(FName.scriptingTest_partial_schemaXML);
            var target = MockData.GetSchema(FName.scriptingTest_partial_schemaXML);

            // add
            source.Add(
                new Table("product_type",
                    new Column("Id", new DataType("int"), autoIncrement: true),
                    new Column("Name", new DataType("varchar", 32)),
                    new Index(IndexType.Index, new ColumnName("Name"))));

            var card = source.Tables.First(x => x.Name == "card");
            card.Indexes.Add(new Index(IndexType.Index, "Level") { Table = card });

            var pack = source.Tables.First(x => x.Name == "pack");
            pack.Columns.Add(new Column("Product_Type_Id", new DataType("int")) { Table = pack });
            pack.ForeignKeys.Add(new ForeignKey("Product_Type_Id", "pack", "Id") { Table = pack });

            // remove
            var attribute = source.Tables.First(x => x.Name == "attribute");
            attribute.Indexes.RemoveAll(x => x.Type == IndexType.Index);

            source.Tables.RemoveAll(x => x.Name == "effect");
            pack.Columns.RemoveAll(x => x.Name == "Size");

            // edit
            var fKey = card.ForeignKeys.First(x => x.ForeignTable.Contains("monster_type"));
            fKey.OnUpdate = ReferentialAction.NoAction;
            fKey.OnDelete = ReferentialAction.NoAction;

            var builder = CreateNewScriptBuilder();
            var sut = new SchemaComparer();

            // Act
            MigrationState result;
            string changeScript, errorMsg;
            bool changeScriptRanSuccessfully;

            using (var connection = GetNewConnection())
            {
                result = sut.Compare(source, target, builder);
                changeScript = builder.GetContent();

                connection.UseEmptyDatabase();
                builder.Clear();
                builder.Append(target);
                bool dbInitScriptFailed = !ConnectionFactory.TryExecuteScript(connection, builder.GetContent(), out errorMsg);
                if (dbInitScriptFailed) Assert.Fail(errorMsg);

                changeScriptRanSuccessfully = ConnectionFactory.TryExecuteScript(connection, changeScript, out errorMsg);
                changeScript = (changeScriptRanSuccessfully ? changeScript : string.Format("/*{0}{1}*/{0}{2}", Environment.NewLine, errorMsg, changeScript));
            }

            // Assert
            result.ShouldBe(MigrationState.PendingChanges);
            Approvals.Verify(changeScript);
            changeScriptRanSuccessfully.ShouldBeTrue(errorMsg);
        }

        [TestMethod]
        public void Compare_should_report_that_the_source_schema_is_empty_when_it_is()
        {
            // Arrange
            var source = new Schema();
            var target = new Schema();
            target.Add(new Table("tbl",
                new Column("Id", new DataType("int"), autoIncrement: true),
                new Column("Name", new DataType("varchar", 32))));

            var builder = CreateNewScriptBuilder();
            var sut = new SchemaComparer();

            // Act
            MigrationState result;
            string changeScript, errorMsg;
            bool changeScriptRanSuccessfully;
            using (var connection = GetNewConnection())
            {
                result = sut.Compare(source, target, builder);
                changeScript = builder.GetContent();

                connection.UseEmptyDatabase();
                builder.Clear();
                builder.Append(target);
                bool dbInitScriptFailed = !ConnectionFactory.TryExecuteScript(connection, builder.GetContent(), out errorMsg);
                if (dbInitScriptFailed) Assert.Fail(errorMsg);

                changeScriptRanSuccessfully = ConnectionFactory.TryExecuteScript(connection, changeScript, out errorMsg);
                changeScript = (changeScriptRanSuccessfully ? changeScript : string.Format("/*{0}{1}*/{0}{2}", Environment.NewLine, errorMsg, changeScript));
            }

            // Assert
            result.HasFlag(MigrationState.SourceIsEmpty).ShouldBeTrue();
            Approvals.Verify(changeScript);
            changeScriptRanSuccessfully.ShouldBeTrue(errorMsg);
        }

        [TestMethod]
        public void Compare_should_report_that_the_target_schema_is_empty_when_it_is()
        {
            // Arrange
            var source = MockData.GetSchema(FName.scriptingTest_partial_schemaXML);
            var target = new Schema();

            var builder = CreateNewScriptBuilder();
            var sut = new SchemaComparer();

            // Act
            MigrationState result;
            string changeScript, errorMsg;
            bool changeScriptRanSuccessfully;
            using (var connection = GetNewConnection())
            {
                result = sut.Compare(source, target, builder);
                changeScript = builder.GetContent();

                connection.UseEmptyDatabase();
                changeScriptRanSuccessfully = ConnectionFactory.TryExecuteScript(connection, changeScript, out errorMsg);
                changeScript = (changeScriptRanSuccessfully ? changeScript : string.Format("/*{0}{1}*/{0}{2}", Environment.NewLine, errorMsg, changeScript));
            }

            // Assert
            result.HasFlag(MigrationState.TargetIsEmpty).ShouldBeTrue();
            Approvals.Verify(changeScript);
            changeScriptRanSuccessfully.ShouldBeTrue(errorMsg);
        }

        #region Helper Methods

        private static IDbConnection GetNewConnection() => ConnectionFactory.CreateMySQLConnection("dtpl_compare");

        private static IScriptBuilder CreateNewScriptBuilder()
        {
            return new MySQLScriptBuilder(new SqlScriptBuilderSettings()
            {
                ShowHeader = false,
                IgnoreScripts = true,
                IgnoreComments = true
            });
        }

        #endregion Helper Methods
    }
}
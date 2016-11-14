using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.Migration;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Linq;
using Tests.Daterpillar.Constants;
using Tests.Daterpillar.Helpers;

namespace Test.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(SampleData.Folder)]
    [DeploymentItem(KnownFile.DbConfig)]
    public class SqlComparerTest
    {
        // MySQL

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void Comparer_should()
        {
            using (var connection = DatabaseHelper.CreateMySQLConnection())
            {
                RunSqlComparerEqualsTest(connection, new MySQLScriptBuilder(), new SqlDiff()
                {
                    Changes = 0,
                    Summary = SqlDiffSummary.Equal
                });
            }
        }

        // T-SQL

        #region Private Members

        private void RunSqlComparerTest(IDbConnection connection, IScriptBuilder builder, SqlDiff expected)
        {
            // Arrange
            var sut = new SqlComparer();

            var source = Schema.Load(SampleData.GetFile(KnownFile.MockSchema3XML).OpenRead());
            var target = Schema.Load(SampleData.GetFile(KnownFile.MockSchema3XML).OpenRead());

            var table1 = source.Tables.First(x => x.Name == "table1");
            table1.Columns[1].Name = "FullName";

            var table2 = source.Tables.First(x => x.Name == "table2");
            table2.CreateColumn("Id", new DataType("int"), autoIncrement: true);
            table2.CreateColumn("Name", new DataType("varchar", 64));
            table2.CreateColumn("OtherId", new DataType("int"));
            table2.CreateIndex("other_id_idx", IndexType.Index, false, new IndexColumn("OtherId"));
            table2.CreateForeignKey("OtherId", "table1", "Id");

            var table3 = source.CreateTable("table3");
            table3.CreateColumn("Id", new DataType("int"), true);
            table3.CreateColumn("Name", new DataType("varchar"), true);

            source.RemoveTable("table4");

            // Act
            DatabaseHelper.TryDropDatabase(connection, source.Name);
            DatabaseHelper.CreateSchema(connection, builder, source);

            DatabaseHelper.TryDropDatabase(connection, target.Name);
            DatabaseHelper.CreateSchema(connection, builder, target);

            var results = sut.Compare(builder, source, target);

            // Assert
            Assert.AreEqual(expected, results);
        }

        private void RunSqlComparerEqualsTest(IDbConnection connection, IScriptBuilder builder, SqlDiff expected)
        {
            // Arrange
            var sut = new SqlComparer();
            var source = Schema.Load(SampleData.GetFile(KnownFile.MockSchema1XML).OpenRead());
            var target = Schema.Load(SampleData.GetFile(KnownFile.MockSchema1XML).OpenRead());

            source.Name = "daterpillar1";
            target.Name = "daterpillar2";

            // Act
            DatabaseHelper.TryDropDatabase(connection, source.Name);
            DatabaseHelper.CreateSchema(connection, builder, source);

            DatabaseHelper.TryDropDatabase(connection, target.Name);
            DatabaseHelper.CreateSchema(connection, builder, target);

            var results = sut.Compare(builder, source, target);

            // Assert
            Assert.AreEqual(expected, results);
        }

        #endregion Private Members
    }
}
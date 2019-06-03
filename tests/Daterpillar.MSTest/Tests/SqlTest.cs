using Acklann.Daterpillar.Fakes;
using Acklann.Daterpillar.Linq;
using Acklann.Diffa;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Data;
using System.Linq;
using System.Text;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class SqlTest
    {
        [TestMethod]
        public void Can_query_database()
        {
            // Arrange
            Contact[] results = null;
            var database = MockDatabase.CreateDatabase<Contact>();
            var records = Sample.CreateInstances<Contact>(100).ToArray();

            // Act
            if (database.State != ConnectionState.Open) database.Open();

            using (database)
            using (var command = database.CreateCommand())
            {
                if (!database.TryExecute(SqlComposer.GenerateJoinedInsertStatements(records), out string errorMsg))
                    Assert.Fail(errorMsg);

                results = database.Select<Contact>($"select * from {nameof(Contact)};").ToArray();
            }

            // Assert
            results.ShouldNotBeEmpty();
            records.ShouldAllBe(x => !string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(x.Email));
            records.ShouldAllBe(x => x.TimeBorn != default);
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

            // Assert
            case1.ShouldMatch(@"(?i)select\s+\*\s+from profile\s*;");
            case2.ShouldMatch(@"(?i)select\s+id, name\s+from profile\s+where id = 234\s+;");
            case3.ShouldMatch(@"(?i)select top 10\s+id, name\s+from profile\s+where id = 234\s+order by\s+name\s+;");
            case4.ShouldMatch(@"(?i)select\s+id, name\s+from profile\s+where id = 234\s+order by\s+name\s+offset \d+ rows fetch next \d+ rows only\s+;");
            case5.ShouldMatch(@"(?i)select\s+id, name\s+from profile\s+where id = 234\s+order by\s+name\s+limit \d+\s+offset \d+\s+;");
        }

        [DataTestMethod]
        [DataRow(Language.TSQL)]
        [DataRow(Language.MySQL)]
        [DataRow(Language.SQLite)]
        public void Can_generate_insert_commands(Language kind)
        {
            // Arrange
            var script = new StringBuilder();
            var tableName = "foo";
            var createStatement = $"create table {tableName}(id int, name varchar(64), age int);";
            var columns = new string[] { "id", "name", "age" };
            var values1 = new object[] { 1, "sally", 12 };
            var values2 = new object[] { 2, "peggy", 21 };
            var values3 = new object[] { 3, "don", 40 };

            var sample1 = A.Fake<IEntity>();
            A.CallTo(() => sample1.GetTableName()).Returns(tableName);
            A.CallTo(() => sample1.GetColumnList()).Returns(columns);

            var sample2 = A.Fake<IEntity>();
            A.CallTo(() => sample2.GetTableName()).Returns(tableName);
            A.CallTo(() => sample2.GetColumnList()).Returns(columns);

            // Act
            var case1 = SqlComposer.GenerateInsertStatements();

            A.CallTo(() => sample1.GetValueList()).Returns(new object[] { 1, "'sally'", 12 });
            var case2 = SqlComposer.GenerateInsertStatements(sample1);

            A.CallTo(() => sample1.GetValueList()).Returns(new object[] { 2, "'mark'", 21 });
            A.CallTo(() => sample2.GetValueList()).Returns(new object[] { 3, "'mary'", 25 });
            var case3 = SqlComposer.GenerateInsertStatements(sample1, sample2);

            A.CallTo(() => sample1.GetValueList()).Returns(new object[] { 4, "'jim'", 40 });
            A.CallTo(() => sample2.GetValueList()).Returns(new object[] { 5, "'sal'", 50 });
            var case4 = SqlComposer.GenerateJoinedInsertStatements(sample1, sample2);

            using (var connection = MockDatabase.CreateConnection(kind))
            {
                System.Diagnostics.Debug.WriteLine($"connection: {connection.ConnectionString}");
                connection.TryExecute($"drop table {tableName};", out string errorMsg);
                bool failed = !connection.TryExecute(createStatement, out errorMsg);
                if (failed) Assert.Fail($"Failed to create {tableName} table.\n\n{errorMsg}");

                var separator = string.Concat(Enumerable.Repeat('=', 50));
                foreach (var item in (case2.Concat(case3).Append(case4)))
                {
                    connection.TryExecute(item, out errorMsg);
                    script.Append(errorMsg)
                          .AppendLine(item)
                          .AppendLine();
                }
            }

            // Assert
            case1.ShouldBeEmpty();
            Diff.Approve(script, ".sql", kind);
        }

        [DataTestMethod]
        [DataRow("", "''")]
        [DataRow(22, "'22'")]
        [DataRow(null, "null")]
        [DataRow("foo", "'foo'")]
        [DataRow(12.54f, "'12.54'")]
        [DataRow(DayOfWeek.Friday, "'5'")]
        [DataRow("abc ' '' def", "'abc '' '''' def'")]
        [DataRow("2015-1-1 1:1:1", "'2015-01-01 01:01:01'")]
        public void Can_serialize_an_object_to_a_sql_value(object input, string expectedValue)
        {
            if (DateTime.TryParse(input?.ToString(), out DateTime dt))
                input = dt;

            SqlComposer.Serialize(input).ShouldBe(expectedValue);
        }
    }
}
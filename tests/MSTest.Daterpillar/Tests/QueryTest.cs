using Acklann.Daterpillar;
using Acklann.Daterpillar.Linq;
using ApprovalTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Daterpillar.Fake;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    public class QueryTest
    {
        [TestMethod]
        public void ToString_should_return_an_empty_string_when_query_is_instantiated()
        {
            var query = new Query();
            Assert.AreEqual(string.Empty, query.ToString());
        }

        [TestMethod]
        public void ToString_should_return_a_query_with_a_select_and_from_clause_only()
        {
            var sut = new Query()
                .Select("Id", "Name")
                .From("Employee");

            Approvals.Verify(sut.ToString());
        }

        [TestMethod]
        public void ToString_should_return_a_query_with_a_where_clause()
        {
            var sut = new Query()
                .Select("Name")
                .From("tableA", "tableB")
                .Where("Id = 3");

            Approvals.Verify(sut.ToString());
        }

        [TestMethod]
        public void ToString_should_return_query_with_a_groupBy_clause()
        {
            var sut = new Query()
                .SelectAll()
                .From("tableA")
                .GroupBy("Name", "Age");

            Approvals.Verify(sut.ToString());
        }

        [TestMethod]
        public void ToString_should_return_a_query_with_a_orderBy_clause()
        {
            var sut = new Query()
                .SelectAll()
                .From("tableA")
                .OrderBy("Name", "Age");

            Approvals.Verify(sut.ToString());
        }

        [TestMethod]
        public void ToString_should_return_a_query_with_a_limit_clause()
        {
            var sut = new Query()
                .SelectAll()
                .From("tableA")
                .Limit(10);

            Approvals.Verify(sut.ToString());
        }

        [TestMethod]
        public void ToString_should_return_a_query_with_a_top_clause()
        {
            var sut = new Query(Syntax.MSSQL)
                .SelectAll().Top(10)
                .From("tableA");

            Approvals.Verify(sut.ToString());
        }

        [TestMethod]
        public void ToString_should_return_a_mysql_query_when_the_appropriate_enum_is_supplied()
        {
            var sut = new Query(Syntax.MySQL)
                .Select("Id", "Name")
                .From("tableA", "tableB")
                .Where("Id = 12")
                .GroupBy("Name")
                .OrderBy("Age")
                .Limit(100);

            Approvals.Verify(sut.ToString());
        }

        [TestMethod]
        public void ToString_should_return_a_sqlite_query_when_the_appropriate_enum_is_supplied()
        {
            var sut = new Query(Syntax.SQLite)
                .Select("Id", "Name")
                .From("tableA", "tableB")
                .Where("")
                .GroupBy("Name")
                .OrderBy("Age")
                .Limit(100);

            Approvals.Verify(sut.ToString());
        }

        [TestMethod]
        public void ToString_should_return_a_minified_non_mssql_query()
        {
            var sut = new Query()
                .SelectAll()
                .From("tableA", "tableB")
                .Where("Id = 101")
                .GroupBy("Name")
                .OrderBy("Age")
                .Limit(100);

            Approvals.Verify(sut.ToString(minify: true));
        }

        [TestMethod]
        public void ToString_should_return_a_minified_mssql_query()
        {
            var sut = new Query(Syntax.MSSQL)
                .Select("Id", "Name")
                .From("tableA", "tableB")
                .Where("Id = 22")
                .GroupBy("Name", "Age")
                .OrderBy("Name", "Age");

            Approvals.Verify(sut.ToString(minify: true));
        }

        [TestMethod]
        public void ToString_should_return_a_sql_query_when_linq_expressions_are_passed()
        {
            var query = new Query<SimpleTable>(Syntax.MSSQL)
                .Select(x => new { x.Id, x.Name, x.Sex })
                .From()
                .Where(x => x.Amount >= 1.25M && x.Sex == "m")
                .GroupBy(x => x.Sex)
                .OrderBy(x => x.Amount);

            Approvals.Verify(query);
        }

        [TestMethod]
        public void ToString_should_return_a_mysql_query_when_linq_expressions_are_passed()
        {
            var sample = new SimpleTable() { Id = 23 };

            var query = new Query<SimpleTable>(Syntax.MySQL)
                .Select(x => new { x.Id, x.Name, x.Sex })
                .From()
                .Where(sample, x => x.Id == sample.Id)
                .GroupBy(x => x.Sex)
                .OrderBy(x => x.Amount);

            Approvals.Verify(query);
        }
    }
}
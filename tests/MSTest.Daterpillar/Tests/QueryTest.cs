using Ackara.Daterpillar;
using Ackara.Daterpillar.Linq;
using ApprovalTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    public class QueryTest
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        [TestMethod]
        public void GetValue_should_return_an_empty_string_when_query_is_instantiated()
        {
            var query = new Query();
            Assert.AreEqual(string.Empty, query.GetValue());
        }

        [TestMethod]
        public void GetValue_should_return_a_query_with_a_select_and_from_clause_only()
        {
            var sut = new Query()
                .Select("Id", "Name")
                .From("Employee");

            Approvals.Verify(sut.GetValue());
        }

        [TestMethod]
        public void GetValue_should_return_a_query_with_a_where_clause()
        {
            var sut = new Query()
                .Select("Name")
                .From("tableA", "tableB")
                .Where("Id = 3");

            Approvals.Verify(sut.GetValue());
        }

        [TestMethod]
        public void GetValue_should_return_query_with_a_groupBy_clause()
        {
            var sut = new Query()
                .SelectAll()
                .From("tableA")
                .GroupBy("Name", "Age");

            Approvals.Verify(sut.GetValue());
        }

        [TestMethod]
        public void GetValue_should_return_a_query_with_a_orderBy_clause()
        {
            var sut = new Query()
                .SelectAll()
                .From("tableA")
                .OrderBy("Name", "Age");

            Approvals.Verify(sut.GetValue());
        }

        [TestMethod]
        public void GetValue_should_return_a_query_with_a_limit_clause()
        {
            var sut = new Query()
                .SelectAll()
                .From("tableA")
                .Limit(10);

            Approvals.Verify(sut.GetValue());
        }

        [TestMethod]
        public void GetValue_should_return_a_query_with_a_top_clause()
        {
            var sut = new Query(Syntax.MSSQL)
                .SelectAll().Top(10)
                .From("tableA");

            Approvals.Verify(sut.GetValue());
        }

        [TestMethod]
        public void GetValue_should_return_a_mysql_query_when_the_appropriate_enum_is_supplied()
        {
            var sut = new Query(Syntax.MySQL)
                .Select("Id", "Name")
                .From("tableA", "tableB")
                .Where("Id = 12")
                .GroupBy("Name")
                .OrderBy("Age")
                .Limit(100);

            Approvals.Verify(sut.GetValue());
        }

        [TestMethod]
        public void GetValue_should_return_a_sqlite_query_when_the_appropriate_enum_is_supplied()
        {
            var sut = new Query(Syntax.SQLite)
                .Select("Id", "Name")
                .From("tableA", "tableB")
                .Where("")
                .GroupBy("Name")
                .OrderBy("Age")
                .Limit(100);

            Approvals.Verify(sut.GetValue());
        }

        [TestMethod]
        public void GetValue_should_return_a_minified_non_mssql_query()
        {
            var sut = new Query()
                .SelectAll()
                .From("tableA", "tableB")
                .Where("Id = 101")
                .GroupBy("Name")
                .OrderBy("Age")
                .Limit(100);

            Approvals.Verify(sut.GetValue(minify: true));
        }

        [TestMethod]
        public void GetValue_should_return_a_minified_mssql_query()
        {
            var sut = new Query(Syntax.MSSQL)
                .Select("Id", "Name")
                .From("tableA", "tableB")
                .Where("Id = 22")
                .GroupBy("Name", "Age")
                .OrderBy("Name", "Age");

            Approvals.Verify(sut.GetValue(minify: true));
        }
    }
}
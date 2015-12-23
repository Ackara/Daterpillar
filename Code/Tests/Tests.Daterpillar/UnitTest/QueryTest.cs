using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar.Data.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [UseApprovalSubdirectory(Str.ApprovalsDir)]
    [UseReporter(typeof(FileLauncherReporter), typeof(ClipboardReporter))]
    public class QueryTest
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        /// <summary>
        /// Assert <see cref="Query.GetValue(bool)"/> returns an empty string when instantiated.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ReturnEmptyString()
        {
            var query = new Query();
            Assert.AreEqual(string.Empty, query.GetValue());
        }

        /// <summary>
        /// Assert <see cref="Query.GetValue(bool)"/> returns a formatted SQL query after both <see cref="Query.Select(string[])"/> and <see cref="Query.From(string[])"/> is called.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ReturnSelectQuery()
        {
            var sut = new Query()
                .Select("Id", "Name")
                .From("Employee");

            Approvals.Verify(sut.GetValue());
        }

        /// <summary>
        /// Assert <see cref="Query.GetValue(bool)"/> returns a formatted query after <see cref="Query.Where(string)"/> is called at least once.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ReturnQueryWithWhereFilter()
        {
            var sut = new Query()
                .Select("Name")
                .From("tableA", "tableB")
                .Where("Id = 3");

            Approvals.Verify(sut.GetValue());
        }

        /// <summary>
        /// Assert <see cref="Query.GetValue(bool)"/> returns a formatted query after <see cref="Query.GroupBy(string[])"/> is called at least once.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ReturnQueryWithGrouping()
        {
            var sut = new Query()
                .SelectAll()
                .From("tableA")
                .GroupBy("Name", "Age");

            Approvals.Verify(sut.GetValue());
        }

        /// <summary>
        /// Assert <see cref="Query.GetValue(bool)"/> returns a formatted query after <see cref="Query.OrderBy(string[])"/> is called at least once.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ReturnQueryWithOrdering()
        {
            var sut = new Query()
                .SelectAll()
                .From("tableA")
                .OrderBy("Name", "Age");

            Approvals.Verify(sut.GetValue());
        }

        /// <summary>
        /// Assert <see cref="Query.GetValue(bool)"/> returns a formatted query after <see cref="Query.Limit(int)"/> is called at least once.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ReturnQueryLimit()
        {
            var sut = new Query()
                .SelectAll()
                .From("tableA")
                .Limit(10);

            Approvals.Verify(sut.GetValue());
        }

        /// <summary>
        /// Assert <see cref="Query.GetValue(bool)"/> returns a formatted query after calling <see cref="Query.Top(int)"/>.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ReturnQueryWithTop()
        {
            var sut = new Query(SqlStyle.TSQL)
                .SelectAll().Top(10)
                .From("tableA");

            Approvals.Verify(sut.GetValue());
        }

        /// <summary>
        /// Assert <see cref="Query.GetValue(bool)"/> returns a formatted MySQL query when <see cref="SqlStyle.MySQL"/> is passed.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ReturnMySqlQuery()
        {
            var sut = new Query(SqlStyle.MySQL)
                .Select("Id", "Name")
                .From("tableA", "tableB")
                .Where("Id = 12")
                .GroupBy("Name")
                .OrderBy("Age")
                .Limit(100);

            Approvals.Verify(sut.GetValue());
        }

        /// <summary>
        /// Assert <see cref="Query.GetValue(bool)"/> returns a formatted SQLite query when <see cref="SqlStyle.SQLite"/> is passed.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ReturnSQliteQuery()
        {
            var sut = new Query(SqlStyle.SQLite)
                .Select("Id", "Name")
                .From("tableA", "tableB")
                .Where("")
                .GroupBy("Name")
                .OrderBy("Age")
                .Limit(100);

            Approvals.Verify(sut.GetValue());
        }

        /// <summary>
        /// Assert <see cref="Query.GetValue(bool)"/> returns a non T-SQL query with no new lines or tab characters.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ReturnMinifiedNonTSqlQuery()
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

        /// <summary>
        /// Assert <see cref="Query.GetValue(bool)"/> returns a T-SQL query with no new lines or tab characters.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ReturnMinifiedTSqlQuery()
        {
            var sut = new Query(SqlStyle.TSQL)
                .Select("Id", "Name").Top(100)
                .From("tableA", "tableB")
                .Where("Id = 22")
                .GroupBy("Name", "Age")
                .OrderBy("Name", "Age");

            Approvals.Verify(sut.GetValue(minify: true));
        }
    }
}
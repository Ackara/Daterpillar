using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar.Data.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [UseApprovalSubdirectory(Str.ApprovalsDir)]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class QueryTest
    {
        /// <summary>
        /// Assert <see cref="Query.GetValue"/> returns an empty string when instaniated.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void ReturnEmptyString()
        {
            var query = new Query();
            Assert.AreEqual(string.Empty, query.GetValue());
        }

        [TestMethod]
        [Owner(Str.Ackara)]
        public void ReturnSelectFromQuery()
        {
            var sut = new Query()
                .Select("Id", "Name")
                .From("Employee");

            Approvals.Verify(sut.GetValue());
        }
    }
}
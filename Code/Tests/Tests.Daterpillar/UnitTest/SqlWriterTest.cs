using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar.Data.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [UseApprovalSubdirectory(Str.ApprovalsDir)]
    [UseReporter(typeof(FileLauncherReporter), typeof(ClipboardReporter))]
    public class SqlWriterTest
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertEntityToQuery()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertEntityToInsertCommand()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertEntityToDeleteCommand()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertDateToSqlValue()
        {
            Assert.AreEqual("'2015-01-01 13:11:11'", SqlWriter.EscapeValue(new DateTime(2015, 01, 01, 13, 11, 11, DateTimeKind.Utc)));
        }

        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertIntegerToSqlValue()
        {
            Assert.AreEqual("'123'", SqlWriter.EscapeValue(123));
        }

        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertFloatToSqlValue()
        {
            Assert.AreEqual("'123.45'", SqlWriter.EscapeValue(123.45F));
        }

        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertBooleanToSqlValue()
        {
            Assert.AreEqual("'1'", SqlWriter.EscapeValue(true));
        }

        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertStringToSqlValue()
        {
            Assert.AreEqual("'the dog''s bowl. ''_'' ^_^ ''_'''", SqlWriter.EscapeValue("the dog's bowl. '_' ^_^ '_'"));
        }

        [TestMethod]
        [Owner(Str.Ackara)]
        public void ConvertEnumToSqlValue()
        {
            Assert.AreEqual("'0'", SqlWriter.EscapeValue(DayOfWeek.Sunday));
        }
    }
}
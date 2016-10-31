using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ApprovalTests.Reporters;
using ApprovalTests.Namers;
using Tests.Daterpillar.Constants;

namespace Test.Daterpillar.Tests
{
    [TestClass]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(FileLauncherReporter), typeof(ClipboardReporter))]
    public class MSSQLTemplateBuilderTest
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        [TestInitialize]
        public void Setup()
        {
            // TODO: Truncate database.
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void TestMethod1()
        {

        }
    }
}

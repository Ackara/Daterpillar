using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Tests.Daterpillar.UWP.IntegrationTest
{
    [TestClass]
    public class UwpSQLiteConnectionTest
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            // Create SQLite database

        }

        [TestMethod]
        [Owner(Str.Ackara)]
        public void RunQueryOnSQLitePclConnection()
        {
            // Arrange
            
        }

        internal static string ConnectionString;
    }
}

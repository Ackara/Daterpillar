using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar
{
    [TestClass]
    public class Startup
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            var mssqlFiles = from f in Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
                             where Path.GetFileName(f).StartsWith("dtp-")
                             select f;

            foreach (string filePath in mssqlFiles)
                File.Delete(filePath);
        }
    }
}
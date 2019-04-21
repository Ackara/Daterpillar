using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Acklann.Daterpillar
{
    [TestClass]
    public class Startup
    {
        [AssemblyCleanup]
        public static void Cleanup()
        {
            //RemoveMSSQLFiles();
            //RemoveUnusedApprovalFiles(@"C:\Users\Ackeem\Projects\Daterpillar\tests\Daterpillar.MSTest\Tests\approved-results");
        }

        private static void RemoveUnusedApprovalFiles(string path)
        {
            if (!Directory.Exists(path)) throw new DirectoryNotFoundException($"Could not find directory at '{path}'.");

            var tests = (from t in typeof(Startup).Assembly.ExportedTypes
                         where t.IsDefined(typeof(TestClassAttribute))
                         from m in t.GetMethods()
                         where m.IsDefined(typeof(TestMethodAttribute))
                         select m);

            var activeFiles = new List<string>();
            var directory = new DirectoryInfo(path);
            foreach (MethodInfo tm in tests)
            {
                var files = directory.GetFiles($"{tm.DeclaringType.Name}-{tm.Name}*.*").Select(x => x.FullName);
                activeFiles.AddRange(files);
            }

            var deadFiles = directory.GetFiles().Select(x => x.FullName).Except(activeFiles);
            foreach (string file in deadFiles)
            {
                if (File.Exists(file)) File.Delete(file);
                System.Diagnostics.Debug.WriteLine($"removed '{file}'.");
            }
        }

        private static void RemoveMSSQLFiles()
        {
            var mssqlFiles = from f in Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
                             where Path.GetFileName(f).StartsWith("dtp-")
                             select f;

            foreach (string filePath in mssqlFiles)
                File.Delete(filePath);
        }
    }
}
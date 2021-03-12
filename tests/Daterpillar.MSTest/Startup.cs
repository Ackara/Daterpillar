using Acklann.Daterpillar;
using Acklann.Daterpillar.Attributes;
using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Prototyping;
using Acklann.Diffa;
using Acklann.Diffa.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

[assembly: Reporter(typeof(DiffReporter))]
[assembly: ApprovedFolder("approved-results")]
[assembly: Include(Sample.File.MusicDataXML)]

namespace Acklann.Daterpillar
{
    [TestClass]
    public class Startup
    {
        [AssemblyInitialize]
        public static void Setup(TestContext _)
        {
            RestoreDatabase();
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
        }

        private static void RestoreDatabase()
        {
            Schema schema = Migration.SchemaFactory.CreateFrom(typeof(Album).Assembly);
        }

        private static Schema CreateSchema()
        {
            return Migration.SchemaFactory.CreateFrom(typeof(Album).Assembly);
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
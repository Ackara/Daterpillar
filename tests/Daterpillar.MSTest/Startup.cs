using Acklann.Daterpillar;
using Acklann.Diffa;
using Acklann.Diffa.Reporters;
using AutoBogus;
using AutoBogus.Conventions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

[assembly: ApprovalTests.Reporters.UseReporter(typeof(ApprovalTests.Reporters.DiffReporter))]
[assembly: ApprovalTests.Namers.UseApprovalSubdirectory("approved-results")]
[assembly: Reporter(typeof(DiffReporter))]
[assembly: ApprovedFolder("approved-results")]
[assembly: Acklann.Daterpillar.Annotations.Include(Sample.File.MusicDataXML)]

namespace Acklann.Daterpillar
{
    [TestClass]
    public class Startup
    {
        [AssemblyInitialize]
        public static void Setup(TestContext _)
        {
            AutoFaker.Configure((builder) =>
            {
                builder.WithConventions((context) =>
                {
                });

                builder.WithOverride((context) =>
                {
                    return new Prototyping.Username(context.Faker.Internet.UserName(), context.Faker.Internet.Email());
                });

                builder.WithOverride((context) =>
                {
                    var i = (Prototyping.Contact)context.Instance;
                    i.UserId = new Prototyping.Username(context.Faker.Internet.UserName(), context.Faker.Internet.Email());

                    return i;
                });
            });
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
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
    }
}
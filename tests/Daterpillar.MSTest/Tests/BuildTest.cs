using Acklann.Diffa;
using FakeItEasy;
using Microsoft.Build.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.IO;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class BuildTest
    {
        [TestMethod]
        public void Can_schema_from_dll_file()
        {
            // Arrange
            var mockHost = A.Fake<ITaskHost>();
            var mockEngine = A.Fake<IBuildEngine>();

            var assemblyFile = typeof(BuildTest).Assembly.Location;
            var resultFile = Path.ChangeExtension(assemblyFile, ".schema.xml");

            var task = new GenerateMigrationScriptTask
            {
                AssemblyFile = assemblyFile,
                BuildEngine = mockEngine,
                HostObject = mockHost
            };

            // Act
            var passed = task.Execute();

            // Assert
            passed.ShouldBeTrue();
            File.Exists(resultFile).ShouldBeTrue($"could not find '{assemblyFile}'.");
        }
    }
}
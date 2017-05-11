using Ackara.Daterpillar.MSBuild;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.IO;
using System.Reflection;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    public class GenerateSchemaTaskTest
    {
        [TestMethod]
        public void Execute_should_generate_a_xml_schema_file_when_invoked()
        {
            // Arrange
            var assemblyFile = Assembly.GetExecutingAssembly().Location;

            var sut = new GenerateSchemaTask()
            {
                AssemblyFile = assemblyFile
            };

            // Act
            if (File.Exists(sut.SchemaPath)) File.Delete(sut.SchemaPath);

            sut.Execute();
            bool schemaWasGenerated = File.Exists(sut.SchemaPath);
            string content = File.ReadAllText(sut.SchemaPath);

            // Assert
            schemaWasGenerated.ShouldBeTrue();
            content.ShouldNotBeNullOrWhiteSpace();
        }
    }
}
using Ackara.Daterpillar;
using Ackara.Daterpillar.Cmdlets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.Linq;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    public class ConvertToSchemaCmdletTest
    {
        [TestMethod]
        public void Invoke_should_return_a_schema_object_when_a_file_path_is_passed()
        {
            // Arrange
            var sampleFile = MockData.GetFilePath(FName.cmdlets_source_schemaXML);
            var sut = new ConvertToSchemaCmdlet() { InputObject = sampleFile };

            // Act
            var results = sut.Invoke<Schema>().ToArray();

            // Assert
            results.Length.ShouldBe(1);
            results[0].Tables.Count.ShouldBeGreaterThanOrEqualTo(1);
        }

        [TestMethod]
        public void Invoke_should_return_a_schema_object_when_a_file_info_object_is_passed()
        {
            // Arrange
            var sampleFile = MockData.GetFile(FName.cmdlets_source_schemaXML);
            var sut = new ConvertToSchemaCmdlet() { InputObject = sampleFile };

            // Act
            var results = sut.Invoke<Schema>().ToArray();

            // Assert
            results.Length.ShouldBe(1);
            results[0].Tables.Count.ShouldBeGreaterThanOrEqualTo(1);
        }

        [TestMethod]
        public void Invoke_should_return_a_schema_object_when_a_db_connection_is_passed()
        {
            // Arrange
            var connection = ConnectionFactory.CreateSQLiteConnection("dtpl_cmdlet");
            var sut = new ConvertToSchemaCmdlet() { InputObject = connection };

            // Act
            connection.UseSchema(FName.cmdlets_source_schemaXML);

            Schema[] results;
            try { results = sut.Invoke<Schema>().ToArray(); }
            finally { connection.Dispose(); }
            
            // Assert
            results.Length.ShouldBe(1);
            results[0].Tables.Count.ShouldBeGreaterThanOrEqualTo(1);
        }
    }
}
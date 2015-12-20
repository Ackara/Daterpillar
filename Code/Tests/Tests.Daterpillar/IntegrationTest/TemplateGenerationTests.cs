using Ackara.Daterpillar.Transformation;
using Ackara.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Daterpillar.IntegrationTest
{
    [TestClass]
    public class TemplateGenerationTests
    {
        /// <summary>
        /// Generate a SQLite schema from the <see cref="Filename.EmployeeSchema"/> file.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void GenerateSQLiteSchemaFromFile()
        {
            // Arrange
            var schema = Schema.Load(Samples.GetFile(Filename.EmployeeSchema).OpenRead());
            var template = new SQLiteTemplate(new SQLiteTypeNameResolver());

            // Act
            var sqlite = template.Transform(schema);

            // Assert
            DbFactory.CreateSQLiteConnection(sqlite);
        }
    }
}
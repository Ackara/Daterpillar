using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using Gigobyte.Daterpillar.Utilities;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Daterpillar.IntegrationTest
{
    [TestClass]
    [DeploymentItem(Artifact.x86SQLiteInterop)]
    [DeploymentItem(Artifact.x64SQLiteInterop)]
    [DeploymentItem(Artifact.SamplesFolder + Artifact.SampleSchema)]
    public class EnumGeneratorTest
    {
        /// <summary>
        /// Generates a enum declaration from a database connection.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GenerateEnumDeclarationFromDbConnection()
        {
            // Arrange
            var xml = Samples.GetFileContent(Artifact.SampleSchema);
            var sqlite = new SQLiteTemplate().Transform(Schema.Parse(xml));

            using (var connection = DbFactory.CreateSQLiteConnection(sqlite))
            {
                using (var generator = new EnumGenerator(connection))
                {
                    // Act
                    var enumeration = generator.FetchEnumeration("genre", "Name", "Id");

                    var template = new EnumTemplate();
                    var code = template.Transform(enumeration);

                    var syntax = CSharpSyntaxTree.ParseText(code);
                    var errorList = syntax.GetDiagnostics();
                    int errorCount = 0;

                    foreach (var error in errorList)
                    {
                        errorCount++;
                        System.Diagnostics.Debug.WriteLine(error);
                    }

                    // Assert
                    Assert.AreEqual(0, errorCount);
                    Assert.IsFalse(string.IsNullOrWhiteSpace(code));
                }
            }
        }
    }
}
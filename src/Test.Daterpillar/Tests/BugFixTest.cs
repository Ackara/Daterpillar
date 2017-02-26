using Acklann.Daterpillar;
using Acklann.Daterpillar.TextTransformation;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using Tests.Daterpillar.Constants;
using Tests.Daterpillar.Helpers;

namespace Test.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(SampleData.Folder)]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(FileLauncherReporter), typeof(ClipboardReporter))]
    public class BugFixTest
    {
        /* All the column attribute key field was being set to true. */
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void Load_should_deserialize_a_schema_object_when_mock_schema_5_is_passed()
        {
            // Arrange
            var builder = new CSharpScriptBuilder();
            var schemaFile = TestData.GetFile(KnownFile.MockSchema5XML);

            // Act
            var schema = Schema.Load(File.ReadAllText(schemaFile.FullName));
            var cardTable = schema.Tables.First((x) => x.Name == "card");
            var totalKeys = cardTable.Indexes.Count((x) => x.Type == IndexType.PrimaryKey);
            builder.Create(schema);

            // Assert
            Assert.IsNotNull(schema);
            Assert.AreEqual(1, totalKeys);
            Approvals.Verify(builder.GetContent());
        }
    }
}
using Ackara.Daterpillar.Transformation;
using Ackara.Daterpillar.Transformation.Template;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [UseApprovalSubdirectory(Str.ApprovalsDir)]
    [UseReporter(typeof(FileLauncherReporter), typeof(ClipboardReporter))]
    public class SQLiteTemplateTest
    {
        /// <summary>
        /// Verify the <see cref="SQLiteTemplate.Transform(Schema)"/> method outputs a Schema object 
        /// as a SQLite schema when the default settings is passed.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]        
        public void ReturnSQLiteSchemaWithDefaultSettings()
        {
            // Arrange
            var sut = new SQLiteTemplate();
            var sample = Samples.GetSchema();

            // Act
            var result = sut.Transform(sample);

            // Assert
            Approvals.VerifyXml(result);
        }
    }
}
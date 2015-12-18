using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [UseApprovalSubdirectory(Str.ApprovalsDir)]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class SchemaTest
    {
        /// <summary>
        /// Save writes a s the schema object into a stream when called.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void Save_writes_a_Schema_object_into_a_stream_when_called()
        {
            // Arrange
            var sut = Samples.GetSchema();
            using (var stream = new MemoryStream())
            {
                // Act
                sut.Save(stream);
                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();

                    // Assert
                    Approvals.VerifyXml(result);
                }
            }
        }

        [TestMethod]
        [Owner(Str.Ackara)]
        public void Save_writes_a_Schema_object_into_a_XmlWriter_when_called()
        {
            // Arrange
            var sut = Samples.GetSchema();
            using (var stream = new MemoryStream())
            {
                var settings = new XmlWriterSettings();
                settings.Indent = false;

                var writer = XmlWriter.Create(stream, settings);

                // Act
                sut.Save(writer);
                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();

                    // Assert
                    Approvals.VerifyXml(result);
                }
            }
        }
    }
}
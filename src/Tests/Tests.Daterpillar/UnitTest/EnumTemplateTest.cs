using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    [UseApprovalSubdirectory(Dev.ApprovalsDir)]
    [UseReporter(typeof(FileLauncherReporter), typeof(ClipboardReporter))]
    public class EnumTemplateTest
    {
        /// <summary>
        /// Assert <see cref="EnumTemplate.Transform(Enumeration)"/> generates a enum declaration from a <see cref="Enumeration"/> object.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GenerateEnumDeclaration()
        {
            // Arrange
            var sample = new Enumeration();
            sample.Name = "color";
            sample.Values = new List<KeyValuePair<string, int>>();
            sample.Values.Add(new KeyValuePair<string, int>("Red", 0));
            sample.Values.Add(new KeyValuePair<string, int>("Green", 1));
            sample.Values.Add(new KeyValuePair<string, int>("Yellow", 2));

            var sut = new EnumTemplate();

            // Act
            string code = sut.Transform(sample);

            // Assert
            Approvals.Verify(code);
        }
    }
}
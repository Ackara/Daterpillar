using Acklann.Daterpillar;
using ApprovalTests;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.IO;
using System.Reflection;
using System.Text;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    public class AssemblyToSchemaGeneratorTest
    {
        [TestMethod]
        public void ToSchema_should_return_a_schema_object_that_when_invoked()
        {
            // Arrange
            bool xmlIsValid;
            string xml = "", xmlError;
            var sample = Assembly.GetExecutingAssembly();

            // Act
            var result = AssemblyToSchemaConverter.ToSchema(sample);

            using (var xmlData = new MemoryStream())
            {
                
                result.Save(xmlData);
                xml = Encoding.UTF8.GetString(xmlData.ToArray());
                xmlIsValid = Helper.ValidateXml(xmlData, out xmlError);
            }

            // Assert
            Approvals.VerifyXml((xmlError + xml));
            xmlIsValid.ShouldBeTrue(xmlError);
        }
    }
}
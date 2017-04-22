using Ackara.Daterpillar;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;

namespace MSTest.Daterpillar
{
    public static class Helper
    {
        

        public static bool ValidateXml(Stream inputStream, out string errorMsg)
        {
            bool isValid = true;
            var text = new StringBuilder();

            string pathToSchema = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MockData.daterpillarXSD);
            var xsd = new XmlSchemaSet();
            xsd.Add(Schema.Namespace, pathToSchema);

            var document = XDocument.Load(inputStream);
            document.Validate(xsd, (sender, e) =>
            {
                isValid = false;
                text.AppendLine($"{e.Severity}: {e.Message}");
            });

            errorMsg = text.ToString();
            return isValid;
        }

        

        internal static void CreateEmptyDatabase(IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        internal static void OpenFileUsingDefaultApplication(string script)
        {
            throw new NotImplementedException();
        }
    }
}
using Ackara.Daterpillar;
using System;
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

            string pathToSchema = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FName.daterpillarXSD);
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
    }
}
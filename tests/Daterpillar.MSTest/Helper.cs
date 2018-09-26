using Acklann.Daterpillar;
using Acklann.Daterpillar.Configuration;
using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;

namespace MSTest.Daterpillar
{
    public static class Helper
    {
        public static bool ValidateXml(this string xml, out string errorMsg)
        {
            using (var input = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                return ValidateXml(input, out errorMsg);
            }
        }

        public static bool ValidateXml(this Stream inputStream, out string errorMsg)
        {
            bool isValid = true;
            var text = new StringBuilder();

            string pathToSchema = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "intellisence.xsd");
            var xsd = new XmlSchemaSet();
            xsd.Add(Schema.XMLNS, pathToSchema);

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
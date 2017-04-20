using Ackara.Daterpillar;
using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;

namespace MSTest.Daterpillar
{
    public class Helper
    {
        public static bool ValidateXml(Stream inputStream, out string errorMsg)
        {
            bool isValid = true;
            var err = new StringBuilder();

            string pathToSchema = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "daterpillar.xsd");
            var xsd = new XmlSchemaSet();
            xsd.Add(Schema.Namespace, pathToSchema);

            var document = XDocument.Load(inputStream);
            document.Validate(xsd, (sender, e) =>
            {
                isValid = false;
                err.AppendLine($"{e.Severity}: {e.Message}");
            });

            errorMsg = err.ToString();
            return isValid;
        }
    }
}
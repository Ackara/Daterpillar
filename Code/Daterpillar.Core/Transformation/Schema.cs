using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Ackara.Daterpillar.Transformation
{
    [XmlRoot("schema", Namespace = Xmlns)]
    public class Schema : Class1
    {
        #region Static Members

        public const string Xmlns = "http://schema.gigobyte.com/v1/xddl.xsd";

        public static Schema Load(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(Schema));
            return (Schema)serializer.Deserialize(stream);
        }

        public static Schema Parse(string text)
        {
            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text)))
            {
                var serializer = new XmlSerializer(typeof(Schema));
                return (Schema)serializer.Deserialize(stream);
            }
        }

        #endregion Static Members

        public string Path { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("author")]
        public string Author { get; set; }

        [XmlElement("table")]
        public List<Table> Tables { get; set; }
    }
}
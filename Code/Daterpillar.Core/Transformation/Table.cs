using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ackara.Daterpillar.Transformation
{
    public class Table
    {
        public const string TagName = "table";

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("comment")]
        public string Comment { get; set; }

        [XmlElement("modifier")]
        public List<string> Modifiers { get; set; }

        [XmlElement("column")]
        public List<Column> Members { get; set; }

        [XmlElement("foreignKey")]
        public List<ForeignKey> ForeignKeys { get; set; }
    }
}
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ackara.Daterpillar.Transformation
{
    public class Table
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("comment")]
        public string Comment { get; set; }

        [XmlElement("modifier")]
        public List<string> Modifiers { get; set; }

        [XmlElement("column")]
        public List<Column> Columns { get; set; }

        [XmlElement("foreignKey")]
        public List<ForeignKey> ForeignKeys { get; set; }

        [XmlElement("index")]
        public List<Index> Indexes { get; set; }
    }
}
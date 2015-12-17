using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ackara.Daterpillar.Transformation
{
    public class Index
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("unique")]
        public bool Unique { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlElement("columnName")]
        public List<IndexColumn> Columns { get; set; }
    }
}
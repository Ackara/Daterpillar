using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ackara.Daterpillar.Transformation
{
    public class Column
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("comment")]
        public string Comment { get; set; }

        [XmlElement("dataType")]
        public DataType Type { get; set; }

        [XmlElement("modifier")]
        public List<string> Modifiers { get; set; }
    }
}
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ackara.Daterpillar.Transformation
{
    public class Column
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("autoIncrement")]
        public bool AutoIncrement { get; set; }

        [XmlElement("comment")]
        public string Comment { get; set; }

        [XmlElement("dataType")]
        public DataType DataType { get; set; }

        [XmlElement("modifier")]
        public List<string> Modifiers { get; set; }
    }
}
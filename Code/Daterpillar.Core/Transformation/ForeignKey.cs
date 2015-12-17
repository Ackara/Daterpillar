using System.Xml.Serialization;

namespace Ackara.Daterpillar.Transformation
{
    public class ForeignKey
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("localColumn")]
        public string LocalColumn { get; set; }

        [XmlAttribute("foreignTable")]
        public string ForeignTable { get; set; }

        [XmlAttribute("foreignColumn")]
        public string ForeignColumn { get; set; }

        [XmlElement("onUpdate")]
        public ForeignKeyRule OnUpdate { get; set; }

        [XmlElement("onDelete")]
        public ForeignKeyRule OnDelete { get; set; }

        [XmlElement("onMatch")]
        public ForeignKeyRule OnMatch { get; set; }
    }
}
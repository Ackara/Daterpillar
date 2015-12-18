using System.Xml.Serialization;

namespace Ackara.Daterpillar.Transformation
{
    public class DataType
    {
        [XmlAttribute("min")]
        public int Precision { get; set; }

        [XmlAttribute("max")]
        public int Scale { get; set; }

        [XmlText]
        public string Name { get; set; }
    }
}
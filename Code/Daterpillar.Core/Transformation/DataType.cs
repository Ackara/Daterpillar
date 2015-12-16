using System.Xml.Serialization;

namespace Ackara.Daterpillar.Transformation
{
    public class DataType
    {
        [XmlAttribute("min")]
        public int Min { get; set; }

        [XmlAttribute("max")]
        public int Max { get; set; }

        [XmlText]
        public string TypeName { get; set; }
    }
}
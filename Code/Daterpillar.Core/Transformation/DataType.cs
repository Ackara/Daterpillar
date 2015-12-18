using System.Xml.Serialization;

namespace Ackara.Daterpillar.Transformation
{
    public class DataType
    {
        [XmlAttribute("scale")]
        public int Scale { get; set; }

        [XmlAttribute("precision")]
        public int Precision { get; set; }

        [XmlText]
        public string Name { get; set; }
    }
}
using System.Xml.Serialization;

namespace Gigobyte.Daterpillar.Transformation
{
    public struct DataType
    {
        public DataType(string typeName) : this()
        {
            Name = typeName;
        }

        [XmlAttribute("scale")]
        public int Scale { get; set; }

        [XmlAttribute("precision")]
        public int Precision { get; set; }

        [XmlText]
        public string Name { get; set; }
    }
}
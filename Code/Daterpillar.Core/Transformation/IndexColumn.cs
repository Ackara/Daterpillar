using System.Xml.Serialization;

namespace Gigobyte.Daterpillar.Transformation
{
    public struct IndexColumn
    {
        [XmlText]
        public string Name { get; set; }

        [XmlAttribute("order")]
        public SortOrder Order { get; set; }
    }
}
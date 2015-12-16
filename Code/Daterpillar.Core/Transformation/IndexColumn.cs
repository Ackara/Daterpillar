using System.Xml.Serialization;

namespace Ackara.Daterpillar.Transformation
{
    public struct IndexColumn
    {
        [XmlText]
        public string Name { get; set; }

        [XmlAttribute("order")]
        public SortOrder Order { get; set; }
    }
}
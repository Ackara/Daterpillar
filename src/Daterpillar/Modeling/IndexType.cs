using System.Xml.Serialization;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// A kind of index.
    /// </summary>
    public enum IndexType
    {
        [XmlEnum("index")]
        Index,

        [XmlEnum("primary-key")]
        PrimaryKey,
    }
}
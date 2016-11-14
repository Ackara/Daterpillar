using System.Xml.Serialization;

namespace Gigobyte.Daterpillar
{
    /// <summary>
    /// Database index type
    /// </summary>
    public enum IndexType
    {
        /// <summary>
        /// A index
        /// </summary>
        [XmlEnum("index")]
        Index,

        /// <summary>
        /// A primary key.
        /// </summary>
        [XmlEnum("primaryKey")]
        PrimaryKey,
    }
}
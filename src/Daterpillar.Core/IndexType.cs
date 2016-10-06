using System.Xml.Serialization;

namespace Gigobyte.Daterpillar
{
    /// <summary>
    /// Database index type
    /// </summary>
    public enum IndexType
    {
        /// <summary>
        /// A primary key.
        /// </summary>
        [XmlEnum("primaryKey")]
        PrimaryKey = 0,

        /// <summary>
        /// A index
        /// </summary>
        [XmlEnum("index")]
        Index = 1
    }
}
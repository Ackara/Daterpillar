using System.Xml.Serialization;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// Represents a SQL index type.
    /// </summary>
    public enum IndexType
    {
        /// <summary>
        /// A index
        /// </summary>
        [XmlEnum("index")]
        Index,

        /// <summary>
        /// A primary key
        /// </summary>
        [XmlEnum("primaryKey")]
        PrimaryKey
    }
}
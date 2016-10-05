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
        [XmlEnum]
        Primary = 0,

        /// <summary>
        /// A index
        /// </summary>
        [XmlEnum]
        Index = 1
    }
}
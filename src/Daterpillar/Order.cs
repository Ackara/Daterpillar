using System.Xml.Serialization;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// Sorting direction.
    /// </summary>
    public enum Order
    {
        /// <summary>
        /// Ascending
        /// </summary>
        [XmlEnum("ASC")]
        Ascending,

        /// <summary>
        /// Descending
        /// </summary>
        [XmlEnum("DESC")]
        Descending
    }
}
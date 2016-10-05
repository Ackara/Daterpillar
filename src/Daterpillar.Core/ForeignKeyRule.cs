using System.Xml.Serialization;

namespace Gigobyte.Daterpillar
{
    /// <summary>
    /// Foreign key collision rule.
    /// </summary>
    public enum ForeignKeyRule
    {
        [XmlEnum]
        NONE,

        [XmlEnum]
        CASCADE,

        [XmlEnum]
        SET_NULL,

        [XmlEnum]
        SET_DEFAULT,

        [XmlEnum]
        RESTRICT
    }
}
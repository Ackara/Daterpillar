using System.Xml.Serialization;

namespace Acklann.Daterpillar
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

        [XmlEnum("SET NULL")]
        SET_NULL,

        [XmlEnum("SET DEFAULT")]
        SET_DEFAULT,

        [XmlEnum]
        RESTRICT
    }
}
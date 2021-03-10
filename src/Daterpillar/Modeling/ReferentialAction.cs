using System.Xml.Serialization;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// A kind of foreign-key referential action.
    /// </summary>
    public enum ReferentialAction
    {
        /// <summary>
        /// This means something
        /// </summary>
        [XmlEnum("no-action")]
        NoAction,

        [XmlEnum("cascade")]
        Cascade,

        [XmlEnum("restrict")]
        Restrict,

        [XmlEnum("set-null")]
        SetNull,

        [XmlEnum("set-default")]
        SetDefault,
    }
}
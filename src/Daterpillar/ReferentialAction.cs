using System.Xml.Serialization;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// Foreign key referential actions
    /// </summary>
    public enum ReferentialAction
    {
        /// <summary>
        /// NO ACTION.
        /// </summary>
        /// <remarks>Uses the platform's default action.</remarks>
        [XmlEnum("NO ACTION")]
        NoAction,

        /// <summary>
        /// CASCADE
        /// </summary>
        /// <remarks>Delete or update the row from the parent table, and automatically delete or update the matching rows in the child table.</remarks>
        [XmlEnum("CASCADE")]
        Cascade,

        /// <summary>
        /// SET NULL
        /// </summary>
        /// <remarks>Delete or update the row from the parent table, and set the foreign key column or columns in the child table to <c>null</c>.</remarks>
        [XmlEnum("SET NULL")]
        SetNull,

        /// <summary>
        /// SET DEFAULT
        /// </summary>
        [XmlEnum("SET DEFAULT")]
        SetDefault,

        /// <summary>
        /// RESTRICT
        /// </summary>
        /// <remarks>Rejects the delete or update operation for the parent table.</remarks>
        [XmlEnum("RESTRICT")]
        Restrict
    }
}
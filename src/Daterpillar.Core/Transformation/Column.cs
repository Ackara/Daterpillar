using System.Collections.Generic;
using System.Xml.Serialization;

namespace Gigobyte.Daterpillar.Transformation
{
    /// <summary>
    /// Represents a database column.
    /// </summary>
    public class Column
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column is auto incremented.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic increment]; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute("autoIncrement")]
        public bool AutoIncrement { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        [XmlElement("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        [XmlElement("dataType")]
        public DataType DataType { get; set; }

        /// <summary>
        /// Gets or sets the modifiers.
        /// </summary>
        /// <value>
        /// The modifiers.
        /// </value>
        [XmlElement("modifier")]
        public List<string> Modifiers
        {
            get
            {
                if (_modifiers == null)
                {
                    _modifiers = new List<string>();
                }

                return _modifiers;
            }
            set { _modifiers = value; }
        }

        #region Private Members

        private List<string> _modifiers;

        #endregion Private Members
    }
}
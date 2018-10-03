using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Acklann.Daterpillar.Configuration
{
    /// <summary>
    /// A in-memory representation of a SQL script.
    /// </summary>
    [Serializable]
    public class Script : IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Script"/> class.
        /// </summary>
        public Script()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Script"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="syntax">The syntax.</param>
        /// <param name="name">The name.</param>
        public Script(string content, Syntax syntax = Syntax.Generic, string name = null)
        {
            Name = name;
            Syntax = syntax;
            Content = content;
        }

        /// <summary>
        /// Gets or sets the name of the script.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the syntax.
        /// </summary>
        /// <value>
        /// The syntax.
        /// </value>
        [XmlAttribute("syntax")]
        public Syntax Syntax { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>

        [XmlText]
        public string Content { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Content;
        }

        #region IXmlSerializable

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            Syntax = Syntax.Generic;
            if (Enum.TryParse(reader.GetAttribute("syntax"), out Syntax s))
            {
                Syntax = s;
            }

            if (reader.Read()) Content = reader.Value;
        }

        public void WriteXml(XmlWriter writer)
        {
            if (Syntax != Syntax.Generic)
            {
                writer.WriteAttributeString("syntax", Syntax.ToString());
            }
            writer.WriteCData(Content);
        }

        #endregion IXmlSerializable
    }
}
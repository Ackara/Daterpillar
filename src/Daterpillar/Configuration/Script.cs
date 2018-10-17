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
    public class Script : IXmlSerializable, ISQLObject
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
        [XmlAttribute(name)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the syntax.
        /// </summary>
        /// <value>
        /// The syntax.
        /// </value>
        [XmlAttribute(syntax)]
        public Syntax Syntax { get; set; }

        [XmlAttribute(before)]
        public int Before { get; set; }

        [XmlAttribute(after)]
        public int After { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>

        [XmlText]
        public string Content { get; set; }

        public string GetName() => Name;

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
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "script")
            {
                Syntax = Syntax.Generic;
                if (Enum.TryParse(reader[syntax], out Syntax s))
                    Syntax = s;

                if (int.TryParse(reader[before], out int id))
                    Before = id;

                if (int.TryParse(reader[after], out id))
                    After = id;

                Name = reader[name];
                if (reader.Read()) Content = reader.Value;

                reader.Read();
            }
            reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
            if (Syntax != Syntax.Generic)
                writer.WriteAttributeString(syntax, Syntax.ToString());

            if (Before != 0)
                writer.WriteAttributeString(before, Before.ToString());

            if (After != 0)
                writer.WriteAttributeString(after, After.ToString());

            if (string.IsNullOrEmpty(Name) == false)
                writer.WriteAttributeString(name, Name);

            writer.WriteCData(Content);
        }

        #endregion IXmlSerializable

        #region ICloneable

        public Script Clone()
        {
            return new Script()
            {
                Name = this.Name,
                After = this.After,
                Before = this.Before,
                Syntax = this.Syntax,
                Content = this.Content
            };
        }

        object ICloneable.Clone() => Clone();

        #endregion ICloneable

        #region Private Members

        private const string name = "name", syntax = "syntax", before = "beforeTable", after = "afterTable";

        #endregion Private Members
    }
}
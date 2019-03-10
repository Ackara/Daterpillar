using System;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Acklann.Daterpillar.Configuration
{
    /// <summary>
    /// A in-memory representation of a SQL script.
    /// </summary>
    [Serializable]
    public class Script : ISqlObject
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
        [XmlAttribute(syntax), DefaultValue(Syntax.Generic)]
        public Syntax Syntax { get; set; }

        [XmlAttribute(before)]
        public string Before { get; set; }

        [XmlAttribute(after)]
        public string After { get; set; }

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
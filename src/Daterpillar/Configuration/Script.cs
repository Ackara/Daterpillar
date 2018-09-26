using System.Xml.Serialization;

namespace Acklann.Daterpillar.Configuration
{
    /// <summary>
    /// A in-memory representation of a SQL script.
    /// </summary>
    public struct Script
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Script"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="syntax">The syntax.</param>
        /// <param name="name">The name.</param>
        public Script(object content, Syntax syntax = Syntax.Generic, string name = null)
        {
            Name = name;
            Syntax = syntax;
            Content = content.ToString();
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
        /// <value>The syntax.</value>
        [XmlAttribute("syntax")]
        public Syntax Syntax { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        [XmlText]
        public string Content { get; set; }
    }
}
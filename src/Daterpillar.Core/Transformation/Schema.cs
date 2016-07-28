using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Gigobyte.Daterpillar.Transformation
{
    /// <summary>Represents a database schema.</summary>
    [XmlRoot("schema", Namespace = Xmlns)]
    public class Schema
    {
        #region Static Members

        /// <summary>The XML document default namespace.</summary>
        public const string Xmlns = "http://api.gigobyte.com/schema/v1/xddl.xsd";

        /// <summary>Creates a <see cref="Schema"/> by using the specified <paramref name="stream"/>.</summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static Schema Load(Stream stream)
        {
            using (stream)
            {
                var serializer = new XmlSerializer(typeof(Schema));
                return (Schema)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// Convert the string representation of a <see cref="Schema"/> to it's <see
        /// cref="System.Object"/> equivalent.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static Schema Parse(string text)
        {
            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text)))
            {
                var serializer = new XmlSerializer(typeof(Schema));
                return (Schema)serializer.Deserialize(stream);
            }
        }

        #endregion Static Members

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>Gets or sets the author.</summary>
        /// <value>The author.</value>
        [XmlAttribute("author")]
        public string Author { get; set; }

        /// <summary>Gets or sets the tables.</summary>
        /// <value>The tables.</value>
        [XmlElement("table")]
        public List<Table> Tables
        {
            get
            {
                if (_tables == null)
                {
                    _tables = new List<Table>();
                }

                return _tables;
            }
            set { _tables = value; }
        }

        /// <summary>Gets or sets the script.</summary>
        /// <value>The script.</value>
        [XmlElement("script")]
        public string Script { get; set; }

        /// <summary>Write this <see cref="Schema"/> to the specified <see cref="Stream"/>.</summary>
        /// <param name="stream">The stream to output this <see cref="Schema"/> to.</param>
        public void WriteTo(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(Schema));
            serializer.Serialize(stream, this);
            stream.Position = 0;
        }

        /// <summary>Removes all <see cref="Table"/> object with the specified name.</summary>
        /// <param name="tableName">Name of the table.</param>
        public void RemoveTable(string tableName)
        {
            for (int i = 0; i < _tables.Count; i++)
                if (_tables[i].Name.Equals(tableName, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    _tables.RemoveAt(i);
                }
        }

        #region Private Member

        private List<Table> _tables;

        #endregion Private Member
    }
}
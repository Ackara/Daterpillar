using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// Represents a database schema.
    /// </summary>
    [XmlRoot("schema", Namespace = Xmlns)]
    public class Schema
    {
        #region Static Members

        /// <summary>
        /// The XML document default namespace.
        /// </summary>
        public const string Xmlns = "http://static.acklann.com/schema/v1/daterpillar.xsd";

        /// <summary>
        /// Creates a <see cref="Schema" /> by using the specified <paramref name="stream" />.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static Schema Load(Stream stream)
        {
            using (stream)
            {
                var serializer = new XmlSerializer(typeof(Schema));
                var schema = (Schema)serializer.Deserialize(stream);
                schema.SetReferences();

                return schema;
            }
        }

        /// <summary>
        /// Convert the string representation of a <see cref="Schema" /> to it's <see
        /// cref="System.Object" /> equivalent.
        /// </summary>
        /// <param name="xml">The text.</param>
        /// <returns></returns>
        public static Schema Load(string xml)
        {
            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml)))
            {
                return Load(stream);
            }
        }

        #endregion Static Members

        /// <summary>
        /// Initializes a new instance of the <see cref="Schema"/> class.
        /// </summary>
        public Schema()
        {
            Tables = new List<Table>();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlAttribute("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        [XmlAttribute("author")]
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the date the schema was created.
        /// </summary>
        /// <value>The created on.</value>
        [XmlAttribute("created")]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the tables.
        /// </summary>
        /// <value>The tables.</value>
        [XmlElement("table")]
        public List<Table> Tables { get; set; }

        /// <summary>
        /// Gets or sets the script.
        /// </summary>
        /// <value>The script.</value>
        [XmlElement("script")]
        public string Script { get; set; }

        /// <summary>
        /// Serialize this object into the specified <see cref="Stream" />.
        /// </summary>
        /// <param name="stream">The stream to output this <see cref="Schema" /> to.</param>
        public void WriteTo(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(Schema));
            serializer.Serialize(stream, this);
            stream.Position = 0;
        }

        public Table CreateTable()
        {
            return CreateTable(string.Empty);
        }

        public Table CreateTable(string name)
        {
            var newTable = new Table(name) { SchemaRef = this };
            Tables.Add(newTable);

            return newTable;
        }

        /// <summary>
        /// Removes all <see cref="Table" /> object with the specified name.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        public bool RemoveTable(string tableName)
        {
            for (int i = 0; i < Tables.Count; i++)
                if (Tables[i].Name.Equals(tableName, StringComparison.CurrentCultureIgnoreCase))
                {
                    Tables.RemoveAt(i);
                    return true;
                }

            return false;
        }

        public IEnumerable<Index> GetIndexes()
        {
            foreach (var table in Tables)
            {
                foreach (var index in table.Indexes)
                {
                    yield return index;
                }
            }
        }

        public IEnumerable<ForeignKey> GetForeignKeys()
        {
            foreach (var table in Tables)
            {
                foreach (var key in table.ForeignKeys)
                {
                    yield return key;
                }
            }
        }

        internal void SetReferences()
        {
            foreach (var table in Tables)
            {
                if (table.SchemaRef == null) table.SchemaRef = this;

                foreach (var column in table.Columns)
                {
                    if (column.TableRef == null) column.TableRef = table;
                }

                foreach (var index in table.Indexes)
                {
                    if (index.TableRef == null) index.TableRef = table;
                }

                foreach (var constraint in table.ForeignKeys)
                {
                    if (constraint.TableRef == null) constraint.TableRef = table;
                }
            }
        }
    }
}